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
using System.Net;
using System.Threading.Tasks;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using log4net;
using Nito.AsyncEx;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace Energistics.Etp
{
    /// <summary>
    /// Wraps an <see cref="IEtpSelfHostedWebServer"/> with a proxy using Titanium Web Proxy.
    /// </summary>
    public class EtpSelfHostedProxyWebServer : IEtpSelfHostedWebServer
    {
        private readonly IEtpSelfHostedWebServer _proxiedServer;
        private readonly ProxyServer _proxyServer;

        public EtpSelfHostedProxyWebServer(int port, IEtpSelfHostedWebServer proxiedServer)
        {
            _proxiedServer = proxiedServer;
            _proxyServer = new ProxyServer();

            var uri = proxiedServer.Uri;
            var builder = new UriBuilder(uri.Scheme, uri.Host, port, uri.AbsolutePath, uri.Query);
            ProxyUri = builder.Uri;

            var endPoint = new ExplicitProxyEndPoint(IPAddress.Loopback, ProxyUri.Port, false);
            endPoint.GenericCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2();

            _proxyServer.ProxyBasicAuthenticateFunc = HandleBasicAuthentication;

            _proxyServer.AddEndPoint(endPoint);
        }

        public Uri ProxyUri { get; private set; }

        public bool ProxyAuthenticationSuccessful { get; private set; }

        public void Start()
        {
            AsyncContext.Run(() => StartAsync());
        }

        public async Task StartAsync()
        {
            _proxyServer.Start();
            await _proxiedServer.StartAsync().ConfigureAwait(false);
        }

        private Task<bool> HandleBasicAuthentication(SessionEventArgsBase session, string username, string password)
        {
            if (username == TestSettings.ProxyUsername && password == TestSettings.ProxyPassword)
            {
                ProxyAuthenticationSuccessful = true;
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public void Stop()
        {
            AsyncContext.Run(() => StopAsync());
        }

        public async Task StopAsync()
        {
            _proxyServer.Stop();
            await _proxiedServer.StopAsync().ConfigureAwait(false);
        }

        #region Redirects to Proxied Server

        public Uri Uri => _proxiedServer.Uri;

        public bool IsRunning => _proxiedServer.IsRunning;

        public IEtpServerManager ServerManager => _proxiedServer.ServerManager;

        public EtpWebServerDetails Details => _proxiedServer.Details;

        public void Dispose()
        {
            _proxyServer.Dispose();
            _proxiedServer.Dispose();
        }

        #endregion
    }
}
