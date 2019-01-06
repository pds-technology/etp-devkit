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
    public class EtpContentTypeTests
    {
        [TestMethod]
        public void EtpContentType_Can_Parse_Base_Content_Type_Without_Trailing_Semicolon()
        {
            var expected = "application/x-witsml+xml;version=2.0";
            var contentType = new EtpContentType(expected);

            Assert.IsTrue(contentType.IsValid);
            Assert.IsTrue(contentType.IsBaseType);
            Assert.AreEqual("witsml", contentType.Family);
            Assert.AreEqual("2.0", contentType.Version);
        }

        [TestMethod]
        public void EtpContentType_Can_Parse_Base_Content_Type_With_Trailing_Semicolon()
        {
            var expected = "application/x-witsml+xml;version=2.0";
            var contentType = new EtpContentType(expected);

            Assert.IsTrue(contentType.IsValid);
            Assert.IsTrue(contentType.IsBaseType);
            Assert.AreEqual("witsml", contentType.Family);
            Assert.AreEqual("2.0", contentType.Version);
        }

        [TestMethod]
        public void EtpContentType_Rejects_Content_Type_Without_Version()
        {
            var expected = "application/x-witsml+xml";
            var contentType = new EtpContentType(expected);

            Assert.IsFalse(contentType.IsValid);
        }

        [TestMethod]
        public void EtpContentType_For_Can_Create_1411_Well_Content_Type()
        {
            var expected = "application/x-witsml+xml;version=1.4.1.1";
            var contentType = new EtpContentType(expected).For("well");

            Assert.IsTrue(contentType.IsValid);
            Assert.AreEqual("well", contentType.ObjectType);
            Assert.AreEqual("1.4.1.1", contentType.Version);
            Assert.AreEqual(expected + ";type=well", (string)contentType);
        }

        [TestMethod]
        public void EtpContentType_Can_Parse_Witsml_20_Well_Content_Type()
        {
            var expected = "application/x-witsml+xml;version=2.0;type=Well";
            var contentType = new EtpContentType(expected);

            Assert.IsTrue(contentType.IsValid);
            Assert.AreEqual("Well", contentType.ObjectType);
            Assert.AreEqual("2.0", contentType.Version);
        }

        [TestMethod]
        public void EtpContentType_Can_Parse_Witsml_20_TrajectoryStation_Content_Type()
        {
            var expected = "application/x-witsml+xml;version=2.0;type=part_TrajectoryStation";
            var contentType = new EtpContentType(expected);

            Assert.IsTrue(contentType.IsValid);
            Assert.AreEqual("TrajectoryStation", contentType.ObjectType);
            Assert.AreEqual("2.0", contentType.Version);
        }

        [TestMethod]
        public void EtpContentType_Can_Parse_Witsml_1411_Well_Content_Type()
        {
            var expected = "application/x-witsml+xml;version=1.4.1.1;type=well";
            var contentType = new EtpContentType(expected);

            Assert.IsTrue(contentType.IsValid);
            Assert.AreEqual("well", contentType.ObjectType);
            Assert.AreEqual("1.4.1.1", contentType.Version);
        }

        [TestMethod]
        public void EtpContentType_Can_Be_Parsed_With_Json_Format()
        {
            var expected = "application/x-witsml+json;version=1.4.1.1;type=well";
            var contentType = new EtpContentType(expected);

            Assert.IsTrue(contentType.IsValid);
            Assert.AreEqual("well", contentType.ObjectType);
            Assert.AreEqual("1.4.1.1", contentType.Version);
            Assert.AreEqual("json", contentType.Format);
        }

        [TestMethod]
        public void EtpContentType_Can_Be_Converted_From_Xml_To_Json_Format()
        {
            var expected = "application/x-witsml+xml;version=1.4.1.1;type=well";
            var contentType = new EtpContentType(expected);

            Assert.IsTrue(contentType.IsValid);
            Assert.AreEqual("well", contentType.ObjectType);
            Assert.AreEqual("1.4.1.1", contentType.Version);
            Assert.AreEqual("xml", contentType.Format);

            var converted = "application/x-witsml+json;version=1.4.1.1;type=well";
            contentType = contentType.AsJson();

            Assert.IsTrue(contentType.IsValid);
            Assert.AreEqual("well", contentType.ObjectType);
            Assert.AreEqual("1.4.1.1", contentType.Version);
            Assert.AreEqual("json", contentType.Format);
            Assert.AreEqual(converted, contentType.ToString());
        }

        [TestMethod]
        public void EtpContentType_Can_Be_Converted_From_Json_To_Xml_Format()
        {
            var expected = "application/x-witsml+json;version=1.4.1.1;type=well";
            var contentType = new EtpContentType(expected);

            Assert.IsTrue(contentType.IsValid);
            Assert.AreEqual("well", contentType.ObjectType);
            Assert.AreEqual("1.4.1.1", contentType.Version);
            Assert.AreEqual("json", contentType.Format);

            var converted = "application/x-witsml+xml;version=1.4.1.1;type=well";
            contentType = contentType.AsXml();

            Assert.IsTrue(contentType.IsValid);
            Assert.AreEqual("well", contentType.ObjectType);
            Assert.AreEqual("1.4.1.1", contentType.Version);
            Assert.AreEqual("xml", contentType.Format);
            Assert.AreEqual(converted, contentType.ToString());
        }
    }
}
