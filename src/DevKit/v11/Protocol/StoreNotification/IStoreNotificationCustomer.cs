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
    /// Defines the interface that must be implemented by the customer role of the store notification protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.StoreNotification, Roles.Customer, Roles.Store)]
    public interface IStoreNotificationCustomer : IProtocolHandler
    {
        /// <summary>
        /// Sends a NotificationRequest message to a store.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<NotificationRequest> NotificationRequest(NotificationRequestRecord request);

        /// <summary>
        /// Event raised when there is an exception received in response to a NotificationRequest message.
        /// </summary>
        event EventHandler<VoidResponseEventArgs<NotificationRequest>> OnNotificationRequestException;

        /// <summary>
        /// Handles the ChangeNotification event from a store.
        /// </summary>
        event EventHandler<NotificationEventArgs<NotificationRequestRecord, ChangeNotification>> OnChangeNotification;

        /// <summary>
        /// Handles the DeleteNotification event from a store.
        /// </summary>
        event EventHandler<NotificationEventArgs<NotificationRequestRecord, DeleteNotification>> OnDeleteNotification;

        /// <summary>
        /// Sends a CancelNotification message to a store.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<CancelNotification> CancelNotification(Guid requestUuid);

        /// <summary>
        /// Event raised when there is an exception received in response to a CancelNotification message.
        /// </summary>
        event EventHandler<VoidResponseEventArgs<CancelNotification>> OnCancelNotificationException;
    }
}
