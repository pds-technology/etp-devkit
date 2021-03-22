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
using System.Linq;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Properties;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Defines static fields for the ETP settings.
    /// </summary>
    public static class EtpSettings
    {
        /// <summary>
        /// The maximum time period in seconds that a store keeps the GrowingSatus for a growing object "active" after the last new part resulting in a change to the object's end index was added to the object.
        /// </summary>
        public static long? DefaultActiveTimeoutPeriod { get; set; } = Settings.Default.DefaultActiveTimeoutPeriod;

        /// <summary>
        /// Details on how a client can authorize itself for connecting to a server.
        /// </summary>
        public static string[] DefaultAuthorizationDetails { get; set; } = Settings.Default.DefaultAuthorizationDetails?.Cast<string>().ToArray();

        /// <summary>
        /// The maximum time period in seconds--under normal operation on an uncongested session--for these conditions: 
        /// <list type="bullet">
        /// <item>after a change in an endpoint before that endpoint sends a change notification covering the change to any subscribed endpoint in any session.</item>
        /// <item>if the the change was the result of a message WITHOUT a positive response, it is the maximum time until the change is reflected in read operations in any session.</item>
        /// <item>If the change was the result of a message WITH a positive response, it is the maximum time until the change is reflectedin sessions other than the session where the change was made.RECOMMENDATION: Set as short as possible (i.e.a few seconds).</item>
        /// </list>
        /// </summary>
        public static long? DefaultChangePropagationPeriod { get; set; } = Settings.Default.DefaultChangePropagationPeriod;

        /// <summary>
        /// The maximum time period in seconds time that a store retains the Canonical URI of a deleted data object and any change annotations for channels and growing objects. 
        /// </summary>
        public static long? DefaultChangeRetentionPeriod { get; set; } = Settings.Default.DefaultChangeRetentionPeriod;

        /// <summary>
        /// The maximum time period in seconds for updates to a channel to be visible in ChannelDataFrame (Protocol 2).
        /// </summary>
        public static long? DefaultFrameChangeDetectionPeriod { get; set; } = Settings.Default.DefaultFrameChangeDetectionPeriod;

        /// <summary>
        /// The maximum count of multipart messages allowed in parallel, in a single protocol, from one endpoint to another. The limit applies separately to each protocol, and separately from client to server and from server to client. The limit for an endpoint applies to the multipart messages that endpoint can receive.
        /// </summary>
        public static long? DefaultMaxConcurrentMultipart { get; set; } = Settings.Default.DefaultMaxConcurrentMultipart;

        /// <summary>
        /// The maximum count of contained data objects allowed in a single instance of the data object type that the capability applies to.
        /// </summary>
        public static long? DefaulMaxContainedDataObjectCount { get; set; } = Settings.Default.DefaultMaxContainedDataObjectCount;

        /// <summary>
        /// The maximum size in bytes of a data array allowed in a store. Size in bytes is the product of all array dimensions multiplied by the size in bytes of a single array element.
        /// </summary>
        public static long? DefaultMaxDataArraySize { get; set; } = Settings.Default.DefaultMaxDataArraySize;

        /// <summary>
        /// The maximum size in bytes of a data object allowed in a complete multipart message. Size in bytes is the size in bytes of the uncompressed string representation of the data object in the format in which it is sent or received.
        /// </summary>
        public static long? DefaultMaxDataObjectSize { get; set; } = Settings.Default.DefaultMaxDataObjectSize;

        /// <summary>
        /// The maximum total count of rows allowed in a complete multipart message response to a single request.
        /// </summary>
        public static long? DefaultMaxFrameResponseRowCount { get; set; } = Settings.Default.DefaultMaxFrameResponseRowCount;

        /// <summary>
        /// The maximum indexCount value allowed for StreamingStartIndex.
        /// </summary>
        public static long? DefaultMaxIndexCount { get; set; } = Settings.Default.DefaultMaxIndexCount;

        /// <summary>
        /// The maximum size in bytes of each data object part allowed in a standalone message or a complete multipart message. Size in bytes is the total size in bytes of the uncompressed string representation of the data object part in the format in which it is sent or received.
        /// </summary>
        public static long? DefaultMaxPartSize { get; set; } = Settings.Default.DefaultMaxPartSize;

        /// <summary>
        /// The maximum count of channels allowed in a single range request.
        /// </summary>
        public static long? DefaultMaxRangeChannelCount { get; set; } = Settings.Default.DefaultMaxRangeChannelCount;

        /// <summary>
        /// The maximum total count of DataItem records allowed in a complete multipart range message.
        /// </summary>
        public static long? DefaultMaxRangeDataItemCount { get; set; } = Settings.Default.DefaultMaxRangeDataItemCount;

        /// <summary>
        /// The maximum total count of responses allowed in a complete multipart message response to a single request.
        /// </summary>
        public static long? DefaultMaxResponseCount { get; set; } = Settings.Default.DefaultMaxResponseCount;

        /// <summary>
        /// The maximum count of concurrent ETP sessions that may be established for a given endpoint across all clients. The determination of whether this limit is exceeded should be made at the time of receiving the HTTP WebSocket upgrade or connect request.
        /// </summary>
        public static long? DefaultMaxSessionGlobalCount { get; set; } = Settings.Default.DefaultMaxSessionGlobalCount;

        /// <summary>
        /// The maximum count of concurrent ETP sessions that may be established for a given endpoint, by a specific client.
        /// </summary>
        public static long? DefaultMaxSessionClientCount { get; set; } = Settings.Default.DefaultMaxSessionClientCount;

        /// <summary>
        /// The maximum total count of channels allowed to be concurrently open for streaming in a session. The limit applies separately for each protocol with the capability.
        /// </summary>
        public static long? DefaultMaxStreamingChannelsSessionCount { get; set; } = Settings.Default.DefaultMaxStreamingChannelsSessionCount;

        /// <summary>
        /// The maximum total count of concurrent subscriptions allowed in a session. The limit applies separately for each protocol with the capability. 
        /// </summary>
        public static long? DefaultMaxSubscriptionSessionCount { get; set; } = Settings.Default.DefaultMaxSubscriptionSessionCount;

        /// <summary>
        /// The maximum count of transactions allowed in parallel in a session.
        /// </summary>
        public static long? DefaultMaxTransactionCount { get; set; } = Settings.Default.DefaultMaxTransactionCount;

        /// <summary>
        /// The maximum size in bytes allowed for a single WebSocket frame. The limit to use during a session is the minimum of the client's and the server's endpoint capability, which should be determined by the limits imposed by the WebSocket library used by each endpoint.
        /// </summary>
        public static long? DefaultMaxWebSocketFramePayloadSize { get; set; } = Settings.Default.DefaultMaxWebSocketFramePayloadSize;

        /// <summary>
        /// The maximum size in bytes allowed for a complete WebSocket message, which is composed of one or more WebSocket frames. The limit to use during a session is the minimum of the client's and the server's endpoint capability, which should be determined by the limits imposed by the WebSocket library used by each endpoint.
        /// </summary>
        public static long? DefaultMaxWebSocketMessagePayloadSize { get; set; } = Settings.Default.DefaultMaxWebSocketMessagePayloadSize;

        /// <summary>
        /// The maximum time period in seconds--under normal operation on an uncongested session--allowed between subsequent messages in the SAME multipart request or response. The period is measured as the time between when each message has been fully sent or received via the WebSocket.
        /// </summary>
        public static long? DefaultMultipartMessageTimeoutPeriod { get; set; } = Settings.Default.DefaultMultipartMessageTimeoutPeriod;

        /// <summary>
        /// For a data object type that contains other data objects by value, indicates whether any contained data objects that are orphaned when an instance of the data object type is deleted are also deleted.
        /// </summary>
        public static bool? DefaultOrphanedChildrenPrunedOnDelete { get; set; } = Settings.Default.DefaultOrphanedChildrenPrunedOnDelete;

        /// <summary>
        /// The maximum time period in seconds allowed between a request and the standalone response message or the first message in the multipart response message. The period is measured as the time between when the request message has been successfully sent via the WebSocket and when the first or only response message has been fully received via the WebSocket. When calculating this period, any Acknowledge messages or empty placeholder responses are ignored EXCEPT where these are the only and final response(s) to the request.
        /// </summary>
        public static long? DefaultResponseTimeoutPeriod { get; set; } = Settings.Default.DefaultResponseTimeoutPeriod;

        /// <summary>
        /// Indicates whether an endpoint is acting as a simple streamer or not.
        /// </summary>
        public static bool? DefaultSimpleStreamer { get; set; } = Settings.Default.DefaultSimpleStreamer;

        /// <summary>
        /// Indicates whether an endpoint supports alternate URI formats--beyond the canonical Energistics URIs, which MUST be supported--for requests.
        /// </summary>
        public static bool? DefaultSupportsAlternateRequestUris { get; set; } = Settings.Default.DefaultSupportsAlternateRequestUris;

        /// <summary>
        /// Indicates whether Delete operations are supported for the data object type. If the operation can be technically supported by an endpoint, this capability should be true.
        /// </summary>
        public static bool? DefaultSupportsDelete { get; set; } = Settings.Default.DefaultSupportsDelete;

        /// <summary>
        /// Indicates whether Get operations are supported for the data object type.
        /// </summary>
        public static bool? DefaultSupportsGet { get; set; } = Settings.Default.DefaultSupportsGet;

        /// <summary>
        /// Indicates whether an endpoint supports message header extensions.
        /// </summary>
        public static bool? DefaultSupportsMessageHeaderExtension { get; set; } = Settings.Default.DefaultSupportsMessageHeaderExtension;

        /// <summary>
        /// Indicates whether Put operations are supported for the data object type. If the operation can be technically supported by an endpoint, this capability should be true.
        /// </summary>
        public static bool? DefaultSupportsPut { get; set; } = Settings.Default.DefaultSupportsPut;

        /// <summary>
        /// The maximum time period in seconds allowed between receiving a StartTransactionResponse message and sending the corresponding CommitTransaction or RollbackTransaction request.
        /// </summary>
        public static long? DefaultTransactionTimeoutPeriod { get; set; } = Settings.Default.DefaultTransactionTimeoutPeriod;

        /// <summary>
        /// The default ETP encoding.
        /// </summary>
        public static EtpEncoding DefaultEncoding { get; set; } = Settings.Default.DefaultEncoding;

        /// <summary>
        /// The default websocket type.
        /// </summary>
        public static WebSocketType DefaultWebSocketType { get; set; } = Settings.Default.DefaultWebSocketType;
    }
}
