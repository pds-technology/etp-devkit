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
    /// Base implementation of the <see cref="ITransactionStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.Transaction.ITransactionStore" />
    public class TransactionStoreHandler : Etp12ProtocolHandler, ITransactionStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionStoreHandler"/> class.
        /// </summary>
        public TransactionStoreHandler() : base((int)Protocols.Transaction, "store", "customer")
        {
            RegisterMessageHandler<StartTransaction>(Protocols.Transaction, MessageTypes.Transaction.StartTransaction, HandleStartTransaction);
            RegisterMessageHandler<CommitTransaction>(Protocols.Transaction, MessageTypes.Transaction.CommitTransaction, HandleCommitTransaction);
            RegisterMessageHandler<RollbackTransaction>(Protocols.Transaction, MessageTypes.Transaction.RollbackTransaction, HandleRollbackTransaction);
        }

        /// <summary>
        /// Handles the StartTransaction event from a customer.
        /// </summary>
        public event ProtocolEventHandler<StartTransaction, TransactionResponse> OnStartTransaction;

        /// <summary>
        /// Sends a StartTransactionResponse message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="transactionUuid">The transaction UUID.</param>
        /// <returns>The message identifier.</returns>
        public virtual long StartTransactionResponse(IMessageHeader request, Guid transactionUuid)
        {
            var header = CreateMessageHeader(Protocols.Transaction, MessageTypes.Transaction.StartTransactionResponse, request.MessageId);

            var response = new StartTransactionResponse
            {
                TransactionUuid = transactionUuid.ToUuid(),
            };

            return Session.SendMessage(header, response);
        }

        /// <summary>
        /// Handles the CommitTransaction event from a customer.
        /// </summary>
        public event ProtocolEventHandler<CommitTransaction, CommitResponse> OnCommitTransaction;

        /// <summary>
        /// Sends a CommitTransactionResponse message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="transactionUuid">The transaction UUID.</param>
        /// <param name="successful">A flag that indicates the success or failure of the transaction.</param>
        /// <param name="failureReason">An optional description explaining why or how the transaction failed.</param>
        /// <returns>The message identifier.</returns>
        public virtual long CommitTransactionResponse(IMessageHeader request, Guid transactionUuid, bool successful, string failureReason)
        {
            var header = CreateMessageHeader(Protocols.Transaction, MessageTypes.Transaction.CommitTransactionResponse, request.MessageId);

            var response = new CommitTransactionResponse
            {
                TransactionUuid = transactionUuid.ToUuid(),
                Successful = successful,
                FailureReason = failureReason,
            };

            return Session.SendMessage(header, response);
        }

        /// <summary>
        /// Handles the RollbackTransaction event from a customer.
        /// </summary>
        public event ProtocolEventHandler<RollbackTransaction> OnRollbackTransaction;

        /// <summary>
        /// Handles the StartTransaction message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The StartTransaction message.</param>
        protected virtual void HandleStartTransaction(IMessageHeader header, StartTransaction message)
        {
            var args = Notify(OnStartTransaction, header, message, new TransactionResponse { TransactionUuid = Guid.NewGuid() });
            if (args.Cancel)
                return;

            if (!HandleStartTransaction(header, message, args.Context))
                return;

            StartTransactionResponse(header, args.Context.TransactionUuid);
        }

        /// <summary>
        /// Handles the StartTransaction message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="response">The response.</param>
        protected virtual bool HandleStartTransaction(IMessageHeader header, StartTransaction message, TransactionResponse response)
        {
            return true;
        }

        /// <summary>
        /// Handles the CommitTransaction message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The CommitTransaction message.</param>
        protected virtual void HandleCommitTransaction(IMessageHeader header, CommitTransaction message)
        {
            var args = Notify(OnCommitTransaction, header, message, new CommitResponse { TransactionUuid = message.TransactionUuid.ToGuid() });
            if (args.Cancel)
                return;

            if (!HandleCommitTransaction(header, message, args.Context))
                return;

            CommitTransactionResponse(header, args.Context.TransactionUuid, args.Context.Successful, args.Context.FailureReason);
        }

        /// <summary>
        /// Handles the CommitTransaction message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="response">The response.</param>
        protected virtual bool HandleCommitTransaction(IMessageHeader header, CommitTransaction message, CommitResponse response)
        {
            return true;
        }

        /// <summary>
        /// Handles the RollbackTransaction message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The RollbackTransaction message.</param>
        protected virtual void HandleRollbackTransaction(IMessageHeader header, RollbackTransaction message)
        {
            Notify(OnRollbackTransaction, header, message);
        }
    }
}
