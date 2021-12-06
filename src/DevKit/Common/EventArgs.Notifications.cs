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

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Provides data for protocol handler events.
    /// </summary>
    /// <typeparam name="TSubscription">The subscription type.</typeparam>
    /// <typeparam name="TNotification">The type of the notification message body.</typeparam>
    public class NotificationEventArgs<TSubscription, TNotification> : EventArgs
        where TNotification : IEtpMessageBody
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationEventArgs{T, TSubscription}"/> class.
        /// </summary>
        /// <param name="subscription">The original subscription.</param>
        /// <param name="notification">The notification message.</param>
        public NotificationEventArgs(EtpSubscription<TSubscription> subscription, EtpMessage<TNotification> notification)
        {
            Notification = notification;
            Subscription = subscription;
        }

        /// <summary>
        /// Gets the original subscription this message is a notification for.
        /// </summary>
        /// <value>The subscription message.</value>
        public EtpSubscription<TSubscription> Subscription { get; }

        /// <summary>
        /// Gets the notification message.
        /// </summary>
        /// <value>The notification message.</value>
        public EtpMessage<TNotification> Notification { get; }
    }

    /// <summary>
    /// Provides data for protocol handler events.
    /// </summary>
    /// <typeparam name="TSubscription">The subscription type.</typeparam>
    /// <typeparam name="TNotification">The type of the notification message body.</typeparam>
    /// <typeparam name="TData">Type type of the notification data message body.</typeparam>
    public class NotificationWithDataEventArgs<TSubscription, TNotification, TData> : NotificationEventArgs<TSubscription, TNotification>
        where TNotification : IEtpMessageBody
        where TData : IEtpMessageBody
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationEventArgs{T, TSubscription}"/> class.
        /// </summary>
        /// <param name="subscription">The original subscription.</param>
        /// <param name="notification">The notification message.</param>
        /// <param name="data">The notification data.</param>
        public NotificationWithDataEventArgs(EtpSubscription<TSubscription> subscription, EtpMessage<TNotification> notification, EtpMessage<TData> data)
            : base(subscription, notification)
        {
            Data = data;
        }

        /// <summary>
        /// Gets the notification data.
        /// </summary>
        public EtpMessage<TData> Data { get; }

    }
}
