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
using System.Threading;
using System.Threading.Tasks;
using Energistics.Etp.Common;
using Nito.AsyncEx;

namespace Energistics.Etp.Native
{
    /// <summary>
    /// A common base implementation for client and server <see cref="EtpSession"/>s using .NET WebSockets.
    /// </summary>
    public abstract class EtpSessionNativeBase : EtpSession
    {
        private const int BufferSize = 4096;

        private Task _receiveLoopTask;
        private readonly SemaphoreSlim _receiveLoopLock = new SemaphoreSlim(1, 1);
        private CancellationTokenSource _receiveLoopTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpSessionNativeBase"/> class.
        /// </summary>
        /// <param name="webSocket">The web socket.</param>
        /// <param name="etpVersion">The ETP version.</param>
        /// <param name="encoding">The ETP encoding for the session.</param>
        /// <param name="info">The endpoint's information.</param>
        /// <param name="parameters">The endpoint's parameters.</param>
        /// <param name="headers">The WebSocket or HTTP headers.</param>
        /// <param name="isClient">Whether or not this is the client-side of the session.</param>
        /// <param name="sessionId">The session ID if this is a server.</param>
        public EtpSessionNativeBase(EtpVersion etpVersion, EtpEncoding encoding, WebSocket webSocket, EtpEndpointInfo info, EtpEndpointParameters parameters, IDictionary<string, string> headers, bool isClient, string sessionId)
            : base(etpVersion, encoding, info, parameters, headers, isClient, sessionId, !isClient)
        {
            Socket = webSocket;
        }

        /// <summary>
        /// The cancellation token for the message receive loop.
        /// </summary>
        protected CancellationToken ReceiveLoopToken => _receiveLoopTokenSource.Token;

        /// <summary>
        /// Whether or not the receive loop has been stopped.
        /// </summary>
        private bool IsReceiveLoopStopped { get; set; } = false;

