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

using Energistics.Etp.Common;
using SuperWebSocket;
using System;
using System.Collections.Generic;

namespace Energistics.Etp.WebSocket4Net
{
    /// <summary>
    /// Class representing a WebSocketForNet server-side WebSocket.
    /// </summary>
    public class EtpServerWebSocket : IEtpServerWebSocket
    {
        private readonly object _lock = new object();
        private bool _dataReceivedCallbackRegistered;
        private List<ArraySegment<byte>> _dataBuffer = new List<ArraySegment<byte>>();
        private Action<ArraySegment<byte>> _dataReceivedCallback;
        private Action _closedCallback;

        /// <summary>
        /// The server-side websocket session.
        /// </summary>
        public WebSocketSession WebSocketSession { get; set; }

        /// <summary>
        /// Registers the callback to call when the socket is closed.
        /// </summary>
        /// <param name="closedCallback">The callback to register.</param>
        public void RegisterClosedCallback(Action closedCallback)
        {
            _closedCallback = closedCallback;
        }

        /// <summary>
        /// Registers the callback to call when data is received.
        /// </summary>
        /// <param name="dataReceivedCallback">The callback to register.</param>
        public void RegisterDataReceivedCallback(Action<ArraySegment<byte>> dataReceivedCallback)
        {
            lock (_lock)
            {
                _dataReceivedCallback = dataReceivedCallback;
                _dataReceivedCallbackRegistered = dataReceivedCallback != null;

                if (_dataReceivedCallbackRegistered)
                {
                    foreach (var data in _dataBuffer)
                        _dataReceivedCallback.Invoke(data);

                    _dataBuffer.Clear();
                }
            }
        }

        /// <summary>
        /// Invokes the data received callback if one is registered.
        /// </summary>
        /// <param name="data">The data that was received.</param>
        public void DataReceived(ArraySegment<byte> data)
        {
            if (_dataReceivedCallbackRegistered)
                _dataReceivedCallback?.Invoke(data);
            else
            {
                lock(_lock)
                {
                    if (_dataReceivedCallbackRegistered)
                        _dataReceivedCallback?.Invoke(data);
                    else
                        _dataBuffer.Add(data);
                }
            }
        }

        /// <summary>
        /// Invokes the closed callback if one is registered.
        /// </summary>
        public void Closed() => _closedCallback?.Invoke();
    }
}
