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
using System.Text;

namespace Energistics.Etp.Security
{
    /// <summary>
    /// Provides methods that can be used to create a dictionary containing an Authorization header.
    /// </summary>
    public class Authorization
    {
        /// <summary>
        /// Whether or not this instance has an authorization value.
        /// </summary>
        public bool HasValue => !string.IsNullOrEmpty(Value);

        /// <summary>
        /// Whether or not this is basic authorization.
        /// </summary>
        public bool IsBasic { get; }

        /// <summary>
        /// The Authorization header value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new <see cref="Authorization"/> instance with the specified value.
        /// </summary>
        /// <param name="value">The authorization value.</param>
        /// <param name="isBasic">Whether or not the authorization is basic.</param>
        private Authorization(string value, bool isBasic)
        {
            Value = value;
            IsBasic = isBasic;
        }

        /// <summary>
        /// Creates a dictionary containing an Authorization header for the specified username and password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>A <see cref="Authorization"/> initialized for basic authentication.</returns>
        public static Authorization Basic(string username, string password)
        {
            var credentials = string.IsNullOrWhiteSpace(username)
                ? string.Empty
                : string.Concat(username, ":", password);

            var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

            return new Authorization(GetAuthorizationValue("Basic", encodedCredentials), true);
        }

        /// <summary>
        /// Creates a dictionary containing an Authorization header for the specified JSON web token.
        /// </summary>
        /// <param name="token">The JSON web token.</param>
        /// <returns>A dictionary containing an Authorization header.</returns>
        public static Authorization Bearer(string token)
        {
            return new Authorization(GetAuthorizationValue("Bearer", string.IsNullOrWhiteSpace(token) ? string.Empty : token), false);
        }

        /// <summary>
        /// Creates a an Authorization header value for the specified schema and encoded string.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="encodedString">The encoded string.</param>
        /// <returns>An Authorization header value.</returns>
        private static string GetAuthorizationValue(string schema, string encodedString)
        {
            if (!string.IsNullOrWhiteSpace(encodedString))
            {
                return string.Concat(schema, " ", encodedString);
            }

            return null;
        }
    }
}
