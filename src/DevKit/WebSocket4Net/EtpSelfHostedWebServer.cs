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
using System.Threading.Tasks;
using Energistics.Etp.Common;
using log4net;
using Nito.AsyncEx;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperWebSocket;
using SuperWebSocket.SubProtocol;

namespace Energistics.Etp.WebSocket4Net
{
    /// <summary>
    /// A wrapper for the SuperWebSocket library providing a base ETP server implementation.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.EtpWebServerBase" />
    public class EtpSelfHostedWebServer : IEtpSelfHostedWebServer
    {
        private static readonly object EtpSessionKey = typeof(IEtpSession);
        private readonly object _sync = new object();
        private WebSocketServer _server;

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpSelfHostedWebServer"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="application">The server application name.</param>
        /// <param name="version">The server application version.</param>
        public EtpSelfHostedWebServer(int port, string application, string version)
        {
            Logger = LogManager.GetLogger(GetType());

            ServerManager = new EtpServerManager(application, version);

            _server = new WebSocketServer(EtpSettings.EtpSubProtocols.Select(p => new BasicSubProtocol(p)));

            _server.Setup(new ServerConfig
            {
                Ip = "Any",
                Port = port,
                MaxRequestLength = int.MaxValue,
            });

            _server.NewSessionConnected += OnNewSessionConnected;
            //_server.NewMessageReceived += OnNewMessageReceived;
            _server.NewDataReceived += OnNewDataReceived;
            _server.SessionClosed += OnSessionClosed;

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
        /// The root URL for the server.
        /// </summary>
        public Uri Uri { get; }

        /// <summary>
        /// Gets a value indicating whether the WebSocket server is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if the WebSocket server is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning
        {
            get
            {
                CheckDisposed();
                return _server.State == ServerState.Running;
            }
        }

        /// <summary>
        /// Starts the web server.
        /// </summary>
        public void Start()
        {
            AsyncContext.Run(() => StartAsync());
        }

        /// <summary>
        /// Asynchronously starts the web server.
        /// </summary>
        public Task StartAsync()
        {
            Logger.Trace($"Starting");
            if (!IsRunning)
            {
                _server.Start();
            }
            Logger.Verbose($"Started");

            return Task.FromResult(true);
        }

        /// <summary>
        /// Stops the web server.
        /// </summary>
        public void Stop()
        {
            AsyncContext.Run(() => StopAsync());
        }

        /// <summary>
        /// Asynchronously stops the web server.
        /// </summary>
        public Task StopAsync()
        {
            Logger.Trace($"Stopping");
            if (IsRunning)
            {
                CloseSessions();
                _server.Stop();
            }
            Logger.Verbose($"Stopped");

            return Task.FromResult(true);
        }

        /// <summary>
        /// Called when a WebSocket session is connected.
        /// </summary>
        /// <param name="session">The session.</param>
        private void OnNewSessionConnected(WebSocketSession session)
        {
            lock (_sync)
            {
                var headers = new Dictionary<string, string>();
                foreach (var item in session.Items)
                    headers[item.Key.ToString()] = item.Value.ToString();

                var server = new EtpServer(session, ServerManager.ApplicationName, ServerManager.ApplicationVersion, ServerManager.ServerKey, headers);

                session.Items[EtpSessionKey] = server;

                ServerManager.InitializeServer(server);
            }
        }

        /// <summary>
        /// Called when a WebSocket session is closed.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="value">The value.</param>
        private void OnSessionClosed(WebSocketSession session, CloseReason value)
        {
            lock (_sync)
            {
                var server = GetEtpServer(session);

                if (server != null)
                {
                    session.Items.Remove(EtpSessionKey);
                    ServerManager.ServerDisconnected(server);
                    server.Dispose();
                }
            }
        }

        /// <summary>
        /// Called when a new message is received.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="message">The message.</param>
        private void OnNewMessageReceived(WebSocketSession session, string message)
        {
            var server = GetEtpServer(session);
            server?.OnMessageReceived(message);
        }

        /// <summary>
        /// Called when new WebSocket data is received.
        /// </summary>
        /// <param name="session">The WebSocket session.</param>
        /// <param name="data">The data.</param>
        private void OnNewDataReceived(WebSocketSession session, byte[] data)
        {
            var server = GetEtpServer(session);
            server?.OnDataReceived(data);
        }

        /// <summary>
        /// Closes all connected WebSocket sessions.
        /// </summary>
        private void CloseSessions()
        {
            CheckDisposed();
            const string reason = "Server stopping";

            lock (_sync)
            {
                foreach (var session in _server.GetAllSessions())
                {
                    var server = GetEtpServer(session);
                    session.Items.Remove(EtpSessionKey);

                    if (server == null) continue;

                    server.CloseWebSocket(reason);
                    ServerManager.ServerDisconnected(server);
                    server.Dispose();
                }
            }
        }

        /// <summary>
        /// Gets the ETP session associated with the specified WebSocket session.
        /// </summary>
        /// <param name="session">The WebSocket session.</param>
        /// <returns>The <see cref="IEtpServer"/> associated with the WebSocket session.</returns>
        private IEtpServer GetEtpServer(WebSocketSession session)
        {
            IEtpServer etpSession = null;
            object item;

            lock (_sync)
            {
                if (session.Items.TryGetValue(EtpSessionKey, out item))
                {
                    etpSession = item as IEtpServer;
                }
            }

            return etpSession;
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
            if (disposing && !_disposedValue && _server != null)
            {
                Logger.Trace($"Disposing {GetType().Name}");

                // Unregister event handlers
                _server.NewSessionConnected -= OnNewSessionConnected;
                //_server.NewMessageReceived -= OnNewMessageReceived;
                _server.NewDataReceived -= OnNewDataReceived;
                _server.SessionClosed -= OnSessionClosed;

                Stop();
                _server.Dispose();
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
