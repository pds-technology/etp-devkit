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

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Read-only interface for ETP data object capabilities.
    /// </summary>
    public interface IDataObjectCapabilities : IReadOnlyCapabilities
    {
        /// <summary>
        /// The minimum time period in seconds that a store keeps the GrowingSatus for a growing object "active" after the last new part resulting in a change to the object's end index was added to the object.
        /// </summary>
        long? ActiveTimeoutPeriod { get; }

        /// <summary>
        /// The maximum count of contained data objects allowed in a single instance of the data object type that the capability applies to.
        /// </summary>
        long? MaxContainedDataObjectCount { get; }

        /// <summary>
        /// The maximum size in bytes of a data object allowed in a complete multipart message. Size in bytes is the size in bytes of the uncompressed string representation of the data object in the format in which it is sent or received.
        /// </summary>
        long? MaxDataObjectSize { get; }

        /// <summary>
        /// For a container data object type (i.e., a data object type that may contain other data objects by value), this capability indicates whether contained data objects that are orphaned as a result of an operation on its container data object may be deleted (pruned).
        /// </summary>
        bool? OrphanedChildrenPrunedOnDelete { get; }

        /// <summary>
        /// Indicates whether get operations are supported for the data object type.
        /// </summary>
        bool? SupportsGet { get; }

        /// <summary>
        /// Indicates whether put operations are supported for the data object type. If the operation can be technically supported by an endpoint, this capability should be true.
        /// </summary>
        bool? SupportsPut { get; }

        /// <summary>
        /// Indicates whether delete operations are supported for the data object type. If the operation can be technically supported by an endpoint, this capability should be true.
        /// </summary>
        bool? SupportsDelete { get; }

        /// <summary>
        /// The maximum count of secondary indexes allowed in a single instance of the data object type that the capability applies to, which may be Channel or ChannelSet.
        /// </summary>
        long? MaxSecondaryIndexCount { get; }
    }
}
