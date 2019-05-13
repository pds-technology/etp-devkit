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

using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Energistics.Etp.Common.Datatypes
{
    [TestClass]
    public class ByteArrayConverterTests
    {
        private const string Xml = "<Well xmlns=\"http://www.energistics.org/energyml/data/witsmlv2\" schemaVersion=\"2.0\" />";
        private const string Hex = "\\u003C\\u0057\\u0065\\u006C\\u006C\\u0020\\u0078\\u006D\\u006C\\u006E\\u0073\\u003D\\u0022\\u0068\\u0074\\u0074\\u0070\\u003A\\u002F\\u002F\\u0077\\u0077\\u0077\\u002E\\u0065\\u006E\\u0065\\u0072\\u0067\\u0069\\u0073\\u0074\\u0069\\u0063\\u0073\\u002E\\u006F\\u0072\\u0067\\u002F\\u0065\\u006E\\u0065\\u0072\\u0067\\u0079\\u006D\\u006C\\u002F\\u0064\\u0061\\u0074\\u0061\\u002F\\u0077\\u0069\\u0074\\u0073\\u006D\\u006C\\u0076\\u0032\\u0022\\u0020\\u0073\\u0063\\u0068\\u0065\\u006D\\u0061\\u0056\\u0065\\u0072\\u0073\\u0069\\u006F\\u006E\\u003D\\u0022\\u0032\\u002E\\u0030\\u0022\\u0020\\u002F\\u003E";
        private const string HexGzip = "\\u001F\\u008B\\u0008\\u0000\\u0000\\u0000\\u0000\\u0000\\u0004\\u0000\\u001D\\u00C7\\u0051\\u000A\\u0080\\u0020\\u000C\\u0000\\u00D0\\u00AB\\u0088\\u0007\\u0070\\u00E1\\u0067\\u0064\\u00C7\\u00A8\\u006F\\u00A9\\u00A1\\u00C2\\u00D4\\u0070\\u00A3\\u00D5\\u00ED\\u0083\\u00DE\\u00DF\\u005B\\u0076\\u0024\\u0032\\u004F\\u00A5\\u00C6\\u00C1\\u0066\\u0091\\u006B\\u0006\\u0050\\u0055\\u0087\\u000D\\u0047\\u002A\\u002C\\u00E5\\u0060\\u00D7\\u0047\\u0082\\u00FF\\u006F\\u0025\\u0038\\u00A3\\u0044\\u00D0\\u0022\\u005C\\u00E9\\u00F6\\u00D6\\u00F0\\u0091\\u00B1\\u00C6\\u000D\\u0007\\u0097\\u00DE\\u0082\\u00F5\\u006E\\u00B2\\u0006\\u00D6\\u000F\\u008D\\u00B7\\u003C\\u00A0\\u0056\\u0000\\u0000\\u0000";

        private ByteArrayConverter _converter;

        [TestInitialize]
        public void TestSetup()
        {
            _converter = new ByteArrayConverter();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _converter = null;
        }

        [TestMethod]
        public void ByteArrayConverter_CanConvert_Detects_Supported_Type()
        {
            Assert.IsTrue(_converter.CanConvert(typeof(byte[])));
        }

        [TestMethod]
        public void ByteArrayConverter_CanConvert_Detects_Unsupported_Type()
        {
            Assert.IsFalse(_converter.CanConvert(typeof(int[])));
        }

        [TestMethod]
        public void ByteArrayConverter_WriteJson_Can_Serialize_Null_Byte_Array()
        {
            var dataObject = new v11.Datatypes.Object.DataObject();
            var json = EtpExtensions.Serialize(dataObject, true);

            Assert.IsTrue(json.Contains("\"data\": null"));
        }

        [TestMethod]
        public void ByteArrayConverter_WriteJson_Can_Serialize_Byte_Array_In_Avro_Format()
        {
            var dataObject = new v11.Datatypes.Object.DataObject();
            dataObject.SetString(Xml, false);

            var json = EtpExtensions.Serialize(dataObject, true);
            var hexEscaped = Escape(Hex);

            Assert.IsTrue(json.Contains(hexEscaped));
        }

        [TestMethod]
        public void ByteArrayConverter_WriteJson_Can_Serialize_Compressed_Byte_Array_In_Avro_Format()
        {
            var dataObject = new v11.Datatypes.Object.DataObject();
            dataObject.SetString(Xml);

            var json = EtpExtensions.Serialize(dataObject, true);
            var hexEscaped = Escape(HexGzip);

            Assert.IsTrue(json.Contains(hexEscaped));
        }

        [TestMethod]
        public void ByteArrayConverter_ReadJson_Can_Deserialize_Null_Byte_Array()
        {
            const string json = "{ \"data\": null }";

            var instance = EtpExtensions.Deserialize<v11.Datatypes.Object.DataObject>(json);

            Assert.IsNull(instance.Data);
        }

        [TestMethod]
        public void ByteArrayConverter_ReadJson_Can_Deserialize_Byte_Array_In_Avro_Format()
        {
            var json = "{ \"data\": \"" + Escape(Hex) + "\" }";

            var instance = EtpExtensions.Deserialize<v11.Datatypes.Object.DataObject>(json);
            var expected = Encoding.UTF8.GetBytes(Xml);

            CollectionAssert.AreEqual(expected, instance.Data);
        }

        [TestMethod]
        public void ByteArrayConverter_ReadJson_Can_Deserialize_Compressed_Byte_Array_In_Avro_Format()
        {
            var json = "{ \"data\": \"" + Escape(HexGzip) + "\", \"contentEncoding\": \"gzip\" }";

            var instance = EtpExtensions.Deserialize<v11.Datatypes.Object.DataObject>(json);

            var dataObject = new v11.Datatypes.Object.DataObject();
            dataObject.SetString(Xml);

            var expected = dataObject.Data;

            CollectionAssert.AreEqual(expected, instance.Data);
        }

        private string Escape(string value)
        {
            return value.Replace("\\", "\\\\");
        }
    }
}
