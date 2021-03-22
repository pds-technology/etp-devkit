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
using Avro.Specific;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Protocol.Core;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Provides information about an <see cref="IEtpSession"/> instance.
    /// </summary>
    public class EtpServerEventArgs : EventArgs
    {
        /// <summary>
        /// Whether or not the session was successfully opened.
        /// </summary>
        public IEtpServer Server { get; }

        /// <summary>
        /// Initializes a new <see cref="EtpServerEventArgs"/> instance.
        /// </summary>
        /// <param name="server">The ETP server.</param>
        public EtpServerEventArgs(IEtpServer server)
        {
            Server = server;
        }
    }

    /// <summary>
    /// Provides information about a session that has been opened.
    /// </summary>
    public class SessionOpenedEventArgs : EventArgs
    {
        /// <summary>
        /// Whether or not the session was successfully opened.
        /// </summary>
        public bool OpenedSuccessfully { get; }

        /// <summary>
        /// Initializes a new <see cref="SessionOpenedEventArgs"/> instance.
        /// </summary>
        /// <param name="openedSuccessfully">Whether or not the session was successfully opened.</param>
        public SessionOpenedEventArgs(bool openedSuccessfully)
        {
            OpenedSuccessfully = openedSuccessfully;
        }
    }

    /// <summary>
    /// Provides information about a binary data that has been received.
    /// </summary>
    public class DataReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Whether or not the session was successfully opened.
        /// </summary>
        public ArraySegment<byte> Data { get; }

        /// <summary>
        /// Initializes a new <see cref="DataReceivedEventArgs"/> instance.
        /// </summary>
        /// <param name="data">The data that has been received.</param>
        public DataReceivedEventArgs(ArraySegment<byte> data)
        {
            Data = data;
        }
    }

    /// <summary>
    /// Provides information about text data that has been received.
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Whether or not the session was successfully opened.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Initializes a new <see cref="MessageReceivedEventArgs"/> instance.
        /// </summary>
        /// <param name="message">The message that has been received.</param>
        public MessageReceivedEventArgs(string message)
        {
            Message = message;
        }
    }

    /// <summary>
    /// Provides information about a session that has been closed.
    /// </summary>
    public class SessionClosedEventArgs : EventArgs
    {
        /// <summary>
        /// Whether or not the session was successfully opened.
        /// </summary>
        public bool ClosedSuccessfully { get; }

        /// <summary>
        /// The reason provided when the session was closed.
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// Initializes a new <see cref="SessionOpenedEventArgs"/> instance.
        /// </summary>
        /// <param name="closedSuccessfully">Whether or not the session was successfully closed.</param>
        /// <param name="reason">The reason provided when the session was closed.</param>
        public SessionClosedEventArgs(bool closedSuccessfully, string reason)
        {
            ClosedSuccessfully = closedSuccessfully;
            Reason = reason;
        }
    }

    /// <summary>
    /// Provides information about a message that has been received.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class MessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MessageEventArgs(EtpMessage message)
        {
            Message = message;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public EtpMessage Message { get; }
    }

    /// <summary>
    /// Provides information about a message that has been received.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message body.</typeparam>
    /// <seealso cref="System.EventArgs" />
    public class MessageEventArgs<TMessage> : EventArgs where TMessage : ISpecificRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEventArgs{TMessage}"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MessageEventArgs(EtpMessage<TMessage> message)
        {
            Message = message;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public EtpMessage<TMessage> Message { get; }
    }

    /// <summary>
    /// Provides information about a fire and forget message that has been received.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message body.</typeparam>
    /// <seealso cref="System.EventArgs" />
    public class FireAndForgetEventArgs<TMessage> : EventArgs where TMessage : ISpecificRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FireAndForgetEventArgs{TMessage}"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public FireAndForgetEventArgs(EtpMessage<TMessage> message)
        {
            Message = message;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public EtpMessage<TMessage> Message { get; }
    }
}
