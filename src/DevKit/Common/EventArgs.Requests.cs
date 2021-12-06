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
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Protocol.Core;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Provides a common base class for providing information about a request message that has been received.
    /// </summary>
    /// <typeparam name="TRequest">The type of the message body.</typeparam>
    /// <seealso cref="System.EventArgs" />
    public abstract class RequestEventArgsBase<TRequest> : EventArgs
        where TRequest : IEtpMessageBody
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyRequestEventArgs{TRequest}"/> class.
        /// </summary>
        /// <param name="request">The request message.</param>
        public RequestEventArgsBase(EtpMessage<TRequest> request)
        {
            Request = request;
        }

        /// <summary>
        /// Gets the request.
        /// </summary>
        /// <value>The request.</value>
        public EtpMessage<TRequest> Request { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the DevKit should send a response.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the DevKit should send a response; otherwise, <c>false</c>.
        /// </value>
        public bool SendResponse { get; set; } = true;

        /// <summary>
        /// Whether or not the response is multi-part.
        /// </summary>
        public virtual bool IsResponseMultiPart => false;

        /// <summary>
        /// Gets a value indicating whether there is a non-error response to send.
        /// </summary>
        public virtual bool HasNonErrorResponse => true;

        /// <summary>
        /// Whether or not there are any errors associated with the response to the request.
        /// </summary>
        public virtual bool HasErrors => HasFinalError || HasErrorMapErrors;

        /// <summary>
        /// Whether or not there are any errors in the error map associated with the response to the request.
        /// </summary>
        public virtual bool HasErrorMapErrors => ErrorMap?.Count > 0;

        /// <summary>
        /// Whether or not there is a final error to send in response to the request.
        /// </summary>
        public bool HasFinalError => FinalError != null;

        /// <summary>
        /// Gets or sets the errors.
        /// </summary>
        public Dictionary<string, IErrorInfo> ErrorMap { get; set; } = new Dictionary<string, IErrorInfo>();

        /// <summary>
        /// Gets the message header extension to use in the response, if any.
        /// </summary>
        public IMessageHeaderExtension ErrorMapExtension { get; set; }

        /// <summary>
        /// Gets or sets a final error to send in response to the request.
        /// </summary>
        public IErrorInfo FinalError { get; set; }

        /// <summary>
        /// Gets or sets a message header extension to send with the final error.
        /// </summary>
        public IMessageHeaderExtension FinalErrorExtension { get; set; }

        /// <summary>
        /// An optional action to execute after sending the full response including any errors.
        /// </summary>
        public Action PostResponseAction { get; set; }
    }

    /// <summary>
    /// Provides information about a request message that has been received that does not need any additional information in order to provide a response.
    /// Set <see cref="SendResponse"/> to <c>true</c> if the DevKit should send the response to the request.
    /// Set <see cref="SendResponse"/> to <c>false</c> if your code has already sent any necessary responses to the request.
    /// Set <see cref="ResponseExtension"/> to provide a message header extension that will be used for the response to this message.
    /// Set <see cref="Error"/> to provide a terminating protocol exception in response to the message.
    /// Set <see cref="ErrorExtension"/> to provide a message header extension that will be used for the terminating protocol exception.
    /// </summary>
    /// <typeparam name="TRequest">The type of the message body.</typeparam>
    /// <seealso cref="System.EventArgs" />
    public class EmptyRequestEventArgs<TRequest> : RequestEventArgsBase<TRequest>
        where TRequest : IEtpMessageBody
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyRequestEventArgs{TRequest}"/> class.
        /// </summary>
        /// <param name="request">The request message.</param>
        public EmptyRequestEventArgs(EtpMessage<TRequest> request)
            : base(request)
        {
        }

        /// <summary>
        /// Gets the message header extension to use in the response, if any.
        /// </summary>
        public IMessageHeaderExtension ResponseExtension { get; set; }
    }

    /// <summary>
    /// Provides information about a request message that has been received that does not generate a positive response but may generate an exception.
    /// Set <see cref="SendResponse"/> to <c>true</c> if the DevKit should send the response to the request.
    /// Set <see cref="SendResponse"/> to <c>false</c> if your code has already sent any necessary responses to the request.
    /// Set <see cref="Error"/> to provide a terminating protocol exception in response to the message.
    /// Set <see cref="ErrorExtension"/> to provide a message header extension that will be used for the terminating protocol exception.
    /// </summary>
    /// <typeparam name="TRequest">The type of the message body.</typeparam>
    /// <seealso cref="System.EventArgs" />
    public class VoidRequestEventArgs<TRequest> : RequestEventArgsBase<TRequest>
        where TRequest : IEtpMessageBody
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VoidRequestEventArgs{TRequest}"/> class.
        /// </summary>
        /// <param name="request">The request message.</param>
        public VoidRequestEventArgs(EtpMessage<TRequest> request)
            : base(request)
        {
        }

        /// <summary>
        /// Gets a value indicating whether there is a non-error response to send.
        /// </summary>
        public override bool HasNonErrorResponse => false;
    }

    /// <summary>
    /// Provides information about a cancellation request message that has been received.
    /// Set <see cref="SendResponse"/> to <c>true</c> if the DevKit should send the response to the request.
    /// Set <see cref="SendResponse"/> to <c>false</c> if your code has already sent any necessary responses to the request.
    /// Set <see cref="Error"/> to provide a terminating protocol exception in response to the message.
    /// Set <see cref="ErrorExtension"/> to provide a message header extension that will be used for the terminating protocol exception.
    /// </summary>
    /// <typeparam name="TRequest">The type of the original request message body.</typeparam>
    /// <typeparam name="TCancellation">The type of the cancellation request message body.</typeparam>
    /// <seealso cref="System.EventArgs" />
    public class CancellationRequestEventArgs<TRequest, TCancellation> : RequestEventArgsBase<TRequest>
        where TRequest : IEtpMessageBody where TCancellation : IEtpMessageBody
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancellationRequestEventArgs{TRequest, TCancellation}"/> class.
        /// </summary>
        /// <param name="request">The original request message.</param>
        /// <param name="cancellation">The cancellation request message.</param>
        public CancellationRequestEventArgs(EtpMessage<TRequest> request, EtpMessage<TCancellation> cancellation)
            : base(request)
        {
            Cancellation = cancellation;
        }

        /// <summary>
        /// Gets the cancellation request.
        /// </summary>
        /// <value>The cancellation request.</value>
        public EtpMessage<TCancellation> Cancellation { get; }

        /// <summary>
        /// Gets a value indicating whether there is a non-error response to send.
        /// </summary>
        public override bool HasNonErrorResponse => false;
    }

    /// <summary>
    /// Provides information about a request message that has been received and that produces a single response object and optionally collects information for the response.
    /// Set <see cref="SendResponse"/> to <c>true</c> if the DevKit should send the response to the request.
    /// Set <see cref="SendResponse"/> to <c>false</c> if your code has already sent any necessary responses to the request.
    /// Set <see cref="Response"/> to the content that should be sent in the response message.
    /// Set <see cref="ResponseExtension"/> to provide a message header extension that will be used for the response to this message.
    /// Set <see cref="Error"/> to provide a terminating protocol exception in response to the message.
    /// Set <see cref="ErrorExtension"/> to provide a message header extension that will be used for the terminating protocol exception.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request message body.</typeparam>
    /// <typeparam name="TResponse">The type of the response content to send in the response message.</typeparam>
    /// <seealso cref="System.EventArgs" />
    public class RequestEventArgs<TRequest, TResponse> : RequestEventArgsBase<TRequest>
        where TRequest : IEtpMessageBody
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestEventArgs{TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="request">The request message.</param>
        public RequestEventArgs(EtpMessage<TRequest> request)
            : base(request)
        {
        }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        public TResponse Response { get; set; }

        /// <summary>
        /// Gets the message header extension to use in the response, if any.
        /// </summary>
        public IMessageHeaderExtension ResponseExtension { get; set; }

        /// <summary>
        /// Gets a value indicating whether there is a non-error response to send.
        /// </summary>
        public override bool HasNonErrorResponse => Response != null;
    }


    /// <summary>
    /// Provides information about a request message that has been received and that produces a single response object and optionally collects information for the response.
    /// Set <see cref="SendResponse"/> to <c>true</c> if the DevKit should send the response to the request.
    /// Set <see cref="SendResponse"/> to <c>false</c> if your code has already sent any necessary responses to the request.
    /// Set <see cref="Response"/> to the content that should be sent in the response message.
    /// Set <see cref="ResponseExtension"/> to provide a message header extension that will be used for the response to this message.
    /// Set <see cref="Error"/> to provide a terminating protocol exception in response to the message.
    /// Set <see cref="ErrorExtension"/> to provide a message header extension that will be used for the terminating protocol exception.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request message body.</typeparam>
    /// <typeparam name="TResponse">The type of the response content to send in the response message.</typeparam>
    /// <typeparam name="TContext">Type type of additional context provided with the response.</typeparam>
    /// <seealso cref="System.EventArgs" />
    public class RequestWithContextEventArgs<TRequest, TResponse, TContext> : RequestEventArgs<TRequest, TResponse>
        where TRequest : IEtpMessageBody
        where TContext : class, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestWithContextEventArgs{TRequest, TResponse, TContext}"/> class.
        /// </summary>
        /// <param name="request">The request message.</param>
        public RequestWithContextEventArgs(EtpMessage<TRequest> request)
            : base(request)
        {
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        public TContext Context { get; } = new TContext();
    }

    /// <summary>
    /// Provides information about a request message that has been received and that produces a list of response objects and optionally collects information for the response.
    /// Set <see cref="SendResponse"/> to <c>true</c> if the DevKit should send the responses to the request.
    /// Set <see cref="SendResponse"/> to <c>false</c> if your code has already sent any necessary responses to the request.
    /// Add response content to the <see cref="Responses"/> list that should be sent in the response messages.
    /// Add header extensions that should be sent with the responses to the <see cref="ResponseExtensions"/> list.
    /// Set <see cref="Error"/> to provide a terminating protocol exception in response to the message.
    /// Set <see cref="ErrorExtension"/> to provide a message header extension that will be used for the terminating protocol exception.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request message body.</typeparam>
    /// <typeparam name="TResponse">The type of the response content to send in the response messages.</typeparam>
    public class ListRequestEventArgs<TRequest, TResponse> : RequestEventArgsBase<TRequest>
        where TRequest : IEtpMessageBody
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListRequestEventArgs{TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="request">The request message.</param>
        public ListRequestEventArgs(EtpMessage<TRequest> request)
            : base(request)
        {
        }

        /// <summary>
        /// Gets or sets the responses.
        /// </summary>
        public List<TResponse> Responses { get; set; } = new List<TResponse>();

        /// <summary>
        /// Gets or sets the message header extension to use in the response, if any.
        /// </summary>
        public IMessageHeaderExtension ResponseExtension { get; set; }

        /// <summary>
        /// Whether or not the response is multi-part.
        /// </summary>
        public override bool IsResponseMultiPart => true;
    }

    /// <summary>
    /// Provides information about a request message that has been received and that produces a list of response objects and optionally collects information for the response.
    /// Set <see cref="SendResponse"/> to <c>true</c> if the DevKit should send the responses to the request.
    /// Set <see cref="SendResponse"/> to <c>false</c> if your code has already sent any necessary responses to the request.
    /// Add response content to the <see cref="Responses"/> list that should be sent in the response messages.
    /// Add header extensions that should be sent with the responses to the <see cref="ResponseExtensions"/> list.
    /// Set <see cref="Error"/> to provide a terminating protocol exception in response to the message.
    /// Set <see cref="ErrorExtension"/> to provide a message header extension that will be used for the terminating protocol exception.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request message body.</typeparam>
    /// <typeparam name="TResponse">The type of the response content to send in the response messages.</typeparam>
    /// <typeparam name="TContext">Type type of additional context provided with the response.</typeparam>
    public class ListRequestWithContextEventArgs<TRequest, TResponse, TContext> : ListRequestEventArgs<TRequest, TResponse>
        where TRequest : IEtpMessageBody
        where TContext : class, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListRequestWithContextEventArgs{TRequest, TResponse, TContext}"/> class.
        /// </summary>
        /// <param name="request">The request message.</param>
        public ListRequestWithContextEventArgs(EtpMessage<TRequest> request)
            : base(request)
        {
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        public TContext Context { get; } = new TContext();
    }

    /// <summary>
    /// Provides information about a request message that has been received and that produces two lists of responses and optionally collects information for the response.
    /// Set <see cref="SendResponse"/> to <c>true</c> if the DevKit should send the responses to the request.
    /// Set <see cref="SendResponse"/> to <c>false</c> if your code has already sent any necessary responses to the request.
    /// Add response content to the <see cref="Responses1"/> and <see cref="Responses2"/> lists that should be sent in the response messages.
    /// Set <see cref="Response1Extension"/> and <see cref="Response2Extension"/> to provide a message header extensions that will be used for the responses to this message.
    /// Set <see cref="Error"/> to provide a terminating protocol exception in response to the message.
    /// Set <see cref="ErrorExtension"/> to provide a message header extension that will be used for the terminating protocol exception.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request message body.</typeparam>
    /// <typeparam name="TResponse1">The first type of response content to send in the response messages.</typeparam>
    /// <typeparam name="TResponse2">The second type of response content to send in the response messages.</typeparam>
    public class DualListRequestEventArgs<TRequest, TResponse1, TResponse2> : RequestEventArgsBase<TRequest>
        where TRequest : IEtpMessageBody
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DualListRequestEventArgs{TRequest, TResponse1, TResponse2}"/> class.
        /// </summary>
        /// <param name="request">The request message.</param>
        public DualListRequestEventArgs(EtpMessage<TRequest> request)
            : base(request)
        {
        }

        /// <summary>
        /// Gets or sets the responses.
        /// </summary>
        public List<TResponse1> Responses1 { get; set; } = new List<TResponse1>();

        /// <summary>
        /// Gets or sets the message header extension to use in the response, if any.
        /// </summary>
        public IMessageHeaderExtension Response1Extension { get; set; }

        /// <summary>
        /// Gets or sets the responses.
        /// </summary>
        public List<TResponse2> Responses2 { get; set; } = new List<TResponse2>();

        /// <summary>
        /// Gets or sets the message header extension to use in the response, if any.
        /// </summary>
        public IMessageHeaderExtension Response2Extension { get; set; }

        /// <summary>
        /// Whether or not the response is multi-part.
        /// </summary>
        public override bool IsResponseMultiPart => true;
    }

    /// <summary>
    /// Provides information about a request message that has been received and that produces two lists of responses and optionally collects information for the response.
    /// Set <see cref="SendResponse"/> to <c>true</c> if the DevKit should send the responses to the request.
    /// Set <see cref="SendResponse"/> to <c>false</c> if your code has already sent any necessary responses to the request.
    /// Add response content to the <see cref="Responses1"/> and <see cref="Responses2"/> lists that should be sent in the response messages.
    /// Set <see cref="Response1Extension"/> and <see cref="Response2Extension"/> to provide a message header extensions that will be used for the responses to this message.
    /// Set <see cref="Error"/> to provide a terminating protocol exception in response to the message.
    /// Set <see cref="ErrorExtension"/> to provide a message header extension that will be used for the terminating protocol exception.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request message body.</typeparam>
    /// <typeparam name="TResponse1">The first type of response content to send in the response messages.</typeparam>
    /// <typeparam name="TResponse2">The second type of response content to send in the response messages.</typeparam>
    /// <typeparam name="TContext">Type type of additional context provided with the response.</typeparam>
    public class DualListRequestWithContextEventArgs<TRequest, TResponse1, TResponse2, TContext> : RequestEventArgsBase<TRequest>
        where TRequest : IEtpMessageBody
        where TContext : class, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DualListRequestWithContextEventArgs{TRequest, TResponse1, TResponse2, TContext}"/> class.
        /// </summary>
        /// <param name="request">The request message.</param>
        public DualListRequestWithContextEventArgs(EtpMessage<TRequest> request)
            : base(request)
        {
        }

        /// <summary>
        /// Gets or sets the responses.
        /// </summary>
        public List<TResponse1> Responses1 { get; set; } = new List<TResponse1>();

        /// <summary>
        /// Gets or sets the message header extension to use in the response, if any.
        /// </summary>
        public IMessageHeaderExtension Response1Extension { get; set; }

        /// <summary>
        /// Gets or sets the responses.
        /// </summary>
        public List<TResponse2> Responses2 { get; set; } = new List<TResponse2>();

        /// <summary>
        /// Gets or sets the message header extensions to use with the second responses, if any.
        /// </summary>
        public List<IMessageHeaderExtension> Response2Extensions { get; set; }

        /// <summary>
        /// Whether or not the response is multi-part.
        /// </summary>
        public override bool IsResponseMultiPart => true;
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        public TContext Context { get; } = new TContext();
    }

    /// <summary>
    /// Provides information about a request message that has been received and that a list of response and a second, single response and optionally collects information for the response.
    /// Set <see cref="SendResponse"/> to <c>true</c> if the DevKit should send the responses to the request.
    /// Set <see cref="SendResponse"/> to <c>false</c> if your code has already sent any necessary responses to the request.
    /// Add response content to the <see cref="Responses1"/> list and set the <see cref="Response2"/> that should be sent in the response messages.
    /// Set <see cref="Response1Extension"/> and <see cref="Response2Extension"/> to provide a message header extensions that will be used for the responses to this message.
    /// Set <see cref="Error"/> to provide a terminating protocol exception in response to the message.
    /// Set <see cref="ErrorExtension"/> to provide a message header extension that will be used for the terminating protocol exception.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request message body.</typeparam>
    /// <typeparam name="TResponse1">The first type of response content to send in the response messages.</typeparam>
    /// <typeparam name="TResponse2">The second type of response content to send in the response messages.</typeparam>
    public class ListAndSingleRequestEventArgs<TRequest, TResponse1, TResponse2> : RequestEventArgsBase<TRequest>
        where TRequest : IEtpMessageBody
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListAndSingleRequestEventArgs{TRequest, TResponse1, TResponse2}"/> class.
        /// </summary>
        /// <param name="request">The request message.</param>
        public ListAndSingleRequestEventArgs(EtpMessage<TRequest> request)
            : base(request)
        {
        }

        /// <summary>
        /// Gets or sets the responses.
        /// </summary>
        public List<TResponse1> Responses1 { get; set; } = new List<TResponse1>();

        /// <summary>
        /// Gets or sets the message header extension to use in the response, if any.
        /// </summary>
        public IMessageHeaderExtension Response1Extension { get; set; }

        /// <summary>
        /// Gets or sets the responses.
        /// </summary>
        public TResponse2 Response2 { get; set; }

        /// <summary>
        /// Gets or sets the message header extension to use in the response, if any.
        /// </summary>
        public IMessageHeaderExtension Response2Extension { get; set; }

        /// <summary>
        /// Whether or not the response is multi-part.
        /// </summary>
        public override bool IsResponseMultiPart => true;
    }

    /// <summary>
    /// Provides information about a request message that has been received and that a list of response and a second, single response and optionally collects information for the response.
    /// Set <see cref="SendResponse"/> to <c>true</c> if the DevKit should send the responses to the request.
    /// Set <see cref="SendResponse"/> to <c>false</c> if your code has already sent any necessary responses to the request.
    /// Set the <see cref="Response2"/> and add response content to the <see cref="Responses2"/> list that should be sent in the response messages.
    /// Set <see cref="Response1Extension"/> and <see cref="Response2Extension"/> to provide a message header extensions that will be used for the responses to this message.
    /// Set <see cref="Error"/> to provide a terminating protocol exception in response to the message.
    /// Set <see cref="ErrorExtension"/> to provide a message header extension that will be used for the terminating protocol exception.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request message body.</typeparam>
    /// <typeparam name="TResponse1">The first type of response content to send in the response messages.</typeparam>
    /// <typeparam name="TResponse2">The second type of response content to send in the response messages.</typeparam>
    public class SingleAndListRequestEventArgs<TRequest, TResponse1, TResponse2> : RequestEventArgsBase<TRequest>
        where TRequest : IEtpMessageBody
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleAndListRequestEventArgs{TRequest, TResponse1, TResponse2}"/> class.
        /// </summary>
        /// <param name="request">The request message.</param>
        public SingleAndListRequestEventArgs(EtpMessage<TRequest> request)
            : base(request)
        {
        }

        /// <summary>
        /// Gets or sets the responses.
        /// </summary>
        public TResponse1 Response1 { get; set; }

        /// <summary>
        /// Gets or sets the message header extension to use in the response, if any.
        /// </summary>
        public IMessageHeaderExtension Response1Extension { get; set; }

        /// <summary>
        /// Gets or sets the responses.
        /// </summary>
        public List<TResponse2> Responses2 { get; set; } = new List<TResponse2>();

        /// <summary>
        /// Gets or sets the message header extension to use in the response, if any.
        /// </summary>
        public IMessageHeaderExtension Response2Extension { get; set; }

        /// <summary>
        /// Whether or not the response is multi-part.
        /// </summary>
        public override bool IsResponseMultiPart => true;
    }

    /// <summary>
    /// Provides information about a map request message that has been received and that produces a map response and optionally collects information for the response.
    /// Set <see cref="SendResponse"/> to <c>true</c> if the DevKit should send the responses to the request.
    /// Set <see cref="SendResponse"/> to <c>false</c> if your code has already sent any necessary responses to the request.
    /// Add response content to the <see cref="ResponseMap"/> dictionary that should be sent in the map response message.
    /// Set <see cref="ResponseMapExtension"/> to provide a message header extension that will be used for the map response message.
    /// Add errors to the <see cref="ErrorMap"/> dictionary that should be sent in the map protocol exception message.
    /// Set <see cref="ErrorMapExtension"/> to provide a message header extension that will be used for the map protocol exception message.
    /// Set <see cref="Error"/> to provide a terminating protocol exception in response to the message.
    /// Set <see cref="ErrorExtension"/> to provide a message header extension that will be used for the terminating protocol exception.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request message body.</typeparam>
    /// <typeparam name="TResponse">The type of the response content to the message.</typeparam>
    public class MapRequestEventArgs<TRequest, TResponse> : RequestEventArgsBase<TRequest>
        where TRequest : IEtpMessageBody
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapRequestEventArgs{TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="request">The request message.</param>
        public MapRequestEventArgs(EtpMessage<TRequest> request)
            : base(request)
        {
        }

        /// <summary>
        /// Gets or sets the responses.
        /// </summary>
        public Dictionary<string, TResponse> ResponseMap { get; set; } = new Dictionary<string, TResponse>();

        /// <summary>
        /// Gets or sets the message header extension to use in the response, if any.
        /// </summary>
        public IMessageHeaderExtension ResponseMapExtension { get; set; }

        /// <summary>
        /// Whether or not the response is multi-part.
        /// </summary>
        public override bool IsResponseMultiPart => true;
    }

    /// <summary>
    /// Provides information about a map request message that has been received and that produces a map response and optionally collects information for the response.
    /// Set <see cref="SendResponse"/> to <c>true</c> if the DevKit should send the responses to the request.
    /// Set <see cref="SendResponse"/> to <c>false</c> if your code has already sent any necessary responses to the request.
    /// Add response content to the <see cref="ResponseMap"/> dictionary that should be sent in the map response message.
    /// Set <see cref="ResponseMapExtension"/> to provide a message header extension that will be used for the map response message.
    /// Add errors to the <see cref="ErrorMap"/> dictionary that should be sent in the map protocol exception message.
    /// Set <see cref="ErrorMapExtension"/> to provide a message header extension that will be used for the map protocol exception message.
    /// Set <see cref="Error"/> to provide a terminating protocol exception in response to the message.
    /// Set <see cref="ErrorExtension"/> to provide a message header extension that will be used for the terminating protocol exception.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request message body.</typeparam>
    /// <typeparam name="TResponse">The type of the response content to the message.</typeparam>
    /// <typeparam name="TContext">Type type of additional context provided with the map response.</typeparam>
    public class MapRequestWithContextEventArgs<TRequest, TResponse, TContext> : MapRequestEventArgs<TRequest, TResponse>
        where TRequest : IEtpMessageBody
        where TContext : class, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapRequestWithContextEventArgs{TRequest, TResponse, TContext}"/> class.
        /// </summary>
        /// <param name="request">The request message.</param>
        public MapRequestWithContextEventArgs(EtpMessage<TRequest> request)
            : base(request)
        {
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        public TContext Context { get; } = new TContext();
    }

    /// <summary>
    /// Provides information about a map request message that has been received and that produces a map response and optionally collects information for the response.
    /// Set <see cref="SendResponse"/> to <c>true</c> if the DevKit should send the responses to the request.
    /// Set <see cref="SendResponse"/> to <c>false</c> if your code has already sent any necessary responses to the request.
    /// Add response content to the <see cref="ResponseMap"/> dictionary that should be sent in the map response message.
    /// Set <see cref="ResponseMapExtension"/> to provide a message header extension that will be used for the map response message.
    /// Add errors to the <see cref="ErrorMap"/> dictionary that should be sent in the map protocol exception message.
    /// Set <see cref="ErrorMapExtension"/> to provide a message header extension that will be used for the map protocol exception message.
    /// Set <see cref="Error"/> to provide a terminating protocol exception in response to the message.
    /// Set <see cref="ErrorExtension"/> to provide a message header extension that will be used for the terminating protocol exception.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request message body.</typeparam>
    /// <typeparam name="TData">Type type of the request data message body.</typeparam>
    /// <typeparam name="TResponse">The type of the response content to the message.</typeparam>
    public class MapRequestWithDataEventArgs<TRequest, TData, TResponse> : MapRequestEventArgs<TRequest, TResponse>
        where TRequest : IEtpMessageBody
        where TData : IEtpMessageBody
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapRequestWithDataEventArgs{TRequest, TData, TResponse}"/> class.
        /// </summary>
        /// <param name="request">The request message.</param>
        /// <param name="data">The request data.</param>
        public MapRequestWithDataEventArgs(EtpMessage<TRequest> request, EtpMessage<TData> data)
            : base(request)
        {
            Data = data;
        }

        /// <summary>
        /// Gets the request data.
        /// </summary>
        public EtpMessage<TData> Data { get; }
    }

    /// <summary>
    /// Provides information about a map request message that has been received and that produces a map response and optional list response and optionally collects information for the response.
    /// Set <see cref="SendResponse"/> to <c>true</c> if the DevKit should send the responses to the request.
    /// Set <see cref="SendResponse"/> to <c>false</c> if your code has already sent any necessary responses to the request.
    /// Add response content to the <see cref="Response1Map"/> dictionary and the <see cref="Responses2"/> list that should be sent in the response messages.
    /// Set <see cref="Response1MapExtension"/> and <see cref="Response2Extension"/> to provide a message header extensions that will be used for the responses to this message.
    /// Add errors to the <see cref="ErrorMap"/> dictionary that should be sent in the map protocol exception message.
    /// Set <see cref="ErrorMapExtension"/> to provide a message header extension that will be used for the map protocol exception message.
    /// Set <see cref="Error"/> to provide a terminating protocol exception in response to the message.
    /// Set <see cref="ErrorExtension"/> to provide a message header extension that will be used for the terminating protocol exception.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request message body.</typeparam>
    /// <typeparam name="TResponse1">The type of the first response content to the message.</typeparam>
    /// <typeparam name="TResponse2">The type of the second response content to the message.</typeparam>
    public class MapAndListRequestEventArgs<TRequest, TResponse1, TResponse2> : RequestEventArgsBase<TRequest>
        where TRequest : IEtpMessageBody
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapAndListRequestEventArgs{TRequest, TResponse1, TResponse2}"/> class.
        /// </summary>
        /// <param name="request">The request message.</param>
        public MapAndListRequestEventArgs(EtpMessage<TRequest> request)
            : base(request)
        {
        }

        /// <summary>
        /// Gets or sets the responses.
        /// </summary>
        public Dictionary<string, TResponse1> Response1Map { get; set; } = new Dictionary<string, TResponse1>();

        /// <summary>
        /// Gets or sets the message header extension to use in the response, if any.
        /// </summary>
        public IMessageHeaderExtension Response1MapExtension { get; set; }

        /// <summary>
        /// Gets or sets the second responses.
        /// </summary>
        public List<TResponse2> Responses2 { get; set; }

        /// <summary>
        /// Gets or sets the message header extensions to use with the second responses, if any.
        /// </summary>
        public List<IMessageHeaderExtension> Response2Extensions { get; set; }

        /// <summary>
        /// Whether or not the response is multi-part.
        /// </summary>
        public override bool IsResponseMultiPart => true;
    }
}
