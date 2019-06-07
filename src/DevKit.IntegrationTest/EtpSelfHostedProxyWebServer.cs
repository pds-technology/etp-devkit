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
            StartAsync().Wait();
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
            StopAsync().Wait();
        }

        public async Task StopAsync()
        {
            _proxyServer.Stop();
            await _proxiedServer.StopAsync().ConfigureAwait(false);
        }

        #region Redirects to Proxied Server

        public Uri Uri { get { return _proxiedServer.Uri; } }

        public bool IsRunning { get { return _proxiedServer.IsRunning; } }

        public ILog Logger { get { return _proxiedServer.Logger; } }

        public string ApplicationName { get { return _proxiedServer.ApplicationName; } }

        public string ApplicationVersion { get { return _proxiedServer.ApplicationVersion; } }

        public string SupportedCompression { get { return _proxiedServer.SupportedCompression; } set { _proxiedServer.SupportedCompression = value; } }
        public IList<string> SupportedObjects { get { return _proxiedServer.SupportedObjects; } set { _proxiedServer.SupportedObjects = value; } }
        public IList<string> SupportedEncodings { get { return _proxiedServer.SupportedEncodings; } set { _proxiedServer.SupportedEncodings = value; } }
        public Action<string> Output { get { return _proxiedServer.Output; } set { _proxiedServer.Output = value; } }

        public event EventHandler<IEtpSession> SessionConnected {  add { _proxiedServer.SessionConnected += value; } remove { _proxiedServer.SessionConnected -= value; } }
        public event EventHandler<IEtpSession> SessionClosed { add { _proxiedServer.SessionClosed += value; } remove { _proxiedServer.SessionClosed -= value; } }

        public IList<ISupportedProtocol> GetSupportedProtocols(EtpVersion version)
        {
            return _proxiedServer.GetSupportedProtocols(version);
        }

        public string Log(string message)
        {
            return _proxiedServer.Log(message);
        }

        public string Log(string message, params object[] args)
        {
            return _proxiedServer.Log(message, args);
        }

        public void Register<TContract, THandler>()
            where TContract : IProtocolHandler
            where THandler : TContract
        {
            _proxiedServer.Register<TContract, THandler>();
        }

        public void Register<TContract>(Func<TContract> factory) where TContract : IProtocolHandler
        {
            _proxiedServer.Register<TContract>(factory);
        }

        public IEtpAdapter ResolveEtpAdapter(EtpVersion version)
        {
            return _proxiedServer.ResolveEtpAdapter(version);
        }

        public void Dispose()
        {
            _proxyServer.Dispose();
            _proxiedServer.Dispose();
        }

        #endregion
    }
}
