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
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Protocol.Core;

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
        public CoreClientHandler() : base((int)Protocols.Core, Roles.Client, Roles.Server)
        {
            RegisterMessageHandler<Ping>(Protocols.Core, MessageTypes.Core.Ping, HandlePing);
            RegisterMessageHandler<Pong>(Protocols.Core, MessageTypes.Core.Pong, HandlePong);
            RegisterMessageHandler<RenewSecurityTokenResponse>(Protocols.Core, MessageTypes.Core.RenewSecurityTokenResponse, HandleRenewSecurityTokenResponse);
        }

        /// <summary>
        /// Sends a Ping message.
        /// </summary>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<Ping> Ping(IMessageHeaderExtension extension = null)
        {
            var body = new Ping
            {
            };

            return SendRequest(body, extension: extension, onBeforeSend: (m) => m.Body.CurrentDateTime = DateTime.UtcNow.ToEtpTimestamp());
        }

        /// <summary>
        /// Handles the Ping event from a server.
        /// </summary>
        public event EventHandler<EmptyRequestEventArgs<Ping>> OnPing;

        /// <summary>
        /// Sends a Pong response message.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<Pong> Pong(IMessageHeader correlatedHeader, IMessageHeaderExtension extension = null)
        {
            var body = new Pong
            {
            };

            return SendResponse(body, correlatedHeader, extension: extension, onBeforeSend: (m) => m.Body.CurrentDateTime = DateTime.UtcNow.ToEtpTimestamp());
        }

        /// <summary>
        /// Handles the Pong event from a server.
        /// </summary>
        public event EventHandler<ResponseEventArgs<Ping, Pong>> OnPong;

        /// <summary>
        /// Renews the security token.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="token">The token.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<RenewSecurityToken> RenewSecurityToken(IMessageHeader correlatedHeader, string token, IMessageHeaderExtension extension = null)
        {
            var body = new RenewSecurityToken
            {
                Token = token
            };

            return SendResponse(body, correlatedHeader, extension: extension);
        }

        /// <summary>
        /// Handles the RenewSecurityTokenResponse event from a server.
        /// </summary>
        public event EventHandler<ResponseEventArgs<RenewSecurityToken, RenewSecurityTokenResponse>> OnRenewSecurityTokenResponse;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<Ping>)
                HandleResponseMessage(request as EtpMessage<Ping>, message, OnPong, HandlePong);
            else if (request is EtpMessage<RenewSecurityToken>)
                HandleResponseMessage(request as EtpMessage<RenewSecurityToken>, message, OnRenewSecurityTokenResponse, HandleRenewSecurityTokenResponse);
        }

        /// <summary>
        /// Handles the Ping message from a server.
        /// </summary>
        /// <param name="message">The Ping message.</param>
        protected virtual void HandlePing(EtpMessage<Ping> message)
        {
            HandleRequestMessage(message, OnPing, HandlePing,
                responseMethod: (args) => Pong(args.Request?.Header, extension: args.ResponseExtension));
        }

        /// <summary>
        /// Handles the Ping message from a server.
        /// </summary>
        /// <param name="args">The <see cref="EmptyRequestEventArgs{Ping}"/> instance containing the event data.</param>
        protected virtual void HandlePing(EmptyRequestEventArgs<Ping> args)
        {
        }

        /// <summary>
        /// Handles the Pong message from a server.
        /// </summary>
        /// <param name="message">The Pong message.</param>
        protected virtual void HandlePong(EtpMessage<Pong> message)
        {
            var request = TryGetCorrelatedMessage<Ping>(message);
            HandleResponseMessage(request, message, OnPong, HandlePong);
        }

        /// <summary>
        /// Handles the response to a Pong message from a server.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{Ping, Pong}"/> instance containing the event data.</param>
        protected virtual void HandlePong(ResponseEventArgs<Ping, Pong> args)
        {
        }

        /// <summary>
        /// Handles the RenewSecurityTokenResponse message from the server.
        /// </summary>
        /// <param name="message">The RenewSecurityTokenResponse message.</param>
        protected virtual void HandleRenewSecurityTokenResponse(EtpMessage<RenewSecurityTokenResponse> message)
        {
            var request = TryGetCorrelatedMessage<RenewSecurityToken>(message);
            HandleResponseMessage(request, message, OnRenewSecurityTokenResponse, HandleRenewSecurityTokenResponse);
        }

        /// <summary>
        /// Handles the RenewSecurityTokenResponse message from a server.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{RenewSecurityToken, RenewSecurityTokenResponse}"/> instance containing the event data.</param>
        protected virtual void HandleRenewSecurityTokenResponse(ResponseEventArgs<RenewSecurityToken, RenewSecurityTokenResponse> args)
        {
        }
    }
}
