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
    /// Base implementation of the <see cref="ITransactionCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.Transaction.ITransactionCustomer" />
    public class TransactionCustomerHandler : Etp12ProtocolHandler, ITransactionCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionCustomerHandler"/> class.
        /// </summary>
        public TransactionCustomerHandler() : base((int)Protocols.Transaction, "customer", "store")
        {
            RegisterMessageHandler<StartTransactionResponse>(Protocols.Transaction, MessageTypes.Transaction.StartTransactionResponse, HandleStartTransactionResponse);
            RegisterMessageHandler<CommitTransactionResponse>(Protocols.Transaction, MessageTypes.Transaction.CommitTransactionResponse, HandleCommitTransactionResponse);
        }

        /// <summary>
        /// Sends a StartTransaction message to a store.
        /// </summary>
        /// <param name="readOnly">Whether or not this transaction is read-only.</param>
        /// <param name="message">The message accompanying the transaction.</param>
        /// <returns>The message identifier.</returns>
        public virtual long StartTransaction(bool readOnly, string message)
        {
            var header = CreateMessageHeader(Protocols.Transaction, MessageTypes.Transaction.StartTransaction);

            var startTransaction = new StartTransaction()
            {
                ReadOnly = readOnly,
                Message = message,
            };

            return Session.SendMessage(header, startTransaction);
        }

        /// <summary>
        /// Handles the StartTransactionResponse event from a store.
        /// </summary>
        public event ProtocolEventHandler<StartTransactionResponse> OnStartTransactionResponse;

        /// <summary>
        /// Sends a CommitTransaction message to a store.
        /// </summary>
        /// <param name="transactionUuid">The transaction UUID.</param>
        /// <returns>The message identifier.</returns>
        public virtual long CommitTransaction(Guid transactionUuid)
        {
            var header = CreateMessageHeader(Protocols.Transaction, MessageTypes.Transaction.CommitTransaction);

            var message = new CommitTransaction()
            {
                TransactionUuid = transactionUuid.ToUuid(),
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Handles the CommitTransactionResponse event from a store.
        /// </summary>
        public event ProtocolEventHandler<CommitTransactionResponse> OnCommitTransactionResponse;

        /// <summary>
        /// Sends a RollbackTransaction message to a store.
        /// </summary>
        /// <param name="transactionUuid">The transaction UUID.</param>
        /// <returns>The message identifier.</returns>
        public virtual long RollbackTransaction(Guid transactionUuid)
        {
            var header = CreateMessageHeader(Protocols.Transaction, MessageTypes.Transaction.RollbackTransaction);

            var message = new RollbackTransaction()
            {
                TransactionUuid = transactionUuid.ToUuid(),
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Handles the StartTransactionResponse message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The StartTransactionResponse message.</param>
        protected virtual void HandleStartTransactionResponse(IMessageHeader header, StartTransactionResponse message)
        {
            Notify(OnStartTransactionResponse, header, message);
        }

        /// <summary>
        /// Handles the CommitTransactionResponse message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The CommitTransactionResponse message.</param>
        protected virtual void HandleCommitTransactionResponse(IMessageHeader header, CommitTransactionResponse message)
        {
            Notify(OnCommitTransactionResponse, header, message);
        }
    }
}
