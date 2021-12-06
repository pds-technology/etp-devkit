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
using Energistics.Etp.Common.Protocol.Core;
using System;

namespace Energistics.Etp.v11.Protocol.WitsmlSoap
{
    /// <summary>
    /// Base implementation of the <see cref="IWitsmlSoapCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp11ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v11.Protocol.WitsmlSoap.IWitsmlSoapCustomer" />
    public class WitsmlSoapCustomerHandler : Etp11ProtocolHandler, IWitsmlSoapCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WitsmlSoapCustomerHandler"/> class.
        /// </summary>
        public WitsmlSoapCustomerHandler() : base((int)Protocols.WitsmlSoap, Roles.Customer, Roles.Store)
        {
            RegisterMessageHandler<WMSL_GetVersionResponse>(Protocols.WitsmlSoap, MessageTypes.WitsmlSoap.WMSL_GetVersionResponse, HandleWMSL_GetVersionResponse);
            RegisterMessageHandler<WMSL_GetCapResponse>(Protocols.WitsmlSoap, MessageTypes.WitsmlSoap.WMSL_GetCapResponse, HandleWMSL_GetCapResponse);
            RegisterMessageHandler<WMSL_GetBaseMsgResponse>(Protocols.WitsmlSoap, MessageTypes.WitsmlSoap.WMSL_GetBaseMsgResponse, HandleWMSL_GetBaseMsgResponse);
            RegisterMessageHandler<WMSL_GetFromStoreResponse>(Protocols.WitsmlSoap, MessageTypes.WitsmlSoap.WMSL_GetFromStoreResponse, HandleWMSL_GetFromStoreResponse);
            RegisterMessageHandler<WMSL_AddToStoreResponse>(Protocols.WitsmlSoap, MessageTypes.WitsmlSoap.WMSL_AddToStoreResponse, HandleWMSL_AddToStoreResponse);
            RegisterMessageHandler<WMSL_UpdateInStoreResponse>(Protocols.WitsmlSoap, MessageTypes.WitsmlSoap.WMSL_UpdateInStoreResponse, HandleWMSL_UpdateInStoreResponse);
            RegisterMessageHandler<WMSL_DeleteFromStoreResponse>(Protocols.WitsmlSoap, MessageTypes.WitsmlSoap.WMSL_DeleteFromStoreResponse, HandleWMSL_DeleteFromStoreResponse);
        }

        /// <summary>
        /// Sends a WMLS_GetVersion message to a store.
        /// </summary>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<WMLS_GetVersion> WMLS_GetVersion()
        {
            var body = new WMLS_GetVersion()
            {
            };

            return SendRequest(body);
        }

        /// <summary>
        /// Handles the WMSL_GetVersionResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<WMLS_GetVersion, WMSL_GetVersionResponse>> OnWMSL_GetVersionResponse;

        /// <summary>
        /// Sends a WMLS_GetCap message to a store.
        /// </summary>
        /// <param name="optionsIn">The input options.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<WMLS_GetCap> WMLS_GetCap(string optionsIn)
        {
            var body = new WMLS_GetCap()
            {
                OptionsIn = optionsIn ?? string.Empty,
            };

            return SendRequest(body);
        }

        /// <summary>
        /// Handles the WMSL_GetCapResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<WMLS_GetCap, WMSL_GetCapResponse>> OnWMSL_GetCapResponse;

        /// <summary>
        /// Sends a WMLS_GetBaseMsg message to a store.
        /// </summary>
        /// <param name="returnValueIn">The input return value.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<WMLS_GetBaseMsg> WMLS_GetBaseMsg(int returnValueIn)
        {
            var body = new WMLS_GetBaseMsg()
            {
                ReturnValueIn = returnValueIn,
            };

            return SendRequest(body);
        }

        /// <summary>
        /// Handles the WMSL_GetBaseMsgResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<WMLS_GetBaseMsg, WMSL_GetBaseMsgResponse>> OnWMSL_GetBaseMsgResponse;

