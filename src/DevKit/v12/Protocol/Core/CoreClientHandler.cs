//----------------------------------------------------------------------- 
// ETP DevKit, 1.2
//
// Copyright 2019 Energistics
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v12.Datatypes;

namespace Energistics.Etp.v12.Protocol.Core
{
    /// <summary>
    /// Base implementation of the <see cref="ICoreClient"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.Core.ICoreClient" />
    public class CoreClientHandler : Etp12ProtocolHandler, ICoreClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreClientHandler"/> class.
        /// </summary>
        public CoreClientHandler() : base((int)Protocols.Core, "client", "server")
        {
            RequestedProtocols = new List<ISupportedProtocol>(0);
            SupportedProtocols = new List<ISupportedProtocol>(0);
            ServerObjects = new List<string>(0);

            RegisterMessageHandler<OpenSession>(Protocols.Core, MessageTypes.Core.OpenSession, HandleOpenSession);
            RegisterMessageHandler<CloseSession>(Protocols.Core, MessageTypes.Core.CloseSession, HandleCloseSession);
        }

        /// <summary>
        /// Gets the list of requested protocols.
        /// </summary>
        /// <value>The server protocols.</value>
        public IList<ISupportedProtocol> RequestedProtocols { get; private set; }

        /// <summary>
        /// Gets the list of supported protocols.
        /// </summary>
        /// <value>The supported protocols.</value>
        public IList<ISupportedProtocol> SupportedProtocols { get; private set; }

        /// <summary>
        /// Gets the list of supported server objects.
        /// </summary>
        /// <value>The server objects.</value>
        public IList<string> ServerObjects { get; private set; }

        /// <summary>
        /// Gets the requested compression type.
        /// </summary>
        public string RequestedCompression { get; private set; }

        /// <summary>
        /// Sends a RequestSession message to a server.
        /// </summary>
        /// <param name="applicationName">The application name.</param>
        /// <param name="applicationVersion">The application version.</param>
        /// <param name="requestedProtocols">The requested protocols.</param>
        /// <param name="requestedCompression">The requested compression.</param>
        /// <returns>The message identifier.</returns>
        public virtual long RequestSession(string applicationName, string applicationVersion, IList<ISupportedProtocol> requestedProtocols, string requestedCompression)
        {
            var header = CreateMessageHeader(Protocols.Core, MessageTypes.Core.RequestSession);

            var requestSession = new RequestSession
            {
                ApplicationName = applicationName,
                ApplicationVersion = applicationVersion,
                RequestedProtocols = requestedProtocols.Cast<SupportedProtocol>().ToList(),
                SupportedObjects = new List<string>(),
                SupportedCompression = requestedCompression ?? string.Empty
            };

            RequestedProtocols = requestedProtocols;
            RequestedCompression = requestedCompression;

            return Session.SendMessage(header, requestSession);
        }

        /// <summary>
        /// Sends a CloseSession message to a server.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <returns>The message identifier.</returns>
        public virtual long CloseSession(string reason = null)
        {
            var header = CreateMessageHeader(Protocols.Core, MessageTypes.Core.CloseSession);

            var closeSession = new CloseSession
            {
                Reason = reason ?? "Session closed"
            };

            var messageId = Session.SendMessage(header, closeSession);

            if (messageId == header.MessageId)
            {
                Notify(OnCloseSession, header, closeSession);
                Session.OnSessionClosed();
            }

            return messageId;
        }

        /// <summary>
        /// Renews the security token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The message identifier.</returns>
        public virtual long RenewSecurityToken(string token)
        {
            var header = CreateMessageHeader(Protocols.Core, MessageTypes.Core.RenewSecurityToken);

            var renewSecurityToken = new RenewSecurityToken
            {
                Token = token
            };

            return Session.SendMessage(header, renewSecurityToken);
        }

        /// <summary>
        /// Handles the OpenSession event from a server.
        /// </summary>
        public event ProtocolEventHandler<OpenSession> OnOpenSession;

        /// <summary>
        /// Handles the CloseSession event from a server.
        /// </summary>
        public event ProtocolEventHandler<CloseSession> OnCloseSession;

        /// <summary>
        /// Handles the OpenSession message from the server.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="openSession">The OpenSession message.</param>
        protected virtual void HandleOpenSession(IMessageHeader header, OpenSession openSession)
        {
            SupportedProtocols = openSession.SupportedProtocols.Cast<ISupportedProtocol>().ToList();
            ServerObjects = openSession.SupportedObjects;
            Session.SessionId = openSession.SessionId;
            Session.SupportedCompression = openSession.SupportedCompression;
            Notify(OnOpenSession, header, openSession);
            Session.OnSessionOpened(RequestedProtocols, SupportedProtocols);
        }

        /// <summary>
        /// Handles the CloseSession message from the server.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="closeSession">The CloseSession message.</param>
        protected virtual void HandleCloseSession(IMessageHeader header, CloseSession closeSession)
        {
            Notify(OnCloseSession, header, closeSession);
            Session.OnSessionClosed();
            Session.Close(closeSession.Reason);
        }
    }
}
