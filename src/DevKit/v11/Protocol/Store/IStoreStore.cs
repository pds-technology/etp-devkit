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

namespace Energistics.Etp.v11.Protocol.Store
{
    /// <summary>
    /// Defines the interface that must be implemented by the store role of the Store protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.Store, Roles.Store, Roles.Customer)]
    public interface IStoreStore : IProtocolHandler
    {
        /// <summary>
        /// Sends an Object message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="dataObject">The data object.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<Object> Object(IMessageHeader correlatedHeader, DataObject dataObject);

        /// <summary>
        /// Handles the GetObject event from a customer.
        /// </summary>
        event EventHandler<RequestEventArgs<GetObject, DataObject>> OnGetObject;

        /// <summary>
        /// Handles the PutObject event from a customer.
        /// </summary>
        event EventHandler<VoidRequestEventArgs<PutObject>> OnPutObject;

        /// <summary>
        /// Handles the DeleteObject event from a customer.
        /// </summary>
        event EventHandler<VoidRequestEventArgs<DeleteObject>> OnDeleteObject;
    }
}