        /// <summary>
        /// Sends a WMLS_GetFromStore message to a store.
        /// </summary>
        /// <param name="wmlTypeIn">The input WITSML type.</param>
        /// <param name="xmlIn">The input XML.</param>
        /// <param name="optionsIn">The input options.</param>
        /// <param name="capabilitiesIn">The input capabilities.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<WMLS_GetFromStore> WMLS_GetFromStore(string wmlTypeIn, string xmlIn, string optionsIn, string capabilitiesIn)
        {
            var body = new WMLS_GetFromStore()
            {
                WMLtypeIn = wmlTypeIn ?? string.Empty,
                XMLin = xmlIn ?? string.Empty,
                OptionsIn = optionsIn ?? string.Empty,
                CapabilitiesIn = capabilitiesIn ?? string.Empty,
            };

            return SendRequest(body);
        }

        /// <summary>
        /// Handles the WMSL_GetFromStoreResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<WMLS_GetFromStore, WMSL_GetFromStoreResponse>> OnWMSL_GetFromStoreResponse;

        /// <summary>
        /// Sends a WMLS_AddToStore message to a store.
        /// </summary>
        /// <param name="wmlTypeIn">The input WITSML type.</param>
        /// <param name="xmlIn">The input XML.</param>
        /// <param name="optionsIn">The input options.</param>
        /// <param name="capabilitiesIn">The input capabilities.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<WMLS_AddToStore> WMLS_AddToStore(string wmlTypeIn, string xmlIn, string optionsIn, string capabilitiesIn)
        {
            var body = new WMLS_AddToStore()
            {
                WMLtypeIn = wmlTypeIn ?? string.Empty,
                XMLin = xmlIn ?? string.Empty,
                OptionsIn = optionsIn ?? string.Empty,
                CapabilitiesIn = capabilitiesIn ?? string.Empty,
            };

            return SendRequest(body);
        }

        /// <summary>
        /// Handles the WMSL_AddToStoreResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<WMLS_AddToStore, WMSL_AddToStoreResponse>> OnWMSL_AddToStoreResponse;

        /// <summary>
        /// Sends a WMLS_UpdateInStore message to a store.
        /// </summary>
        /// <param name="wmlTypeIn">The input WITSML type.</param>
        /// <param name="xmlIn">The input XML.</param>
        /// <param name="optionsIn">The input options.</param>
        /// <param name="capabilitiesIn">The input capabilities.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<WMLS_UpdateInStore> WMLS_UpdateInStore(string wmlTypeIn, string xmlIn, string optionsIn, string capabilitiesIn)
        {
            var body = new WMLS_UpdateInStore()
            {
                WMLtypeIn = wmlTypeIn ?? string.Empty,
                XMLin = xmlIn ?? string.Empty,
                OptionsIn = optionsIn ?? string.Empty,
                CapabilitiesIn = capabilitiesIn ?? string.Empty,
            };

            return SendRequest(body);
        }

        /// <summary>
        /// Handles the WMSL_UpdateInStoreResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<WMLS_UpdateInStore, WMSL_UpdateInStoreResponse>> OnWMSL_UpdateInStoreResponse;

        /// <summary>
        /// Sends a WMLS_DeleteFromStore message to a store.
        /// </summary>
        /// <param name="wmlTypeIn">The input WITSML type.</param>
        /// <param name="xmlIn">The input XML.</param>
        /// <param name="optionsIn">The input options.</param>
        /// <param name="capabilitiesIn">The input capabilities.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<WMLS_DeleteFromStore> WMLS_DeleteFromStore(string wmlTypeIn, string xmlIn, string optionsIn, string capabilitiesIn)
        {
            var body = new WMLS_DeleteFromStore()
            {
                WMLtypeIn = wmlTypeIn ?? string.Empty,
                XMLin = xmlIn ?? string.Empty,
                OptionsIn = optionsIn ?? string.Empty,
                CapabilitiesIn = capabilitiesIn ?? string.Empty,
            };

            return SendRequest(body);
        }

