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

using Energistics.Etp.Common.Datatypes;
using System;
using System.Runtime.Serialization;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Represents errors that occur during ETP method execution.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class EtpException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EtpException" /> class.
        /// </summary>
        /// <param name="errorInfo">The error info.</param>
        /// <param name="correlationId">The correlation ID, if any.</param>
        public EtpException(IErrorInfo errorInfo, long correlationId = 0) : base(errorInfo.ToErrorMessage())
        {
            ErrorInfo = errorInfo;
            CorrelationId = correlationId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpException"/> class.
        /// </summary>
        /// <param name="errorInfo">The error info.</param>
        /// <param name="innerException">The inner exception.</param>
        public EtpException(IErrorInfo errorInfo, Exception innerException)
            : this(errorInfo, 0, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpException"/> class.
        /// </summary>
        /// <param name="errorInfo">The error info.</param>
        /// <param name="correlationId">The correlation ID, if any.</param>
        /// <param name="innerException">The inner exception.</param>
        public EtpException(IErrorInfo errorInfo, long correlationId, Exception innerException) : base(errorInfo.ToErrorMessage(), innerException)
        {
            ErrorInfo = errorInfo;
            CorrelationId = correlationId;
        }

        /// <summary>
        /// Gets the ETP error info.
        /// </summary>
        /// <value>The error info.</value>
        public IErrorInfo ErrorInfo { get; }

        /// <summary>
        /// The correlation ID, if any, for the error.
        /// </summary>
        public long CorrelationId { get; }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter" />
        /// </PermissionSet>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("ErrorInfo", ErrorInfo, ErrorInfo.GetType());
        }
    }

}
