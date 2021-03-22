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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using log4net;
using Nito.AsyncEx;

namespace Energistics.Etp.Native
{
    /// <summary>
    /// A self-hosted HTTP Listener ETP websocket server.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.EtpWebServerBase" />
    public class EtpSelfHostedWebServer : IEtpSelfHostedWebServer
    {
        private HttpListener _httpListener = new HttpListener();
        private Task _listenerTask;
        private CancellationTokenSource _source;

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpSocketServer"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="endpointInfo">The web server's endpoint information.</param>
        /// <param name="endpointParameters">The web server's endpoint parameters.</param>
        /// <param name="details">The web server's details.</param>
        public EtpSelfHostedWebServer(int port, EtpEndpointInfo endpointInfo, EtpEndpointParameters endpointParameters = null, EtpWebServerDetails details = null)
        {
            Logger = LogManager.GetLogger(GetType());

            Details = details ?? new EtpWebServerDetails();
            ServerManager = new EtpServerManager(Details, endpointInfo, endpointParameters);

            Uri = new Uri($"http://localhost:{port}");
        }

        /// <summary>
        /// Gets the logger used by this instance.
        /// </summary>
        /// <value>The logger instance.</value>
        public ILog Logger { get; }

        /// <summary>
        /// Gets the server manager for this instance.
        /// </summary>
        public IEtpServerManager ServerManager { get; }

        /// <summary>
        /// Gets the server's parameters.
        /// </summary>
        public EtpWebServerDetails Details { get; }

        /// <summary>
        /// The root URL for the server.
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the web server is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if the web server is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning
        {
            get
            {
                CheckDisposed();
                return _httpListener.IsListening;
            }
        }

        /// <summary>
        /// Starts the WebSocket server.
        /// </summary>
        public void Start()
        {
            AsyncContext.Run(() => StartAsync());
        }

        /// <summary>
        /// Asynchronously starts the WebSocket server.
        /// </summary>
        public Task StartAsync()
        {
            Logger.Trace($"Starting");
            if (!IsRunning)
            {
                try
                {
                    _httpListener.Prefixes.Add($"http://+:{Uri.Port}/");
                    _httpListener.Start();
                }
                catch (HttpListenerException) // Will happen when not run as administrator...
                {
                    Logger.Debug($"Could not listen on all addresses for port {Uri.Port}.  Falling back to listening on loopback addresses.  Run as administrator to listen on all addresses.");
                    _httpListener = new HttpListener();
                    _httpListener.Prefixes.Add(Uri.ToString());
                    _httpListener.Prefixes.Add($"http://127.0.0.1:{Uri.Port}/");
                    _httpListener.Start();
                }
                _source = new CancellationTokenSource();
                _listenerTask = Task.Factory.StartNew(
                    async () => await HandleListener(_source.Token).ConfigureAwait(false), _source.Token,
                    TaskCreationOptions.DenyChildAttach | TaskCreationOptions.LongRunning,
                    TaskScheduler.Default).Unwrap();
            }
            Logger.Verbose($"Started");

            return Task.FromResult(true);
        }

        /// <summary>
        /// Stops the WebSocket server.
        /// </summary>
        public void Stop()
        {
            AsyncContext.Run(() => StopAsync());
        }

        /// <summary>
        /// Asynchronously stops the WebSocket server.
        /// </summary>
        public async Task StopAsync()
        {
            Logger.Trace($"Stopping");
            if (IsRunning)
            {
                _source.Cancel();

                await ServerManager.StopAllAsync("Stopping server").ConfigureAwait(false);

                _httpListener.Stop();

                try { await _listenerTask.ConfigureAwait(false); }
                catch (OperationCanceledException) { }

                _source.Dispose();
                _source = null;
                _listenerTask = null;
            }
            Logger.Verbose($"Stopped");
        }

        private static IDictionary<string, string> GetCombinedHeaders(NameValueCollection headers, NameValueCollection queryString)
        {
            var combined = new Dictionary<string, string>();

            foreach (var key in queryString.AllKeys)
                if (!string.IsNullOrWhiteSpace(key))
                    combined[key] = queryString[key];

            foreach (var key in headers.AllKeys)
                if (!string.IsNullOrWhiteSpace(key))
                    combined[key] = headers[key];

            return combined;
        }

        private void CleanUpContext(HttpListenerContext context)
        {
            context.Request.InputStream.Close();
            context.Response.Close();
        }

        private async Task HandleListener(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await HandleListenerCore(token);
            }
        }

        private static bool IsServerCapabilitiesRequest(HttpListenerRequest request)
        {
            if (request.HttpMethod == "GET" && request.Url.AbsolutePath == "/.well-known/etp-server-capabilities")
                return true;

            return false;
        }

