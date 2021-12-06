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
using Energistics.Etp.v12.PrivateProtocols.WitsmlSoap;
using System;

namespace Energistics.Etp.v12.Protocol.WitsmlSoap
{
    /// <summary>
    /// Base implementation of the <see cref="IWitsmlSoapStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.WitsmlSoap.IWitsmlSoapStore" />
    public class WitsmlSoapStoreHandler : Etp12ProtocolHandler, IWitsmlSoapStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WitsmlSoapStoreHandler"/> class.
        /// </summary>
        public WitsmlSoapStoreHandler() : base((int)Protocols.WitsmlSoap, Roles.Store, Roles.Customer)
        {
            RegisterMessageHandler<WMLS_GetVersion>(Protocols.WitsmlSoap, MessageTypes.WitsmlSoap.WMLS_GetVersion, HandleWMLS_GetVersion);
            RegisterMessageHandler<WMLS_GetCap>(Protocols.WitsmlSoap, MessageTypes.WitsmlSoap.WMLS_GetCap, HandleWMLS_GetCap);
            RegisterMessageHandler<WMLS_GetBaseMsg>(Protocols.WitsmlSoap, MessageTypes.WitsmlSoap.WMLS_GetBaseMsg, HandleWMLS_GetBaseMsg);
            RegisterMessageHandler<WMLS_GetFromStore>(Protocols.WitsmlSoap, MessageTypes.WitsmlSoap.WMLS_GetFromStore, HandleWMLS_GetFromStore);
            RegisterMessageHandler<WMLS_AddToStore>(Protocols.WitsmlSoap, MessageTypes.WitsmlSoap.WMLS_AddToStore, HandleWMLS_AddToStore);
            RegisterMessageHandler<WMLS_UpdateInStore>(Protocols.WitsmlSoap, MessageTypes.WitsmlSoap.WMLS_UpdateInStore, HandleWMLS_UpdateInStore);
            RegisterMessageHandler<WMLS_DeleteFromStore>(Protocols.WitsmlSoap, MessageTypes.WitsmlSoap.WMLS_DeleteFromStore, HandleWMLS_DeleteFromStore);
        }

        /// <summary>
        /// Handles the WMLS_GetVersion event from a customer.
        /// </summary>
        public event EventHandler<RequestEventArgs<WMLS_GetVersion, StringResponse>> OnWMLS_GetVersion;

        /// <summary>
        /// Sends a WMLS_GetVersionResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="result">The result.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<WMLS_GetVersionResponse> WMLS_GetVersionResponse(IMessageHeader correlatedHeader, string result, IMessageHeaderExtension extension = null)
        {
            var body = new WMLS_GetVersionResponse()
            {
                Result = result ?? string.Empty,
            };

            return SendResponse(body, correlatedHeader, extension: extension);
        }

        /// <summary>
        /// Handles the WMLS_GetCap event from a customer.
        /// </summary>
        public event EventHandler<RequestEventArgs<WMLS_GetCap, GetCapResponse>> OnWMLS_GetCap;

        /// <summary>
        /// Sends a WMLS_GetVersionResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="result">The result code.</param>
        /// <param name="capabilitiesOut">The output capabilities.</param>
        /// <param name="suppMsgOut">The supplementary message out.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<WMLS_GetCapResponse> WMLS_GetCapResponse(IMessageHeader correlatedHeader, int result, string capabilitiesOut, string suppMsgOut, IMessageHeaderExtension extension = null)
        {
            var body = new WMLS_GetCapResponse()
            {
                Result = result,
                CapabilitiesOut = capabilitiesOut ?? string.Empty,
                SuppMsgOut = suppMsgOut ?? string.Empty,
            };

            return SendResponse(body, correlatedHeader, extension: extension);
        }

        /// <summary>
        /// Handles the WMLS_GetBaseMsg event from a customer.
        /// </summary>
        public event EventHandler<RequestEventArgs<WMLS_GetBaseMsg, StringResponse>> OnWMLS_GetBaseMsg;

        /// <summary>
        /// Sends a WMLS_GetBaseMsgResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="result">The result.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<WMLS_GetBaseMsgResponse> WMLS_GetBaseMsgResponse(IMessageHeader correlatedHeader, string result, IMessageHeaderExtension extension = null)
        {
            var body = new WMLS_GetBaseMsgResponse()
            {
                Result = result ?? string.Empty,
            };

            return SendResponse(body, correlatedHeader, extension: extension);
        }

        /// <summary>
        /// Handles the WMLS_GetFromStore event from a customer.
        /// </summary>
        public event EventHandler<RequestEventArgs<WMLS_GetFromStore, GetFromStoreResponse>> OnWMLS_GetFromStore;

