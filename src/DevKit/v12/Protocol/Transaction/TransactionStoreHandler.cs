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
using Energistics.Etp.Common.Protocol.Core;
using Energistics.Etp.v12.Datatypes;

namespace Energistics.Etp.v12.Protocol.Transaction
{
    /// <summary>
    /// Base implementation of the <see cref="ITransactionStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.Transaction.ITransactionStore" />
    public class TransactionStoreHandler : Etp12ProtocolHandlerWithCapabilities<CapabilitiesStore, ICapabilitiesStore>, ITransactionStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionStoreHandler"/> class.
        /// </summary>
        public TransactionStoreHandler() : base((int)Protocols.Transaction, Roles.Store, Roles.Customer)
        {
            RegisterMessageHandler<StartTransaction>(Protocols.Transaction, MessageTypes.Transaction.StartTransaction, HandleStartTransaction);
            RegisterMessageHandler<CommitTransaction>(Protocols.Transaction, MessageTypes.Transaction.CommitTransaction, HandleCommitTransaction);
            RegisterMessageHandler<RollbackTransaction>(Protocols.Transaction, MessageTypes.Transaction.RollbackTransaction, HandleRollbackTransaction);
        }

        /// <summary>
        /// Handles the StartTransaction event from a customer.
        /// </summary>
        public event EventHandler<RequestEventArgs<StartTransaction, TransactionResponse>> OnStartTransaction;

        /// <summary>
        /// Sends a StartTransactionResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="transactionUuid">The transaction UUID.</param>
        /// <param name="successful">A flag that indicates the success or failure of the transaction.</param>
        /// <param name="failureReason">An optional description explaining why or how the transaction failed.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<StartTransactionResponse> StartTransactionResponse(IMessageHeader correlatedHeader, Guid transactionUuid, bool successful = true, string failureReason = "", IMessageHeaderExtension extension = null)
        {
            var body = new StartTransactionResponse
            {
                TransactionUuid = transactionUuid.ToUuid<Uuid>(),
                Successful = successful,
                FailureReason = failureReason ?? string.Empty,
            };

            return SendResponse(body, correlatedHeader, extension: extension);
        }

        /// <summary>
        /// Handles the CommitTransaction event from a customer.
        /// </summary>
        public event EventHandler<RequestEventArgs<CommitTransaction, TransactionResponse>> OnCommitTransaction;

        /// <summary>
        /// Sends a CommitTransactionResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="transactionUuid">The transaction UUID.</param>
        /// <param name="successful">A flag that indicates the success or failure of the transaction.</param>
        /// <param name="failureReason">An optional description explaining why or how the transaction failed.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<CommitTransactionResponse> CommitTransactionResponse(IMessageHeader correlatedHeader, Guid transactionUuid, bool successful = true, string failureReason = "", IMessageHeaderExtension extension = null)
        {
            var body = new CommitTransactionResponse
            {
                TransactionUuid = transactionUuid.ToUuid<Uuid>(),
                Successful = successful,
                FailureReason = failureReason ?? string.Empty,
            };

            return SendResponse(body, correlatedHeader, extension: extension);
        }

        /// <summary>
        /// Handles the RollbackTransaction event from a customer.
        /// </summary>
        public event EventHandler<RequestEventArgs<RollbackTransaction, TransactionResponse>> OnRollbackTransaction;

        /// <summary>
        /// Sends a RollbackTransactionResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="transactionUuid">The transaction UUID.</param>
        /// <param name="successful">A flag that indicates the success or failure of the transaction.</param>
        /// <param name="failureReason">An optional description explaining why or how the transaction failed.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<RollbackTransactionResponse> RollbackTransactionResponse(IMessageHeader correlatedHeader, Guid transactionUuid, bool successful = true, string failureReason = "", IMessageHeaderExtension extension = null)
        {
            var body = new RollbackTransactionResponse
            {
                TransactionUuid = transactionUuid.ToUuid<Uuid>(),
                Successful = successful,
                FailureReason = failureReason ?? string.Empty,
            };

            return SendResponse(body, correlatedHeader, extension: extension);
        }

        /// <summary>
        /// Handles the StartTransaction message from a client.
        /// </summary>
        /// <param name="message">The StartTransaction message.</param>
        protected virtual void HandleStartTransaction(EtpMessage<StartTransaction> message)
        {
            HandleRequestMessage(message, OnStartTransaction, HandleStartTransaction,
                responseMethod: (args) => StartTransactionResponse(args.Request?.Header, args.Response?.TransactionUuid ?? default(Guid), successful: args.Response?.Successful ?? false, failureReason: args.Response?.FailureReason, extension: args.ResponseExtension));
        }

        /// <summary>
        /// Handles the StartTransaction message from a client.
        /// </summary>
        /// <param name="args">The <see cref="RequestEventArgs{StartTransaction, TransactionResponse}"/> instance containing the event data.</param>
        protected virtual void HandleStartTransaction(RequestEventArgs<StartTransaction, TransactionResponse> args)
        {
        }

        /// <summary>
        /// Handles the CommitTransaction message from a client.
        /// </summary>
        /// <param name="message">The CommitTransaction message.</param>
        protected virtual void HandleCommitTransaction(EtpMessage<CommitTransaction> message)
        {
            HandleRequestMessage(message, OnCommitTransaction, HandleCommitTransaction,
                responseMethod: (args) => CommitTransactionResponse(args.Request?.Header, args.Response?.TransactionUuid ?? default(Guid), successful: args.Response?.Successful ?? false, failureReason: args.Response?.FailureReason, extension: args.ResponseExtension));
        }

        /// <summary>
        /// Handles the CommitTransaction message from a client.
        /// </summary>
        /// <param name="args">The <see cref="RequestEventArgs{CommitTransaction, TransactionResponse}"/> instance containing the event data.</param>
        protected virtual void HandleCommitTransaction(RequestEventArgs<CommitTransaction, TransactionResponse> args)
        {
        }

        /// <summary>
        /// Handles the RollbackTransaction message from a client.
        /// </summary>
        /// <param name="message">The RollbackTransaction message.</param>
        protected virtual void HandleRollbackTransaction(EtpMessage<RollbackTransaction> message)
        {
            HandleRequestMessage(message, OnRollbackTransaction, HandleRollbackTransaction,
                responseMethod: (args) => RollbackTransactionResponse(args.Request?.Header, args.Response?.TransactionUuid ?? default(Guid), successful: args.Response?.Successful ?? false, failureReason: args.Response?.FailureReason, extension: args.ResponseExtension));
        }

        /// <summary>
        /// Handles the RollbackTransaction message from a client.
        /// </summary>
        /// <param name="args">The <see cref="RequestEventArgs{RollbackTransaction, TransactionResponse}"/> instance containing the event data.</param>
        protected virtual void HandleRollbackTransaction(RequestEventArgs<RollbackTransaction, TransactionResponse> args)
        {
        }
    }
}
