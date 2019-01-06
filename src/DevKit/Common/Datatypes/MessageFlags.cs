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

namespace Energistics.Etp.Common.Datatypes
{
    /// <summary>
    /// An enumeration of message flag values.
    /// </summary>
    [Flags]
    public enum MessageFlags : int
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0x0,

        /// <summary>
        /// A part of a multi-part message.
        /// </summary>
        MultiPart = 0x1,

        /// <summary>
        /// The final part of a multi-part message.
        /// </summary>
        FinalPart = 0x2,

        /// <summary>
        /// Short-hand for both mutli-part and final part: 0x1 | 0x2
        /// </summary>
        MultiPartAndFinalPart = 0x3,

        /// <summary>
        /// No data is available.
        /// </summary>
        NoData = 0x4,

        /// <summary>
        /// The message body is compressed.
        /// </summary>
        Compressed = 0x8,

        /// <summary>
        /// An Acknowledge message is requested by the sender.
        /// </summary>
        Acknowledge = 0x10
    }
}
