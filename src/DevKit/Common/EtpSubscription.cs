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
using System;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Represents an ETP Subscription
    /// </summary>
    public class EtpSubscription
    {
        /// <summary>
        /// Initializes a new <see cref="EtpSubscription"/> instance.
        /// </summary>
        public EtpSubscription()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="EtpSubscription"/> instance.
        /// </summary>
        /// <param name="uuid">The subscription UUID.</param>
        /// <param name="message">The subscription message.</param>
        /// <param name="subscription">The subscription.</param>
        public EtpSubscription(Guid uuid, EtpMessage message, object subscription)
        {
            Uuid = uuid;
            Message = message;
            Subscription = subscription;
        }

        /// <summary>
        /// The subscription UUID.
        /// </summary>
        public Guid Uuid { get; set; }

        /// <summary>
        /// The subscription message.
        /// </summary>
        public EtpMessage Message { get; set; }

        /// <summary>
        /// The subscription.
        /// </summary>
        public object Subscription { get; set; }

        /// <summary>
        /// Gets the ETP Version for this subscription.
        /// </summary>
        public EtpVersion EtpVersion => Message?.EtpVersion ?? EtpVersion.Unknown;

        /// <summary>
        /// The subscription message header.
        /// </summary>
        public IMessageHeader Header => Message?.Header;

        /// <summary>
        /// Gets the name of the subscription message.
        /// </summary>
        public string MessageName => Message?.Header.ToMessageName();
    }

    /// <summary>
    /// Represents an ETP Subscription
    /// </summary>
    public class EtpSubscription<TSubscription> : EtpSubscription
    {
        /// <summary>
        /// Initializes a new <see cref="EtpSubscription{TSubscription}"/> instance.
        /// </summary>
        public EtpSubscription()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="EtpSubscription{TSubscription}"/> instance.
        /// </summary>
        /// <param name="uuid">The subscription UUID.</param>
        /// <param name="message">The subscription message.</param>
        /// <param name="subscription">The subscription.</param>
        public EtpSubscription(Guid uuid, EtpMessage message, TSubscription subscription)
            : base(uuid, message, subscription)
        {
        }

        /// <summary>
        /// The subscription.
        /// </summary>
        new public TSubscription Subscription { get { return (TSubscription)base.Subscription; } set { base.Subscription = value; } }
    }

    /// <summary>
    /// Represents an ETP Subscription
    /// </summary>
    public class EtpSubscription<TMessage, TSubscription> : EtpSubscription<TSubscription>
        where TMessage : Avro.Specific.ISpecificRecord
    {
        /// <summary>
        /// Initializes a new <see cref="EtpSubscription{TMessage, TSubscription}"/> instance.
        /// </summary>
        public EtpSubscription()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="EtpSubscription{TMessage, TSubscription}"/> instance.
        /// </summary>
        /// <param name="uuid">The subscription UUID.</param>
        /// <param name="message">The subscription message.</param>
        /// <param name="subscription">The subscription.</param>
        public EtpSubscription(Guid uuid, EtpMessage<TMessage> message, TSubscription subscription)
            : base(uuid, message, subscription)
        {
        }

        /// <summary>
        /// The subscription message.
        /// </summary>
        new public EtpMessage<TMessage> Message { get { return (EtpMessage<TMessage>)base.Message; } set { base.Message = value; } }
    }
}
