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

using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v12.Datatypes;
using System;
using System.Collections.Generic;

namespace Energistics.Etp.v12.Protocol
{
    /// <summary>
    /// Provides common functionality for ETP 1.2 protocol handlers.
    /// </summary>
    /// <seealso cref="EtpProtocolHandler" />
    public abstract class Etp12ProtocolHandler<TCapabilities, TCapabilitiesInterface, TCounterpartCapabilities, TCounterpartCapabilitiesInterface> : EtpProtocolHandler<TCapabilities, TCapabilitiesInterface, TCounterpartCapabilities, TCounterpartCapabilitiesInterface>, IEtp12ProtocolHandler
        where TCapabilities : Etp12ProtocolCapabilities, TCapabilitiesInterface, new()
        where TCapabilitiesInterface : class, IProtocolCapabilities
        where TCounterpartCapabilities : Etp12ProtocolCapabilities, TCounterpartCapabilitiesInterface, new()
        where TCounterpartCapabilitiesInterface : class, IProtocolCapabilities
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Etp12ProtocolHandler"/> class.
        /// </summary>
        /// <param name="protocol">The protocol.</param>
        /// <param name="role">This handler's role in the protocol.</param>
        /// <param name="counterpartRole">The role for this handler's counterpart in the protocol.</param>
        protected Etp12ProtocolHandler(int protocol, string role, string counterpartRole)
            : base(EtpVersion.v12, protocol, role, counterpartRole)
        {
        }

        /// <summary>
        /// Sends a complete multi-part set of response and ProtocolException messages to a customer.
        /// If there is no content for the positive response, an empty response message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <typeparam name="TBody">The response message body type.</typeparam>
        /// <typeparam name="TResponse">The response content type.</typeparam>
        /// <param name="responseMethod">The method to send the response message.</param>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="response">The response content.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the OpenChannelsResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<TBody> SendMapResponse<TBody, TResponse>(Func<IMessageHeader, IDictionary<string, TResponse>, bool, IMessageHeaderExtension, EtpMessage<TBody>> responseMethod, IMessageHeader correlatedHeader, IDictionary<string, TResponse> response, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
            where TBody : IEtpMessageBody
        {
            var message = responseMethod(correlatedHeader, response, /* isFinalPart: */ ((errors == null || errors.Count == 0) && setFinalPart), /* extension: */ responseExtension);
            if (message == null)
                return null;

            if (errors?.Count > 0)
            {
                var ret = ProtocolException(errors, correlatedHeader: correlatedHeader, setFinalPart: setFinalPart, extension: exceptionExtension);
                if (ret == null)
                    return null;
            }

            return message;
        }

        /// <summary>
        /// Sends a complete multi-part set of response and ProtocolException messages to a customer.
        /// If there is no content for the positive response, an empty response message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <typeparam name="TBody">The response message body type.</typeparam>
        /// <typeparam name="TResponse">The response content type.</typeparam>
        /// <typeparam name="TContext">The response context type.</typeparam>
        /// <param name="responseMethod">The method to send the response message.</param>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="response">The response content.</param>
        /// <param name="context">The response context.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the OpenChannelsResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<TBody> SendMapResponse<TBody, TResponse, TContext>(Func<IMessageHeader, IDictionary<string, TResponse>, TContext, bool, IMessageHeaderExtension, EtpMessage<TBody>> responseMethod, IMessageHeader correlatedHeader, IDictionary<string, TResponse> response, TContext context, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
            where TBody : IEtpMessageBody
        {
            var message = responseMethod(correlatedHeader, response, context, /* isFinalPart: */ ((errors == null || errors.Count == 0) && setFinalPart), /* extension: */ responseExtension);
            if (message == null)
                return null;

            if (errors?.Count > 0)
            {
                var ret = ProtocolException(errors, correlatedHeader: correlatedHeader, setFinalPart: setFinalPart, extension: exceptionExtension);
                if (ret == null)
                    return null;
            }

            return message;
        }
    }

    /// <summary>
    /// Provides default common functionality for ETP 1.2 protocol handlers.
    /// </summary>
    /// <seealso cref="EtpProtocolHandler" />
    public abstract class Etp12ProtocolHandler : Etp12ProtocolHandler<Etp12ProtocolCapabilities, IProtocolCapabilities, Etp12ProtocolCapabilities, IProtocolCapabilities>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Etp12ProtocolHandler{TCapabilities, TCapabilitiesInterface, TCounterpartCapabilities, TCounterpartCapabilitiesInterface}"/> class.
        /// </summary>
        /// <param name="protocol">The protocol.</param>
        /// <param name="role">This handler's role in the protocol.</param>
        /// <param name="counterpartRole">The role for this handler's counterpart in the protocol.</param>
        protected Etp12ProtocolHandler(int protocol, string role, string counterpartRole)
            : base(protocol, role, counterpartRole)
        {
        }
    }

    /// <summary>
    /// Provides default common functionality for ETP 1.2 protocol handlers.
    /// </summary>
    /// <seealso cref="EtpProtocolHandler" />
    public abstract class Etp12ProtocolHandlerWithCounterpartCapabilities<TCounterpartCapabilities, TCounterpartCapabilitiesInterface> : Etp12ProtocolHandler<Etp12ProtocolCapabilities, IProtocolCapabilities, TCounterpartCapabilities, TCounterpartCapabilitiesInterface>
        where TCounterpartCapabilities : Etp12ProtocolCapabilities, TCounterpartCapabilitiesInterface, new()
        where TCounterpartCapabilitiesInterface : class, IProtocolCapabilities
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Etp12ProtocolHandlerWithCounterpartCapabilities{TCounterpartCapabilities, TCounterpartCapabilitiesInterface}"/> class.
        /// </summary>
        /// <param name="protocol">The protocol.</param>
        /// <param name="role">This handler's role in the protocol.</param>
        /// <param name="counterpartRole">The role for this handler's counterpart in the protocol.</param>
        protected Etp12ProtocolHandlerWithCounterpartCapabilities(int protocol, string role, string counterpartRole)
            : base(protocol, role, counterpartRole)
        {
        }
    }

    /// <summary>
    /// Provides default common functionality for ETP 1.2 protocol handlers.
    /// </summary>
    /// <seealso cref="EtpProtocolHandler" />
    public abstract class Etp12ProtocolHandlerWithCapabilities<TCapabilities, TCapabilitiesInterface> : Etp12ProtocolHandler<TCapabilities, TCapabilitiesInterface, Etp12ProtocolCapabilities, IProtocolCapabilities>
        where TCapabilities : Etp12ProtocolCapabilities, TCapabilitiesInterface, new()
        where TCapabilitiesInterface : class, IProtocolCapabilities
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Etp12ProtocolHandlerWithCapabilities{TCapabilities, TCapabilitiesInterface}"/> class.
        /// </summary>
        /// <param name="protocol">The protocol.</param>
        /// <param name="role">This handler's role in the protocol.</param>
        /// <param name="Role">The role for this handler's  in the protocol.</param>
        protected Etp12ProtocolHandlerWithCapabilities(int protocol, string role, string Role)
            : base(protocol, role, Role)
        {
        }
    }
}
