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
            RegisterMessageHandler<OpenSession>(Protocols.Core, MessageTypes.Core.OpenSession, HandleOpenSession);
            RegisterMessageHandler<CloseSession>(Protocols.Core, MessageTypes.Core.CloseSession, HandleCloseSession);
        }

        /// <summary>
        /// Sends a RequestSession message to a server.
        /// </summary>
        /// <param name="requestedProtocols">The requested protocols.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long RequestSession(IReadOnlyList<EtpSessionProtocol> requestedProtocols)
        {
            var header = CreateMessageHeader(Protocols.Core, MessageTypes.Core.RequestSession);

            var capabilities = new EtpEndpointCapabilities();
            Session.GetInstanceCapabilities(capabilities);

            var requestSession = new RequestSession
            {
                ApplicationName = Session.ClientApplicationName,
                ApplicationVersion = Session.ClientApplicationVersion,
                ClientInstanceId = Guid.Parse(Session.ClientInstanceId).ToUuid(),
                RequestedProtocols = requestedProtocols.Select(rp => rp.AsSupportedProtocol<SupportedProtocol, Datatypes.Version, DataValue>()).ToList(),
                SupportedDataObjects = Session.InstanceSupportedDataObjects.Select(o => new SupportedDataObject { QualifiedType = (string)o.DataObjectType }).ToList(),
                SupportedCompression = Session.InstanceSupportedCompression,
                SupportedFormats = Session.InstanceSupportedFormats,
                EndpointCapabilities = capabilities.AsDataValueDictionary<DataValue>(),
            };

            return Session.SendMessage(header, requestSession);
        }

        /// <summary>
        /// Sends a CloseSession message to a server.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long CloseSession(string reason = null)
        {
            var header = CreateMessageHeader(Protocols.Core, MessageTypes.Core.CloseSession);

            var closeSession = new CloseSession
            {
                Reason = reason ?? "Session closed"
            };

            var messageId = Session.SendMessage(header, closeSession);

            Session.OnSessionClosed(messageId >= 0);

            return messageId;
        }

        /// <summary>
        /// Renews the security token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
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
            var success = Session.InitializeSession(
                openSession.ServerInstanceId.ToGuid().ToString(), // TODO: Temporary until SessionId added back to message.
                openSession.ApplicationName,
                openSession.ApplicationVersion,
                openSession.ServerInstanceId.ToGuid().ToString(),
                openSession.SupportedProtocols.Cast<ISupportedProtocol>().ToList(),
                openSession.SupportedDataObjects.Select(o => (IDataObjectType)new EtpDataObjectType(o.QualifiedType)).ToList(),
                openSession.SupportedCompression?.Split(';') ?? new string[0],
                openSession.SupportedFormats.ToList(),
                new EtpEndpointCapabilities(openSession.EndpointCapabilities.ToDictionary(kvp => kvp.Key, kvp => (IDataValue)kvp.Value))
            );

            Notify(OnOpenSession, header, openSession);
            Session.OnSessionOpened(success);
        }

        /// <summary>
        /// Handles the CloseSession message from the server.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="closeSession">The CloseSession message.</param>
        protected virtual void HandleCloseSession(IMessageHeader header, CloseSession closeSession)
        {
            Notify(OnCloseSession, header, closeSession);
            Session.OnSessionClosed(true);
            Session.CloseWebSocket(closeSession.Reason);
        }
    }
}
