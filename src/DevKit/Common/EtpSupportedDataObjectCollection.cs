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
using System.Collections;
using System.Collections.Generic;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Provides common information about a collection of supported data object
    /// </summary>
    public class EtpSupportedDataObjectCollection : ISessionSupportedDataObjectCollection
    {
        /// <summary>
        /// The list of supported all data objects.
        /// </summary>
        private List<EtpSupportedDataObject> AllDataObjects { get; } = new List<EtpSupportedDataObject>();

        /// <summary>
        /// A dictionary of supported data object families
        /// </summary>
        private Dictionary<string, EtpSupportedDataObject> SupportedDataObjectsByFamily { get; } = new Dictionary<string, EtpSupportedDataObject>();

        /// <summary>
        /// A dictionary of specific supported data objects by type.
        /// </summary>
        private Dictionary<string, EtpSupportedDataObject> SupportedDataObjectsByType { get; } = new Dictionary<string, EtpSupportedDataObject>();

        /// <summary>
        /// An enumerable of all <see cref="EtpSupportedDataObject"/> instances that represent data object families (i.e. are wildcards).
        /// </summary>
        public IEnumerable<EtpSupportedDataObject> SupportedFamilies => SupportedDataObjectsByFamily.Values;

        /// <summary>
        /// An enumerable of all <see cref="EtpSupportedDataObject"/> instances that represent specific data object types (i.e. are NOT wildcards).
        /// </summary>
        public IEnumerable<EtpSupportedDataObject> SupportedTypes => SupportedDataObjectsByType.Values;

        /// <summary>
        /// An enumerable of all <see cref="ISessionSupportedDataObject"/> instances that represent data object families (i.e. are wildcards).
        /// </summary>
        IEnumerable<ISessionSupportedDataObject> ISessionSupportedDataObjectCollection.SupportedFamilies => SupportedFamilies;

        /// <summary>
        /// An enumerable of all <see cref="ISessionSupportedDataObject"/> instances that represent specific data object types (i.e. are NOT wildcards).
        /// </summary>
        IEnumerable<ISessionSupportedDataObject> ISessionSupportedDataObjectCollection.SupportedTypes => SupportedTypes;

        /// <summary>
        /// Initializes a new <see cref="EtpSupportedDataObjectCollection"/> instance.
        /// </summary>
        public EtpSupportedDataObjectCollection()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="EtpSupportedDataObjectCollection"/> instance.
        /// </summary>
        /// <param name="supportedDataObjects">The list of supported data objects.</param>
        public EtpSupportedDataObjectCollection(IEnumerable<EtpSupportedDataObject> supportedDataObjects)
        {
            foreach (var supportedDataObject in supportedDataObjects)
            {
                AddSupportedDataObject(supportedDataObject);
            }
        }

        /// <summary>
        /// Returns a new <see cref="EtpSupportedDataObjectCollection"/> that representes the intersection of supported data objects between this instance and the other instance.
        /// </summary>
        /// <param name="instanceSupportedDataObjects">The ETP endpoint's supported data objects.</param>
        /// <param name="counterpartSupportedDataObjects">The ETP endpoint's counterpart's supported data objects.</param>
        /// <param name="intersection">If <c>true</c>, the collection is the intersection of data objects supported by the instance and its counterpart; if <c>false</c> it is the union.</param>
        /// <returns>The new collection representing the intersection of the supported data objects.</returns>
        public static EtpSupportedDataObjectCollection GetSupportedDataObjectCollection(IEnumerable<EtpSupportedDataObject> instanceSupportedDataObjects, IEnumerable<EtpSupportedDataObject> counterpartSupportedDataObjects, bool intersection)
        {
            var instanceCollection = new EtpSupportedDataObjectCollection(instanceSupportedDataObjects);
            var counterpartCollection = new EtpSupportedDataObjectCollection(counterpartSupportedDataObjects);

            var supportedDataObjects = new List<EtpSupportedDataObject>();

            // First handle wildcards found in both sets
            foreach (var instanceFamily in instanceCollection.SupportedFamilies)
            {
                var counterpartFamily = counterpartCollection.TryGetMatchingDataObject(instanceFamily.QualifiedType);
                if (!intersection || counterpartFamily != null)
                {
                    supportedDataObjects.Add(instanceFamily);
                    instanceFamily.CounterpartCapabilities = counterpartFamily?.Capabilities ?? new EtpDataObjectCapabilities();
                }
            }

            if (!intersection)
            {
                foreach (var counterpartFamily in counterpartCollection.SupportedFamilies)
                {
                    var instanceFamily = instanceCollection.TryGetMatchingDataObject(counterpartFamily.QualifiedType);
                    if (instanceFamily == null)
                    {
                        var supportedDataObject = new EtpSupportedDataObject(counterpartFamily.QualifiedType, null);
                        supportedDataObject.CounterpartCapabilities = counterpartFamily.Capabilities;

                        supportedDataObjects.Add(supportedDataObject);
                    }
                }
            }

            // Next handle explicit types from instance found in counterpart as either an explicit type or a family
            foreach (var instanceType in instanceCollection.SupportedTypes)
            {
                var counterpartDataObject = counterpartCollection.TryGetMatchingDataObject(instanceType.QualifiedType);
                if (!intersection || counterpartDataObject != null)
                {
                    supportedDataObjects.Add(instanceType);
                    instanceType.CounterpartCapabilities = counterpartDataObject?.Capabilities ?? new EtpDataObjectCapabilities();
                }
            }

            // Last handle explicit types from counterpart
            foreach (var counterpartType in counterpartCollection.SupportedTypes)
            {
                var instanceDataObject = instanceCollection.TryGetMatchingDataObject(counterpartType.QualifiedType);
                if ((!intersection && (instanceDataObject == null || instanceDataObject.QualifiedType.IsWildcard)) || (intersection && instanceDataObject != null && instanceDataObject.QualifiedType.IsWildcard))
                {
                    var supportedDataObject = new EtpSupportedDataObject(counterpartType.QualifiedType, instanceDataObject?.Capabilities);
                    supportedDataObject.CounterpartCapabilities = counterpartType.Capabilities;

                    supportedDataObjects.Add(supportedDataObject);
                }
            }

            return new EtpSupportedDataObjectCollection(supportedDataObjects);
        }

        /// <summary>
        /// Adds a supported data object to this collection.
        /// </summary>
        /// <param name="supportedDataObject">The supported data object to add.</param>
        /// <returns><c>true</c> if the supported data object was successfully added; <c>false</c> otherwise.</returns>
        public bool AddSupportedDataObject(EtpSupportedDataObject supportedDataObject)
        {
            if (!supportedDataObject.QualifiedType.IsValid || supportedDataObject.QualifiedType.IsBaseType)
                return false;

            AllDataObjects.Add(supportedDataObject);
            if (supportedDataObject.QualifiedType.IsWildcard)
                SupportedDataObjectsByFamily[supportedDataObject.Key] = supportedDataObject;
            else
                SupportedDataObjectsByType[supportedDataObject.Key] = supportedDataObject;

            return true;
        }

        /// <summary>
        /// Checks whether or not the specificied <see cref="IDataObjectType"/> is supported or not.
        /// </summary>
        /// <param name="dataObjectType">The type to check.</param>
        /// <returns><c>true</c> if the data object type is supported; <c>false</c> otherwise.</returns>
        public bool IsSupported(IDataObjectType dataObjectType)
        {
            return TryGetMatchingDataObject(dataObjectType) != null;
        }

        /// <summary>
        /// Tries to get the <see cref="EtpSupportedDataObject"/> matching the specified <see cref="IDataObjectType"/>.
        /// </summary>
        /// <param name="dataObjectType">The type to try to get the matching <see cref="EtpSupportedDataObject"/> for.</param>
        /// <returns>
        /// If there is a specific type matching the specified data object type, it is returned.
        /// Otherwise, if there is a family that matches the specified data object type, it is returned.
        /// Otherwise, <c>null</c> is returned.
        /// </returns>
        public EtpSupportedDataObject TryGetMatchingDataObject(IDataObjectType dataObjectType)
        {
            if (dataObjectType == null || !dataObjectType.IsValid || dataObjectType.IsBaseType)
                return null;

            EtpSupportedDataObject sessionDataObject;
            if (!dataObjectType.IsWildcard)
            {
                if (SupportedDataObjectsByType.TryGetValue(dataObjectType.Key, out sessionDataObject))
                    return sessionDataObject;

                dataObjectType = dataObjectType.ToWildCard();
            }

            if (SupportedDataObjectsByFamily.TryGetValue(dataObjectType.Key, out sessionDataObject))
                return sessionDataObject;

            return null;
        }

        /// <summary>
        /// Tries to get the <see cref="ISessionSupportedDataObject"/> matching the specified <see cref="IDataObjectType"/>.
        /// </summary>
        /// <param name="dataObjectType">The type to try to get the matching <see cref="ISessionSupportedDataObject"/> for.</param>
        /// <returns>
        /// If there is a specific type matching the specified data object type, it is returned.
        /// Otherwise, if there is a family that matches the specified data object type, it is returned.
        /// Otherwise, <c>null</c> is returned.
        /// </returns>
        ISessionSupportedDataObject ISessionSupportedDataObjectCollection.TryGetMatchingDataObject(IDataObjectType dataObjectType) => TryGetMatchingDataObject(dataObjectType);

        #region IReadOnlyList Members

        public int Count => AllDataObjects.Count;

        public ISessionSupportedDataObject this[int index] => AllDataObjects[index];

        public IEnumerator<ISessionSupportedDataObject> GetEnumerator()
        {
            return AllDataObjects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)AllDataObjects).GetEnumerator();
        }

        #endregion
    }
}