        /// <summary>
        /// Sends a WMLS_GetFromStoreResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="result">The result code.</param>
        /// <param name="xmlOut">The XML result.</param>
        /// <param name="suppMsgOut">The supplementary message out.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<WMLS_GetFromStoreResponse> WMLS_GetFromStoreResponse(IMessageHeader correlatedHeader, int result, string xmlOut, string suppMsgOut, IMessageHeaderExtension extension = null)
        {
            var body = new WMLS_GetFromStoreResponse()
            {
                Result = result,
                XMLout = xmlOut ?? string.Empty,
                SuppMsgOut = suppMsgOut ?? string.Empty,
            };

            return SendResponse(body, correlatedHeader, extension: extension);
        }

        /// <summary>
        /// Handles the WMLS_AddToStore event from a customer.
        /// </summary>
        public event EventHandler<RequestEventArgs<WMLS_AddToStore, BaseStoreResponse>> OnWMLS_AddToStore;

        /// <summary>
        /// Sends a WMLS_AddToStoreResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="result">The result code.</param>
        /// <param name="suppMsgOut">The supplementary message out.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<WMLS_AddToStoreResponse> WMLS_AddToStoreResponse(IMessageHeader correlatedHeader, int result, string suppMsgOut, IMessageHeaderExtension extension = null)
        {
            var body = new WMLS_AddToStoreResponse()
            {
                Result = result,
                SuppMsgOut = suppMsgOut ?? string.Empty,
            };

            return SendResponse(body, correlatedHeader, extension: extension);
        }

        /// <summary>
        /// Handles the WMLS_UpdateInStore event from a customer.
        /// </summary>
        public event EventHandler<RequestEventArgs<WMLS_UpdateInStore, BaseStoreResponse>> OnWMLS_UpdateInStore;

        /// <summary>
        /// Sends a WMLS_UpdateInStoreResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="result">The result code.</param>
        /// <param name="suppMsgOut">The supplementary message out.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<WMLS_UpdateInStoreResponse> WMLS_UpdateInStoreResponse(IMessageHeader correlatedHeader, int result, string suppMsgOut, IMessageHeaderExtension extension = null)
        {
            var body = new WMLS_UpdateInStoreResponse()
            {
                Result = result,
                SuppMsgOut = suppMsgOut ?? string.Empty,
            };

            return SendResponse(body, correlatedHeader, extension: extension);
        }

        /// <summary>
        /// Handles the WMLS_DeleteFromStore event from a customer.
        /// </summary>
        public event EventHandler<RequestEventArgs<WMLS_DeleteFromStore, BaseStoreResponse>> OnWMLS_DeleteFromStore;

        /// <summary>
        /// Sends a WMLS_DeleteFromStoreResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="result">The result code.</param>
        /// <param name="suppMsgOut">The supplementary message out.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<WMLS_DeleteFromStoreResponse> WMLS_DeleteFromStoreResponse(IMessageHeader correlatedHeader, int result, string suppMsgOut, IMessageHeaderExtension extension = null)
        {
            var body = new WMLS_DeleteFromStoreResponse()
            {
                Result = result,
                SuppMsgOut = suppMsgOut ?? string.Empty,
            };

            return SendResponse(body, correlatedHeader, extension: extension);
        }

        /// <summary>
        /// Handles the WMLS_GetVersion message from a customer.
        /// </summary>
        /// <param name="message">The WMLS_GetVersion message.</param>
        protected virtual void HandleWMLS_GetVersion(EtpMessage<WMLS_GetVersion> message)
        {
            HandleRequestMessage(message, OnWMLS_GetVersion, HandleWMLS_GetVersion,
                responseMethod: (args) => WMLS_GetVersionResponse(args.Request?.Header, args.Response?.Result));
        }

        /// <summary>
        /// Handles the WMLS_GetVersion message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="RequestEventArgs{WMLS_GetVersion, StringResponse}"/> instance containing the event data.</param>
        protected virtual void HandleWMLS_GetVersion(RequestEventArgs<WMLS_GetVersion, StringResponse> args)
        {
        }

        /// <summary>
        /// Handles the WMLS_GetCap message from a customer.
        /// </summary>
        /// <param name="message">The WMLS_GetCap message.</param>
        protected virtual void HandleWMLS_GetCap(EtpMessage<WMLS_GetCap> message)
        {
            HandleRequestMessage(message, OnWMLS_GetCap, HandleWMLS_GetCap,
                responseMethod: (args) => WMLS_GetCapResponse(args.Request?.Header, args.Response?.Result ?? 0, args.Response?.CapabilitiesOut, args.Response?.SuppMsgOut));
        }

        /// <summary>
        /// Handles the WMLS_GetCap message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="RequestEventArgs{WMLS_GetCap, GetCapResponse}"/> instance containing the event data.</param>
        protected virtual void HandleWMLS_GetCap(RequestEventArgs<WMLS_GetCap, GetCapResponse> args)
        {
        }

