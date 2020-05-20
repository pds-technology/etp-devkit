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
using System.Security.Cryptography;
using Energistics.Etp.Common.Datatypes;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Provides common functionality for ETP protocol capabilities.
    /// </summary>
    public class EtpProtocolCapabilities
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EtpProtocolCapabilities"/> class.
        /// </summary>
        public EtpProtocolCapabilities()
        {
            Capabilities = new Dictionary<string, IDataValue>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpProtocolCapabilities"/> class.
        /// </summary>
        /// <param name="capabilities">The protocol capabilities.</param>
        public EtpProtocolCapabilities(System.Collections.IDictionary capabilities)
        {
            Capabilities = capabilities;
        }

        /// <summary>
        /// The protocol capabilities
        /// </summary>
        public System.Collections.IDictionary Capabilities { get; set; }

        /// <summary>
        /// The protocol capabilities as a data value dictionary.
        /// </summary>
        /// <typeparam name="TDataValue">The data value type.</typeparam>
        /// <returns>The capabilities as a data value dictionary.</returns>
        public Dictionary<string, TDataValue> AsDataValueDictionary<TDataValue>() where TDataValue : IDataValue, new()
        {
            var dictionary = new Dictionary<string, TDataValue>();
            foreach (var key in Capabilities.Keys)
            {
                dictionary[(string)key] = new TDataValue { Item = ((IDataValue)Capabilities[key]).Item };
            }

            return dictionary;
        }

        /// <summary>
        /// Indicates the producer does not accept requests to stream individual channels but always sends all of its channels.
        /// </summary>
        public bool? SimpleStreamer
        {
            get { return TryGetValue<bool>("SimpleStreamer"); }
            set { SetValue("SimpleStreamer", value); }
        }

        /// <summary>
        /// Sets limits on maximum indexCount (number of indexes "back" from the current index that a producer will provide) for StreamingStartIndex.
        /// Integer value between 1 and the max number or indexes a producer will provide at the start of streaming.Recommended max value = 100.
        /// </summary>
        public long? MaxIndexCount
        {
            get { return TryGetValue<long>("MaxIndexCount"); }
            set { SetValue("MaxIndexCount", value); }
        }

        /// <summary>
        /// Maximum number of data points returned in each message.
        /// Consumer advises the producer of its maxdataitemcount.  Producer uses it when it sends those messages.
        /// </summary>
        public long? MaxDataItemCount
        {
            get { return TryGetValue<long>("MaxDataItemCount"); }
            set { SetValue("MaxDataItemCount", value); }
        }

        /// <summary>
        /// Indicates to a customer the maximum number of response messages a store will return. Integer value of [max message count].
        /// </summary>
        public long? MaxResponseCount
        {
            get { return TryGetValue<long>("MaxResponseCount"); }
            set { SetValue("MaxResponseCount", value); }
        }

        /// <summary>
        /// Indicates the maximum delay in a store before it sends a change notification.
        /// Integer value of [number of seconds].  The maximum value for this variable is 600 seconds.
        /// </summary>
        public long? ChangeDetectionPeriod
        {
            get { return TryGetValue<long>("ChangeDetectionPeriod"); }
            set { SetValue("ChangeDetectionPeriod", value); }
        }

        /// <summary>
        /// Indicates the the maximum amount of time that a store retains change information about a data object.
        /// The name of the variable is ChangeNotificationRetentionPeriod and it MUST have an integer value of [number of seconds].  The minimum value for this variable is 3600 seconds.
        /// </summary>
        public long? ChangeNotificationRetentionPeriod
        {
            get { return TryGetValue<long>("ChangeNotificationRetentionPeriod"); }
            set { SetValue("ChangeNotificationRetentionPeriod", value); }
        }

        /// <summary>
        /// Indicates the amount of time that a store must retain the ID of a deleted record. Minimum value is 24 hours, and the store must be able to report its start time.
        /// </summary>
        public long? DeleteNotificationRetentionPeriod
        {
            get { return TryGetValue<long>("DeleteNotificationRetentionPeriod"); }
            set { SetValue("DeleteNotificationRetentionPeriod", value); }
        }

        /// <summary>
        /// Integer value of [number of seconds].  Indicates the maximum time a server allows to process a transaction.
        /// </summary>
        public long? TransactionTimeout
        {
            get { return TryGetValue<long>("TransactionTimeout"); }
            set { SetValue("TransactionTimeout", value); }
        }

        /// <summary>
        /// Indicates the maximum time a producer allows no streaming data to occur before setting the channelStatus to 'inactive'. Integer value of [number of seconds].
        /// </summary>
        public long? StreamingTimeoutPeriod
        {
            get { return TryGetValue<long>("StreamingTimeoutPeriod"); }
            set { SetValue("StreamingTimeoutPeriod", value); }
        }

        /// <summary>
        /// Indicates the maximum time a server allows no data points to be added to the growing part of a growing object before setting the growingStatus to 'inactive'. Integer value of [number of seconds].
        /// </summary>
        public long? GrowingTimeoutPeriod
        {
            get { return TryGetValue<long>("GrowingTimeoutPeriod"); }
            set { SetValue("GrowingTimeoutPeriod", value); }
        }

        /// <summary>
        /// This is the largest data array size the store can get or put. A store can optionally specify this for protocols that handle data arrays.  Property of numberofbytes.
        /// </summary>
        public long? MaxDataArraySize
        {
            get { return TryGetValue<long>("MaxDataArraySize"); }
            set { SetValue("MaxDataArraySize", value); }
        }

        /// <summary>
        /// The maximum allowed time for updates to a channel to be visible in ChannelDataFrame (Protocol 2).
        /// Updates to channels are not guarenteed to be visible in response in less than this period.
        /// </summary>
        public long? FrameChangeDetectionPeriod
        {
            get { return TryGetValue<long>("FrameChangeDetectionPeriod"); }
            set { SetValue("FrameChangeDetectionPeriod", value); }
        }

        /// <summary>
        /// Tries to get the specified value from the dictionary as the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="name">The name of the value to retrieve.</param>
        /// <returns>The value if present or the default value if it is not present or there is an error retrieving it.</returns>
        private T? TryGetValue<T>(string name)
            where T : struct
        {
            if (!Capabilities.Contains(name))
                return null;

            object obj = Capabilities[name];
            if (obj == null || !(obj is IDataValue))
                return null;

            var value = (IDataValue)obj;
            if (value.Item == null)
                return null;

            try
            {
                return (T)Convert.ChangeType(value.Item, typeof(T));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Sets the specified value in the dictionary.
        /// </summary>
        /// <typeparam name="T">The type of the item value.</typeparam>
        /// <param name="name">The name of the value.</param>
        /// <param name="value">The value to set.</param>
        private void SetValue<T>(string name, T? value)
            where T : struct
        {
            if (value == null)
                Capabilities.Remove(name);
            else
                Capabilities[name] = new v12.Datatypes.DataValue { Item = value.Value };
        }
    }
}
