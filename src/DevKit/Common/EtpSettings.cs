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
using Energistics.Etp.Properties;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Defines static fields for the ETP settings.
    /// </summary>
    public static class EtpSettings
    {
        /// <summary>
        /// The ETP v1.1 sub protocol name
        /// </summary>
        public const string Etp11SubProtocol = "energistics-tp";

        /// <summary>
        /// The ETP v1.2 sub protocol name
        /// </summary>
        public const string Etp12SubProtocol = "etp12.energistics.org";

        /// <summary>
        /// A list of supported ETP sub protocols
        /// </summary>
        public static readonly List<string> EtpSubProtocols = new List<string>
        {
            Etp12SubProtocol, Etp11SubProtocol
        };

        /// <summary>
        /// The default ETP sub protocol name
        /// </summary>
        public static string EtpSubProtocolName { get; set; } = Settings.Default.EtpSubProtocolName;

        /// <summary>
        /// The default ETP encoding header
        /// </summary>
        public static string EtpEncodingHeader { get; set; } = Settings.Default.EtpEncodingHeader;

        /// <summary>
        /// The binary ETP encoding.
        /// </summary>
        public static string EtpEncodingBinary { get; set; } = Settings.Default.EtpEncodingBinary;

        /// <summary>
        /// The JSON ETP encoding.
        /// </summary>
        public static string EtpEncodingJson { get; set; } = Settings.Default.EtpEncodingJson;

        /// <summary>
        /// The default GetVersion header
        /// </summary>
        public static string GetVersionHeader { get; set; } = Settings.Default.GetVersionHeader;

        /// <summary>
        /// The default GetVersions header
        /// </summary>
        public static string GetVersionsHeader { get; set; } = Settings.Default.GetVersionsHeader;

        /// <summary>
        /// This is the largest part size the store or customer can get or put. A store can optionally specify this for protocols that handle object parts. Property of numberofbytes.
        /// </summary>
        public static bool SupportsAlternateRequestUris { get; set; } = Settings.Default.SupportsAlternateRequestUris;

        /// <summary>
        /// The default maximum data items
        /// </summary>
        public static long DefaultMaxDataItemCount { get; set; } = Settings.Default.DefaultMaxDataItemCount;

        /// <summary>
        /// The default maximum response count
        /// </summary>
        public static long DefaultMaxResponseCount { get; set; } = Settings.Default.DefaultMaxResponseCount;

        /// <summary>
        /// This is the largest data object the store or customer can get or put. A store or customer can optionally specify these for protocols that handle data objects.
        /// </summary>
        public static long DefaultMaxDataObjectSize { get; set; } = Settings.Default.DefaultMaxDataObjectSize;

        /// <summary>
        /// This is the largest part size the store or customer can get or put. A store can optionally specify this for protocols that handle object parts. Property of numberofbytes.
        /// </summary>
        public static long DefaultMaxPartSize { get; set; } = Settings.Default.DefaultMaxPartSize;

        /// <summary>
        /// Maximum time interval between subsequent messages in the SAME multipart request or response.
        /// </summary>
        public static long DefaultMaxMultipartMessageTimeInterval { get; set; } = Settings.Default.DefaultMaxMultipartMessageTimeInterval;

        /// <summary>
        /// Maximum size allowed for a WebSocket frame (which is determined by the library you use to implement WebSocket). WebSocket is the transport protocol used by ETP.
        /// </summary>
        public static long DefaultMaxWebSocketFramePayloadSize { get; set; } = Settings.Default.DefaultMaxWebSocketFramePayloadSize;

        /// <summary>
        /// Maximum size allowed for a WebSocket message (which is composed of multiple WebSocket frames, which is determined by the library you use to implement WebSocket). WebSocket is the transport protocol used by ETP.
        /// </summary>
        public static long DefaultMaxWebSocketMessagePayloadSize { get; set; } = Settings.Default.DefaultMaxWebSocketMessagePayloadSize;

        /// <summary>
        /// Sets limits on maximum indexCount (number of indexes "back" from the current index that a producer will provide) for StreamingStartIndex.
        /// </summary>
        public static long DefaultMaxIndexCount { get; set; } = Settings.Default.DefaultMaxIndexCount;

        /// <summary>
        /// Indicates the maximum delay in a store before it sends a change notification.
        /// </summary>
        public static long DefaultChangeDetectionPeriod { get; set; } = Settings.Default.DefaultChangeDetectionPeriod;

        /// <summary>
        /// The maximum allowed time for updates to a channel to be visible in ChannelDataFrame.
        /// </summary>
        public static long DefaultFrameChangeDetectionPeriod { get; set; } = Settings.Default.DefaultFrameChangeDetectionPeriod;

        /// <summary>
        /// Indicates the the maximum amount of time that a store retains change information about a data object.
        /// The name of the variable is ChangeNotificationRetentionPeriod and it MUST have an integer value of number of seconds.The minimum value for this variable is 3600 seconds.
        /// </summary>
        public static long DefaultChangeNotificationRetentionPeriod { get; set; } = Settings.Default.DefaultChangeNotificationRetentionPeriod;

        /// <summary>
        /// Indicates the amount of time that a store must retain the ID of a deleted record. Minimum value is 24 hours, and the store must be able to report its start time.
        /// </summary>
        public static long DefaultDeleteNotificationRetentionPeriod { get; set; } = Settings.Default.DefaultDeleteNotificationRetentionPeriod;

        /// <summary>
        /// Indicates the maximum time a server allows to process a transaction.
        /// </summary>
        public static long DefaultTransactionTimeout { get; set; } = Settings.Default.DefaultTransactionTimeout;

        /// <summary>
        /// Indicates the maximum time a producer allows no streaming data to occur before setting the channelStatus to 'inactive'.
        /// </summary>
        public static long DefaultStreamingTimeoutPeriod { get; set; } = Settings.Default.DefaultStreamingTimeoutPeriod;

        /// <summary>
        /// Indicates the maximum time a server allows no data points to be added to the growing part of a growing object before setting the growingStatus to 'inactive'.
        /// </summary>
        public static long DefaultGrowingTimeoutPeriod { get; set; } = Settings.Default.DefaultGrowingTimeoutPeriod;

        /// <summary>
        /// This is the largest data array size the store can get or put. A store can optionally specify this for protocols that handle data arrays.
        /// </summary>
        public static long DefaultMaxDataArraySize { get; set; } = Settings.Default.DefaultMaxDataArraySize;
    }
}
