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
using Energistics.Avro.Encoding;
using Energistics.Avro.Encoding.Json;
using Energistics.Etp.Common;
using Energistics.Json;
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
            Authorization = Authorization.Basic(username, password);
            Proxy = proxy;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonClient" /> class.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="proxy">The proxy.</param>
        public JsonClient(string token, IWebProxy proxy = null)
        {
            Authorization = Authorization.Bearer(token);
            Proxy = proxy;
        }

        /// <summary>
        /// Gets or sets the proxy.
        /// </summary>
        /// <value>The proxy.</value>
        public IWebProxy Proxy { get; set; }

        /// <summary>
        /// The authorization to use to get a token.
        /// </summary>
        private Authorization Authorization { get; }

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

                client.Headers.SetAuthorization(Authorization);

                client.Headers[HttpRequestHeader.ContentType] = UrlEncodedContentType;
                client.Headers[HttpRequestHeader.CacheControl] = NoCache;

                var payload = PreparePayload(grantType);
                var response = client.UploadString(url, payload);
                string token;

                try
                {
                    using (var reader = new JsonReader(response))
                        token = reader.Read().GetValue("access_token");
                }
                catch
                {
                    // If we can't parse the expected response format, use the raw response as the token
                    token = response;
                }

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
            if (url.IndexOf(EtpHeaders.GetVersions, StringComparison.OrdinalIgnoreCase) < 0)
            {
                var delim = url.IndexOf("?", StringComparison.OrdinalIgnoreCase) < 0 ? "?" : "&";
                url = $"{url}{delim}{EtpHeaders.GetVersions}=true";
            }

            var headers = new Dictionary<string, string>();
            headers.SetAuthorization(Authorization);
            var json = DownloadJson(url);

            using (var reader = new JsonReader(json))
                return (reader.Read() as Json.Tokens.Array).Values.Select(token => token.GetValue()).ToList();

        }

        /// <summary>
        /// Gets the server capabilities object using the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>The server capabilities object.</returns>
        public object GetServerCapabilities(string url)
        {

            var json = DownloadJson(url);
            var capServerType = GetServerCapabilitiesType(url);

            using (var decoder = new JsonAvroDecoder(json))
                return decoder.DecodeAvroObject<v11.Datatypes.ServerCapabilities>();
        }

        /// <summary>
        /// Gets a JSON response using the specified URL and authorization header.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>The JSON response.</returns>
        private string DownloadJson(string url)
        {
            using (var client = new WebClient())
            {
                client.Proxy = Proxy;

                client.Headers.SetAuthorization(Authorization);

                client.Headers[HttpRequestHeader.Accept] = JsonContentType;

                return client.DownloadString(url);
            }
        }

        /// <summary>
        /// Prepares the payload for the JWT request.
        /// </summary>
        /// <param name="authorization">The authorization.</param>
        /// <param name="grantType">The grant type.</param>
        /// <returns>The URL encoded name/value pairs.</returns>
        private string PreparePayload(string grantType)
        {
            var payload = new StringBuilder()
                .AppendFormat("grant_type={0}", grantType);

            try
            {
                if (Authorization.HasValue && Authorization.IsBasic)
                {
                    // Skip authorization type
                    var header = Authorization.Value.Substring(6);

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
            var etpVersion = queryString[EtpHeaders.GetVersion];

            return etpVersion == EtpSubProtocols.v12 ? etp12Type : etp11Type;
        }
    }
}
