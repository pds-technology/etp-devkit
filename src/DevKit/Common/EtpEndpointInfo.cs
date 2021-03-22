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

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Provides identifying information about an ETP Endpoint.
    /// </summary>
    public class EtpEndpointInfo
    {
        /// <summary>
        /// Initializes a new <see cref="EtpEndpointInfo"/> instance.
        /// </summary>
        /// <param name="applicationName">The endpoint's application name.</param>
        /// <param name="applicationVersion">The endpoint's application version.</param>
        /// <param name="instanceId">The endpoint's instance identifier.</param>
        private EtpEndpointInfo(string applicationName, string applicationVersion, Guid instanceId)
        {
            ApplicationName = applicationName;
            ApplicationVersion = applicationVersion;
            InstanceId = instanceId;
        }

        /// <summary>
        /// Creates a new <see cref="EtpEndpointInfo"/> from a key.
        /// The endpoint's instance identifier is generated from the provided key.
        /// The key should be consistent for the same application name, application version and user details.
        /// </summary>
        /// <param name="applicationName">The endpoint's application name.</param>
        /// <param name="applicationVersion">The endpoint's application version.</param>
        /// <param name="key">The endpoint's key used to generate its instance identifier.</param>
        /// <returns>The new instance.</returns>
        public static EtpEndpointInfo FromKey(string applicationName, string applicationVersion, string key)
        {
            var instanceId = GuidUtility.CreateEnergisticsEtpGuid(key);
            return new EtpEndpointInfo(applicationName, applicationVersion, instanceId);
        }

        /// <summary>
        /// Creates a new <see cref="EtpEndpointInfo"/> from an instance identifier.
        /// </summary>
        /// <param name="applicationName">The endpoint's application name.</param>
        /// <param name="applicationVersion">The endpoint's application version.</param>
        /// <param name="instanceId">The endpoint's instance identifier.</param>
        public static EtpEndpointInfo FromId(string applicationName, string applicationVersion, Guid instanceId)
        {
            return new EtpEndpointInfo(applicationName, applicationVersion, instanceId);
        }

        /// <summary>
        /// Creates a new <see cref="EtpEndpointInfo"/> from an instance identifier.
        /// </summary>
        /// <param name="applicationName">The endpoint's application name.</param>
        /// <param name="applicationVersion">The endpoint's application version.</param>
        public static EtpEndpointInfo WithoutId(string applicationName, string applicationVersion)
        {
            return new EtpEndpointInfo(applicationName, applicationVersion, new Guid());
        }

        /// <summary>
        /// Gets the name of the endpoint's application name.
        /// </summary>
        /// <value>The name of the endpoint's application name.</value>
        public string ApplicationName { get; }

        /// <summary>
        /// Gets the endpoint's application version.
        /// </summary>
        /// <value>The endpoint's application version.</value>
        public string ApplicationVersion { get; }

        /// <summary>
        /// Gets the endpoint's instance identifier.
        /// </summary>
        /// <value>The endpoint's instance identifier.</value>
        public Guid InstanceId { get; }
    }
}
