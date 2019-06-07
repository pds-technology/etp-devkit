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

namespace Energistics.Etp.Native
{
    /// <summary>
    /// A self-hosted HTTP Listener ETP websocket server.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.EtpWebServerBase" />
    public class EtpSelfHostedWebServer : EtpWebServerBase, IEtpSelfHostedWebServer
    {
        private int _port;
        private HttpListener _httpListener = new HttpListener();
        private Task _listenerTask;
        private CancellationTokenSource _source;
        private ConcurrentDictionary<string, EtpServer> _servers = new ConcurrentDictionary<string, EtpServer>();
        private ConcurrentDictionary<string, Task> _serverTasks = new ConcurrentDictionary<string, Task>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpSocketServer"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="application">The server application name.</param>
        /// <param name="version">The server application version.</param>
        public EtpSelfHostedWebServer(int port, string application, string version)
            : base(application, version)
        {
            _port = port;
            Uri = new Uri($"http://localhost:{port}");
        }

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
            StartAsync().Wait();
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
                    async () => await HandleListener(_source.Token).ConfigureAwait(CaptureAsyncContext), _source.Token,
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
            StopAsync().Wait();
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

                await CleanUpServers().ConfigureAwait(CaptureAsyncContext);

                _httpListener.Stop();

                try { await _listenerTask.ConfigureAwait(CaptureAsyncContext); }
                catch (OperationCanceledException) { }

                _source.Dispose();
                _source = null;
                _listenerTask = null;
            }
            Logger.Verbose($"Stopped");
        }

        private async Task CleanUpServers()
        {
            var keys = _servers.Keys.ToArray();
            var tasks = new List<Task>();
            var servers = new List<EtpServer>();

            // Wait for receiving to stop...
            foreach (var key in keys)
            {
                Task task;
                if (_serverTasks.TryRemove(key, out task))
                    tasks.Add(task);
            }

            try { await Task.WhenAll(tasks).ConfigureAwait(CaptureAsyncContext); }
            catch (OperationCanceledException) { }

            // Close sockets...
            tasks.Clear();
            foreach (var key in keys)
            {
                EtpServer server;
                if (_servers.TryRemove(key, out server))
                {
                    tasks.Add(server.CloseAsync("Shutting down"));
                    servers.Add(server);
                }
            }

            try { await Task.WhenAll(tasks).ConfigureAwait(CaptureAsyncContext); }
            catch (OperationCanceledException) { }

            foreach (var server in servers)
                server.Dispose();
        }

        private static IDictionary<string, string> GetWebSocketHeaders(NameValueCollection headers, NameValueCollection queryString)
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

        private void CleanUpServer(EtpServer server)
        {
            EtpServer s;
            _servers.TryRemove(server.SessionId, out s);
            Task t;
            _serverTasks.TryRemove(server.SessionId, out t);

            server.Dispose();
        }

        private void CleanUpContext(HttpListenerContext context)
        {
            context.Request.InputStream.Close();
            context.Response.Close();
        }

        private async Task HandleListener(CancellationToken token)
        {
            try
            {
                // TODO: Handle server cap URL
                HttpListenerContext context = await _httpListener.GetContextAsync().ConfigureAwait(CaptureAsyncContext);
                if (token.IsCancellationRequested)
                {
                    CleanUpContext(context);
                    return;
                }

                var headers = GetWebSocketHeaders(context.Request.Headers, context.Request.QueryString);
                if (!context.Request.IsWebSocketRequest)
                {
                    CleanUpContext(context);
                    return;
                }

                if (!EtpWebSocketValidation.IsWebSocketRequestUpgrading(headers))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.UpgradeRequired;
                    context.Response.StatusDescription = "Invalid web socket request";
                    CleanUpContext(context);
                    return;
                }

                var preferredProtocol = EtpWebSocketValidation.GetPreferredSubProtocol(headers);
                if (preferredProtocol == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.StatusDescription = "Invalid web socket request";
                    CleanUpContext(context);
                    return;
                }

                if (!EtpWebSocketValidation.IsEtpEncodingValid(headers))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
                    context.Response.StatusDescription = "Invalid etp-encoding header";
                    CleanUpContext(context);
                    return;
                }

                HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(preferredProtocol).ConfigureAwait(CaptureAsyncContext);
                if (token.IsCancellationRequested)
                {
                    webSocketContext.WebSocket.Dispose();
                    context.Response.Close();
                    return;
                }

                var server = new EtpServer(webSocketContext.WebSocket, ApplicationName, ApplicationVersion, headers);

                server.SupportedObjects = SupportedObjects;
                RegisterAll(server);

                _servers[server.SessionId] = server;
                _serverTasks[server.SessionId] = Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        InvokeSessionConnected(server);
                        await server.HandleConnection(token).ConfigureAwait(CaptureAsyncContext);
                    }
                    finally
                    {
                        InvokeSessionClosed(server);
                        CleanUpServer(server);
                        webSocketContext.WebSocket.Dispose();
                        CleanUpContext(context);
                    }
                }, token,
                    TaskCreationOptions.DenyChildAttach | TaskCreationOptions.LongRunning,
                    TaskScheduler.Default).Unwrap();
            }
            catch (Exception ex)
            {
                if (!ex.ExceptionMeansConnectionTerminated())
                {
                    Log("Error: Exception caught when handling a websocket connection: {0}", ex.Message);
                    Logger.DebugFormat("Exception caught when handling a websocket connection: {0}", ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopAsync().Wait();
                _httpListener?.Close();
                _httpListener = null;
            }

            base.Dispose(disposing);
        }

    }
}
