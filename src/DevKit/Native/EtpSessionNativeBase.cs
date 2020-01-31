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
using System.IO;
using System.Net.WebSockets;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;

namespace Energistics.Etp.Native
{
    /// <summary>
    /// A common base implementation for client and server <see cref="EtpSession"/>s using .NET WebSockets.
    /// </summary>
    public abstract class EtpSessionNativeBase : EtpSession
    {
        private const int BufferSize = 4096;
        private int _socketOpenedEventCount = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpSessionNativeBase"/> class.
        /// </summary>
        /// <param name="webSocket">The web socket.</param>
        /// <param name="etpVersion">The ETP version.</param>
        /// <param name="application">The server application name.</param>
        /// <param name="version">The server application version.</param>
        /// <param name="headers">The WebSocket or HTTP headers.</param>
        /// <param name="isClient">Whether or not this is the client-side of the session.</param>
        public EtpSessionNativeBase(EtpVersion etpVersion, WebSocket webSocket, string application, string version, IDictionary<string, string> headers, bool isClient)
            : base(etpVersion, application, version, headers, isClient, !isClient)
        {
            Socket = webSocket;
        }

        /// <summary>
        /// The websocket for the session.
        /// </summary>
        protected WebSocket Socket { get; private set; }

        /// <summary>
        /// Occurs when the WebSocket is opened.
        /// </summary>
        public event EventHandler SocketOpened;

        /// <summary>
        /// Occurs when the WebSocket is closed.
        /// </summary>
        public event EventHandler SocketClosed;

        /// <summary>
        /// Occurs when the WebSocket has an error.
        /// </summary>
        public event EventHandler<Exception> SocketError;

        /// <summary>
        /// Gets a value indicating whether the connection is open.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the connection is open; otherwise, <c>false</c>.
        /// </value>
        public override bool IsOpen => (Socket?.State ?? WebSocketState.None) == WebSocketState.Open;

        /// <summary>
        /// Asynchronously closes the WebSocket connection for the specified reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        protected override async Task CloseAsyncCore(string reason)
        {
            if (!IsOpen)
                return;

            try
            {
                await Socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, reason, CancellationToken.None).ConfigureAwait(CaptureAsyncContext);
                Logger.Debug(Log("[{0}] Socket session closed.", SessionId));
                InvokeSocketClosed();
            }
            catch (Exception ex)
            {
                if (ex.ExceptionMeansConnectionTerminated())
                {
                    InvokeSocketClosed();
                }
                else
                {
                    Log("Error: Exception caught when closing a websocket connection: {0}", ex.Message);
                    Logger.DebugFormat("Exception caught when closing a websocket connection: {0}", ex);
                    InvokeSocketError(ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Called to let derived classes register details of a new connection.
        /// </summary>
        protected virtual void RegisterNewConnection()
        {
        }

        /// <summary>
        /// Called to let derived classes cleanup after a connection has ended.
        /// </summary>
        protected virtual void CleanupAfterConnection()
        {
        }

        /// <summary>
        /// Handles the WebSocket connection represented by the specified context.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        /// <remarks>Runs an infinite loop to handle communication until the connection is closed.</remarks>
        [HandleProcessCorruptedStateExceptions]
        public async Task HandleConnection(CancellationToken token)
        {
            try
            {
                RegisterNewConnection();

                using (var stream = new MemoryStream())
                {
                    while (Socket.State == WebSocketState.Open)
                    {
                        var buffer = new ArraySegment<byte>(new byte[BufferSize]);
                        var result = await Socket.ReceiveAsync(buffer, token).ConfigureAwait(CaptureAsyncContext);

                        // transfer received data to MemoryStream
                        stream.Write(buffer.Array, 0, result.Count);

                        // do not process data until EndOfMessage received
                        if (!result.EndOfMessage || result.CloseStatus.HasValue)
                            continue;

                        // filter null bytes from data buffer
                        var bytes = stream.GetBuffer();

                        try
                        {
                            if (result.MessageType == WebSocketMessageType.Binary)
                            {
                                OnDataReceived(bytes);
                            }
                            else // json encoding
                            {
                                var message = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                                OnMessageReceived(message);
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error($"Error processing received data.", e);
                        }

                        // clear and reuse MemoryStream
                        stream.Clear();
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                if (!ex.ExceptionMeansConnectionTerminated())
                {
                    Log("Error: {0}", ex.Message);
                    Logger.Debug(ex);
                    InvokeSocketError(ex);
                    throw;
                }
            }
            finally
            {
                InvokeSocketClosed();
                CleanupAfterConnection();
            }
        }

        /// <summary>
        /// Sends the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        protected override async Task SendAsync(byte[] data, int offset, int length)
        {
            CheckDisposed();

            var buffer = new ArraySegment<byte>(data, offset, length);

            try
            {
                await Socket.SendAsync(buffer, WebSocketMessageType.Binary, true, CancellationToken.None).ConfigureAwait(CaptureAsyncContext);
            }
            catch (Exception ex)
            {
                if (ex.ExceptionMeansConnectionTerminated())
                {
                    InvokeSocketClosed();
                }
                else
                {
                    Log("Error: {0}", ex.Message);
                    Logger.Debug(ex);
                    InvokeSocketError(ex);
                }
                throw;
            }
        }

        /// <summary>
        /// Sends the specified messages.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override async Task SendAsync(string message)
        {
            CheckDisposed();

            var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));

            try
            {
                await Socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None).ConfigureAwait(CaptureAsyncContext);
            }
            catch (Exception ex)
            {
                if (ex.ExceptionMeansConnectionTerminated())
                {
                    InvokeSocketClosed();
                }
                else
                {
                    Log("Error: {0}", ex.Message);
                    Logger.Debug(ex);
                    InvokeSocketError(ex);
                }
                throw;
            }
        }

        /// <summary>
        /// Raises the SocketOpened event.
        /// </summary>
        protected void InvokeSocketOpened()
        {
            var prevSocketOpenedEventCount = Interlocked.CompareExchange(ref _socketOpenedEventCount, 1, 0);

            if (prevSocketOpenedEventCount == 0)
                SocketOpened?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the SocketClosed event.
        /// </summary>
        protected void InvokeSocketClosed()
        {
            var prevSocketOpenedEventCount = Interlocked.CompareExchange(ref _socketOpenedEventCount, 0, 1);

            if (prevSocketOpenedEventCount == 1)
                SocketClosed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the SocketError event.
        /// </summary>
        /// <param name="ex">The socket exception.</param>
        protected void InvokeSocketError(Exception ex)
        {
            var prevSocketOpenedEventCount = Interlocked.CompareExchange(ref _socketOpenedEventCount, 0, 1);

            if (prevSocketOpenedEventCount == 1)
                SocketError?.Invoke(this, ex);
        }
    }
}
