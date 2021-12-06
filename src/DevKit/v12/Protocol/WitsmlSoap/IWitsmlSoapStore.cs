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
    /// Defines the interface that must be implemented by the store role of the WitsmlSoap protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.WitsmlSoap, Roles.Store, Roles.Customer)]
    public interface IWitsmlSoapStore : IProtocolHandler
    {
        /// <summary>
        /// Handles the WMLS_GetVersion event from a customer.
        /// </summary>
        event EventHandler<RequestEventArgs<WMLS_GetVersion, StringResponse>> OnWMLS_GetVersion;

        /// <summary>
        /// Sends a WMLS_GetVersionResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="result">The result.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<WMLS_GetVersionResponse> WMLS_GetVersionResponse(IMessageHeader correlatedHeader, string result, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the WMLS_GetCap event from a customer.
        /// </summary>
        event EventHandler<RequestEventArgs<WMLS_GetCap, GetCapResponse>> OnWMLS_GetCap;

        /// <summary>
        /// Sends a WMLS_GetVersionResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="result">The result code.</param>
        /// <param name="capabilitiesOut">The output capabilities.</param>
        /// <param name="suppMsgOut">The supplementary message out.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<WMLS_GetCapResponse> WMLS_GetCapResponse(IMessageHeader correlatedHeader, int result, string capabilitiesOut, string suppMsgOut, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the WMLS_GetBaseMsg event from a customer.
        /// </summary>
        event EventHandler<RequestEventArgs<WMLS_GetBaseMsg, StringResponse>> OnWMLS_GetBaseMsg;

        /// <summary>
        /// Sends a WMLS_GetBaseMsgResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="result">The result.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<WMLS_GetBaseMsgResponse> WMLS_GetBaseMsgResponse(IMessageHeader correlatedHeader, string result, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the WMLS_GetFromStore event from a customer.
        /// </summary>
        event EventHandler<RequestEventArgs<WMLS_GetFromStore, GetFromStoreResponse>> OnWMLS_GetFromStore;

        /// <summary>
        /// Sends a WMLS_GetFromStoreResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="result">The result code.</param>
        /// <param name="xmlOut">The XML result.</param>
        /// <param name="suppMsgOut">The supplementary message out.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<WMLS_GetFromStoreResponse> WMLS_GetFromStoreResponse(IMessageHeader correlatedHeader, int result, string xmlOut, string suppMsgOut, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the WMLS_AddToStore event from a customer.
        /// </summary>
        event EventHandler<RequestEventArgs<WMLS_AddToStore, BaseStoreResponse>> OnWMLS_AddToStore;

        /// <summary>
        /// Sends a WMLS_AddToStoreResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="result">The result code.</param>
        /// <param name="suppMsgOut">The supplementary message out.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<WMLS_AddToStoreResponse> WMLS_AddToStoreResponse(IMessageHeader correlatedHeader, int result, string suppMsgOut, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the WMLS_UpdateInStore event from a customer.
        /// </summary>
        event EventHandler<RequestEventArgs<WMLS_UpdateInStore, BaseStoreResponse>> OnWMLS_UpdateInStore;

        /// <summary>
        /// Sends a WMLS_UpdateInStoreResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="result">The result code.</param>
        /// <param name="suppMsgOut">The supplementary message out.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<WMLS_UpdateInStoreResponse> WMLS_UpdateInStoreResponse(IMessageHeader correlatedHeader, int result, string suppMsgOut, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the WMLS_DeleteFromStore event from a customer.
        /// </summary>
        event EventHandler<RequestEventArgs<WMLS_DeleteFromStore, BaseStoreResponse>> OnWMLS_DeleteFromStore;

        /// <summary>
        /// Sends a WMLS_DeleteFromStoreResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="result">The result code.</param>
        /// <param name="suppMsgOut">The supplementary message out.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<WMLS_DeleteFromStoreResponse> WMLS_DeleteFromStoreResponse(IMessageHeader correlatedHeader, int result, string suppMsgOut, IMessageHeaderExtension extension = null);
    }

    public class StringResponse
    {
        public string Result { get; set; }
    }

    public class BaseStoreResponse
    {
        public int Result { get; set; }

        public string SuppMsgOut { get; set; }
    }

    public class GetCapResponse : BaseStoreResponse
    {
        public string CapabilitiesOut { get; set; }
    }

    public class GetFromStoreResponse : BaseStoreResponse
    {
        public string XmlOut { get; set; }
    }
}
