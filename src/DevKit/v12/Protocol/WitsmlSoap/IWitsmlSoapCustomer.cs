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
    /// Defines the interface that must be implemented by the customer role of the WitsmlSoap protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.WitsmlSoap, Roles.Customer, Roles.Store)]
    public interface IWitsmlSoapCustomer : IProtocolHandler
    {
        /// <summary>
        /// Sends a WMLS_GetVersion message to a store.
        /// </summary>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<WMLS_GetVersion> WMLS_GetVersion(IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the WMLS_GetVersionResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<WMLS_GetVersion, WMLS_GetVersionResponse>> OnWMLS_GetVersionResponse;

        /// <summary>
        /// Sends a WMLS_GetCap message to a store.
        /// </summary>
        /// <param name="optionsIn">The input options.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<WMLS_GetCap> WMLS_GetCap(string optionsIn, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the WMLS_GetCapResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<WMLS_GetCap, WMLS_GetCapResponse>> OnWMLS_GetCapResponse;

        /// <summary>
        /// Sends a WMLS_GetBaseMsg message to a store.
        /// </summary>
        /// <param name="returnValueIn">The input return value.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<WMLS_GetBaseMsg> WMLS_GetBaseMsg(int returnValueIn, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the WMLS_GetBaseMsgResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<WMLS_GetBaseMsg, WMLS_GetBaseMsgResponse>> OnWMLS_GetBaseMsgResponse;

        /// <summary>
        /// Sends a WMLS_GetFromStore message to a store.
        /// </summary>
        /// <param name="wmlTypeIn">The input WITSML type.</param>
        /// <param name="xmlIn">The input XML.</param>
        /// <param name="optionsIn">The input options.</param>
        /// <param name="capabilitiesIn">The input capabilities.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<WMLS_GetFromStore> WMLS_GetFromStore(string wmlTypeIn, string xmlIn, string optionsIn, string capabilitiesIn, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the WMLS_GetFromStoreResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<WMLS_GetFromStore, WMLS_GetFromStoreResponse>> OnWMLS_GetFromStoreResponse;

        /// <summary>
        /// Sends a WMLS_AddToStore message to a store.
        /// </summary>
        /// <param name="wmlTypeIn">The input WITSML type.</param>
        /// <param name="xmlIn">The input XML.</param>
        /// <param name="optionsIn">The input options.</param>
        /// <param name="capabilitiesIn">The input capabilities.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<WMLS_AddToStore> WMLS_AddToStore(string wmlTypeIn, string xmlIn, string optionsIn, string capabilitiesIn, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the WMLS_AddToStoreResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<WMLS_AddToStore, WMLS_AddToStoreResponse>> OnWMLS_AddToStoreResponse;

        /// <summary>
        /// Sends a WMLS_UpdateInStore message to a store.
        /// </summary>
        /// <param name="wmlTypeIn">The input WITSML type.</param>
        /// <param name="xmlIn">The input XML.</param>
        /// <param name="optionsIn">The input options.</param>
        /// <param name="capabilitiesIn">The input capabilities.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<WMLS_UpdateInStore> WMLS_UpdateInStore(string wmlTypeIn, string xmlIn, string optionsIn, string capabilitiesIn, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the WMLS_UpdateInStoreResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<WMLS_UpdateInStore, WMLS_UpdateInStoreResponse>> OnWMLS_UpdateInStoreResponse;

        /// <summary>
        /// Sends a WMLS_DeleteFromStore message to a store.
        /// </summary>
        /// <param name="wmlTypeIn">The input WITSML type.</param>
        /// <param name="xmlIn">The input XML.</param>
        /// <param name="optionsIn">The input options.</param>
        /// <param name="capabilitiesIn">The input capabilities.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<WMLS_DeleteFromStore> WMLS_DeleteFromStore(string wmlTypeIn, string xmlIn, string optionsIn, string capabilitiesIn, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the WMLS_DeleteFromStoreResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<WMLS_DeleteFromStore, WMLS_DeleteFromStoreResponse>> OnWMLS_DeleteFromStoreResponse;
    }
}