        private void HandleServerCapabilitiesRequest(HttpListenerRequest request, HttpListenerResponse response, IDictionary<string, string> headers)
        {
            if (headers.ContainsKey(EtpHeaders.GetVersions) && string.Equals(headers[EtpHeaders.GetVersions], "true", StringComparison.OrdinalIgnoreCase))
            {
                response.ContentType = "application/json";
                var @string = EtpExtensions.Serialize(EtpWebSocketValidation.GetSupportedSubProtocols(Details.SupportedVersions));
                var bytes = System.Text.Encoding.UTF8.GetBytes(@string);
                response.ContentLength64 = bytes.Length;
                response.OutputStream.Write(bytes, 0, bytes.Length);
            }
            else
            {
                var version = EtpVersion.v11;
                if (headers.ContainsKey(EtpHeaders.GetVersion) && string.Equals(headers[EtpHeaders.GetVersion], EtpSubProtocols.v12, StringComparison.OrdinalIgnoreCase))
                    version = EtpVersion.v12;

                var serverCapabilities = ServerManager.ServerCapabilities(version);
                response.ContentType = "application/json";
                var @string = EtpExtensions.Serialize(serverCapabilities);
                var bytes = System.Text.Encoding.UTF8.GetBytes(@string);
                response.ContentLength64 = bytes.Length;
                response.OutputStream.Write(bytes, 0, bytes.Length);
            }
        }

        private async Task HandleListenerCore(CancellationToken token)
        {
            try
            {
                // TODO: Handle server cap URL
                HttpListenerContext context = await _httpListener.GetContextAsync().ConfigureAwait(false);
                if (token.IsCancellationRequested)
                {
                    CleanUpContext(context);
                    return;
                }

                var headers = GetCombinedHeaders(context.Request.Headers, context.Request.QueryString);
                if (IsServerCapabilitiesRequest(context.Request))
                {
                    HandleServerCapabilitiesRequest(context.Request, context.Response, headers);
                    CleanUpContext(context);
                    return;
                }

                if (!context.Request.IsWebSocketRequest)
                {
                    CleanUpContext(context);
                    return;
                }

                if (!EtpWebSocketValidation.IsWebSocketRequestUpgrading(headers))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.UpgradeRequired;
                    Logger.Debug($"Invalid web socket request");
                    context.Response.StatusDescription = "Invalid web socket request";
                    CleanUpContext(context);
                    return;
                }

                var preferredProtocol = EtpWebSocketValidation.GetPreferredSubProtocol(headers);
                if (preferredProtocol == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.StatusDescription = "Invalid web socket request";
                    Logger.Debug($"Invalid web socket request");
                    CleanUpContext(context);
                    return;
                }

                var encoding = EtpWebSocketValidation.GetEtpEncoding(headers);
                if (encoding == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
                    Logger.Debug($"Error getting ETP encoding.");
                    context.Response.StatusDescription = "Invalid etp-encoding header";
                    CleanUpContext(context);
                    return;
                }
                if (!Details.IsEncodingSupported(encoding.Value))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
                    Logger.Debug($"Encoding not supported: {encoding.Value}");
                    context.Response.StatusDescription = "Unsupported etp-encoding";
                    CleanUpContext(context);
                    return;
                }

                HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(preferredProtocol).ConfigureAwait(false);
                if (token.IsCancellationRequested)
                {
                    webSocketContext.WebSocket.Dispose();
                    context.Response.Close();
                    return;
                }

                var version = EtpWebSocketValidation.GetEtpVersion(preferredProtocol);
                if (!Details.IsVersionSupported(version))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
                    Logger.Debug($"Sub protocol not supported: {preferredProtocol}");
                    context.Response.StatusDescription = "Sub protocol not supported";
                    CleanUpContext(context);
                    return;
                }

                var ws = new EtpServerWebSocket { WebSocket = webSocketContext.WebSocket };
                var server = ServerManager.CreateServer(ws, version, encoding.Value, headers);
                server.Start();
            }
            catch (Exception ex)
            {
                if (!ex.ExceptionMeansConnectionTerminated())
                {
                    ServerManager.Log("Error: Exception caught when handling a websocket connection: {0}", ex.Message);
                    Logger.DebugFormat("Exception caught when handling a websocket connection: {0}", ex);
                    throw;
                }
            }
        }

        #region IDisposable Support

        private bool _disposedValue; // To detect redundant calls

        /// <summary>
        /// Checks whether the current instance has been disposed.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException"></exception>
        protected virtual void CheckDisposed()
        {
            if (_disposedValue)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposedValue)
            {
                Logger.Trace($"Disposing {GetType().Name}");
                AsyncContext.Run(() => StopAsync());
                _httpListener?.Close();
                _httpListener = null;
            }

            _disposedValue = true;
        }

        // NOTE: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~EtpBase() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // NOTE: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}
