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
using Energistics.Etp.v12.Datatypes;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.GrowingObject
{
    /// <summary>
    /// Defines the interface that must be implemented by the store role of the growing object protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.GrowingObject, Roles.Store, Roles.Customer)]
    public interface IGrowingObjectStore : IProtocolHandler<ICapabilitiesStore, ICapabilitiesCustomer>
    {
        /// <summary>
        /// Handles the GetGrowingDataObjectsHeader event from a customer.
        /// </summary>
        event EventHandler<MapRequestEventArgs<GetGrowingDataObjectsHeader, DataObject>> OnGetGrowingDataObjectsHeader;

        /// <summary>
        /// Sends a GetGrowingDataObjectsHeaderResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="dataObjects">The data objects.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetGrowingDataObjectsHeaderResponse> GetGrowingDataObjectsHeaderResponse(IMessageHeader correlatedHeader, IDictionary<string, DataObject> dataObjects, bool isFinalPart = true, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a complete multi-part set of GetGrowingDataObjectsHeaderResponse and ProtocolException messages to a customer.
        /// If there are no data objects, an empty GetGrowingDataObjectsHeaderResponse message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="dataObjects">The data objects.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the GetGrowingDataObjectsHeaderResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetGrowingDataObjectsHeaderResponse> GetGrowingDataObjectsHeaderResponse(IMessageHeader correlatedHeader, IDictionary<string, DataObject> dataObjects, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null);

        /// <summary>
        /// Handles the PutGrowingDataObjectsHeader event from a customer.
        /// </summary>
        event EventHandler<MapRequestEventArgs<PutGrowingDataObjectsHeader, string>> OnPutGrowingDataObjectsHeader;

        /// <summary>
        /// Sends a PutGrowingDataObjectsHeaderResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<PutGrowingDataObjectsHeaderResponse> PutGrowingDataObjectsHeaderResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, bool isFinalPart = true, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a complete multi-part set of PutGrowingDataObjectsHeaderResponse and ProtocolException messages to a customer.
        /// If there are no URIs, an empty PutGrowingDataObjectsHeaderResponse message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the PutGrowingDataObjectsHeaderResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        EtpMessage<PutGrowingDataObjectsHeaderResponse> PutGrowingDataObjectsHeaderResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null);

        /// <summary>
        /// Handles the GetPartsMetadata event from a customer.
        /// </summary>
        event EventHandler<MapRequestEventArgs<GetPartsMetadata, PartsMetadataInfo>> OnGetPartsMetadata;

        /// <summary>
        /// Sends a GetPartsMetadataResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="metadata">The parts metadata.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetPartsMetadataResponse> GetPartsMetadataResponse(IMessageHeader correlatedHeader, IDictionary<string, PartsMetadataInfo> metadata, bool isFinalPart = true, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a complete multi-part set of GetPartsMetadataResponse and ProtocolException messages to a customer.
        /// If there are no parts metadata, an empty GetPartsMetadataResponse message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="metadata">The metadata.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the GetPartsMetadataResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetPartsMetadataResponse> GetPartsMetadataResponse(IMessageHeader correlatedHeader, IDictionary<string, PartsMetadataInfo> metadata, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null);
        
        /// <summary>
        /// Handles the GetChangeAnnotations event from a customer.
        /// </summary>
        event EventHandler<MapRequestEventArgs<GetChangeAnnotations, ChangeResponseInfo>> OnGetChangeAnnotations;

        /// <summary>
        /// Sends a GetChangeAnnotationsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="changes">The changes.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetChangeAnnotationsResponse> GetChangeAnnotationsResponse(IMessageHeader correlatedHeader, IDictionary<string, ChangeResponseInfo> changes, bool isFinalPart = true, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a complete multi-part set of GetChangeAnnotationsResponse and ProtocolException messages to a customer.
        /// If there are no changes, an empty ChangeAnnotationsRecord message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="changes">The changes.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the GetChangeAnnotationsResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetChangeAnnotationsResponse> GetChangeAnnotationsResponse(IMessageHeader correlatedHeader, IDictionary<string, ChangeResponseInfo> changes, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null);

        /// <summary>
        /// Handles the GetParts event from a customer.
        /// </summary>
        event EventHandler<MapRequestWithContextEventArgs<GetParts, ObjectPart, ResponseContext>> OnGetParts;

        /// <summary>
        /// Sends a GetPartsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="parts">The parts.</param>
        /// <param name="context">The response context.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetPartsResponse> GetPartsResponse(IMessageHeader correlatedHeader, IDictionary<string, ObjectPart> parts, ResponseContext context, bool isFinalPart = true, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a GetPartsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="parts">The parts.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetPartsResponse> GetPartsResponse(IMessageHeader correlatedHeader, string uri, IDictionary<string, ObjectPart> parts, string format = Formats.Xml, bool isFinalPart = true, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a complete multi-part set of GetPartsResponse and ProtocolException messages to a customer.
        /// If there are no parts, an empty GetPartsResponse message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="parts">The parts.</param>
        /// <param name="context">The response context.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the GetPartsResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetPartsResponse> GetPartsResponse(IMessageHeader correlatedHeader, IDictionary<string, ObjectPart> parts, ResponseContext context, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null);

        /// <summary>
        /// Sends a complete multi-part set of GetPartsResponse and ProtocolException messages to a customer.
        /// If there are no parts, an empty GetPartsResponse message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="parts">The parts.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the GetPartsResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetPartsResponse> GetPartsResponse(IMessageHeader correlatedHeader, string uri, IDictionary<string, ObjectPart> parts, IDictionary<string, IErrorInfo> errors, string format = Formats.Xml, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null);

        /// <summary>
        /// Handles the GetPartsByRange event from a customer.
        /// </summary>
        event EventHandler<ListRequestWithContextEventArgs<GetPartsByRange, ObjectPart, ResponseContext>> OnGetPartsByRange;

        /// <summary>
        /// Sends a GetPartsByRangeResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="parts">The UIDs and data of the parts being returned.</param>
        /// <param name="context">The response context.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetPartsByRangeResponse> GetPartsByRangeResponse(IMessageHeader correlatedHeader, IList<ObjectPart> parts, ResponseContext context, bool isFinalPart = true, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a GetPartsByRangeResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="parts">The UIDs and data of the parts being returned.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetPartsByRangeResponse> GetPartsByRangeResponse(IMessageHeader correlatedHeader, string uri, IList<ObjectPart> parts, string format = Formats.Xml, bool isFinalPart = true, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the PutParts event from a customer.
        /// </summary>
        event EventHandler<MapRequestEventArgs<PutParts, string>> OnPutParts;

        /// <summary>
        /// Sends a PutPartsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<PutPartsResponse> PutPartsResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, bool isFinalPart = true, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a complete multi-part set of PutPartsResponse and ProtocolException messages to a customer.
        /// If there are no successes, an empty PutPartsResponse message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the PutPartsResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        EtpMessage<PutPartsResponse> PutPartsResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null);

        /// <summary>
        /// Handles the DeleteParts event from a customer.
        /// </summary>
        event EventHandler<MapRequestEventArgs<DeleteParts, string>> OnDeleteParts;

        /// <summary>
        /// Sends a DeletePartsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<DeletePartsResponse> DeletePartsResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, bool isFinalPart = true, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a complete multi-part set of DeletePartsResponse and ProtocolException messages to a customer.
        /// If there are no successes, an empty DeletePartsResponse message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the DeletePartsResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        EtpMessage<DeletePartsResponse> DeletePartsResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null);

        /// <summary>
        /// Handles the ReplacePartsByRange event from a customer.
        /// </summary>
        event EventHandler<EmptyRequestEventArgs<ReplacePartsByRange>> OnReplacePartsByRange;

        /// <summary>
        /// Sends a ReplacePartsByRangeResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ReplacePartsByRangeResponse> ReplacePartsByRangeResponse(IMessageHeader correlatedHeader, IMessageHeaderExtension extension = null);
    }

    public class ResponseContext
    {
        public string Uri { get; set; }

        public string Format { get; set; } = Formats.Xml;
    }
}
