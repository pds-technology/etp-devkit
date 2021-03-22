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
using System.Collections.Generic;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Protocol.Core;
using Energistics.Etp.v12.Datatypes;

namespace Energistics.Etp.v12.Protocol.Transaction
{
    /// <summary>
    /// Base implementation of the <see cref="ITransactionCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.Transaction.ITransactionCustomer" />
    public class TransactionCustomerHandler : Etp12ProtocolHandlerWithCounterpartCapabilities<CapabilitiesStore, ICapabilitiesStore>, ITransactionCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionCustomerHandler"/> class.
        /// </summary>
        public TransactionCustomerHandler() : base((int)Protocols.Transaction, Roles.Customer, Roles.Store)
        {
            RegisterMessageHandler<StartTransactionResponse>(Protocols.Transaction, MessageTypes.Transaction.StartTransactionResponse, HandleStartTransactionResponse);
            RegisterMessageHandler<CommitTransactionResponse>(Protocols.Transaction, MessageTypes.Transaction.CommitTransactionResponse, HandleCommitTransactionResponse);
            RegisterMessageHandler<RollbackTransactionResponse>(Protocols.Transaction, MessageTypes.Transaction.RollbackTransactionResponse, HandleRollbackTransactionResponse);
        }

        /// <summary>
        /// Sends a StartTransaction message to a store.
        /// </summary>
        /// <param name="readOnly">Whether or not this transaction is read-only.</param>
        /// <param name="message">The message accompanying the transaction.</param>
        /// <param name="dataspaceUris">The URIs of the dataspaces to include in the transaction.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<StartTransaction> StartTransaction(bool readOnly, string message = "", IList<string> dataspaceUris = null, IMessageHeaderExtension extension = null)
        {
            var body = new StartTransaction
            {
                ReadOnly = readOnly,
                Message = message ?? string.Empty,
                DataspaceUris = dataspaceUris ?? new List<string> { string.Empty },
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Handles the StartTransactionResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<StartTransaction, StartTransactionResponse>> OnStartTransactionResponse;

        /// <summary>
        /// Sends a CommitTransaction message to a store.
        /// </summary>
        /// <param name="transactionUuid">The transaction UUID.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<CommitTransaction> CommitTransaction(Guid transactionUuid, IMessageHeaderExtension extension = null)
        {
            var body = new CommitTransaction
            {
                TransactionUuid = transactionUuid.ToUuid<Uuid>(),
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Handles the CommitTransactionResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<CommitTransaction, CommitTransactionResponse>> OnCommitTransactionResponse;

        /// <summary>
        /// Sends a RollbackTransaction message to a store.
        /// </summary>
        /// <param name="transactionUuid">The transaction UUID.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<RollbackTransaction> RollbackTransaction(Guid transactionUuid, IMessageHeaderExtension extension = null)
        {
            var body = new RollbackTransaction
            {
                TransactionUuid = transactionUuid.ToUuid<Uuid>(),
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Handles the RollbackTransactionResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<RollbackTransaction, RollbackTransactionResponse>> OnRollbackTransactionResponse;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<StartTransaction>)
                HandleResponseMessage(request as EtpMessage<StartTransaction>, message, OnStartTransactionResponse, HandleStartTransactionResponse);
            else if (request is EtpMessage<CommitTransaction>)
                HandleResponseMessage(request as EtpMessage<CommitTransaction>, message, OnCommitTransactionResponse, HandleCommitTransactionResponse);
            else if (request is EtpMessage<RollbackTransaction>)
                HandleResponseMessage(request as EtpMessage<RollbackTransaction>, message, OnRollbackTransactionResponse, HandleRollbackTransactionResponse);
        }

        /// <summary>
        /// Handles the StartTransactionResponse message from the server.
        /// </summary>
        /// <param name="message">The StartTransactionResponse message.</param>
        protected virtual void HandleStartTransactionResponse(EtpMessage<StartTransactionResponse> message)
        {
            var request = TryGetCorrelatedMessage<StartTransaction>(message);
            HandleResponseMessage(request, message, OnStartTransactionResponse, HandleStartTransactionResponse);
        }

        /// <summary>
        /// Handles the StartTransactionResponse message from a server.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{StartTransaction, StartTransactionResponse}"/> instance containing the event data.</param>
        protected virtual void HandleStartTransactionResponse(ResponseEventArgs<StartTransaction, StartTransactionResponse> args)
        {
        }

        /// <summary>
        /// Handles the CommitTransactionResponse message from the server.
        /// </summary>
        /// <param name="message">The CommitTransactionResponse message.</param>
        protected virtual void HandleCommitTransactionResponse(EtpMessage<CommitTransactionResponse> message)
        {
            var request = TryGetCorrelatedMessage<CommitTransaction>(message);
            HandleResponseMessage(request, message, OnCommitTransactionResponse, HandleCommitTransactionResponse);
        }

        /// <summary>
        /// Handles the CommitTransactionResponse message from a server.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{CommitTransaction, CommitTransactionResponse}"/> instance containing the event data.</param>
        protected virtual void HandleCommitTransactionResponse(ResponseEventArgs<CommitTransaction, CommitTransactionResponse> args)
        {
        }

        /// <summary>
        /// Handles the RollbackTransactionResponse message from the server.
        /// </summary>
        /// <param name="message">The RollbackTransactionResponse message.</param>
        protected virtual void HandleRollbackTransactionResponse(EtpMessage<RollbackTransactionResponse> message)
        {
            var request = TryGetCorrelatedMessage<RollbackTransaction>(message);
            HandleResponseMessage(request, message, OnRollbackTransactionResponse, HandleRollbackTransactionResponse);
        }

        /// <summary>
        /// Handles the RollbackTransactionResponse message from a server.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{RollbackTransaction, RollbackTransactionResponse}"/> instance containing the event data.</param>
        protected virtual void HandleRollbackTransactionResponse(ResponseEventArgs<RollbackTransaction, RollbackTransactionResponse> args)
        {
        }
    }
}
