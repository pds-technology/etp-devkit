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

namespace Energistics.Protocol
{
    /// <summary>
    /// Provides enumerations of protocol message types.
    /// </summary>
    public static class MessageTypes
    {
        /// <summary>
        /// An enumeration of Core protocol message types.
        /// </summary>
        public enum Core
        {
            RequestSession = 1,
            OpenSession = 2,
            CloseSession = 5,
            RenewSecurityToken = 6,
            ProtocolException = 1000,
            Acknowledge = 1001
        }

        /// <summary>
        /// An enumeration of Channel Streaming protocol message types.
        /// </summary>
        public enum ChannelStreaming
        {
            Start = 0,
            ChannelDescribe = 1,
            ChannelMetadata = 2,
            ChannelData = 3,
            ChannelStreamingStart = 4,
            ChannelStreamingStop = 5,
            ChannelDataChange = 6,
            //NotUsed = 7,
            ChannelRemove = 8,
            ChannelRangeRequest = 9,
            ChannelStatusChange = 10
        }

        /// <summary>
        /// An enumeration of Channel Data Frame protocol message types.
        /// </summary>
        public enum ChannelDataFrame
        {
            RequestChannelData = 1,
            ChannelNotUsed,
            ChannelMetadata,
            ChannelDataFrameSet
        }

        /// <summary>
        /// An enumeration of Discovery protocol message types.
        /// </summary>
        public enum Discovery
        {
            GetResources = 1,
            GetResourcesResponse
        }

        /// <summary>
        /// An enumeration of Store protocol message types.
        /// </summary>
        public enum Store
        {
            GetObject = 1,
            PutObject,
            DeleteObject,
            Object
        }

        /// <summary>
        /// An enumeration of Store Notification protocol message types.
        /// </summary>
        public enum StoreNotification
        {
            NotificationRequest = 1,
            ChangeNotification,
            DeleteNotification,
            CancelNotification
        }

        /// <summary>
        /// An enumeration of Growing Object protocol message types.
        /// </summary>
        public enum GrowingObject
        {
            GrowingObjectDelete = 1,
            GrowingObjectDeleteRange,
            GrowingObjectGet,
            GrowingObjectGetRange,
            GrowingObjectPut,
            ObjectFragment
        }

        /// <summary>
        /// An enumeration of Data Array protocol message types.
        /// </summary>
        public enum DataArray
        {
            DataArray = 1,
            GetDataArray,
            GetDataArraySlice,
            PutDataArray,
            PutDataArraySlice
        }

        /// <summary>
        /// An enumeration of Witsml Soap protocol message types.
        /// </summary>
        public enum WitsmlSoap
        {
            AddToStore = 1,
            AddToStoreResponse,
            DeleteFromStore,
            DeleteFromStoreResponse,
            GetBaseMsg,
            GetBaseMsgResponse,
            GetCap,
            GetCapResponse,
            GetFromStore,
            GetFromStoreResponse,
            GetVersion,
            GetVersionResponse,
            UpdateInStore,
            UpdateInStoreResponse
        }
    }
}
