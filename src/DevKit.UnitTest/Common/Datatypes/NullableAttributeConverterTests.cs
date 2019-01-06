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

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Energistics.Etp.Common.Datatypes
{
    [TestClass]
    public class NullableAttributeConverterTests
    {
        private const string UuidValue = "242ddb4f-864a-4359-ae4b-4c12ff37124a";
        //private const string ContentType = "application/x-witsml+xml;version=2.0;type=Well";
        //private const string UriValue = "eml://witsml20/Well(" + UuidValue + ")";

        [TestMethod]
        public void NullableStringConverter_WriteJson_serializes_Resource_uuid_with_value()
        {
            var resource = new v11.Datatypes.Object.Resource
            {
                Uuid = UuidValue
            };

            const string expected = "\"uuid\":{\"string\":\"" + UuidValue + "\"}";
            var json = EtpExtensions.Serialize(resource);

            Assert.IsTrue(json.Contains(expected));
        }

        [TestMethod]
        public void NullableStringConverter_ReadJson_deserializes_Resource_uuid_with_value()
        {
            const string json = "{\"uuid\":{\"string\":\"" + UuidValue + "\"}}";
            var resource = EtpExtensions.Deserialize<v11.Datatypes.Object.Resource>(json);

            Assert.IsNotNull(resource.Uuid);
            Assert.AreEqual(UuidValue, resource.Uuid);
        }

        [TestMethod]
        public void NullableStringConverter_WriteJson_serializes_Resource_uuid_with_empty_string()
        {
            var resource = new v11.Datatypes.Object.Resource
            {
                Uuid = string.Empty
            };

            const string expected = "\"uuid\":{\"string\":\"\"}";
            var json = EtpExtensions.Serialize(resource);

            Assert.IsTrue(json.Contains(expected));
        }

        [TestMethod]
        public void NullableStringConverter_ReadJson_deserializes_Resource_uuid_with_empty_string()
        {
            const string json = "{\"uuid\":{\"string\":\"\"}}";
            var resource = EtpExtensions.Deserialize<v11.Datatypes.Object.Resource>(json);

            Assert.IsNotNull(resource.Uuid);
            Assert.AreEqual(string.Empty, resource.Uuid);
        }

        [TestMethod]
        public void NullableStringConverter_WriteJson_serializes_Resource_uuid_with_null_value()
        {
            var resource = new v11.Datatypes.Object.Resource
            {
                Uuid = null
            };

            const string expected = "\"uuid\":null";
            var json = EtpExtensions.Serialize(resource);

            Assert.IsTrue(json.Contains(expected));
        }

        [TestMethod]
        public void NullableStringConverter_ReadJson_deserializes_Resource_uuid_with_null_value()
        {
            const string json = "{\"uuid\":null}";
            var resource = EtpExtensions.Deserialize<v11.Datatypes.Object.Resource>(json);

            Assert.IsNull(resource.Uuid);
        }
    }
}
