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
using Energistics.Etp.Common.Protocol.Core;
using Energistics.Etp.v12.PrivateProtocols.WitsmlSoap;
using System;

namespace Energistics.Etp.v12.Protocol.WitsmlSoap
{
    /// <summary>
    /// Base implementation of the <see cref="IWitsmlSoapCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.WitsmlSoap.IWitsmlSoapCustomer" />
    public class WitsmlSoapCustomerHandler : Etp12ProtocolHandler, IWitsmlSoapCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WitsmlSoapCustomerHandler"/> class.
        /// </summary>
        public WitsmlSoapCustomerHandler() : base((int)Protocols.WitsmlSoap, Roles.Customer, Roles.Store)
        {
            RegisterMessageHandler<WMLS_GetVersionResponse>(Protocols.WitsmlSoap, MessageTypes.WitsmlSoap.WMLS_GetVersionResponse, HandleWMLS_GetVersionResponse);
            RegisterMessageHandler<WMLS_GetCapResponse>(Protocols.WitsmlSoap, MessageTypes.WitsmlSoap.WMLS_GetCapResponse, HandleWMLS_GetCapResponse);
            RegisterMessageHandler<WMLS_GetBaseMsgResponse>(Protocols.WitsmlSoap, MessageTypes.WitsmlSoap.WMLS_GetBaseMsgResponse, HandleWMLS_GetBaseMsgResponse);
            RegisterMessageHandler<WMLS_GetFromStoreResponse>(Protocols.WitsmlSoap, MessageTypes.WitsmlSoap.WMLS_GetFromStoreResponse, HandleWMLS_GetFromStoreResponse);
            RegisterMessageHandler<WMLS_AddToStoreResponse>(Protocols.WitsmlSoap, MessageTypes.WitsmlSoap.WMLS_AddToStoreResponse, HandleWMLS_AddToStoreResponse);
            RegisterMessageHandler<WMLS_UpdateInStoreResponse>(Protocols.WitsmlSoap, MessageTypes.WitsmlSoap.WMLS_UpdateInStoreResponse, HandleWMLS_UpdateInStoreResponse);
            RegisterMessageHandler<WMLS_DeleteFromStoreResponse>(Protocols.WitsmlSoap, MessageTypes.WitsmlSoap.WMLS_DeleteFromStoreResponse, HandleWMLS_DeleteFromStoreResponse);
        }

