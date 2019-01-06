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

using System.Collections.Generic;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v12.Datatypes;
using Energistics.Etp.v12.Datatypes.ChannelData;

namespace Energistics.Etp.v12.Protocol.ChannelDataLoad
{
    /// <summary>
    /// Base implementation of the <see cref="IChannelDataLoadConsumer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.ChannelDataLoad.IChannelDataLoadConsumer" />
    public class ChannelDataLoadConsumerHandler : Etp12ProtocolHandler, IChannelDataLoadConsumer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelDataLoadConsumerHandler"/> class.
        /// </summary>
        public ChannelDataLoadConsumerHandler() : base((int)Protocols.ChannelDataLoad, "consumer", "producer")
        {
            RegisterMessageHandler<OpenChannel>(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.OpenChannel, HandleOpenChannel);
            RegisterMessageHandler<CloseChannel>(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.CloseChannel, HandleCloseChannel);
            RegisterMessageHandler<RealtimeData>(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.RealtimeData, HandleRealtimeData);
            RegisterMessageHandler<InfillData>(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.InfillData, HandleInfillData);
            RegisterMessageHandler<ChangedData>(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.ChangedData, HandleChangedData);
        }

        /// <summary>
        /// Sends a OpenChannelResponse message to a store.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="channels">The channels.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        public virtual long OpenChannelResponse(IMessageHeader request, IList<OpenChannelInfo> channels, IList<ErrorInfo> errors, MessageFlags messageFlag = MessageFlags.MultiPartAndFinalPart)
        {
            var header = CreateMessageHeader(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.OpenChannelResponse, request.MessageId, messageFlag);

            var message = new OpenChannelResponse
            {
                Channels = channels ?? new List<OpenChannelInfo>(),
                Errors = errors ?? new List<ErrorInfo>()
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Handles the OpenChannel event from a store.
        /// </summary>
        public event ProtocolEventHandler<OpenChannel> OnOpenChannel;

        /// <summary>
        /// Handles the CloseChannel event from a store.
        /// </summary>
        public event ProtocolEventHandler<CloseChannel> OnCloseChannel;

        /// <summary>
        /// Handles the RealtimeData event from a store.
        /// </summary>
        public event ProtocolEventHandler<RealtimeData> OnRealtimeData;

        /// <summary>
        /// Handles the InfillData event from a store.
        /// </summary>
        public event ProtocolEventHandler<InfillData> OnInfillData;

        /// <summary>
        /// Handles the ChangedData event from a store.
        /// </summary>
        public event ProtocolEventHandler<ChangedData> OnChangedData;

        /// <summary>
        /// Handles the OpenChannel message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The OpenChannel message.</param>
        protected virtual void HandleOpenChannel(IMessageHeader header, OpenChannel message)
        {
            var args = Notify(OnOpenChannel, header, message);
            var channels = new List<OpenChannelInfo>();
            var errors = new List<ErrorInfo>();

            HandleOpenChannel(args, channels, errors);

            if (!args.Cancel)
            {
                OpenChannelResponse(header, channels, errors);
            }
        }

        /// <summary>
        /// Handles the OpenChannel message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="ProtocolEventArgs{OpenChannel}"/> instance containing the event data.</param>
        /// <param name="channels">The channels.</param>
        /// <param name="errors">The errors.</param>
        protected virtual void HandleOpenChannel(ProtocolEventArgs<OpenChannel> args, List<OpenChannelInfo> channels, List<ErrorInfo> errors)
        {
        }

        /// <summary>
        /// Handles the CloseChannel message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The CloseChannel message.</param>
        protected virtual void HandleCloseChannel(IMessageHeader header, CloseChannel message)
        {
            Notify(OnCloseChannel, header, message);
        }

        /// <summary>
        /// Handles the RealtimeData message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The RealtimeData message.</param>
        protected virtual void HandleRealtimeData(IMessageHeader header, RealtimeData message)
        {
            Notify(OnRealtimeData, header, message);
        }

        /// <summary>
        /// Handles the InfillData message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The InfillData message.</param>
        protected virtual void HandleInfillData(IMessageHeader header, InfillData message)
        {
            Notify(OnInfillData, header, message);
        }

        /// <summary>
        /// Handles the ChangedData message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The ChangedData message.</param>
        protected virtual void HandleChangedData(IMessageHeader header, ChangedData message)
        {
            Notify(OnChangedData, header, message);
        }
    }
}
