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
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Energistics.Etp.Common;
using Newtonsoft.Json.Linq;
using Authorization = Energistics.Etp.Security.Authorization;

namespace Energistics.Etp
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
                catch
                {
                    // If we can't parse the expected response format, use the raw response as the token
                    token = response;
                }

                BearerHeader = Authorization.Bearer(token);
                return token;
            }
        }

        /// <summary>
        /// Get the list of supported ETP versions using the specified URL.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public IList<string> GetEtpVersions(string url)
        {
            // Append GetVersions=true, if not already in the url
            if (url.IndexOf(EtpSettings.GetVersionsHeader, StringComparison.InvariantCultureIgnoreCase) < 0)
            {
                var delim = url.IndexOf("?", StringComparison.InvariantCultureIgnoreCase) < 0 ? "?" : "&";
                url = $"{url}{delim}{EtpSettings.GetVersionsHeader}=true";
            }

            var headers = BearerHeader.Any() ? BearerHeader : BasicHeader;
            var json = DownloadJson(url, headers);

            return EtpExtensions.Deserialize<List<string>>(json);

        }

        /// <summary>
        /// Gets the server capabilities object using the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>The server capabilities object.</returns>
        public object GetServerCapabilities(string url)
        {
            var headers = BearerHeader.Any() ? BearerHeader : BasicHeader;
            return GetServerCapabilities(url, headers);
        }

        /// <summary>
        /// Gets the server capabilities object using the specified URL and authorization header.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="headers">The authorization header.</param>
        /// <returns>The server capabilities object.</returns>
        private object GetServerCapabilities(string url, IDictionary<string, string> headers)
        {
            var json = DownloadJson(url, headers);
            var capServerType = GetServerCapabilitiesType(url);

            return EtpExtensions.Deserialize(capServerType, json);
        }

        /// <summary>
        /// Gets a JSON response using the specified URL and authorization header.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="headers">The authorization header.</param>
        /// <returns>The JSON response.</returns>
        private string DownloadJson(string url, IDictionary<string, string> headers)
        {
            using (var client = new WebClient())
            {
                client.Proxy = Proxy;

                foreach (var header in headers)
                    client.Headers[header.Key] = header.Value;

                client.Headers[HttpRequestHeader.Accept] = JsonContentType;

                return client.DownloadString(url);
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

        private static Type GetServerCapabilitiesType(string url)
        {
            var etp11Type = typeof(v11.Datatypes.ServerCapabilities);
            var etp12Type = typeof(v12.Datatypes.ServerCapabilities);
            var uri = new Uri(url);

            if (string.IsNullOrWhiteSpace(uri.Query))
                return etp11Type;

            var queryString = HttpUtility.ParseQueryString(uri.Query);
            var etpVersion = queryString[EtpSettings.GetVersionHeader];

            return etpVersion == EtpSettings.Etp12SubProtocol ? etp12Type : etp11Type;
        }
    }
}
