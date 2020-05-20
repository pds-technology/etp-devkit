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

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Provides common functionality for ETP endpoint capabilities.
    /// </summary>
    public class EtpEndpointCapabilities
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EtpEndpointCapabilities"/> class.
        /// </summary>
        public EtpEndpointCapabilities()
        {
            Capabilities = new Dictionary<string, IDataValue>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpEndpointCapabilities"/> class.
        /// </summary>
        /// <param name="capabilities">The endpoint capabilities.</param>
        public EtpEndpointCapabilities(IDictionary<string, IDataValue> capabilities)
        {
            Capabilities = capabilities;
        }

        /// <summary>
        /// The endpoint capabilities
        /// </summary>
        public IDictionary<string, IDataValue> Capabilities { get; }

        /// <summary>
        /// The protocol capabilities as a data value dictionary.
        /// </summary>
        /// <typeparam name="TDataValue">The data value type.</typeparam>
        /// <returns>The capabilities as a data value dictionary.</returns>
        public Dictionary<string, TDataValue> AsDataValueDictionary<TDataValue>() where TDataValue : IDataValue, new()
        {
            var dictionary = new Dictionary<string, TDataValue>();
            foreach (var kvp in Capabilities)
            {
                dictionary[kvp.Key] = new TDataValue { Item = kvp.Value.Item };
            }

            return dictionary;
        }

        /// <summary>
        /// This is the largest data object the store or customer can get or put. A store or customer can optionally specify these for protocols that handle data objects. Property of numberofbytes.
        /// </summary>
        public long? MaxDataObjectSize
        {
            get { return TryGetValue<long>("MaxDataObjectSize"); }
            set { SetValue("MaxDataObjectSize", value); }
        }

        /// <summary>
        /// This is the largest part size the store or customer can get or put. A store can optionally specify this for protocols that handle object parts. Property of numberofbytes.
        /// </summary>
        public long? MaxPartSize
        {
            get { return TryGetValue<long>("MaxPartSize"); }
            set { SetValue("MaxPartSize", value); }
        }

        /// <summary>
        /// Maximum number of messages allowed for a multipart request/response at one time.
        /// </summary>
        public long? MaxConcurrentMultipart
        {
            get { return TryGetValue<long>("MaxConcurrentMultipart"); }
            set { SetValue("MaxConcurrentMultipart", value); }
        }

        /// <summary>
        /// Maximum time interval between subsequent messages in the SAME multipart request or response.
        /// </summary>
        public long? MaxMultipartMessageTimeInterval
        {
            get { return TryGetValue<long>("MaxMultipartMessageTimeInterval"); }
            set { SetValue("MaxMultipartMessageTimeInterval", value); }
        }

        /// <summary>
        /// Maximum size of the aggregate of all the parts of the multipart request or response.
        /// </summary>
        public long? MaxMultipartTotalSize
        {
            get { return TryGetValue<long>("MaxMultipartTotalSize"); }
            set { SetValue("MaxMultipartTotalSize", value); }
        }

        /// <summary>
        /// Maximum size allowed for a WebSocket frame (which is determined by the library you use to implement WebSocket). WebSocket is the transport protocol used by ETP.
        /// </summary>
        public long? MaxWebSocketFramePayloadSize
        {
            get { return TryGetValue<long>("MaxWebSocketFramePayloadSize"); }
            set { SetValue("MaxWebSocketFramePayloadSize", value); }
        }

        /// <summary>
        /// Maximum size allowed for a WebSocket message (which is composed of multiple WebSocket frames, which is determined by the library you use to implement WebSocket). WebSocket is the transport protocol used by ETP.
        /// </summary>
        public long? MaxWebSocketMessagePayloadSize
        {
            get { return TryGetValue<long>("MaxWebSocketMessagePayloadSize"); }
            set { SetValue("MaxWebSocketMessagePayloadSize", value); }
        }

        /// <summary>
        /// Indicates whether an agent supports alternate URI formats (beyond the Energistics canonical URI, which MUST be supported) for requests.
        /// </summary>
        public bool? SupportsAlternateRequestUris
        {
            get { return TryGetValue<bool>("SupportsAlternateRequestUris"); }
            set { SetValue("SupportsAlternateRequestUris", value); }
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
            IDataValue value;
            if (!Capabilities.TryGetValue(name, out value))
                return null;

            if (value?.Item == null)
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