        /// <summary>
        /// Sends a WMLS_GetVersion message to a store.
        /// </summary>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<WMLS_GetVersion> WMLS_GetVersion(IMessageHeaderExtension extension = null)
        {
            var body = new WMLS_GetVersion()
            {
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Handles the WMLS_GetVersionResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<WMLS_GetVersion, WMLS_GetVersionResponse>> OnWMLS_GetVersionResponse;

        /// <summary>
        /// Sends a WMLS_GetCap message to a store.
        /// </summary>
        /// <param name="optionsIn">The input options.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<WMLS_GetCap> WMLS_GetCap(string optionsIn, IMessageHeaderExtension extension = null)
        {
            var body = new WMLS_GetCap()
            {
                OptionsIn = optionsIn ?? string.Empty,
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Handles the WMLS_GetCapResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<WMLS_GetCap, WMLS_GetCapResponse>> OnWMLS_GetCapResponse;

        /// <summary>
        /// Sends a WMLS_GetBaseMsg message to a store.
        /// </summary>
        /// <param name="returnValueIn">The input return value.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<WMLS_GetBaseMsg> WMLS_GetBaseMsg(int returnValueIn, IMessageHeaderExtension extension = null)
        {
            var body = new WMLS_GetBaseMsg()
            {
                ReturnValueIn = returnValueIn,
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Handles the WMLS_GetBaseMsgResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<WMLS_GetBaseMsg, WMLS_GetBaseMsgResponse>> OnWMLS_GetBaseMsgResponse;

        /// <summary>
        /// Sends a WMLS_GetFromStore message to a store.
        /// </summary>
        /// <param name="wmlTypeIn">The input WITSML type.</param>
        /// <param name="xmlIn">The input XML.</param>
        /// <param name="optionsIn">The input options.</param>
        /// <param name="capabilitiesIn">The input capabilities.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<WMLS_GetFromStore> WMLS_GetFromStore(string wmlTypeIn, string xmlIn, string optionsIn, string capabilitiesIn, IMessageHeaderExtension extension = null)
        {
            var body = new WMLS_GetFromStore()
            {
                WMLtypeIn = wmlTypeIn ?? string.Empty,
                XMLin = xmlIn ?? string.Empty,
                OptionsIn = optionsIn ?? string.Empty,
                CapabilitiesIn = capabilitiesIn ?? string.Empty,
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Handles the WMLS_GetFromStoreResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<WMLS_GetFromStore, WMLS_GetFromStoreResponse>> OnWMLS_GetFromStoreResponse;

        /// <summary>
        /// Sends a WMLS_AddToStore message to a store.
        /// </summary>
        /// <param name="wmlTypeIn">The input WITSML type.</param>
        /// <param name="xmlIn">The input XML.</param>
        /// <param name="optionsIn">The input options.</param>
        /// <param name="capabilitiesIn">The input capabilities.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<WMLS_AddToStore> WMLS_AddToStore(string wmlTypeIn, string xmlIn, string optionsIn, string capabilitiesIn, IMessageHeaderExtension extension = null)
        {
            var body = new WMLS_AddToStore()
            {
                WMLtypeIn = wmlTypeIn ?? string.Empty,
                XMLin = xmlIn ?? string.Empty,
                OptionsIn = optionsIn ?? string.Empty,
                CapabilitiesIn = capabilitiesIn ?? string.Empty,
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Handles the WMLS_AddToStoreResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<WMLS_AddToStore, WMLS_AddToStoreResponse>> OnWMLS_AddToStoreResponse;

        /// <summary>
        /// Sends a WMLS_UpdateInStore message to a store.
        /// </summary>
        /// <param name="wmlTypeIn">The input WITSML type.</param>
        /// <param name="xmlIn">The input XML.</param>
        /// <param name="optionsIn">The input options.</param>
        /// <param name="capabilitiesIn">The input capabilities.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<WMLS_UpdateInStore> WMLS_UpdateInStore(string wmlTypeIn, string xmlIn, string optionsIn, string capabilitiesIn, IMessageHeaderExtension extension = null)
        {
            var body = new WMLS_UpdateInStore()
            {
                WMLtypeIn = wmlTypeIn ?? string.Empty,
                XMLin = xmlIn ?? string.Empty,
                OptionsIn = optionsIn ?? string.Empty,
                CapabilitiesIn = capabilitiesIn ?? string.Empty,
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Handles the WMLS_UpdateInStoreResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<WMLS_UpdateInStore, WMLS_UpdateInStoreResponse>> OnWMLS_UpdateInStoreResponse;

        /// <summary>
        /// Sends a WMLS_DeleteFromStore message to a store.
        /// </summary>
        /// <param name="wmlTypeIn">The input WITSML type.</param>
        /// <param name="xmlIn">The input XML.</param>
        /// <param name="optionsIn">The input options.</param>
        /// <param name="capabilitiesIn">The input capabilities.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<WMLS_DeleteFromStore> WMLS_DeleteFromStore(string wmlTypeIn, string xmlIn, string optionsIn, string capabilitiesIn, IMessageHeaderExtension extension = null)
        {
            var body = new WMLS_DeleteFromStore()
            {
                WMLtypeIn = wmlTypeIn ?? string.Empty,
                XMLin = xmlIn ?? string.Empty,
                OptionsIn = optionsIn ?? string.Empty,
                CapabilitiesIn = capabilitiesIn ?? string.Empty,
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Handles the WMLS_DeleteFromStoreResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<WMLS_DeleteFromStore, WMLS_DeleteFromStoreResponse>> OnWMLS_DeleteFromStoreResponse;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<WMLS_GetVersion>)
                HandleResponseMessage(request as EtpMessage<WMLS_GetVersion>, message, OnWMLS_GetVersionResponse, HandleWMLS_GetVersionResponse);
            else if (request is EtpMessage<WMLS_GetCap>)
                HandleResponseMessage(request as EtpMessage<WMLS_GetCap>, message, OnWMLS_GetCapResponse, HandleWMLS_GetCapResponse);
            else if (request is EtpMessage<WMLS_GetBaseMsg>)
                HandleResponseMessage(request as EtpMessage<WMLS_GetBaseMsg>, message, OnWMLS_GetBaseMsgResponse, HandleWMLS_GetBaseMsgResponse);
            else if (request is EtpMessage<WMLS_GetFromStore>)
                HandleResponseMessage(request as EtpMessage<WMLS_GetFromStore>, message, OnWMLS_GetFromStoreResponse, HandleWMLS_GetFromStoreResponse);
            else if (request is EtpMessage<WMLS_AddToStore>)
                HandleResponseMessage(request as EtpMessage<WMLS_AddToStore>, message, OnWMLS_AddToStoreResponse, HandleWMLS_AddToStoreResponse);
            else if (request is EtpMessage<WMLS_UpdateInStore>)
                HandleResponseMessage(request as EtpMessage<WMLS_UpdateInStore>, message, OnWMLS_UpdateInStoreResponse, HandleWMLS_UpdateInStoreResponse);
            else if (request is EtpMessage<WMLS_DeleteFromStore>)
                HandleResponseMessage(request as EtpMessage<WMLS_DeleteFromStore>, message, OnWMLS_DeleteFromStoreResponse, HandleWMLS_DeleteFromStoreResponse);
        }

        /// <summary>
        /// Handles the WMLS_GetVersionResponse message from a store.
        /// </summary>
        /// <param name="message">The WMLS_GetVersionResponse message.</param>
        protected virtual void HandleWMLS_GetVersionResponse(EtpMessage<WMLS_GetVersionResponse> message)
        {
            var request = TryGetCorrelatedMessage<WMLS_GetVersion>(message);
            HandleResponseMessage(request, message, OnWMLS_GetVersionResponse, HandleWMLS_GetVersionResponse);
        }

        /// <summary>
        /// Handles the WMLS_GetVersionResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{WMLS_GetVersion, WMLS_GetVersionResponse}"/> instance containing the event data.</param>
        protected virtual void HandleWMLS_GetVersionResponse(ResponseEventArgs<WMLS_GetVersion, WMLS_GetVersionResponse> args)
        {
        }

        /// <summary>
        /// Handles the WMLS_GetCapResponse message from a store.
        /// </summary>
        /// <param name="message">The WMLS_GetCapResponse message.</param>
        protected virtual void HandleWMLS_GetCapResponse(EtpMessage<WMLS_GetCapResponse> message)
        {
            var request = TryGetCorrelatedMessage<WMLS_GetCap>(message);
            HandleResponseMessage(request, message, OnWMLS_GetCapResponse, HandleWMLS_GetCapResponse);
        }

        /// <summary>
        /// Handles the WMLS_GetCapResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{WMLS_GetCap, WMLS_GetCapResponse}"/> instance containing the event data.</param>
        protected virtual void HandleWMLS_GetCapResponse(ResponseEventArgs<WMLS_GetCap, WMLS_GetCapResponse> args)
        {
        }

        /// <summary>
        /// Handles the WMLS_GetBaseMsgResponse message from a store.
        /// </summary>
        /// <param name="message">The WMLS_GetBaseMsgResponse message.</param>
        protected virtual void HandleWMLS_GetBaseMsgResponse(EtpMessage<WMLS_GetBaseMsgResponse> message)
        {
            var request = TryGetCorrelatedMessage<WMLS_GetBaseMsg>(message);
            HandleResponseMessage(request, message, OnWMLS_GetBaseMsgResponse, HandleWMLS_GetBaseMsgResponse);
        }

        /// <summary>
        /// Handles the WMLS_GetBaseMsgResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{WMLS_GetBaseMsg, WMLS_GetBaseMsgResponse}"/> instance containing the event data.</param>
        protected virtual void HandleWMLS_GetBaseMsgResponse(ResponseEventArgs<WMLS_GetBaseMsg, WMLS_GetBaseMsgResponse> args)
        {
        }

        /// <summary>
        /// Handles the WMLS_GetFromStoreResponse message from a store.
        /// </summary>
        /// <param name="message">The WMLS_GetFromStoreResponse message.</param>
        protected virtual void HandleWMLS_GetFromStoreResponse(EtpMessage<WMLS_GetFromStoreResponse> message)
        {
            var request = TryGetCorrelatedMessage<WMLS_GetFromStore>(message);
            HandleResponseMessage(request, message, OnWMLS_GetFromStoreResponse, HandleWMLS_GetFromStoreResponse);
        }

        /// <summary>
        /// Handles the WMLS_GetFromStoreResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{WMLS_GetFromStore, WMLS_GetFromStoreResponse}"/> instance containing the event data.</param>
        protected virtual void HandleWMLS_GetFromStoreResponse(ResponseEventArgs<WMLS_GetFromStore, WMLS_GetFromStoreResponse> args)
        {
        }

        /// <summary>
        /// Handles the WMLS_AddToStoreResponse message from a store.
        /// </summary>
        /// <param name="message">The WMLS_AddToStoreResponse message.</param>
        protected virtual void HandleWMLS_AddToStoreResponse(EtpMessage<WMLS_AddToStoreResponse> message)
        {
            var request = TryGetCorrelatedMessage<WMLS_AddToStore>(message);
            HandleResponseMessage(request, message, OnWMLS_AddToStoreResponse, HandleWMLS_AddToStoreResponse);
        }

        /// <summary>
        /// Handles the WMLS_AddToStoreResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{WMLS_AddToStore, WMLS_AddToStoreResponse}"/> instance containing the event data.</param>
        protected virtual void HandleWMLS_AddToStoreResponse(ResponseEventArgs<WMLS_AddToStore, WMLS_AddToStoreResponse> args)
        {
        }

        /// <summary>
        /// Handles the WMLS_UpdateInStoreResponse message from a store.
        /// </summary>
        /// <param name="message">The WMLS_UpdateInStoreResponse message.</param>
        protected virtual void HandleWMLS_UpdateInStoreResponse(EtpMessage<WMLS_UpdateInStoreResponse> message)
        {
            var request = TryGetCorrelatedMessage<WMLS_UpdateInStore>(message);
            HandleResponseMessage(request, message, OnWMLS_UpdateInStoreResponse, HandleWMLS_UpdateInStoreResponse);
        }

        /// <summary>
        /// Handles the WMLS_UpdateInStoreResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{WMLS_UpdateInStore, WMLS_UpdateInStoreResponse}"/> instance containing the event data.</param>
        protected virtual void HandleWMLS_UpdateInStoreResponse(ResponseEventArgs<WMLS_UpdateInStore, WMLS_UpdateInStoreResponse> args)
        {
        }

        /// <summary>
        /// Handles the WMLS_DeleteFromStoreResponse message from a store.
        /// </summary>
        /// <param name="message">The WMLS_DeleteFromStoreResponse message.</param>
        protected virtual void HandleWMLS_DeleteFromStoreResponse(EtpMessage<WMLS_DeleteFromStoreResponse> message)
        {
            var request = TryGetCorrelatedMessage<WMLS_DeleteFromStore>(message);
            HandleResponseMessage(request, message, OnWMLS_DeleteFromStoreResponse, HandleWMLS_DeleteFromStoreResponse);
        }

        /// <summary>
        /// Handles the WMLS_DeleteFromStoreResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{WMLS_DeleteFromStore, WMLS_DeleteFromStoreResponse}"/> instance containing the event data.</param>
        protected virtual void HandleWMLS_DeleteFromStoreResponse(ResponseEventArgs<WMLS_DeleteFromStore, WMLS_DeleteFromStoreResponse> args)
        {
        }
    }
}
