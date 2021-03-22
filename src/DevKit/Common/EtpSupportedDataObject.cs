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
using System.Collections.Generic;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Provides common information about a supported data object
    /// </summary>
    public class EtpSupportedDataObject : ISessionSupportedDataObject
    {
        /// <summary>
        /// Initializes a new <see cref="EtpSupportedDataObject"/> instance.
        /// </summary>
        /// <param name="qualifiedType">The data object's qualified type.</param>
        /// <param name="capabilities">The data object's capabilities</param>
        public EtpSupportedDataObject(IDataObjectType qualifiedType, List<string> capabilities)
        {
            QualifiedType = qualifiedType;
            Capabilities = capabilities ?? new List<string>();
        }

        /// <summary>
        /// Initializes a new <see cref="EtpSupportedDataObject"/> instance.
        /// </summary>
        /// <param name="supportedDataObject">The supported data object to initialize this instance from.</param>
        public EtpSupportedDataObject(ISupportedDataObject supportedDataObject)
        {
            QualifiedType = supportedDataObject.QualifiedType;
            Capabilities = new List<string>(supportedDataObject.DataObjectCapabilities);
        }

        /// <summary>
        /// Initializes a new <see cref="EtpSupportedDataObject"/> instance.
        /// </summary>
        /// <param name="supportedDataObject">The supported data object to initialize this instance from.</param>
        public EtpSupportedDataObject(IEndpointSupportedDataObject supportedDataObject)
        {
            QualifiedType = supportedDataObject.QualifiedType;
            Capabilities = new List<string>(supportedDataObject.Capabilities);
        }

        /// <summary>
        /// Initializes a new <see cref="EtpSupportedDataObject"/> instance.
        /// </summary>
        /// <param name="qualifiedType">The data object's qualified type.</param>
        public EtpSupportedDataObject(IDataObjectType qualifiedType)
        {
            QualifiedType = qualifiedType;
        }

        /// <summary>
        /// Gets the unique key for this supported data object.
        /// </summary>
        public string Key => QualifiedType.Key;

        /// <summary>
        /// Gets the endpoint's capabilities.
        /// </summary>
        public IDataObjectType QualifiedType { get; }

        /// <summary>
        /// Gets the capabilities supported for this object.
        /// </summary>
        public List<string> Capabilities { get; } = new List<string>();

        /// <summary>
        /// Gets the capabilities supported for this object as a read-only list.
        /// </summary>
        IReadOnlyList<string> IEndpointSupportedDataObject.Capabilities => Capabilities;

        /// <summary>
        /// Gets the capabilities supported for this object.
        /// </summary>
        public List<string> CounterpartCapabilities { get; set; } = new List<string>();

        /// <summary>
        /// Gets the counterpart's capabilities supported for this object as a read-only list.
        /// </summary>
        IReadOnlyList<string> ISessionSupportedDataObject.CounterpartCapabilities => CounterpartCapabilities;
    }
}
