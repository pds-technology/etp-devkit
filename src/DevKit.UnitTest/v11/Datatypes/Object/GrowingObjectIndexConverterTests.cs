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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Energistics.Etp.v11.Datatypes.Object
{
    [TestClass]
    public class GrowingObjectIndexConverterTests
    {
        [TestMethod]
        public void GrowingObjectIndexConverter_WriteJson_serializes_double_value()
        {
            var index = new GrowingObjectIndex
            {
                Item = 2.5
            };

            const string expected = "{\"item\":{\"double\":2.5}}";
            var json = EtpExtensions.Serialize(index);

            Assert.AreEqual(expected, json);
        }

        [TestMethod]
        public void GrowingObjectIndexConverter_ReadJson_deserializes_double_value()
        {
            const string json = "{\"item\":{\"double\":2.5}}";
            var index = EtpExtensions.Deserialize<GrowingObjectIndex>(json);

            Assert.IsTrue(index.Item is double);
            Assert.AreEqual(2.5, (double) index.Item, 0.001);
        }

        [TestMethod]
        public void GrowingObjectIndexConverter_WriteJson_serializes_long_value()
        {
            var index = new GrowingObjectIndex
            {
                Item = 10L
            };

            var expected = "{\"item\":{\"long\":10}}";
            var json = EtpExtensions.Serialize(index);

            Assert.AreEqual(expected, json);
        }

        [TestMethod]
        public void GrowingObjectIndexConverter_ReadJson_deserializes_long_value()
        {
            const string json = "{\"item\":{\"long\":10}}";
            var index = EtpExtensions.Deserialize<GrowingObjectIndex>(json);

            Assert.IsTrue(index.Item is long);
            Assert.AreEqual(10L, (long) index.Item);
        }

        [TestMethod]
        public void GrowingObjectIndexConverter_WriteJson_serializes_null_value()
        {
            var index = new GrowingObjectIndex
            {
                Item = null
            };

            const string expected = "{\"item\":null}";
            var json = EtpExtensions.Serialize(index);

            Assert.AreEqual(expected, json);
        }

        [TestMethod]
        public void GrowingObjectIndexConverter_ReadJson_deserializes_null_value()
        {
            const string json = "{\"item\":null,\"value\":null}";
            var index = EtpExtensions.Deserialize<GrowingObjectIndex>(json);

            Assert.IsNull(index.Item);
        }

        [TestMethod]
        public void GrowingObjectIndexConverter_WriteJson_serializes_null_object()
        {
            const string expected = "null";
            var json = EtpExtensions.Serialize(null);

            Assert.AreEqual(expected, json);
        }

        [TestMethod]
        public void GrowingObjectIndexConverter_ReadJson_deserializes_null_object()
        {
            const string json = "null";
            var index = EtpExtensions.Deserialize<GrowingObjectIndex>(json);

            Assert.IsNull(index);
        }

        [TestMethod]
        public void GrowingObjectIndexConverter_ReadJson_deserializes_default_value()
        {
            const string json = "{\"item\":1}";
            var index = EtpExtensions.Deserialize<GrowingObjectIndex>(json);

            Assert.IsTrue(index.Item is long);
            Assert.AreEqual(1L, (long) index.Item);
        }

        [TestMethod]
        public void GrowingObjectIndexConverter_ReadJson_deserializes_string_value()
        {
            const string json = "{\"item\":\"5\"}";
            var index = EtpExtensions.Deserialize<GrowingObjectIndex>(json);

            Assert.IsTrue(index.Item is long);
            Assert.AreEqual(5L, (long) index.Item);
        }

        [TestMethod]
        public void GrowingObjectIndexConverter_WriteJson_raises_error_for_unsupported_data_type()
        {
            var index = new GrowingObjectIndex
            {
                Item = "abc"
            };

            var pass = false;

            try
            {
                EtpExtensions.Serialize(index);
            }
            catch (JsonSerializationException)
            {
                pass = true;
            }

            Assert.IsTrue(pass);
        }

        [TestMethod]
        public void GrowingObjectIndexConverter_ReadJson_raises_error_for_unsupported_union_type()
        {
            const string json = "{\"item\":{\"string\":\"abc\"}}";
            var pass = false;

            try
            {
                EtpExtensions.Deserialize<GrowingObjectIndex>(json);
            }
            catch (JsonSerializationException)
            {
                pass = true;
            }

            Assert.IsTrue(pass);
        }

        [TestMethod]
        public void GrowingObjectIndexConverter_ReadJson_raises_error_for_unsupported_data_type()
        {
            const string json = "{\"item\":\"abc\"}";
            var pass = false;

            try
            {
                EtpExtensions.Deserialize<GrowingObjectIndex>(json);
            }
            catch (JsonSerializationException)
            {
                pass = true;
            }

            Assert.IsTrue(pass);
        }
    }
}
