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

using System;
using System.Collections.Generic;
using System.Linq;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v11.Datatypes;

namespace Energistics.Etp.v11.Protocol.Core
{
    /// <summary>
    /// Base implementation of the <see cref="ICoreServer"/> interface.
    /// </summary>
    /// <seealso cref="Etp11ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v11.Protocol.Core.ICoreServer" />
    public class CoreServerHandler : Etp11ProtocolHandler, ICoreServer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreServerHandler"/> class.
        /// </summary>
        public CoreServerHandler() : base((int)Protocols.Core, "server", "client")
        {
            RequestedProtocols = new List<ISupportedProtocol>(0);

            RegisterMessageHandler<RequestSession>(Protocols.Core, MessageTypes.Core.RequestSession, HandleRequestSession);
            RegisterMessageHandler<CloseSession>(Protocols.Core, MessageTypes.Core.CloseSession, HandleCloseSession);
            RegisterMessageHandler<RenewSecurityToken>(Protocols.Core, MessageTypes.Core.RenewSecurityToken, HandleRenewSecurityToken);
        }

        /// <summary>
        /// Gets the name of the client application.
        /// </summary>
        /// <value>The name of the client application.</value>
        public string ClientApplicationName { get; private set; }

        /// <summary>
        /// Gets the requested protocols.
        /// </summary>
        /// <value>The requested protocols.</value>
        public IList<ISupportedProtocol> RequestedProtocols { get; private set; }

        /// <summary>
        /// Gets the list of supported protocols.
        /// </summary>
        /// <value>The supported protocols.</value>
        public IList<ISupportedProtocol> SupportedProtocols { get; private set; }

        /// <summary>
        /// Sends an OpenSession message to a client.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="supportedProtocols">The supported protocols.</param>
        /// <returns>The message identifier.</returns>
        public virtual long OpenSession(IMessageHeader request, IList<ISupportedProtocol> supportedProtocols)
        {
            var header = CreateMessageHeader(Protocols.Core, MessageTypes.Core.OpenSession, request.MessageId);

            var openSession = new OpenSession()
            {
                ApplicationName = Session.ApplicationName,
                ApplicationVersion = Session.ApplicationVersion,
                SupportedProtocols = supportedProtocols.Cast<SupportedProtocol>().ToList(),
                SupportedObjects = Session.SupportedObjects,
                SessionId = Session.SessionId
            };

            SupportedProtocols = supportedProtocols;

            Logger.Verbose($"[{Session.SessionId}] Sending open session");

            var messageId = Session.SendMessage(header, openSession);

            Logger.Verbose($"[{Session.SessionId}] Received {header.MessageId} and Sent {messageId}");

            if (messageId == header.MessageId)
            {
                Session.OnSessionOpened(RequestedProtocols, SupportedProtocols);
            }

            return messageId;
        }

        /// <summary>
        /// Sends a CloseSession message to a client.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <returns>The message identifier.</returns>
        public virtual long CloseSession(string reason = null)
        {
            var header = CreateMessageHeader(Protocols.Core, MessageTypes.Core.CloseSession);

            var closeSession = new CloseSession()
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
        /// Handles the RequestSession event from a client.
        /// </summary>
        public event ProtocolEventHandler<RequestSession> OnRequestSession;

        /// <summary>
        /// Handles the CloseSession event from a client.
        /// </summary>
        public event ProtocolEventHandler<CloseSession> OnCloseSession;

        /// <summary>
        /// Handles the RenewSecurityToken event from a client.
        /// </summary>
        public event ProtocolEventHandler<RenewSecurityToken> OnRenewSecurityToken;

        /// <summary>
        /// Handles the RequestSession message from a client.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="requestSession">The RequestSession message.</param>
        protected virtual void HandleRequestSession(IMessageHeader header, RequestSession requestSession)
        {
            ClientApplicationName = requestSession.ApplicationName;
            RequestedProtocols = requestSession.RequestedProtocols.Cast<ISupportedProtocol>().ToList();
            Notify(OnRequestSession, header, requestSession);

            var protocols = RequestedProtocols
                .Select(x => new { x.Protocol, x.Role })
                .ToArray();

            // Only return details for requested protocols
            var supportedProtocols = Session.GetSupportedProtocols()
                .Where(x => protocols.Any(y => x.Protocol == y.Protocol && string.Equals(x.Role, y.Role, StringComparison.InvariantCultureIgnoreCase)))
                .ToList();

            OpenSession(header, supportedProtocols);

            // Only send OpenSession if there are protocols supported
            //if (supportedProtocols.Any())
            //{
            //    OpenSession(header, supportedProtocols);
            //}
            //else // Otherwise, ProtocolException is sent
            //{
            //    ProtocolException((int)ErrorCodes.ENOSUPPORTEDPROTOCOLS, "No protocols supported", header.MessageId);
            //}
        }

        /// <summary>
        /// Handles the CloseSession message from a client.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="closeSession">The CloseSession message.</param>
        protected virtual void HandleCloseSession(IMessageHeader header, CloseSession closeSession)
        {
            Notify(OnCloseSession, header, closeSession);
            Session.OnSessionClosed();
            Session.Close(closeSession.Reason);
        }

        /// <summary>
        /// Handles the RenewSecurityToken message from a client.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="renewSecurityToken">The RenewSecurityToken message.</param>
        protected virtual void HandleRenewSecurityToken(IMessageHeader header, RenewSecurityToken renewSecurityToken)
        {
            Notify(OnRenewSecurityToken, header, renewSecurityToken);
        }
    }
}
