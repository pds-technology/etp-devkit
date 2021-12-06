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
            RegisterMessageHandler<Authorize>(Protocols.Core, MessageTypes.Core.Authorize, HandleAuthorize);
            RegisterMessageHandler<AuthorizeResponse>(Protocols.Core, MessageTypes.Core.AuthorizeResponse, HandleAuthorizeResponse);
        }

        /// <summary>
        /// Sends an Authorize message to a server.
        /// </summary>
        /// <param name="authorization">The authorization.</param>
        /// <param name="supplementalAuthorization">The supplemental authorization.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<Authorize> Authorize(string authorization, IDictionary<string, string> supplementalAuthorization, IMessageHeaderExtension extension = null)
        {
            var body = new Authorize
            {
                Authorization = authorization,
                SupplementalAuthorization = supplementalAuthorization ?? new Dictionary<string, string>(),
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Handles the Authorize event from a server.
        /// </summary>
        public event EventHandler<RequestWithContextEventArgs<Authorize, bool, AuthorizeContext>> OnAuthorize;

        /// <summary>
        /// Sends an AuthorizeResponse response message to a server.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">Whether or not authorization was successful.</param>
        /// <param name="challenges">Challenges that may be used when authorization was not successful.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<AuthorizeResponse> AuthorizeResponse(IMessageHeader correlatedHeader, bool success, IList<string> challenges, IMessageHeaderExtension extension = null)
        {
            var body = new AuthorizeResponse
            {
                Success = success,
                Challenges = challenges ?? new List<string>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension);
        }

        /// <summary>
        /// Handles the AuthorizeResponse event from a server.
        /// </summary>
        public event EventHandler<ResponseEventArgs<Authorize, AuthorizeResponse>> OnAuthorizeResponse;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<Authorize>)
                HandleResponseMessage(request as EtpMessage<Authorize>, message, OnAuthorizeResponse, HandleAuthorizeResponse);
        }

        /// <summary>
        /// Handles the Authorize message from a client.
        /// </summary>
        /// <param name="message">The Authorize message.</param>
        protected virtual void HandleAuthorize(EtpMessage<Authorize> message)
        {
            HandleRequestMessage(message, OnAuthorize, HandleAuthorize,
                responseMethod: (args) => AuthorizeResponse(args.Request?.Header, args.Response, args.Context.Challenges, extension: args.ResponseExtension));
        }

        /// <summary>
        /// Handles the Authorize message from a client.
        /// </summary>
        /// <param name="args">The <see cref="RequestWithContextEventArgs{Authorize, bool, AuthorizeContext}"/> instance containing the event data.</param>
        protected virtual void HandleAuthorize(RequestWithContextEventArgs<Authorize, bool, AuthorizeContext> args)
        {
        }

        /// <summary>
        /// Handles the AuthorizeResponse message from a server.
        /// </summary>
        /// <param name="message">The AuthorizeResponse message.</param>
        protected virtual void HandleAuthorizeResponse(EtpMessage<AuthorizeResponse> message)
        {
            var request = TryGetCorrelatedMessage<Authorize>(message);
            HandleResponseMessage(request, message, OnAuthorizeResponse, HandleAuthorizeResponse);
        }

        /// <summary>
        /// Handles the AuthorizeResponse message from a server.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{Authorize, AuthorizeResponse}"/> instance containing the event data.</param>
        protected virtual void HandleAuthorizeResponse(ResponseEventArgs<Authorize, AuthorizeResponse> args)
        {
        }
    }
}
