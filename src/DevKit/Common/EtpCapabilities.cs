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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Provides common functionality for ETP capabilities.
    /// </summary>
    public abstract class EtpCapabilities : ICapabilities
    {
        /// <summary>
        /// The underlying dictionary holding the capabilities.
        /// </summary>
        private IDataValueDictionary Dictionary { get; }

        /// <summary>
        /// Initializes a new <see cref="EtpCapabilities"/> instance.
        /// </summary>
        protected EtpCapabilities()
            : this(EtpVersion.Unknown)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="EtpCapabilities"/> instance.
        /// </summary>
        /// <param name="version">The ETP version the capabilities are for.</param>
        protected EtpCapabilities(EtpVersion version)
        {
            Dictionary = EtpFactory.CreateDataValueDictionary(version);
            InitializeFromDefaults();
        }

        /// <summary>
        /// Initializes a new <see cref="EtpCapabilities"/> instance.
        /// </summary>
        /// <param name="capabilities">The capabilities to initialize this from.</param>
        protected EtpCapabilities(IReadOnlyCapabilities capabilities)
        {
            Dictionary = capabilities.CloneCapabilities();
            LoadFromDictionary();
        }

        /// <summary>
        /// Initializes a new <see cref="EtpCapabilities"/> instance.
        /// </summary>
        /// <param name="version">The ETP version the capabilities are for.</param>
        /// <param name="capabilities">The capabilities to initialize this from.</param>
        protected EtpCapabilities(EtpVersion version, IReadOnlyDataValueDictionary capabilities)
        {
            Dictionary = EtpFactory.CreateDataValueDictionary(version);
            LoadFrom(capabilities);
        }

        /// <summary>
        /// Loads the property values from the specified capabilities dictionary.
        /// </summary>
        /// <param name="capabilities">The capabilities dictionary to load values from.</param>
        public void LoadFrom(IReadOnlyCapabilities capabilities)
        {
            LoadFrom(capabilities.CloneCapabilities());
        }

        /// <summary>
        /// Loads the property values from the specified capabilities dictionary.
        /// </summary>
        /// <param name="capabilities">The capabilities dictionary to load values from.</param>
        public void LoadFrom(IReadOnlyDataValueDictionary capabilities)
        {
            Dictionary.Clear();

            if (capabilities == null)
                return;

            foreach (var kvp in capabilities)
                Dictionary.SetValue(kvp.Key, kvp.Value.Item);

            LoadFromDictionary();
        }

        /// <summary>
        /// Loads the property values in this from its dictionary.
        /// </summary>
        private void LoadFromDictionary()
        {
            foreach (var property in GetType().GetProperties())
            {
                var value = Dictionary.TryGetValue(property.Name, property.PropertyType);
                property.SetValue(this, value);
            }
        }

        /// <summary>
        /// Saves the property values to the specified capabilities dictionary.
        /// </summary>
        /// <param name="capabilities">The capabilities dictionary to save values to.</param>
        public void SaveTo(IDataValueDictionary capabilities)
        {
            if (capabilities == null)
                return;

            SaveToDictionary();

            foreach (var kvp in Dictionary)
                capabilities.SetValue(kvp.Key, kvp.Value.Item);
        }

        /// <summary>
        /// Saves the property values in this to its dictionary.
        /// </summary>
        private void SaveToDictionary()
        {
            foreach (var property in GetType().GetProperties())
            {
                var value = property.GetValue(this);
                SetValue(property.Name, value);
            }
        }

        /// <summary>
        /// Initializes settings from default values.
        /// </summary>
        public void InitializeFromDefaults()
        {
            var defaultProperties = new Dictionary<string, PropertyInfo>();
            foreach (var property in typeof(EtpSettings).GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                defaultProperties[property.Name] = property;
            }

            foreach (var property in GetType().GetProperties())
            {
                var defaultPropertyName = $"Default{property.Name}";
                PropertyInfo defaultProperty;
                if (!defaultProperties.TryGetValue(defaultPropertyName, out defaultProperty))
                    continue;

                var value = defaultProperty.GetValue(null);
                property.SetValue(this, value);
                Dictionary.SetValue(property.Name, value);
            }
        }

        /// <summary>
        /// Creates a deep copy of capabilities stored in this instance.
        /// </summary>
        /// <returns>A deep copy of capabilities stored in this instance.  The returned instance is a <see cref="IDataValueDictionary"/> but is not guaranteed to be an <see cref="ICapabilities"/> instance.</returns>
        public IDataValueDictionary CloneCapabilities()
        {
            SaveToDictionary();
            var dictionary = Dictionary.Clone();
            return dictionary;
        }

        /// <summary>
        /// Creates a deep copy of capabilities stored in this instance.
        /// </summary>
        /// <returns>A deep copy of capabilities stored in this instance.  The returned instance is a <see cref="IDataValueDictionary"/> but is not guaranteed to be an <see cref="ICapabilities"/> instance.</returns>
        IDataValueDictionary IDataValueDictionary.Clone() => CloneCapabilities();

        /// <summary>
        /// Sets the specified value in the capabilities.
        /// </summary>
        /// <typeparam name="T">The type of the item value.</typeparam>
        /// <param name="key">The name of the value.</param>
        /// <param name="value">The value to set.</param>
        public void SetValue<T>(string key, T? value) where T : struct => Dictionary.SetValue(key, value);

        /// <summary>
        /// Sets the specified value in the capabilities.
        /// </summary>
        /// <param name="key">The name of the value.</param>
        /// <param name="value">The value to set.</param>
        public void SetValue(string key, object value) => Dictionary.SetValue(key, value);

        /// <summary>
        /// Tries to get the specified value from the capabilities as the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="key">The name of the value to retrieve.</param>
        /// <returns>The value if present or the default value if it is not present or there is an error retrieving it.</returns>
        public T? TryGetValue<T>(string key) where T : struct => Dictionary.TryGetValue<T>(key);

        /// <summary>
        /// Tries to get the specified value from the capabilities as the specified type.
        /// </summary>
        /// <param name="key">The name of the value to retrieve.</param>
        /// <param name="valueType">The type of the value.</param>
        /// <returns>The value if present or the default value if it is not present or there is an error retrieving it.</returns>
        public object TryGetValue(string key, Type valueType) => Dictionary.TryGetValue(key, valueType);

        #region IDictionary<string, IDataValue> Members

        ICollection<string> IDictionary<string, IDataValue>.Keys => ((IDictionary<string, IDataValue>)Dictionary).Keys;

        ICollection<IDataValue> IDictionary<string, IDataValue>.Values => ((IDictionary<string, IDataValue>)Dictionary).Values;

        int ICollection<KeyValuePair<string, IDataValue>>.Count => ((IDictionary<string, IDataValue>)Dictionary).Count;

        bool ICollection<KeyValuePair<string, IDataValue>>.IsReadOnly => Dictionary.IsReadOnly;

        IDataValue IDictionary<string, IDataValue>.this[string key]
        {
            get { return ((IDictionary<string, IDataValue>)Dictionary)[key]; }
            set { ((IDictionary<string, IDataValue>)Dictionary)[key] = value; }
        }

        bool IDictionary<string, IDataValue>.ContainsKey(string key) => ((IDictionary<string, IDataValue>)Dictionary).ContainsKey(key);

        void IDictionary<string, IDataValue>.Add(string key, IDataValue value) => Dictionary.Add(key, value);

        bool IDictionary<string, IDataValue>.Remove(string key) => Dictionary.Remove(key);

        bool IDictionary<string, IDataValue>.TryGetValue(string key, out IDataValue value) => ((IDictionary<string, IDataValue>)Dictionary).TryGetValue(key, out value);

        void ICollection<KeyValuePair<string, IDataValue>>.Add(KeyValuePair<string, IDataValue> item) => Dictionary.Add(item);

        void ICollection<KeyValuePair<string, IDataValue>>.Clear() => Dictionary.Clear();

        bool ICollection<KeyValuePair<string, IDataValue>>.Contains(KeyValuePair<string, IDataValue> item) => Dictionary.Contains(item);

        void ICollection<KeyValuePair<string, IDataValue>>.CopyTo(KeyValuePair<string, IDataValue>[] array, int arrayIndex) => Dictionary.CopyTo(array, arrayIndex);

        bool ICollection<KeyValuePair<string, IDataValue>>.Remove(KeyValuePair<string, IDataValue> item) => Dictionary.Remove(item);

        IEnumerator<KeyValuePair<string, IDataValue>> IEnumerable<KeyValuePair<string, IDataValue>>.GetEnumerator() => Dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Dictionary.GetEnumerator();

        #endregion

        #region IReadOnlyDictionary<string, IDataValue>

        IEnumerable<string> IReadOnlyDictionary<string, IDataValue>.Keys => ((IReadOnlyDictionary<string, IDataValue>)Dictionary).Keys;

        IEnumerable<IDataValue> IReadOnlyDictionary<string, IDataValue>.Values => ((IReadOnlyDictionary<string, IDataValue>)Dictionary).Values;

        int IReadOnlyCollection<KeyValuePair<string, IDataValue>>.Count => ((IReadOnlyDictionary<string, IDataValue>)Dictionary).Count;

        IDataValue IReadOnlyDictionary<string, IDataValue>.this[string key] => ((IReadOnlyDictionary<string, IDataValue>)Dictionary)[key];

        bool IReadOnlyDictionary<string, IDataValue>.ContainsKey(string key) => ((IReadOnlyDictionary<string, IDataValue>)Dictionary).ContainsKey(key);

        bool IReadOnlyDictionary<string, IDataValue>.TryGetValue(string key, out IDataValue value) => ((IReadOnlyDictionary<string, IDataValue>)Dictionary).TryGetValue(key, out value);

        #endregion
    }
}
