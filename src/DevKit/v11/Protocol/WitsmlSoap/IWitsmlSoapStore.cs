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

namespace Energistics.Etp.v11.Protocol.WitsmlSoap
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
        /// Sends a WMSL_GetVersionResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="result">The result.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<WMSL_GetVersionResponse> WMSL_GetVersionResponse(IMessageHeader correlatedHeader, string result);

        /// <summary>
        /// Handles the WMLS_GetCap event from a customer.
        /// </summary>
        event EventHandler<RequestEventArgs<WMLS_GetCap, GetCapResponse>> OnWMLS_GetCap;

        /// <summary>
        /// Sends a WMSL_GetVersionResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="result">The result code.</param>
        /// <param name="capabilitiesOut">The output capabilities.</param>
        /// <param name="suppMsgOut">The supplementary message out.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<WMSL_GetCapResponse> WMSL_GetCapResponse(IMessageHeader correlatedHeader, int result, string capabilitiesOut, string suppMsgOut);

        /// <summary>
        /// Handles the WMLS_GetBaseMsg event from a customer.
        /// </summary>
        event EventHandler<RequestEventArgs<WMLS_GetBaseMsg, StringResponse>> OnWMLS_GetBaseMsg;

        /// <summary>
        /// Sends a WMSL_GetBaseMsgResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="result">The result.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<WMSL_GetBaseMsgResponse> WMSL_GetBaseMsgResponse(IMessageHeader correlatedHeader, string result);

        /// <summary>
        /// Handles the WMLS_GetFromStore event from a customer.
        /// </summary>
        event EventHandler<RequestEventArgs<WMLS_GetFromStore, GetFromStoreResponse>> OnWMLS_GetFromStore;

        /// <summary>
        /// Sends a WMSL_GetFromStoreResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="result">The result code.</param>
        /// <param name="xmlOut">The XML result.</param>
        /// <param name="suppMsgOut">The supplementary message out.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<WMSL_GetFromStoreResponse> WMSL_GetFromStoreResponse(IMessageHeader correlatedHeader, int result, string xmlOut, string suppMsgOut);

        /// <summary>
        /// Handles the WMLS_AddToStore event from a customer.
        /// </summary>
        event EventHandler<RequestEventArgs<WMLS_AddToStore, BaseStoreResponse>> OnWMLS_AddToStore;

        /// <summary>
        /// Sends a WMSL_AddToStoreResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="result">The result code.</param>
        /// <param name="suppMsgOut">The supplementary message out.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<WMSL_AddToStoreResponse> WMSL_AddToStoreResponse(IMessageHeader correlatedHeader, int result, string suppMsgOut);

        /// <summary>
        /// Handles the WMLS_UpdateInStore event from a customer.
        /// </summary>
        event EventHandler<RequestEventArgs<WMLS_UpdateInStore, BaseStoreResponse>> OnWMLS_UpdateInStore;

        /// <summary>
        /// Sends a WMSL_UpdateInStoreResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="result">The result code.</param>
        /// <param name="suppMsgOut">The supplementary message out.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<WMSL_UpdateInStoreResponse> WMSL_UpdateInStoreResponse(IMessageHeader correlatedHeader, int result, string suppMsgOut);

        /// <summary>
        /// Handles the WMLS_DeleteFromStore event from a customer.
        /// </summary>
        event EventHandler<RequestEventArgs<WMLS_DeleteFromStore, BaseStoreResponse>> OnWMLS_DeleteFromStore;

        /// <summary>
        /// Sends a WMSL_DeleteFromStoreResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="result">The result code.</param>
        /// <param name="suppMsgOut">The supplementary message out.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<WMSL_DeleteFromStoreResponse> WMSL_DeleteFromStoreResponse(IMessageHeader correlatedHeader, int result, string suppMsgOut);
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
