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
using Avro.Specific;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Protocol.Core;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Provides a common base class for providing information about a response message that has been received.
    /// </summary>
    /// <typeparam name="TRequest">The type of the message body for the for the original request that the message is in response to.</typeparam>
    /// <seealso cref="System.EventArgs" />
    public abstract class ResponseEventArgsBase<TRequest> : EventArgs where TRequest : ISpecificRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseEventArgsBase{TRequest}"/> class.
        /// </summary>
        /// <param name="request">The original request message that the message is in response to.</param>
        /// <param name="exception">The protocol exception received in response to the request, if any.</param>
        public ResponseEventArgsBase(EtpMessage<TRequest> request, EtpMessage<IProtocolException> exception)
        {
            Request = request;
            Exception = exception;
        }

        /// <summary>
        /// Gets the original request this message is a response to.
        /// </summary>
        /// <value>The request message.</value>
        public EtpMessage<TRequest> Request { get; }

        /// <summary>
        /// Gets the protocol exception received in response to the request, if any.
        /// </summary>
        public EtpMessage<IProtocolException> Exception { get; }
    }

    /// <summary>
    /// Provides data for protocol handler events for exceptions received in response to requests that do not have a positive response message.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request message body.</typeparam>
    /// <seealso cref="System.EventArgs" />
    public class VoidResponseEventArgs<TRequest> : ResponseEventArgsBase<TRequest> where TRequest : ISpecificRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseEventArgs{TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="request">The original request message.</param>
        /// <param name="exception">The protocol exception.</param>
        public VoidResponseEventArgs(EtpMessage<TRequest> request, EtpMessage<IProtocolException> exception)
            : base(request, exception)
        {
        }
    }

    /// <summary>
    /// Provides data for protocol handler events.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request message body.</typeparam>
    /// <typeparam name="TResponse">The type of the response message body.</typeparam>
    /// <seealso cref="System.EventArgs" />
    public class ResponseEventArgs<TRequest, TResponse> : ResponseEventArgsBase<TRequest> where TRequest : ISpecificRecord where TResponse : ISpecificRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseEventArgs{TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="request">The original request message.</param>
        /// <param name="exception">The protocol exception.</param>
        public ResponseEventArgs(EtpMessage<TRequest> request, EtpMessage<TResponse> response)
            : base(request, null)
        {
            Response = response;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseEventArgs{TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="request">The original request message.</param>
        /// <param name="exception">The protocol exception.</param>
        public ResponseEventArgs(EtpMessage<TRequest> request, EtpMessage<IProtocolException> exception)
            : base(request, exception)
        {
        }

        /// <summary>
        /// Gets the response message.
        /// </summary>
        /// <value>The response message.</value>
        public EtpMessage<TResponse> Response { get; }
    }

    /// <summary>
    /// Provides data for protocol handler events.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request message.</typeparam>
    /// <typeparam name="TResponse1">The first type of the response message body.</typeparam>
    /// <typeparam name="TResponse2">The second type of the response message body.</typeparam>
    /// <seealso cref="System.EventArgs" />
    public class DualResponseEventArgs<TRequest, TResponse1, TResponse2> : ResponseEventArgsBase<TRequest> where TResponse1 : ISpecificRecord where TResponse2 : ISpecificRecord where TRequest : ISpecificRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DualResponseEventArgs{TRequest, TResponse1, TResponse2}"/> class.
        /// </summary>
        /// <param name="request">The original request message.</param>
        /// <param name="response1">The first type of response message.</param>
        /// <param name="response1">The second type of response message.</param>
        /// <param name="exception">The protocol exception.</param>
        public DualResponseEventArgs(EtpMessage<TRequest> request, EtpMessage<TResponse1> response1)
            : base(request, null)
        {
            Response1 = response1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DualResponseEventArgs{TRequest, TResponse1, TResponse2}"/> class.
        /// </summary>
        /// <param name="request">The original request message.</param>
        /// <param name="response1">The first type of response message.</param>
        /// <param name="response1">The second type of response message.</param>
        /// <param name="exception">The protocol exception.</param>
        public DualResponseEventArgs(EtpMessage<TRequest> request, EtpMessage<TResponse2> response2)
            : base(request, null)
        {
            Response2 = response2;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DualResponseEventArgs{TRequest, TResponse1, TResponse2}"/> class.
        /// </summary>
        /// <param name="request">The original request message.</param>
        /// <param name="response1">The first type of response message.</param>
        /// <param name="response1">The second type of response message.</param>
        /// <param name="exception">The protocol exception.</param>
        public DualResponseEventArgs(EtpMessage<TRequest> request, EtpMessage<IProtocolException> exception)
            : base(request, exception)
        {
        }

        /// <summary>
        /// Gets the first type of response message.
        /// </summary>
        /// <value>The message.</value>
        public EtpMessage<TResponse1> Response1 { get; }

        /// <summary>
        /// Gets the second type of response message.
        /// </summary>
        /// <value>The message.</value>
        public EtpMessage<TResponse2> Response2 { get; }
    }
}
