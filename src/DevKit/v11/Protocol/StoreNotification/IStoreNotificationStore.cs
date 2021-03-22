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
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v11.Datatypes.Object;
using System;

namespace Energistics.Etp.v11.Protocol.StoreNotification
{
    /// <summary>
    /// Defines the interface that must be implemented by the store role of the store notification protocol.
    /// </summary>
    /// <seealso cref="IProtocolHandler" />
    [ProtocolRole((int)Protocols.Store, Roles.Store, Roles.Customer)]
    public interface IStoreNotificationStore : IProtocolHandler
    {
        /// <summary>
        /// Handles the NotificationRequest event from a customer.
        /// </summary>
        event EventHandler<VoidRequestEventArgs<NotificationRequest>> OnNotificationRequest;

        /// <summary>
        /// Sends a ChangeNotification message to a customer.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="change">The object change.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ChangeNotification> ChangeNotification(Guid requestUuid, ObjectChange change);

        /// <summary>
        /// Sends a ChangeNotification message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="change">The object change.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ChangeNotification> ChangeNotification(IMessageHeader correlatedHeader, ObjectChange change);

        /// <summary>
        /// Sends a NotificationRequestDeleteNotification message to a customer.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="change">The object change.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<DeleteNotification> DeleteNotification(Guid requestUuid, ObjectChange change);

        /// <summary>
        /// Sends a NotificationRequestDeleteNotification message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="change">The object change.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<DeleteNotification> DeleteNotification(IMessageHeader correlatedHeader, ObjectChange change);

        /// <summary>
        /// Handles the CancelNotification event from a customer.
        /// </summary>
        event EventHandler<VoidRequestEventArgs<CancelNotification>> OnCancelNotification;
    }
}
