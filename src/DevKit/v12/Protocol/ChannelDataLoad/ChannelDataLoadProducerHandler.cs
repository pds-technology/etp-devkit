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
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.ChannelDataLoad
{
    /// <summary>
    /// Base implementation of the <see cref="IChannelDataLoadProducer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.ChannelDataLoad.IChannelDataLoadProducer" />
    public class ChannelDataLoadProducerHandler : Etp12ProtocolHandler, IChannelDataLoadProducer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelDataLoadProducerHandler"/> class.
        /// </summary>
        public ChannelDataLoadProducerHandler() : base((int)Protocols.ChannelDataLoad, "producer", "consumer")
        {
            RegisterMessageHandler<OpenChannelResponse>(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.OpenChannelResponse, HandleOpenChannelResponse);
        }

        /// <summary>
        /// Sends a OpenChannel message to a store.
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <returns>The message identifier.</returns>
        public virtual long OpenChannel(IList<ChannelMetadataRecord> channels)
        {
            var header = CreateMessageHeader(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.OpenChannel);

            var message = new OpenChannel
            {
                Channels = channels
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Sends a CloseChannel message to a store.
        /// </summary>
        /// <param name="id">The channel identifier.</param>
        /// <param name="reason">The close reason.</param>
        /// <returns>The message identifier.</returns>
        public virtual long CloseChannel(long id, string reason)
        {
            var header = CreateMessageHeader(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.CloseChannel);

            var message = new CloseChannel
            {
                Id = id,
                CloseReason = reason
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Sends a RealtimeData message to a store.
        /// </summary>
        /// <param name="dataItems">The data items.</param>
        /// <returns>The message identifier.</returns>
        public virtual long RealtimeData(IList<DataItem> dataItems)
        {
            var header = CreateMessageHeader(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.RealtimeData);

            var message = new RealtimeData
            {
                Data = dataItems
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Sends a InfillData message to a store.
        /// </summary>
        /// <param name="dataItems">The data items.</param>
        /// <returns>The message identifier.</returns>
        public virtual long InfillData(IList<DataItem> dataItems)
        {
            var header = CreateMessageHeader(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.InfillData);

            var message = new InfillData
            {
                Data = dataItems
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Sends a ChangedData message to a store.
        /// </summary>
        /// <param name="id">The channel identifier.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="depthDatum">The depth datum.</param>
        /// <param name="uom">The unit of measure.</param>
        /// <param name="dataItems">The data items.</param>
        /// <returns>The message identifier.</returns>
        public virtual long ChangedData(long id, object startIndex, object endIndex, string depthDatum, string uom, IList<DataItem> dataItems)
        {
            var header = CreateMessageHeader(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.ChangedData);

            var message = new ChangedData
            {
                ChangedInterval = new IndexInterval
                {
                    StartIndex = new IndexValue { Item = startIndex },
                    EndIndex = new IndexValue { Item = endIndex },
                    DepthDatum = depthDatum,
                    Uom = uom
                },
                Data = dataItems
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Handles the OpenChannelResponse event from a store.
        /// </summary>
        public event ProtocolEventHandler<OpenChannelResponse> OnOpenChannelResponse;

        /// <summary>
        /// Handles the OpenChannelResponse message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The OpenChannelResponse message.</param>
        protected virtual void HandleOpenChannelResponse(IMessageHeader header, OpenChannelResponse message)
        {
            var args = Notify(OnOpenChannelResponse, header, message);
            HandleOpenChannelResponse(args);
        }

        /// <summary>
        /// Handles the OpenChannelResponse message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="ProtocolEventArgs{OpenChannelResponse}"/> instance containing the event data.</param>
        protected virtual void HandleOpenChannelResponse(ProtocolEventArgs<OpenChannelResponse> args)
        {
        }
    }
}
