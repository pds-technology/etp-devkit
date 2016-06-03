//----------------------------------------------------------------------- 
// ETP DevKit, 1.0
//
// Copyright 2016 Petrotechnical Data Systems
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
using Energistics.Datatypes;
using Energistics.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Energistics
{
    /// <summary>
    /// Provides <see cref="System.Net.WebClient"/> functionality needed to execute HTTP web
    /// requests to retrieve data prior to establishing an ETP Web Socket connection.
    /// </summary>
    public class JsonClient
    {
        private const string ContentType = "application/json";
        private const string DefaultGrantType = "password";

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonClient"/> class.
        /// </summary>
        public JsonClient()
        {
            BasicHeader = new Dictionary<string, string>();
            BearerHeader = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonClient"/> class.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public JsonClient(string username, string password)
        {
            BasicHeader = Authorization.Basic(username, password);
            BearerHeader = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonClient"/> class.
        /// </summary>
        /// <param name="token">The token.</param>
        public JsonClient(string token)
        {
            BasicHeader = new Dictionary<string, string>();
            BearerHeader = Authorization.Bearer(token);
        }

        /// <summary>
        /// Gets the Basic authorization header.
        /// </summary>
        /// <value>The Basic authorization header.</value>
        public IDictionary<string, string> BasicHeader { get; private set; }

        /// <summary>
        /// Gets the Bearer authorization header.
        /// </summary>
        /// <value>The Bearer authorization header.</value>
        public IDictionary<string, string> BearerHeader { get; private set; }

        /// <summary>
        /// Gets the cached <see cref="Energistics.Datatypes.ServerCapabilities"/> object.
        /// </summary>
        /// <value>The <see cref="Energistics.Datatypes.ServerCapabilities"/> object.</value>
        public ServerCapabilities ServerCapabilities { get; private set; }

        /// <summary>
        /// Gets the JSON Web Token using the specified URL and grant type.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="grantType">The grant type.</param>
        /// <returns>The JSON Web Token provided by the URL.</returns>
        public string GetJsonWebToken(string url, string grantType = DefaultGrantType)
        {
            using (var client = new System.Net.WebClient())
            {
                foreach (var header in BasicHeader)
                    client.Headers[header.Key] = header.Value;

                var response = client.UploadString(url, "grant_type=" + grantType);
                var json = JObject.Parse(response);

                var token = json["access_token"].Value<string>();
                BearerHeader = Authorization.Bearer(token);

                return token;
            }
        }

        /// <summary>
        /// Gets the <see cref="Energistics.Datatypes.ServerCapabilities"/> object using the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>The <see cref="Energistics.Datatypes.ServerCapabilities"/> object.</returns>
        public ServerCapabilities GetServerCapabilities(string url)
        {
            var headers = BearerHeader.Any()
                ? BearerHeader
                : BasicHeader;

            return GetServerCapabilities(url, headers);
        }

        /// <summary>
        /// Gets the <see cref="Energistics.Datatypes.ServerCapabilities"/> object using the specified URL and authorization header.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="headers">The authorization header.</param>
        /// <returns>The <see cref="Energistics.Datatypes.ServerCapabilities"/> object.</returns>
        private ServerCapabilities GetServerCapabilities(string url, IDictionary<string, string> headers)
        {
            using (var client = new System.Net.WebClient())
            {
                foreach (var header in headers)
                    client.Headers[header.Key] = header.Value;

                client.Headers[System.Net.HttpRequestHeader.Accept] = ContentType;

                var response = client.DownloadString(url);
                var capServer = JsonConvert.DeserializeObject<ServerCapabilities>(response);

                ServerCapabilities = capServer;
                return capServer;
            }
        }
    }
}
