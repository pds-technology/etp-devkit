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
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;

namespace Energistics.Etp.v12.Protocol.Transaction
{
    /// <summary>
    /// Describes the interface that must be implemented by the store role of the Transaction protocol.
    /// </summary>
    /// <seealso cref="IProtocolHandler" />
    [ProtocolRole((int)Protocols.Transaction, "store", "customer")]
    public interface ITransactionStore : IProtocolHandler
    {
        /// <summary>
        /// Handles the StartTransaction event from a customer.
        /// </summary>
        event ProtocolEventHandler<StartTransaction, TransactionResponse> OnStartTransaction;

        /// <summary>
        /// Sends a StartTransactionResponse message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="transactionUuid">The transaction UUID.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long StartTransactionResponse(IMessageHeader request, Guid transactionUuid);

        /// <summary>
        /// Handles the CommitTransaction event from a customer.
        /// </summary>
        event ProtocolEventHandler<CommitTransaction, CommitResponse> OnCommitTransaction;

        /// <summary>
        /// Sends a CommitTransactionResponse message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="transactionUuid">The transaction UUID.</param>
        /// <param name="successful">A flag that indicates the success or failure of the transaction.</param>
        /// <param name="failureReason">An optional description explaining why or how the transaction failed.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long CommitTransactionResponse(IMessageHeader request, Guid transactionUuid, bool successful, string failureReason);

        /// <summary>
        /// Handles the RollbackTransaction event from a customer.
        /// </summary>
        event ProtocolEventHandler<RollbackTransaction> OnRollbackTransaction;
    }

    /// <summary>
    /// Encapsulates the results of a discovery query.
    /// </summary>
    public class TransactionResponse
    {
        /// <summary>
        /// Gets or sets the transaction UUID.
        /// </summary>
        public Guid TransactionUuid{ get; set; }
    }

    /// <summary>
    /// Encapsulates the results of a discovery query.
    /// </summary>
    public class CommitResponse
    {
        /// <summary>
        /// Gets or sets the transaction UUID.
        /// </summary>
        public Guid TransactionUuid { get; set; }

        /// <summary>
        /// A flag that indicates the success or failure of the transaction.
        /// </summary>
        public bool Successful{ get; set; }

        /// <summary>
        /// An optional description explaining why or how the transaction failed.
        /// </summary>
        public string FailureReason { get; set; }
    }
}