        /// <summary>
        /// The websocket for the session.
        /// </summary>
        protected WebSocket Socket { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the underlying websocket connection is open.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the underlying websocket is open; otherwise, <c>false</c>.
        /// </value>
        public override bool IsWebSocketOpen => (Socket?.State ?? WebSocketState.None) == WebSocketState.Open;

        /// <summary>
        /// Asynchronously closes the WebSocket connection for the specified reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        protected override async Task CloseWebSocketAsyncCore(string reason)
        {
            await CancelReceiveLoopAsync();

            if (!IsWebSocketOpen)
                return;

            try
            {
                await Socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, reason, CancellationToken.None).ConfigureAwait(CaptureAsyncContext);
                Logger.Debug(Log($"[{SessionKey}] Socket session closed."));
                RaiseSocketClosed();
            }
            catch (Exception ex)
            {
                if (ex.ExceptionMeansConnectionTerminated())
                {
                    RaiseSocketClosed();
                }
                else
                {
                    Log("Error: Exception caught when closing a websocket connection: {0}", ex.Message);
                    Logger.Debug($"[{SessionKey}] Exception caught when closing a websocket connection: {ex}");
                    RaiseSocketError(ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts background loop to receive messages
        /// </summary>
        protected void StartReceiveLoop()
        {
            if (IsReceiveLoopStopped)
            {
                Logger.Verbose($"[{SessionKey}] Receiving loop previously stopped.  Not restarting.");
                return;
            }

            try
            {
                Logger.Verbose($"[{SessionKey}] Acquiring receive loop lock in StartReceiveLoop.");

                _receiveLoopLock.Wait();

                if (IsReceiveLoopStopped)
                {
                    Logger.Verbose($"[{SessionKey}] Receiving loop already stopped.  Not restarting.");
                    return;
                }

                Logger.Verbose($"[{SessionKey}] Starting receive loop.");

                _receiveLoopTask = Task.Factory.StartNew(
                    async () => await ReceiveLoop(ReceiveLoopToken).ConfigureAwait(CaptureAsyncContext), ReceiveLoopToken,
                    TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach,
                    TaskScheduler.Default).Unwrap();
            }
            finally
            {
                Logger.Verbose($"[{SessionKey}] Releasing receive loop lock in StartReceiveLoop.");
                _receiveLoopLock.Release();
            }
        }

        /// <summary>
        /// Handles receiving messages on the WebSocket connection.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        /// <remarks>Runs an infinite loop to receive messages until the connection is closed.</remarks>
        [HandleProcessCorruptedStateExceptions]
        private async Task ReceiveLoop(CancellationToken token)
        {
            Logger.Debug($"[{SessionKey}] Entering receive loop.");

            try
            {
                using (var stream = new MemoryStream())
                {
                    var receiveBuffer = new byte[BufferSize];
                    while (Socket.State == WebSocketState.Open && !token.IsCancellationRequested)
                    {
                        var buffer = new ArraySegment<byte>(receiveBuffer);
                        var result = await Socket.ReceiveAsync(buffer, token).ConfigureAwait(CaptureAsyncContext);

                        // transfer received data to MemoryStream
                        stream.Write(buffer.Array, 0, result.Count);

                        // do not process data until EndOfMessage received
                        if (!result.EndOfMessage || result.CloseStatus.HasValue)
                            continue;

                        try
                        {
                            var data = new ArraySegment<byte>(stream.GetBuffer(), 0, (int)stream.Length);
                            Decode(data);
                        }
                        catch (Exception e)
                        {
                            Logger.Error($"[{SessionKey}] Error processing received data.", e);
                        }

                        // clear and reuse MemoryStream
                        stream.Clear();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Logger.Trace($"[{SessionKey}] Receive loop canceled.");
            }
            catch (Exception ex)
            {
                if (!ex.ExceptionMeansConnectionTerminated())
                {
                    Log("Error: {0}", ex.Message);
                    Logger.Debug($"[{SessionKey}] Error receiving messages: {ex}.");
                    RaiseSocketError(ex);
                    throw;
                }
            }
            finally
            {
                RaiseSocketClosed();
            }

            Logger.Trace($"[{SessionKey}] Exiting receive loop.");
        }

        /// <summary>
        /// Cancels the background message receive loop.
        /// </summary>
        private async Task CancelReceiveLoopAsync()
        {
            if (IsReceiveLoopStopped)
            {
                Logger.Verbose($"[{SessionKey}] Receiving loop previously stopped.  Not stopping again.");
                return;
            }

            try
            {
                Logger.Verbose($"[{SessionKey}] Acquiring receive loop lock in StopReceiveLoopAsync.");

                await _receiveLoopLock.WaitAsync();

                if (IsReceiveLoopStopped)
                {
                    Logger.Verbose($"[{SessionKey}] Receiving loop already stopped.  Not stopping again.");
                    return;
                }

                Logger.Debug($"[{SessionKey}] Cancelling receive loop.");

                _receiveLoopTokenSource?.Cancel();

                IsReceiveLoopStopped = true;
            }
            finally
            {
                Logger.Verbose($"[{SessionKey}] Releasing receive loop lock in StopReceiveLoopAsync.");
                _receiveLoopLock.Release();
            }
        }

        /// <summary>
        /// Sends the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        protected override async Task<bool> SendAsync(ArraySegment<byte> data)
        {
            CheckDisposed();

            try
            {
                await Socket.SendAsync(data, WebSocketMessageType.Binary, true, CancellationToken.None).ConfigureAwait(CaptureAsyncContext);
                return true;
            }
            catch (Exception ex)
            {
                if (ex.ExceptionMeansConnectionTerminated())
                {
                    RaiseSocketClosed();
                    return false;
                }
                else
                {
                    Log("Error: {0}", ex.Message);
                    Logger.Debug($"[{SessionKey}] Error sending message: {ex}.");
                    RaiseSocketError(ex);
                }
                throw;
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
                Logger.Verbose($"[{SessionKey}] Disposing EtpSessionNativeBase for {GetType().Name}");

                AsyncContext.Run(() => CancelReceiveLoopAsync());

                try
                {
                    Logger.Verbose($"[{SessionKey}] Waiting for receive loop to stop.");

                    if (_receiveLoopTask != null)
                        AsyncContext.Run(() => _receiveLoopTask);
                }
                catch (OperationCanceledException)
                {
                }
                finally
                {
                    _receiveLoopTask = null;
                    _receiveLoopTokenSource.Dispose();
                    _receiveLoopTokenSource = null;
                }

                Logger.Verbose($"[{SessionKey}] Receive loop stopped.");

                _receiveLoopLock.Dispose();

                Logger.Verbose($"[{SessionKey}] Disposed EtpSessionNativeBase for {GetType().Name}");
            }

            base.Dispose(disposing);
        }
    }
}