        /// <summary>
        /// Handles the WMLS_GetBaseMsg message from a customer.
        /// </summary>
        /// <param name="message">The WMLS_GetBaseMsg message.</param>
        protected virtual void HandleWMLS_GetBaseMsg(EtpMessage<WMLS_GetBaseMsg> message)
        {
            HandleRequestMessage(message, OnWMLS_GetBaseMsg, HandleWMLS_GetBaseMsg,
                responseMethod: (args) => WMLS_GetBaseMsgResponse(args.Request?.Header, args.Response?.Result));
        }

        /// <summary>
        /// Handles the WMLS_GetBaseMsg message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="RequestEventArgs{WMLS_GetBaseMsg, StringResponse}"/> instance containing the event data.</param>
        protected virtual void HandleWMLS_GetBaseMsg(RequestEventArgs<WMLS_GetBaseMsg, StringResponse> args)
        {
        }

        /// <summary>
        /// Handles the WMLS_GetFromStore message from a customer.
        /// </summary>
        /// <param name="message">The WMLS_GetFromStore message.</param>
        protected virtual void HandleWMLS_GetFromStore(EtpMessage<WMLS_GetFromStore> message)
        {
            HandleRequestMessage(message, OnWMLS_GetFromStore, HandleWMLS_GetFromStore,
                responseMethod: (args) => WMLS_GetFromStoreResponse(args.Request?.Header, args.Response?.Result ?? 0, args.Response?.XmlOut, args.Response?.SuppMsgOut));
        }

        /// <summary>
        /// Handles the WMLS_GetFromStore message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="RequestEventArgs{WMLS_GetFromStore, GetFromStoreResponse}"/> instance containing the event data.</param>
        protected virtual void HandleWMLS_GetFromStore(RequestEventArgs<WMLS_GetFromStore, GetFromStoreResponse> args)
        {
        }

        /// <summary>
        /// Handles the WMLS_AddToStore message from a customer.
        /// </summary>
        /// <param name="message">The WMLS_AddToStore message.</param>
        protected virtual void HandleWMLS_AddToStore(EtpMessage<WMLS_AddToStore> message)
        {
            HandleRequestMessage(message, OnWMLS_AddToStore, HandleWMLS_AddToStore,
                responseMethod: (args) => WMLS_AddToStoreResponse(args.Request?.Header, args.Response?.Result ?? 0, args.Response?.SuppMsgOut));
        }

        /// <summary>
        /// Handles the WMLS_AddToStore message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="RequestEventArgs{WMLS_AddToStore, BaseStoreResponse}"/> instance containing the event data.</param>
        protected virtual void HandleWMLS_AddToStore(RequestEventArgs<WMLS_AddToStore, BaseStoreResponse> args)
        {
        }

        /// <summary>
        /// Handles the WMLS_UpdateInStore message from a customer.
        /// </summary>
        /// <param name="message">The WMLS_UpdateInStore message.</param>
        protected virtual void HandleWMLS_UpdateInStore(EtpMessage<WMLS_UpdateInStore> message)
        {
            HandleRequestMessage(message, OnWMLS_UpdateInStore, HandleWMLS_UpdateInStore,
                responseMethod: (args) => WMLS_UpdateInStoreResponse(args.Request?.Header, args.Response?.Result ?? 0, args.Response?.SuppMsgOut));
        }

        /// <summary>
        /// Handles the WMLS_UpdateInStore message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="RequestEventArgs{WMLS_UpdateInStore, BaseStoreResponse}"/> instance containing the event data.</param>
        protected virtual void HandleWMLS_UpdateInStore(RequestEventArgs<WMLS_UpdateInStore, BaseStoreResponse> args)
        {
        }

        /// <summary>
        /// Handles the WMLS_DeleteFromStore message from a customer.
        /// </summary>
        /// <param name="message">The WMLS_DeleteFromStore message.</param>
        protected virtual void HandleWMLS_DeleteFromStore(EtpMessage<WMLS_DeleteFromStore> message)
        {
            HandleRequestMessage(message, OnWMLS_DeleteFromStore, HandleWMLS_DeleteFromStore,
                responseMethod: (args) => WMLS_DeleteFromStoreResponse(args.Request?.Header, args.Response?.Result ?? 0, args.Response?.SuppMsgOut));
        }

        /// <summary>
        /// Handles the WMLS_DeleteFromStore message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="RequestEventArgs{WMLS_DeleteFromStore, BaseStoreResponse}"/> instance containing the event data.</param>
        protected virtual void HandleWMLS_DeleteFromStore(RequestEventArgs<WMLS_DeleteFromStore, BaseStoreResponse> args)
        {
        }
    }
}
