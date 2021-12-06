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

namespace Energistics.Etp.v12.Protocol.Core
{
    /// <summary>
    /// Represents the server end of the interface that must be implemented for Protocol 0.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.Core, Roles.Server, Roles.Client)]
    public interface ICoreServer : IProtocolHandler
    {
        /// <summary>
        /// Sends an Authorize message to a client.
        /// </summary>
        /// <param name="authorization">The authorization.</param>
        /// <param name="supplementalAuthorization">The supplemental authorization.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<Authorize> Authorize(string authorization, IDictionary<string, string> supplementalAuthorization, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the Authorize event from a server.
        /// </summary>
        event EventHandler<RequestWithContextEventArgs<Authorize, bool, AuthorizeContext>> OnAuthorize;

        /// <summary>
        /// Sends an AuthorizeResponse response message to a client.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">Whether or not authorization was successful.</param>
        /// <param name="challenges">Challenges that may be used when authorization was not successful.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<AuthorizeResponse> AuthorizeResponse(IMessageHeader correlatedHeader, bool success, IList<string> challenges, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the AuthorizeResponse event from a client.
        /// </summary>
        event EventHandler<ResponseEventArgs<Authorize, AuthorizeResponse>> OnAuthorizeResponse;
    }
}
