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

using Energistics.Etp.Common.Datatypes;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Provides common functionality for ETP data object capabilities.
    /// </summary>
    public class EtpDataObjectCapabilities : EtpCapabilities, IDataObjectCapabilities
    {
        /// <summary>
        /// Initializes a new <see cref="EtpDataObjectCapabilities"/> instance.
        /// </summary>
        public EtpDataObjectCapabilities()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="EtpDataObjectCapabilities"/> instance.
        /// </summary>
        /// <param name="version">The ETP version the capabilities are for.</param>
        public EtpDataObjectCapabilities(EtpVersion version)
            : base(version)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="EtpDataObjectCapabilities"/> instance.
        /// </summary>
        /// <param name="capabilities">The capabilities to initialize this from.</param>
        public EtpDataObjectCapabilities(IReadOnlyCapabilities capabilities)
            : base(capabilities)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="EtpDataObjectCapabilities"/> instance.
        /// </summary>
        /// <param name="version">The ETP version the capabilities are for.</param>
        /// <param name="capabilities">The capabilities to initialize this from.</param>
        public EtpDataObjectCapabilities(EtpVersion version, IReadOnlyDataValueDictionary capabilities)
            : base(version, capabilities)
        {
        }

        /// <summary>
        /// The minimum time period in seconds that a store keeps the GrowingSatus for a growing object "active" after the last new part resulting in a change to the object's end index was added to the object.
        /// </summary>
        public long? ActiveTimeoutPeriod { get; set; }

        /// <summary>
        /// The maximum count of contained data objects allowed in a single instance of the data object type that the capability applies to.
        /// </summary>
        public long? MaxContainedDataObjectCount { get; set; }

        /// <summary>
        /// The maximum size in bytes of a data object allowed in a complete multipart message. Size in bytes is the size in bytes of the uncompressed string representation of the data object in the format in which it is sent or received.
        /// </summary>
        public long? MaxDataObjectSize { get; set; }

        /// <summary>
        /// For a container data object type (i.e., a data object type that may contain other data objects by value), this capability indicates whether contained data objects that are orphaned as a result of an operation on its container data object may be deleted (pruned).
        /// </summary>
        public bool? OrphanedChildrenPrunedOnDelete { get; set; }

        /// <summary>
        /// Indicates whether get operations are supported for the data object type.
        /// </summary>
        public bool? SupportsGet { get; set; }

        /// <summary>
        /// Indicates whether put operations are supported for the data object type. If the operation can be technically supported by an endpoint, this capability should be true.
        /// </summary>
        public bool? SupportsPut { get; set; }

        /// <summary>
        /// Indicates whether delete operations are supported for the data object type. If the operation can be technically supported by an endpoint, this capability should be true.
        /// </summary>
        public bool? SupportsDelete { get; set; }

        /// <summary>
        /// The maximum count of secondary indexes allowed in a single instance of the data object type that the capability applies to, which may be Channel or ChannelSet.
        /// </summary>
        public long? MaxSecondaryIndexCount { get; set; }
    }
}
