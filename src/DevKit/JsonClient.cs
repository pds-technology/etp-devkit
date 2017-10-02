//----------------------------------------------------------------------- 
// ETP DevKit, 1.1
//
// Copyright 2016 Energistics
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
using System.Linq;
using System.Net;
using System.Text;
using Energistics.Datatypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Authorization = Energistics.Security.Authorization;

namespace Energistics
{
    /// <summary>
    /// Provides <see cref="System.Net.WebClient"/> functionality needed to execute HTTP web
    /// requests to retrieve data prior to establishing an ETP Web Socket connection.
    /// </summary>
    public class JsonClient
    {
        private const string UrlEncodedContentType = "application/x-www-form-urlencoded";
        private const string JsonContentType = "application/json";
        private const string DefaultGrantType = "password";
        private const string NoCache = "no-cache";

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonClient" /> class.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        public JsonClient(IWebProxy proxy = null)
        {
            BasicHeader = new Dictionary<string, string>();
            BearerHeader = new Dictionary<string, string>();
            Proxy = proxy;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonClient" /> class.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="proxy">The proxy.</param>
        public JsonClient(string username, string password, IWebProxy proxy = null)
        {
            BasicHeader = Authorization.Basic(username, password);
            BearerHeader = new Dictionary<string, string>();
            Proxy = proxy;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonClient" /> class.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="proxy">The proxy.</param>
        public JsonClient(string token, IWebProxy proxy = null)
        {
            BasicHeader = new Dictionary<string, string>();
            BearerHeader = Authorization.Bearer(token);
            Proxy = proxy;
        }

        /// <summary>
        /// Gets or sets the proxy.
        /// </summary>
        /// <value>The proxy.</value>
        public IWebProxy Proxy { get; set; }

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
            using (var client = new WebClient())
            {
                client.Proxy = Proxy;

                // Include Authorization header
                foreach (var header in BasicHeader)
                    client.Headers[header.Key] = header.Value;

                client.Headers[HttpRequestHeader.ContentType] = UrlEncodedContentType;
                client.Headers[HttpRequestHeader.CacheControl] = NoCache;

                var payload = PreparePayload(BasicHeader, grantType);
                var response = client.UploadString(url, payload);
                string token;

                try
                {
                    var json = JObject.Parse(response);
                    token = json["access_token"].Value<string>();
                }
                catch (Exception e)
                {
                    // If we can't parse the expected response format, use the raw response as the token
                    token = response;
                }

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
            using (var client = new WebClient())
            {
                client.Proxy = Proxy;

                foreach (var header in headers)
                    client.Headers[header.Key] = header.Value;

                client.Headers[HttpRequestHeader.Accept] = JsonContentType;

                var response = client.DownloadString(url);
                var capServer = JsonConvert.DeserializeObject<ServerCapabilities>(response);

                ServerCapabilities = capServer;
                return capServer;
            }
        }

        /// <summary>
        /// Prepares the payload for the JWT request.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <param name="grantType">The grant type.</param>
        /// <returns>The URL encoded name/value pairs.</returns>
        private static string PreparePayload(IDictionary<string, string> headers, string grantType)
        {
            var payload = new StringBuilder()
                .AppendFormat("grant_type={0}", grantType);

            try
            {
                string header;
                if (headers.TryGetValue(Authorization.Header, out header) && header.StartsWith("Basic "))
                {
                    // Skip authorization type
                    header = header.Substring(6);

                    if (!string.IsNullOrWhiteSpace(header))
                    {
                        var bytes = Convert.FromBase64String(header);
                        header = Encoding.UTF8.GetString(bytes);

                        var values = header.Split(new[] {':'}, 2);
                        var username = values.FirstOrDefault();
                        var password = values.Skip(1).FirstOrDefault();

                        if (!string.IsNullOrWhiteSpace(password))
                        {
                            // Include URL encoded credentials in payload
                            payload.AppendFormat("&username={0}&password={1}",
                                WebUtility.UrlEncode(username),
                                WebUtility.UrlEncode(password));
                        }
                    }
                }
            }
            catch
            {
                // Credentials will not be included in payload
            }

            return payload.ToString();
        }
    }
}
