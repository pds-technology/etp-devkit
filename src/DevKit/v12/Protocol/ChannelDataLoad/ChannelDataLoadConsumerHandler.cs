//----------------------------------------------------------------------- 
// ETP DevKit, 1.2
//
// Copyright 2018 Energistics
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
using Avro.IO;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v12.Datatypes;

namespace Energistics.Etp.v12.Protocol.ChannelDataLoad
{
    /// <summary>
    /// Base implementation of the <see cref="IChannelDataLoadConsumer"/> interface.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.EtpProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.ChannelDataLoad.IChannelDataLoadConsumer" />
    public class ChannelDataLoadConsumerHandler : EtpProtocolHandler, IChannelDataLoadConsumer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelDataLoadConsumerHandler"/> class.
        /// </summary>
        public ChannelDataLoadConsumerHandler() : base((int)Protocols.ChannelDataLoad, "consumer", "producer")
        {
        }

        /// <summary>
        /// Sends a OpenChannelResponse message to a store.
        /// </summary>
        /// <param name="uri">The channel URI.</param>
        /// <param name="id">The channel identifier.</param>
        /// <param name="uuid">The channel UUID.</param>
        /// <param name="lastIndex">The last index.</param>
        /// <param name="infill">if set to <c>true</c> provide infill data.</param>
        /// <param name="dataChange">if set to <c>true</c> provide channel data changes.</param>
        /// <returns>The message identifier.</returns>
        public virtual long OpenChannelResponse(string uri, long id, Guid uuid, object lastIndex = null, bool infill = true, bool dataChange = true)
        {
            var header = CreateMessageHeader(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.OpenChannelResponse);

            var message = new OpenChannelResponse
            {
                Uri = uri,
                Id = id,
                Uuid = new Uuid { Value = uuid.ToByteArray() },
                LastIndex = new IndexValue { Item = lastIndex },
                Infill = infill,
                DataChange = dataChange
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
        /// Handles the InfillRealtimeData event from a store.
        /// </summary>
        public event ProtocolEventHandler<InfillRealtimeData> OnInfillRealtimeData;

        /// <summary>
        /// Handles the ChannelDataChange event from a store.
        /// </summary>
        public event ProtocolEventHandler<ChannelDataChange> OnChannelDataChange;

        /// <summary>
        /// Decodes the message based on the message type contained in the specified <see cref="IMessageHeader" />.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="decoder">The message decoder.</param>
        /// <param name="body">The message body.</param>
        protected override void HandleMessage(IMessageHeader header, Decoder decoder, string body)
        {
            switch (header.MessageType)
            {
                case (int)MessageTypes.ChannelDataLoad.OpenChannel:
                    HandleOpenChannel(header, decoder.Decode<OpenChannel>(body));
                    break;

                case (int)MessageTypes.ChannelDataLoad.CloseChannel:
                    HandleCloseChannel(header, decoder.Decode<CloseChannel>(body));
                    break;

                case (int)MessageTypes.ChannelDataLoad.RealtimeData:
                    HandleRealtimeData(header, decoder.Decode<RealtimeData>(body));
                    break;

                case (int)MessageTypes.ChannelDataLoad.InfillRealtimeData:
                    HandleInfillRealtimeData(header, decoder.Decode<InfillRealtimeData>(body));
                    break;

                case (int)MessageTypes.ChannelDataLoad.ChannelDataChange:
                    HandleChannelDataChange(header, decoder.Decode<ChannelDataChange>(body));
                    break;

                default:
                    base.HandleMessage(header, decoder, body);
                    break;
            }
        }

        /// <summary>
        /// Handles the OpenChannel message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The OpenChannel message.</param>
        protected virtual void HandleOpenChannel(IMessageHeader header, OpenChannel message)
        {
            Notify(OnOpenChannel, header, message);
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
        /// Handles the InfillRealtimeData message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The InfillRealtimeData message.</param>
        protected virtual void HandleInfillRealtimeData(IMessageHeader header, InfillRealtimeData message)
        {
            Notify(OnInfillRealtimeData, header, message);
        }

        /// <summary>
        /// Handles the ChannelDataChange message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The ChannelDataChange message.</param>
        protected virtual void HandleChannelDataChange(IMessageHeader header, ChannelDataChange message)
        {
            Notify(OnChannelDataChange, header, message);
        }
    }
}