        /// <summary>
        /// Handles the WMSL_DeleteFromStoreResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<WMLS_DeleteFromStore, WMSL_DeleteFromStoreResponse>> OnWMSL_DeleteFromStoreResponse;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<WMLS_GetVersion>)
                HandleResponseMessage(request as EtpMessage<WMLS_GetVersion>, message, OnWMSL_GetVersionResponse, HandleWMSL_GetVersionResponse);
            else if (request is EtpMessage<WMLS_GetCap>)
                HandleResponseMessage(request as EtpMessage<WMLS_GetCap>, message, OnWMSL_GetCapResponse, HandleWMSL_GetCapResponse);
            else if (request is EtpMessage<WMLS_GetBaseMsg>)
                HandleResponseMessage(request as EtpMessage<WMLS_GetBaseMsg>, message, OnWMSL_GetBaseMsgResponse, HandleWMSL_GetBaseMsgResponse);
            else if (request is EtpMessage<WMLS_GetFromStore>)
                HandleResponseMessage(request as EtpMessage<WMLS_GetFromStore>, message, OnWMSL_GetFromStoreResponse, HandleWMSL_GetFromStoreResponse);
            else if (request is EtpMessage<WMLS_AddToStore>)
                HandleResponseMessage(request as EtpMessage<WMLS_AddToStore>, message, OnWMSL_AddToStoreResponse, HandleWMSL_AddToStoreResponse);
            else if (request is EtpMessage<WMLS_UpdateInStore>)
                HandleResponseMessage(request as EtpMessage<WMLS_UpdateInStore>, message, OnWMSL_UpdateInStoreResponse, HandleWMSL_UpdateInStoreResponse);
            else if (request is EtpMessage<WMLS_DeleteFromStore>)
                HandleResponseMessage(request as EtpMessage<WMLS_DeleteFromStore>, message, OnWMSL_DeleteFromStoreResponse, HandleWMSL_DeleteFromStoreResponse);
        }

        /// <summary>
        /// Handles the WMSL_GetVersionResponse message from a store.
        /// </summary>
        /// <param name="message">The WMSL_GetVersionResponse message.</param>
        protected virtual void HandleWMSL_GetVersionResponse(EtpMessage<WMSL_GetVersionResponse> message)
        {
            var request = TryGetCorrelatedMessage<WMLS_GetVersion>(message);
            HandleResponseMessage(request, message, OnWMSL_GetVersionResponse, HandleWMSL_GetVersionResponse);
        }

        /// <summary>
        /// Handles the WMSL_GetVersionResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{WMLS_GetVersion, WMSL_GetVersionResponse}"/> instance containing the event data.</param>
        protected virtual void HandleWMSL_GetVersionResponse(ResponseEventArgs<WMLS_GetVersion, WMSL_GetVersionResponse> args)
        {
        }

        /// <summary>
        /// Handles the WMSL_GetCapResponse message from a store.
        /// </summary>
        /// <param name="message">The WMSL_GetCapResponse message.</param>
        protected virtual void HandleWMSL_GetCapResponse(EtpMessage<WMSL_GetCapResponse> message)
        {
            var request = TryGetCorrelatedMessage<WMLS_GetCap>(message);
            HandleResponseMessage(request, message, OnWMSL_GetCapResponse, HandleWMSL_GetCapResponse);
        }

        /// <summary>
        /// Handles the WMSL_GetCapResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{WMLS_GetCap, WMSL_GetCapResponse}"/> instance containing the event data.</param>
        protected virtual void HandleWMSL_GetCapResponse(ResponseEventArgs<WMLS_GetCap, WMSL_GetCapResponse> args)
        {
        }

        /// <summary>
        /// Handles the WMSL_GetBaseMsgResponse message from a store.
        /// </summary>
        /// <param name="message">The WMSL_GetBaseMsgResponse message.</param>
        protected virtual void HandleWMSL_GetBaseMsgResponse(EtpMessage<WMSL_GetBaseMsgResponse> message)
        {
            var request = TryGetCorrelatedMessage<WMLS_GetBaseMsg>(message);
            HandleResponseMessage(request, message, OnWMSL_GetBaseMsgResponse, HandleWMSL_GetBaseMsgResponse);
        }

        /// <summary>
        /// Handles the WMSL_GetBaseMsgResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{WMLS_GetBaseMsg, WMSL_GetBaseMsgResponse}"/> instance containing the event data.</param>
        protected virtual void HandleWMSL_GetBaseMsgResponse(ResponseEventArgs<WMLS_GetBaseMsg, WMSL_GetBaseMsgResponse> args)
        {
        }

        /// <summary>
        /// Handles the WMSL_GetFromStoreResponse message from a store.
        /// </summary>
        /// <param name="message">The WMSL_GetFromStoreResponse message.</param>
        protected virtual void HandleWMSL_GetFromStoreResponse(EtpMessage<WMSL_GetFromStoreResponse> message)
        {
            var request = TryGetCorrelatedMessage<WMLS_GetFromStore>(message);
            HandleResponseMessage(request, message, OnWMSL_GetFromStoreResponse, HandleWMSL_GetFromStoreResponse);
        }

        /// <summary>
        /// Handles the WMSL_GetFromStoreResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{WMLS_GetFromStore, WMSL_GetFromStoreResponse}"/> instance containing the event data.</param>
        protected virtual void HandleWMSL_GetFromStoreResponse(ResponseEventArgs<WMLS_GetFromStore, WMSL_GetFromStoreResponse> args)
        {
        }

        /// <summary>
        /// Handles the WMSL_AddToStoreResponse message from a store.
        /// </summary>
        /// <param name="message">The WMSL_AddToStoreResponse message.</param>
        protected virtual void HandleWMSL_AddToStoreResponse(EtpMessage<WMSL_AddToStoreResponse> message)
        {
            var request = TryGetCorrelatedMessage<WMLS_AddToStore>(message);
            HandleResponseMessage(request, message, OnWMSL_AddToStoreResponse, HandleWMSL_AddToStoreResponse);
        }

        /// <summary>
        /// Handles the WMSL_AddToStoreResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{WMLS_AddToStore, WMSL_AddToStoreResponse}"/> instance containing the event data.</param>
        protected virtual void HandleWMSL_AddToStoreResponse(ResponseEventArgs<WMLS_AddToStore, WMSL_AddToStoreResponse> args)
        {
        }

        /// <summary>
        /// Handles the WMSL_UpdateInStoreResponse message from a store.
        /// </summary>
        /// <param name="message">The WMSL_UpdateInStoreResponse message.</param>
        protected virtual void HandleWMSL_UpdateInStoreResponse(EtpMessage<WMSL_UpdateInStoreResponse> message)
        {
            var request = TryGetCorrelatedMessage<WMLS_UpdateInStore>(message);
            HandleResponseMessage(request, message, OnWMSL_UpdateInStoreResponse, HandleWMSL_UpdateInStoreResponse);
        }

        /// <summary>
        /// Handles the WMSL_UpdateInStoreResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{WMLS_UpdateInStore, WMSL_UpdateInStoreResponse}"/> instance containing the event data.</param>
        protected virtual void HandleWMSL_UpdateInStoreResponse(ResponseEventArgs<WMLS_UpdateInStore, WMSL_UpdateInStoreResponse> args)
        {
        }

        /// <summary>
        /// Handles the WMSL_DeleteFromStoreResponse message from a store.
        /// </summary>
        /// <param name="message">The WMSL_DeleteFromStoreResponse message.</param>
        protected virtual void HandleWMSL_DeleteFromStoreResponse(EtpMessage<WMSL_DeleteFromStoreResponse> message)
        {
            var request = TryGetCorrelatedMessage<WMLS_DeleteFromStore>(message);
            HandleResponseMessage(request, message, OnWMSL_DeleteFromStoreResponse, HandleWMSL_DeleteFromStoreResponse);
        }

        /// <summary>
        /// Handles the WMSL_DeleteFromStoreResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{WMLS_DeleteFromStore, WMSL_DeleteFromStoreResponse}"/> instance containing the event data.</param>
        protected virtual void HandleWMSL_DeleteFromStoreResponse(ResponseEventArgs<WMLS_DeleteFromStore, WMSL_DeleteFromStoreResponse> args)
        {
        }
    }
}
