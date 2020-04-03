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

using Avro.Specific;
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
    public abstract class Etp12ProtocolHandler : EtpProtocolHandler, IEtp12ProtocolHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Etp12ProtocolHandler"/> class.
        /// </summary>
        /// <param name="protocol">The protocol.</param>
        /// <param name="role">The role.</param>
        /// <param name="requestedRole">The requested role.</param>
        protected Etp12ProtocolHandler(int protocol, string role, string requestedRole)
            : base(EtpVersion.v12, protocol, role, requestedRole)
        {
        }

        /// <summary>
        /// Creates a new <see cref="ErrorInfo"/> instance.
        /// </summary>
        /// <returns>The <see cref="ErrorInfo"/> instance.</returns>
        public ErrorInfo ErrorInfo()
        {
            return new ErrorInfo();
        }

        /// <summary>
        /// Convers a <see cref="IErrorInfo"/> instance to an <see cref="ErrorInfo"/> instance.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> to convert to a <see cref="ErrorInfo"/>.</param>
        /// <returns>The <see cref="ErrorInfo"/> instance.</returns>
        public ErrorInfo ErrorInfo(IErrorInfo errorInfo)
        {
            return (errorInfo as ErrorInfo) ?? new ErrorInfo { Code = errorInfo?.Code ?? 0, Message = errorInfo?.Message };
        }

        /// <summary>
        /// Sends an ETP 1.2 multipart response for item maps including possible errors.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message being sent.</typeparam>
        /// <typeparam name="TItem">The type of map item.</typeparam>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="items">The items to send.</param>
        /// <param name="errors">The errors to send, if any.</param>
        /// <param name="setItems">The action to use to update the message with the specified items.</param>
        protected long SendMultipartResponse<TMessage, TItem>(IMessageHeader header, TMessage message, IDictionary<string, TItem> items, IDictionary<string, ErrorInfo> errors, Action<TMessage, IDictionary<string, TItem>> setItems)
            where TMessage : ISpecificRecord
        {
            header.MessageFlags = (int)MessageFlags.MultiPart;

            var messageId = 0L;

            if (items.Count == 0)
            {
                var isFinal = errors.Count == 0;
                if (isFinal)
                    header.MessageFlags = (int)MessageFlags.MultiPartAndFinalPart;

                setItems(message, new Dictionary<string, TItem>());

                messageId = Session.SendMessage(header, message);
            }

            var count = 0;
            foreach (var kvp in items)
            {
                var isFinal = errors.Count == 0 && count == items.Count - 1;
                if (isFinal)
                    header.MessageFlags = (int)MessageFlags.MultiPartAndFinalPart;

                count++;

                setItems(message, new Dictionary<string, TItem> { [kvp.Key] = kvp.Value });

                messageId = Session.SendMessage(header, message);
            }

            header.MessageType = Convert.ToInt32(MessageTypes.Core.ProtocolException);

            var exception = new Core.ProtocolException();

            count = 0;
            foreach (var kvp in errors)
            {
                var isFinal = count == errors.Count - 1;
                if (isFinal)
                    header.MessageFlags = (int)MessageFlags.MultiPartAndFinalPart;

                count++;

                exception.Errors = new Dictionary<string, ErrorInfo> { [kvp.Key] = kvp.Value };

                messageId = Session.SendMessage(header, exception);
            }

            return messageId;
        }

        /// <summary>
        /// Sends an ETP 1.2 multipart response for a list of items.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message being sent.</typeparam>
        /// <typeparam name="TItem">The type of item.</typeparam>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="items">The items to send.</param>
        /// <param name="setItems">The action to use to update the message with the specified items.</param>
        protected long SendMultipartResponse<TMessage, TItem>(IMessageHeader header, TMessage message, IList<TItem> items, Action<TMessage, IList<TItem>> setItems)
            where TMessage : ISpecificRecord
        {
            header.MessageFlags = (int)MessageFlags.MultiPart;

            var messageId = 0L;

            if (items.Count == 0)
            {
                header.MessageFlags = (int)MessageFlags.MultiPartAndFinalPart;

                setItems(message, new List<TItem>());

                messageId = Session.SendMessage(header, message);
            }

            var count = 0;
            for (int i = 0; i < items.Count; i++)
            {
                var isFinal = i == items.Count - 1;
                if (isFinal)
                    header.MessageFlags = (int)MessageFlags.MultiPartAndFinalPart;

                count++;

                setItems(message, new List<TItem> { items[i] });

                messageId = Session.SendMessage(header, message);
            }

            return messageId;
        }

        /// <summary>
        /// Sends an ETP 1.2 multipart response for item maps including possible errors.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message being sent.</typeparam>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="errors">The errors to send, if any.</param>
        protected long SendMultipartResponse<TMessage>(IMessageHeader header, TMessage message, IDictionary<string, ErrorInfo> errors)
            where TMessage : ISpecificRecord
        {
            header.MessageFlags = (int)MessageFlags.MultiPart;

            var messageId = 0L;

            header.MessageType = Convert.ToInt32(MessageTypes.Core.ProtocolException);

            var exception = new Core.ProtocolException();

            var count = 0;
            foreach (var kvp in errors)
            {
                var isFinal = count == errors.Count - 1;
                if (isFinal)
                    header.MessageFlags = (int)MessageFlags.MultiPartAndFinalPart;

                count++;

                exception.Errors = new Dictionary<string, ErrorInfo> { [kvp.Key] = kvp.Value };

                messageId = Session.SendMessage(header, exception);
            }

            return messageId;
        }
    }
}
