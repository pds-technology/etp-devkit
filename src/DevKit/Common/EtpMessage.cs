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

using Energistics.Etp.Common.Datatypes;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Represents a complete ETP Message
    /// </summary>
    public class EtpMessage
    {
        /// <summary>
        /// Initializes a new <see cref="EtpMessage"/> instance.
        /// </summary>
        public EtpMessage()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="EtpMessage"/> instance.
        /// </summary>
        /// <param name="header">The message header</param>
        /// <param name="body">The message body</param>
        /// <param name="extension">The message extension</param>
        public EtpMessage(IMessageHeader header, Avro.Specific.ISpecificRecord body, IMessageHeaderExtension extension = null)
        {
            Header = header;
            Extension = extension;
            Body = body;
        }

        /// <summary>
        /// The message header.
        /// </summary>
        public IMessageHeader Header { get; set; }

        /// <summary>
        /// The message header extension or <c>null</c> if there is no header extension.
        /// </summary>
        public IMessageHeaderExtension Extension { get; set; }

        /// <summary>
        /// The message body.
        /// </summary>
        public Avro.Specific.ISpecificRecord Body { get; set; }

        /// <summary>
        /// Gets the ETP Version for this message.
        /// </summary>
        public EtpVersion EtpVersion => Header?.EtpVersion ?? EtpVersion.Unknown;

        /// <summary>
        /// Gets the name of this message.
        /// </summary>
        public string MessageName => Header.ToMessageName();
    }

    /// <summary>
    /// Represents a complete ETP Message
    /// </summary>
    public class EtpMessage<T> : EtpMessage
        where T : Avro.Specific.ISpecificRecord
    {
        /// <summary>
        /// Initializes a new <see cref="EtpMessage{T}"/> instance.
        /// </summary>
        public EtpMessage()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="EtpMessage{T}"/> instance.
        /// </summary>
        /// <param name="header">The message header</param>
        /// <param name="body">The message body</param>
        /// <param name="extension">The message extension</param>
        public EtpMessage(IMessageHeader header, T body, IMessageHeaderExtension extension = null)
            : base(header, body, extension: extension)
        {
        }

        /// <summary>
        /// The message body.
        /// </summary>
        new public T Body { get { return (T)base.Body; } set { base.Body = value; } }
    }
}
