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
    public class EtpDataObjectTypeTests
    {
        [TestMethod]
        public void EtpDataType_Can_Parse_Base_DataObjectType_With_Trailing_Semicolon()
        {
            var expected = "witsml20.";
            var dataType = new EtpDataObjectType(expected);

            Assert.IsTrue(dataType.IsValid);
            Assert.IsTrue(dataType.IsBaseType);
            Assert.AreEqual("witsml", dataType.Family);
            Assert.AreEqual("2.0", dataType.Version);
        }

        [TestMethod]
        public void EtpDataType_Rejects_DataObjectType_Without_Version()
        {
            var expected = "witsml";
            var dataType = new EtpDataObjectType(expected);

            Assert.IsFalse(dataType.IsValid);
        }

        [TestMethod]
        public void EtpDataType_For_Can_Create_1411_Well_DataObjectType()
        {
            var expected = "witsml14.";
            var dataType = new EtpDataObjectType(expected).For("well");

            Assert.IsTrue(dataType.IsValid);
            Assert.AreEqual("well", dataType.ObjectType);
            Assert.AreEqual("1.4.1.1", dataType.Version);
            Assert.AreEqual(expected + "well", (string)dataType);
        }

        [TestMethod]
        public void EtpDataType_Can_Parse_Witsml_20_Well_DataObjectType()
        {
            var expected = "witsml20.Well";
            var dataType = new EtpDataObjectType(expected);

            Assert.IsTrue(dataType.IsValid);
            Assert.AreEqual("Well", dataType.ObjectType);
            Assert.AreEqual("2.0", dataType.Version);
        }

        [TestMethod]
        public void EtpDataType_Can_Parse_Witsml_20_TrajectoryStation_DataObjectType()
        {
            var expected = "witsml20.part_TrajectoryStation";
            var dataType = new EtpDataObjectType(expected);

            Assert.IsTrue(dataType.IsValid);
            Assert.AreEqual("TrajectoryStation", dataType.ObjectType);
            Assert.AreEqual("2.0", dataType.Version);
        }

        [TestMethod]
        public void EtpDataType_Can_Parse_Witsml_1411_Well_DataObjectType()
        {
            var expected = "witsml14.well";
            var dataType = new EtpDataObjectType(expected);

            Assert.IsTrue(dataType.IsValid);
            Assert.AreEqual("well", dataType.ObjectType);
            Assert.AreEqual("1.4.1.1", dataType.Version);
        }

        [TestMethod]
        public void EtpDataType_Can_Be_Converted_To_ContentType()
        {
            var dataType = new EtpDataObjectType("witsml14.well");

            Assert.IsTrue(dataType.IsValid);
            Assert.AreEqual("well", dataType.ObjectType);
            Assert.AreEqual("1.4.1.1", dataType.Version);

            var converted = "application/x-witsml+xml;version=1.4.1.1;type=well";
            var contentType = dataType.ToContentType();

            Assert.IsTrue(contentType.IsValid);
            Assert.AreEqual("well", contentType.ObjectType);
            Assert.AreEqual("1.4.1.1", contentType.Version);
            Assert.AreEqual(Formats.Xml, contentType.Format);
            Assert.AreEqual(converted, contentType.ToString());
        }
    }
}
