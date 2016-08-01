//----------------------------------------------------------------------- 
// ETP DevKit, 1.1
//
// Copyright 2016 Energistics
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

using System.ComponentModel;

namespace Energistics
{
    /// <summary>
    /// Enumeration of ETP error codes.
    /// </summary>
    public enum EtpErrorCodes
    {
        /// <summary>UNSET</summary>
        [Description("UNSET")]
        Unset = 0,
        /// <summary>ENOROLE</summary>
        [Description("ENOROLE")]
        NoRole = 1,
        /// <summary>ENOSUPPORTEDPROTOCOLS</summary>
        [Description("ENOSUPPORTEDPROTOCOLS")]
        NoSupportedProtocols = 2,
        /// <summary>EINVALID_MESSAGETYPE</summary>
        [Description("EINVALID_MESSAGETYPE")]
        InvalidMessageType = 3,
        /// <summary>EUNSUPPORTED_PROTOCOL</summary>
        [Description("EUNSUPPORTED_PROTOCOL")]
        UnsupportedProtocol = 4,
        /// <summary>EINVALID_ARGUMENT</summary>
        [Description("EINVALID_ARGUMENT")]
        InvalidArgument = 5,
        /// <summary>EPERMISSION_DENIED</summary>
        [Description("EPERMISSION_DENIED")]
        PermissionDenied = 6,
        /// <summary>ENOTSUPPORTED</summary>
        [Description("ENOTSUPPORTED")]
        NotSupported = 7,
        /// <summary>EINVALID_STATE</summary>
        [Description("EINVALID_STATE")]
        InvalidState = 8,

        /// <summary>EINVALID_URI</summary>
        [Description("EINVALID_URI")]
        InvalidUri = 1001,
        /// <summary>EINVALID_CHANNELID</summary>
        [Description("EINVALID_CHANNELID")]
        InvalidChannelId = 1002,

        /// <summary>EUNSUPPORTED_OBJECT</summary>
        [Description("EUNSUPPORTED_OBJECT")]
        UnsupportedObject = 3001,
        /// <summary>EINVALID_OBJECT</summary>
        [Description("EINVALID_OBJECT")]
        InvalidObject = 3002,
        /// <summary>ENOCASCADE_DELETE</summary>
        [Description("ENOCASCADE_DELETE")]
        NoCascaseDelete = 3003
    }
}
