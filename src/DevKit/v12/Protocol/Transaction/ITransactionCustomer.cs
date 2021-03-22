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
using System;
using System.Collections.Generic;

namespace Energistics.Etp.v12.Protocol.Transaction
{
    /// <summary>
    /// Describes the interface that must be implemented by the customer role of the Transaction protocol.
    /// </summary>
    /// <seealso cref="IProtocolHandler" />
    [ProtocolRole((int)Protocols.Transaction, Roles.Customer, Roles.Store)]
    public interface ITransactionCustomer : IProtocolHandlerWithCounterpartCapabilities<ICapabilitiesStore>
    {
        /// <summary>
        /// Sends a StartTransaction message to a store.
        /// </summary>
        /// <param name="readOnly">Whether or not this transaction is read-only.</param>
        /// <param name="message">The message accompanying the transaction.</param>
        /// <param name="dataspaceUris">The URIs of the dataspaces to include in the transaction.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<StartTransaction> StartTransaction(bool readOnly, string message = "", IList<string> dataspaceUris = null, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the StartTransactionResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<StartTransaction, StartTransactionResponse>> OnStartTransactionResponse;

        /// <summary>
        /// Sends a CommitTransaction message to a store.
        /// </summary>
        /// <param name="transactionUuid">The transaction UUID.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<CommitTransaction> CommitTransaction(Guid transactionUuid, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the CommitTransactionResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<CommitTransaction, CommitTransactionResponse>> OnCommitTransactionResponse;

        /// <summary>
        /// Sends a RollbackTransaction message to a store.
        /// </summary>
        /// <param name="transactionUuid">The transaction UUID.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<RollbackTransaction> RollbackTransaction(Guid transactionUuid, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the RollbackTransactionResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<RollbackTransaction, RollbackTransactionResponse>> OnRollbackTransactionResponse;
    }
}
