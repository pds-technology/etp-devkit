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
    /// Base implementation of the <see cref="ICoreServer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.Core.ICoreServer" />
    public class CoreServerHandler : Etp12ProtocolHandler, ICoreServer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreServerHandler"/> class.
        /// </summary>
        public CoreServerHandler() : base((int)Protocols.Core, "server", "client")
        {
            RegisterMessageHandler<RequestSession>(Protocols.Core, MessageTypes.Core.RequestSession, HandleRequestSession);
            RegisterMessageHandler<CloseSession>(Protocols.Core, MessageTypes.Core.CloseSession, HandleCloseSession);
            RegisterMessageHandler<Ping>(Protocols.Core, MessageTypes.Core.Ping, HandlePing);
            RegisterMessageHandler<Pong>(Protocols.Core, MessageTypes.Core.Pong, HandlePong);
            RegisterMessageHandler<RenewSecurityToken>(Protocols.Core, MessageTypes.Core.RenewSecurityToken, HandleRenewSecurityToken);
        }

        /// <summary>
        /// Handles the RequestSession event from a client.
        /// </summary>
        public event ProtocolEventHandler<RequestSession> OnRequestSession;

        /// <summary>
        /// Sends an OpenSession message to a client.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long OpenSession(IMessageHeader request)
        {
            var header = CreateMessageHeader(Protocols.Core, MessageTypes.Core.OpenSession, request.MessageId);

            var capabilities = new EtpEndpointCapabilities();
            Session.GetInstanceCapabilities(capabilities);

            var openSession = new OpenSession
            {
                ApplicationName = Session.ServerApplicationName,
                ApplicationVersion = Session.ServerApplicationVersion,
                ServerInstanceId = Guid.Parse(Session.ServerInstanceId).ToUuid(),
                SupportedProtocols = Session.SessionSupportedProtocols.Select(sp => sp.AsSupportedProtocol<SupportedProtocol, Datatypes.Version, DataValue>()).ToList(),
                SupportedDataObjects = Session.SessionSupportedDataObjects.Select(o => new SupportedDataObject { QualifiedType = (string)o.DataObjectType }).ToList(),
                SupportedCompression = Session.SessionSupportedCompression ?? string.Empty,
                SupportedFormats = Session.SessionSupportedFormats.ToList(),
                EndpointCapabilities = capabilities.AsDataValueDictionary<DataValue>(),
            };

            Logger.Verbose($"[{Session.SessionKey}] Sending open session");

            var messageId = Session.SendMessage(header, openSession);

            Logger.Verbose($"[{Session.SessionKey}] Received {header.MessageId} and Sent {messageId}");

            Session.OnSessionOpened(messageId >= 0);

            return messageId;
        }

        /// <summary>
        /// Sends a Ping message.
        /// </summary>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long Ping()
        {
            var header = CreateMessageHeader(Protocols.Core, MessageTypes.Core.Ping);

            var message = new Ping
            {
            };

            return Session.SendMessage(header, message, (_, m) => m.CurrentDateTime = DateTime.UtcNow.ToEtpTimestamp());
        }

        /// <summary>
        /// Handles the Ping event from a client.
        /// </summary>
        public event ProtocolEventHandler<Ping> OnPing;

        /// <summary>
        /// Sends a Pong response message.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long Pong(IMessageHeader request)
        {
            var header = CreateMessageHeader(Protocols.Core, MessageTypes.Core.Pong, request.MessageId);

            var message = new Pong
            {
            };

            return Session.SendMessage(header, message, (_, m) => m.CurrentDateTime = DateTime.UtcNow.ToEtpTimestamp());
        }

        /// <summary>
        /// Handles the Pong event from a client.
        /// </summary>
        public event ProtocolEventHandler<Pong> OnPong;

        /// <summary>
        /// Handles the RenewSecurityToken event from a client.
        /// </summary>
        public event ProtocolEventHandler<RenewSecurityToken> OnRenewSecurityToken;

        /// <summary>
        /// Sends a RenewSecurityTokenResponse response message to a client.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long RenewSecurityTokenResponse(IMessageHeader request)
        {
            var header = CreateMessageHeader(Protocols.Core, MessageTypes.Core.RenewSecurityTokenResponse, request.MessageId);

            var message = new RenewSecurityTokenResponse
            {
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Sends a CloseSession message to a client.
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
        /// Handles the CloseSession event from a client.
        /// </summary>
        public event ProtocolEventHandler<CloseSession> OnCloseSession;

        /// <summary>
        /// Handles the RequestSession message from a client.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="requestSession">The RequestSession message.</param>
        protected virtual void HandleRequestSession(IMessageHeader header, RequestSession requestSession)
        {
            var success = Session.InitializeSession(
                Session.SessionId,
                requestSession.ApplicationName,
                requestSession.ApplicationVersion,
                requestSession.ClientInstanceId.ToGuid().ToString(),
                requestSession.RequestedProtocols.Cast<ISupportedProtocol>().ToList(),
                requestSession.SupportedDataObjects.Select(o => (IDataObjectType)new EtpDataObjectType(o.QualifiedType)).ToList(),
                requestSession.SupportedCompression.ToList() ?? new List<string>(),
                requestSession.SupportedFormats?.ToList() ?? new List<string> { "xml" },
                new EtpEndpointCapabilities(requestSession.EndpointCapabilities.ToDictionary(kvp => kvp.Key, kvp => (IDataValue)kvp.Value))
            );

            var args = Notify(OnRequestSession, header, requestSession);
            if (args.Cancel)
                return;

            if (success)
            {
                if (Session.SessionSupportedProtocols.Count == 0)
                {
                    this.NoSupportedProtocols(header);
                    Session.OnSessionOpened(false);
                }
                else if (Session.SessionSupportedFormats.Count == 0)
                {
                    this.InvalidState("No supported formats", header.MessageId);
                    Session.OnSessionOpened(false);
                }
                else
                {
                    OpenSession(header);
                }
            }
            else
            {
                this.InvalidState("Error initializing session", header.MessageId);
                Session.OnSessionOpened(false);
            }
        }

        /// <summary>
        /// Handles the Ping message from the client.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The Ping message.</param>
        protected virtual void HandlePing(IMessageHeader header, Ping message)
        {
            var args = Notify(OnPing, header, message);
            if (args.Cancel)
                return;

            Pong(header);
        }

        /// <summary>
        /// Handles the Pong message from the client.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The Pong message.</param>
        protected virtual void HandlePong(IMessageHeader header, Pong message)
        {
            Notify(OnPong, header, message);
        }

        /// <summary>
        /// Handles the RenewSecurityToken message from a client.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="renewSecurityToken">The RenewSecurityToken message.</param>
        protected virtual void HandleRenewSecurityToken(IMessageHeader header, RenewSecurityToken renewSecurityToken)
        {
            var args = Notify(OnRenewSecurityToken, header, renewSecurityToken);
            if (args.Cancel)
                return;

            RenewSecurityTokenResponse(header);
        }

        /// <summary>
        /// Handles the CloseSession message from a client.
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
