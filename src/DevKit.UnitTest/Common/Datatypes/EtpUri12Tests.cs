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

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Energistics.Etp.Common.Datatypes
{
    [TestClass]
    public class EtpUri12Tests
    {
        [TestMethod]
        public void EtpUri_12_IsRoot_Can_Detect_Root_Uri()
        {
            var uri = "eml:///";
            var etpUri = new EtpUri(uri);

            Assert.IsTrue(etpUri.IsEtp12);
            Assert.IsTrue(etpUri.IsRoot);
            Assert.IsTrue(etpUri.IsValid);
            Assert.AreEqual(EtpUri.RootUri12, etpUri);

            uri = "EML:///";
            etpUri = new EtpUri(uri);

            Assert.IsTrue(etpUri.IsEtp12);
            Assert.IsTrue(etpUri.IsRoot);
            Assert.IsTrue(etpUri.IsValid);
            Assert.AreEqual(EtpUri.RootUri12, etpUri);
        }

        [TestMethod]
        public void EtpUri_12_IsRoot_Can_Detect_Invalid_Root_Uri()
        {
            var uri = "eml:";
            var etpUri = new EtpUri(uri);

            Assert.IsFalse(etpUri.IsValid);

            uri = "eml://";
            etpUri = new EtpUri(uri);

            Assert.IsFalse(etpUri.IsEtp12);
        }

        [TestMethod]
        public void EtpUri_12_Can_Detect_Invalid_Uri()
        {
            var expected = "eml://witsml.well";
            var uri = new EtpUri(expected);

            Assert.IsFalse(uri.IsValid);
            Assert.IsNull(uri.Version);
            Assert.AreEqual(uri, uri.Parent);
        }

        [TestMethod]
        public void EtpUri_12_Can_Parse_Uri_With_DataSpace()
        {
            var uri = new EtpUri("eml:///dataspace(custom-database)");

            Assert.IsTrue(uri.IsValid);
            Assert.IsTrue(uri.IsPrefix);
            Assert.AreEqual(uri, uri.Parent);
            Assert.AreEqual("custom-database", uri.Dataspace);
        }

        [TestMethod]
        public void EtpUri_12_Can_Parse_Uri_With_Hierarchical_DataSpace()
        {
            var uri = new EtpUri("eml:///dataspace(custom-database/second-level)");

            Assert.IsTrue(uri.IsValid);
            Assert.IsTrue(uri.IsPrefix);
            Assert.AreEqual(uri, uri.Parent);
            Assert.AreEqual("custom-database/second-level", uri.Dataspace);
        }

        [TestMethod]
        public void EtpUri_12_Can_Parse_Witsml_20_Well_Uri()
        {
            var uuid = Uuid();
            var uri = new EtpUri("eml:///witsml20.Well(" + uuid + ")");
            var clone = new EtpUri(uri);

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual(uuid, uri.ObjectId);
            Assert.AreEqual("Well", uri.ObjectType);
            Assert.AreEqual("2.0", uri.Version);

            Assert.IsTrue(uri.IsCanonical);

            // Assert Equals and GetHashCode
            Assert.IsTrue(uri.Equals(clone));
            Assert.IsTrue(uri.Equals((object)clone));
            Assert.IsFalse(uri.Equals((string)clone));
            Assert.AreEqual(uri.GetHashCode(), clone.GetHashCode());
        }

        [TestMethod]
        public void EtpUri_12_Can_Parse_Witsml_14_Well_Uri_With_Equals_In_The_Uid()
        {
            var uuid = Uuid() + "=";
            var uri = new EtpUri("eml:///witsml14.obj_well(" + uuid + ")");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual(uuid, uri.ObjectId);
            Assert.AreEqual("well", uri.ObjectType);
            Assert.AreEqual("1.4.1.1", uri.Version);
        }

        [TestMethod]
        public void EtpUri_12_Can_Parse_Witsml_20_Log_Channel_Uri()
        {
            var uuid = Uuid();
            var uri = new EtpUri("eml:///witsml20.Log(" + uuid + ")/witsml20.Channel(ROPA)");
            var ids = uri.GetObjectIds().FirstOrDefault();

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("Channel", uri.ObjectType);
            Assert.AreEqual("ROPA", uri.ObjectId);
            Assert.AreEqual("2.0", uri.Version);

            Assert.IsNotNull(ids);
            Assert.AreEqual("Log", ids.ObjectType);
            Assert.AreEqual(uuid, ids.ObjectId);
        }

        [TestMethod]
        public void EtpUri_12_Can_Parse_Witsml_20_TrajectoryStation_Uri()
        {
            var uuid1 = Uuid();
            var uuid2 = Uuid();
            var contentType = "application/x-witsml+xml;version=2.0;type=part_TrajectoryStation";
            var uri = new EtpUri($"eml:///witsml20.Trajectory({uuid1})/witsml20.TrajectoryStation({uuid2})");
            var ids = uri.GetObjectIds().FirstOrDefault();

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("TrajectoryStation", uri.ObjectType);
            Assert.AreEqual(contentType, uri.ContentType);
            Assert.AreEqual(uuid2, uri.ObjectId);
            Assert.AreEqual("2.0", uri.Version);

            Assert.IsNotNull(ids);
            Assert.AreEqual("Trajectory", ids.ObjectType);
            Assert.AreEqual(uuid1, ids.ObjectId);

            uri = new EtpUri($"eml:///witsml20.TrajectoryStation({uuid2})");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("TrajectoryStation", uri.ObjectType);
            Assert.AreEqual(contentType, uri.ContentType);
            Assert.AreEqual(uuid2, uri.ObjectId);
            Assert.AreEqual("2.0", uri.Version);
        }

        [TestMethod]
        public void EtpUri_12_IsRelatedTo_Can_Detect_Different_Families()
        {
            var uriResqml = new EtpUri("eml:///resqml20.AbstractObject");
            var uriWitsml = new EtpUri("eml:///witsml20.AbstractObject");

            Assert.IsTrue(uriResqml.IsValid);
            Assert.IsTrue(uriWitsml.IsValid);
            Assert.IsFalse(uriResqml.IsRelatedTo(uriWitsml));
        }

        [TestMethod]
        public void EtpUri_12_IsRelatedTo_Can_Detect_Different_Versions()
        {
            var uri14 = new EtpUri("eml:///witsml14.well");
            var uri20 = new EtpUri("eml:///witsml20.Well");

            Assert.IsTrue(uri14.IsValid);
            Assert.IsTrue(uri20.IsValid);
            Assert.IsFalse(uri14.IsRelatedTo(uri20));
        }

        [TestMethod]
        public void EtpUri_12_Append_Can_Append_Object_Type_To_Root_Uri()
        {
            var uri20 = new EtpUri("eml:///");
            var uriWell = uri20.Append("witsml", "2.0", "Well");

            Assert.IsTrue(uriWell.IsValid);
            Assert.IsFalse(uriWell.IsPrefix);
            Assert.AreEqual("2.0", uriWell.Version);
            Assert.AreEqual("Well", uriWell.ObjectType);

            Assert.AreEqual(uri20, uriWell.Parent);
        }

        [TestMethod]
        public void EtpUri_12_Append_Can_Append_Object_Type_And_Id_To_Base_Uri()
        {
            var uri = new EtpUri("eml:///witsml14.well(w-01)").Append("witsml", "1.4.1.1", "wellbore", "wb-01");

            Assert.IsTrue(uri.IsValid);
            Assert.IsFalse(uri.IsPrefix);
            Assert.AreEqual("1.4.1.1", uri.Version);
            Assert.AreEqual("wellbore", uri.ObjectType);
            Assert.AreEqual("wb-01", uri.ObjectId);
        }

        [TestMethod]
        public void EtpUri_12_Can_Parse_Witsml_1411_Well_Uri()
        {
            var uuid = Uuid();
            var expected = "eml:///witsml14.well(" + uuid + ")";
            var contentType = "application/x-witsml+xml;version=1.4.1.1;type=well";
            var uri = new EtpUri(expected);

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual(uuid, uri.ObjectId);
            Assert.AreEqual("well", uri.ObjectType);
            Assert.AreEqual("1.4.1.1", uri.Version);
            Assert.AreEqual(expected, uri.ToString());
            Assert.AreEqual(contentType, uri.ContentType.ToString());
        }

        [TestMethod]
        public void EtpUri_12_Can_Parse_Witsml_1411_Wellbore_Uri()
        {
            var uuid = Uuid();
            var uriWell = new EtpUri("eml:///witsml14.well(" + Uuid() + ")");
            var uriWellbore = uriWell.Append(uriWell.Family, uriWell.Version, "wellbore", uuid);

            Assert.IsNotNull(uriWellbore);
            Assert.IsTrue(uriWellbore.IsValid);
            Assert.AreEqual(uuid, uriWellbore.ObjectId);
            Assert.AreEqual("wellbore", uriWellbore.ObjectType);
            Assert.AreEqual("1.4.1.1", uriWellbore.Version);

            Assert.IsTrue(uriWellbore.IsRelatedTo(uriWell));
            var parent = uriWellbore.Parent;
            Assert.AreEqual(uriWell, parent);
        }

        [TestMethod]
        public void EtpUri_12_Can_Parse_Witsml_20_Log_Uri()
        {
            var uuid = Uuid();
            var uri = new EtpUri("eml:///witsml20.Well(" + Uuid() + ")/witsml20.Wellbore(" + Uuid() + ")/witsml20.Log(" + uuid + ")");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual(uuid, uri.ObjectId);
            Assert.AreEqual("Log", uri.ObjectType);
            Assert.AreEqual("2.0", uri.Version);
        }

        [TestMethod]
        public void EtpUri_12_Can_Parse_Full_Uri_Using_Schema_Types()
        {
            var uri = new EtpUri("eml:///dataspace(custom-database)/witsml14.obj_well(" + Uuid() + ")/witsml14.obj_wellbore(" + Uuid() + ")/witsml14.obj_log(" + Uuid() + ")/witsml14.cs_logCurveInfo(GR)");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("custom-database", uri.Dataspace);
            Assert.AreEqual("witsml", uri.Family);
            Assert.AreEqual("1.4.1.1", uri.Version);
            Assert.AreEqual("logCurveInfo", uri.ObjectType);
            Assert.AreEqual("GR", uri.ObjectId);
        }

        [TestMethod]
        public void EtpUri_12_Can_Parse_Full_Uri_Using_Object_Types()
        {
            var uri = new EtpUri("eml:///dataspace(custom-database)/witsml14.well(" + Uuid() + ")/witsml14.wellbore(" + Uuid() + ")/witsml14.log(" + Uuid() + ")/witsml14.logCurveInfo(GR)");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("custom-database", uri.Dataspace);
            Assert.AreEqual("witsml", uri.Family);
            Assert.AreEqual("1.4.1.1", uri.Version);
            Assert.AreEqual("logCurveInfo", uri.ObjectType);
            Assert.AreEqual("GR", uri.ObjectId);
        }

        [TestMethod]
        public void EtpUri_12_Can_Parse_Uri_With_No_Format_Specified()
        {
            var uri = new EtpUri("eml:///witsml14.well(" + Uuid() + ")");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("1.4.1.1", uri.Version);
            Assert.AreEqual("well", uri.ObjectType);
            Assert.AreEqual("xml", uri.Format);
        }

        [TestMethod]
        public void EtpUri_12_Can_Parse_Uri_With_Xml_Format_Query_String_Specified()
        {
            var uri = new EtpUri("eml:///witsml14.well(" + Uuid() + ")?$format=xml");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("1.4.1.1", uri.Version);
            Assert.AreEqual("well", uri.ObjectType);
            Assert.AreEqual("xml", uri.Format);
        }

        [TestMethod]
        public void EtpUri_12_Can_Parse_Uri_With_Json_Format_Query_String_Specified()
        {
            var uri = new EtpUri("eml:///witsml14.well(" + Uuid() + ")?$format=json");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("1.4.1.1", uri.Version);
            Assert.AreEqual("well", uri.ObjectType);
            Assert.AreEqual("json", uri.Format);
        }

        [TestMethod]
        public void EtpUri_12_Can_Parse_Uri_With_Query_String()
        {
            var uri = new EtpUri("eml:///witsml20.Well?name=value");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("2.0", uri.Version);
            Assert.AreEqual("Well", uri.ObjectType);
            Assert.AreEqual("?name=value", uri.Query);
            Assert.AreEqual(string.Empty, uri.Hash);
        }

        [TestMethod]
        public void EtpUri_12_Can_Parse_Uri_With_Hash_Segment()
        {
            var uri = new EtpUri("eml:///witsml20.Well#/value");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("2.0", uri.Version);
            Assert.AreEqual("Well", uri.ObjectType);
            Assert.AreEqual("#/value", uri.Hash);
            Assert.AreEqual(string.Empty, uri.Query);
        }

        [TestMethod]
        public void EtpUri_12_Can_Parse_Uri_With_Query_String_And_Hash_Segment()
        {
            var uri = new EtpUri("eml:///witsml20.Well?name=value#/value");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("2.0", uri.Version);
            Assert.AreEqual("Well", uri.ObjectType);
            Assert.AreEqual("?name=value", uri.Query);
            Assert.AreEqual("#/value", uri.Hash);
        }

        [TestMethod]
        public void EtpUri_12_Can_Parse_Uri_With_Url_Encoded_Values()
        {
            var uri = new EtpUri("eml:///witsml20.Well(abc%20123%3D)");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("2.0", uri.Version);
            Assert.AreEqual("Well", uri.ObjectType);
            Assert.AreEqual("abc 123=", uri.ObjectId);
        }

        [TestMethod, Description("Tests object UIDs with a space are invalid")]
        public void EtpUri_12_UIDs_With_Spaces_Are_Invalid()
        {
            var wellUid = Uuid();
            wellUid = wellUid.Insert(wellUid.Length / 2, " ");
            Assert.IsTrue(wellUid.Contains(" "));

            var uri = new EtpUri($"eml:///dataspace(custom-database)/witsml14.well({wellUid})/witsml.wellbore({Uuid()})");

            Assert.IsFalse(uri.IsValid);

            uri = new EtpUri($"eml:///witsml14.well({wellUid.Replace(" ", "")})/witsml14.wellbore({Uuid()})");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("witsml", uri.Family);
            Assert.AreEqual("1.4.1.1", uri.Version);
        }

        [TestMethod, Description("Tests object UIDs with special characters are valid")]
        public void EtpUri_12_UIDs_With_Special_Characters_Are_Valid()
        {
            var specialCharacterUid = "UID-~!@#$%=^&*_{}|(<>?;:'./[]\"";
            var uri = new EtpUri($"eml:///witsml20.Well({Uuid()})/witsml20.Wellbore({specialCharacterUid})");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("witsml", uri.Family);
            Assert.AreEqual("2.0", uri.Version);
            Assert.AreEqual(uri.ObjectId, specialCharacterUid);
        }

        [TestMethod, Description("Tests Uris with components and must end with the component ")]
        public void EtpUri_12_Components_Without_Identifiers_Must_End_At_Component()
        {
            var uri = new EtpUri("eml:///witsml20.Log/");

            Assert.IsFalse(uri.IsValid);

            uri = new EtpUri("eml:///witsml20.Log");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("witsml", uri.Family);
            Assert.AreEqual("2.0", uri.Version);
            Assert.AreEqual("Log", uri.ObjectType);
        }

        [TestMethod, Description("Tests Uris with components and identifier must end with end parenthesis")]
        public void EtpUri_12_Components_With_Identifiers_Must_End_With_End_Parenthesis()
        {
            var uuid = Uuid();
            var uri = new EtpUri($"eml:///witsml20.Well({uuid}))");

            Assert.IsFalse(uri.IsValid);

            uri = new EtpUri($"eml:///witsml20.Well({uuid})/");

            Assert.IsFalse(uri.IsValid);

            uri = new EtpUri($"eml:///witsml20.Well({uuid})");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("witsml", uri.Family);
            Assert.AreEqual("2.0", uri.Version);
            Assert.AreEqual("Well", uri.ObjectType);
        }

        [TestMethod]
        public void EtpUri_12_Can_Convert_To_11()
        {
            var uuid = Uuid();
            var uuid2 = Uuid();

            var uri12 = new EtpUri("eml:///");
            var uri11 = new EtpUri("eml://");
            Assert.AreEqual(uri11, uri12.AsEtp11());

            uri12 = new EtpUri("eml:/");
            uri11 = new EtpUri("eml://");
            Assert.AreEqual(uri11, uri12.AsEtp11());

            uri12 = new EtpUri("eml:///dataspace(some-data/space)");
            uri11 = new EtpUri("eml://some-data/space");
            Assert.AreEqual(uri11, uri12.AsEtp11());

            uri12 = new EtpUri("eml:///dataspace(some-data/space)/witsml20.Well");
            uri11 = new EtpUri("eml://some-data/space/witsml20/Well");
            Assert.AreEqual(uri11, uri12.AsEtp11());

            uri12 = new EtpUri($"eml:///dataspace(some-data/space)/witsml20.Well({uuid})");
            uri11 = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})");
            Assert.AreEqual(uri11, uri12.AsEtp11());

            uri12 = new EtpUri($"eml:///dataspace(some-data/space)/witsml20.Well({uuid}, 1.0)");
            uri11 = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})");
            Assert.AreEqual(uri11, uri12.AsEtp11());

            uri12 = new EtpUri($"eml:///dataspace(some-data/space)/witsml20.Well({uuid})/witsml20.Wellbore");
            uri11 = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})/Wellbore");
            Assert.AreEqual(uri11, uri12.AsEtp11());

            uri12 = new EtpUri($"eml:///dataspace(some-data/space)/witsml20.Well({uuid})/witsml20.Wellbore({uuid2})");
            uri11 = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})/Wellbore({uuid2})");
            Assert.AreEqual(uri11, uri12.AsEtp11());

            uri12 = new EtpUri($"eml:///dataspace(some-data/space)/witsml20.Well({uuid})/eml21.DataAssuranceRecord({uuid2})");
            uri11 = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})/DataAssuranceRecord({uuid2})");
            Assert.AreEqual(uri11, uri12.AsEtp11());

            uri12 = new EtpUri($"eml:///dataspace(some-data/space)/eml21.DataAssuranceRecord({uuid})");
            uri11 = new EtpUri($"eml://some-data/space/eml21/DataAssuranceRecord({uuid})");
            Assert.AreEqual(uri11, uri12.AsEtp11());

            uri12 = new EtpUri($"eml:///witsml20.Well({uuid})?query#hash");
            uri11 = new EtpUri($"eml://witsml20/Well({uuid})?query#hash");
            Assert.AreEqual(uri11, uri12.AsEtp11());
        }

        [TestMethod]
        public void EtpUri_12_Template_Segments_Handled_Correctly()
        {
            var uuid = Uuid();
            var uuid2 = Uuid();

            var uri = new EtpUri($"eml:///dataspace(custom-database)/witsml14.well/witsml14.wellbore({uuid})/witsml14.log/witsml14.logCurveInfo({uuid2})");
            var segments = uri.GetObjectIds().ToList();

            Assert.AreEqual(null, segments[0].ObjectId);
            Assert.AreEqual(uuid, segments[1].ObjectId);
            Assert.AreEqual(null, segments[2].ObjectId);
            Assert.AreEqual(uuid2, segments[3].ObjectId);
        }

        [TestMethod]
        public void EtpUri_12_Valid_Uris()
        {
            var uuid = Uuid();

            // Root URIs
            Assert.IsTrue(new EtpUri("eml:///").IsValid);

            // Dataspace URIs
            Assert.IsTrue(new EtpUri("eml:///dataspace(data-space)/").IsValid);
            Assert.IsTrue(new EtpUri("eml:///dataspace(data-space/second-level)").IsValid);

            // Canonical URIs
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})").IsValid);

            // Canonical Query URIs
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well").IsValid);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well?query").IsValid);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore").IsValid);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore?query").IsValid);

            // Hierarcical URIs
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})").IsValid);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})?query").IsValid);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})#hash").IsValid);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log({Uuid()})").IsValid);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log({Uuid()})?query").IsValid);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log({Uuid()})#hash").IsValid);

            // Hierarchical Query URIs
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log").IsValid);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log?query").IsValid);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log#hash").IsValid);

            // Template URIs
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore").IsValid);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore?query").IsValid);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore#hash").IsValid);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log").IsValid);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log?query").IsValid);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log#hash").IsValid);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log({Uuid()})").IsValid);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log({Uuid()})?query").IsValid);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log({Uuid()})#hash").IsValid);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore({uuid})/witsml20.Log").IsValid);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore({uuid})/witsml20.Log?query").IsValid);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore({uuid})/witsml20.Log#hash").IsValid);

            // Alternate URIs
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})?query").IsValid);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well#hash").IsValid);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})#hash").IsValid);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore#hash").IsValid);
        }

        [TestMethod]
        public void EtpUri_12_Root_Uris()
        {
            var uuid = Uuid();

            // Root URIs
            Assert.IsTrue(new EtpUri("eml:///").IsRoot);

            // Dataspace URIs
            Assert.IsFalse(new EtpUri("eml:///dataspace(data-space)/").IsRoot);
            Assert.IsFalse(new EtpUri("eml:///dataspace(data-space/second-level)").IsRoot);

            // Canonical URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})").IsRoot);

            // Canonical Query URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well").IsRoot);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well?query").IsRoot);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore").IsRoot);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore?query").IsRoot);

            // Hierarcical URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})").IsRoot);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})?query").IsRoot);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})#hash").IsRoot);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log({Uuid()})").IsRoot);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log({Uuid()})?query").IsRoot);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log({Uuid()})#hash").IsRoot);

            // Hierarchical Query URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log").IsRoot);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log?query").IsRoot);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log#hash").IsRoot);

            // Template URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore").IsRoot);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore?query").IsRoot);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore#hash").IsRoot);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log").IsRoot);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log?query").IsRoot);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log#hash").IsRoot);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log({Uuid()})").IsRoot);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log({Uuid()})?query").IsRoot);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log({Uuid()})#hash").IsRoot);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore({uuid})/witsml20.Log").IsRoot);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore({uuid})/witsml20.Log?query").IsRoot);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore({uuid})/witsml20.Log#hash").IsRoot);

            // Alternate URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})?query").IsRoot);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well#hash").IsRoot);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})#hash").IsRoot);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore#hash").IsRoot);
        }

        [TestMethod]
        public void EtpUri_12_Prefix_Uris()
        {
            var uuid = Uuid();

            // Root URIs
            Assert.IsTrue(new EtpUri("eml:///").IsPrefix);

            // Dataspace URIs
            Assert.IsTrue(new EtpUri("eml:///dataspace(data-space)/").IsPrefix);
            Assert.IsTrue(new EtpUri("eml:///dataspace(data-space/second-level)").IsPrefix);

            // Canonical URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})").IsPrefix);

            // Canonical Query URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well?query").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore?query").IsPrefix);

            // Hierarcical URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})?query").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})#hash").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log({Uuid()})").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log({Uuid()})?query").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log({Uuid()})#hash").IsPrefix);

            // Hierarchical Query URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log?query").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log#hash").IsPrefix);

            // Template URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore?query").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore#hash").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log?query").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log#hash").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log({Uuid()})").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log({Uuid()})?query").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log({Uuid()})#hash").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore({uuid})/witsml20.Log").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore({uuid})/witsml20.Log?query").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore({uuid})/witsml20.Log#hash").IsPrefix);

            // Alternate URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})?query").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well#hash").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})#hash").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore#hash").IsPrefix);
        }

        [TestMethod]
        public void EtpUri_12_Canonical_Uris()
        {
            var uuid = Uuid();

            // Root URIs
            Assert.IsTrue(new EtpUri("eml:///").IsCanonical);

            // Dataspace URIs
            Assert.IsTrue(new EtpUri("eml:///dataspace(data-space)/").IsCanonical);
            Assert.IsTrue(new EtpUri("eml:///dataspace(data-space/second-level)").IsCanonical);

            // Canonical URIs
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})").IsCanonical);

            // Canonical Query URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well?query").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore?query").IsCanonical);

            // Hierarcical URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})?query").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})#hash").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log({Uuid()})").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log({Uuid()})?query").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log({Uuid()})#hash").IsCanonical);

            // Hierarchical Query URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log?query").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log#hash").IsCanonical);

            // Template URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore?query").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore#hash").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log?query").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log#hash").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log({Uuid()})").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log({Uuid()})?query").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log({Uuid()})#hash").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore({uuid})/witsml20.Log").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore({uuid})/witsml20.Log?query").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore({uuid})/witsml20.Log#hash").IsCanonical);

            // Alternate URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})?query").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well#hash").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})#hash").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore#hash").IsCanonical);
        }

        [TestMethod]
        public void EtpUri_12_Query_Uris()
        {
            var uuid = Uuid();

            // Root URIs
            Assert.IsFalse(new EtpUri("eml:///").IsCanonicalQuery);

            // Dataspace URIs
            Assert.IsFalse(new EtpUri("eml:///dataspace(data-space)/").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri("eml:///dataspace(data-space/second-level)").IsCanonicalQuery);

            // Canonical URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})").IsCanonicalQuery);

            // Canonical Query URIs
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well").IsCanonicalQuery);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well?query").IsCanonicalQuery);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore").IsCanonicalQuery);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore?query").IsCanonicalQuery);

            // Hierarcical URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})?query").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})#hash").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log({Uuid()})").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log({Uuid()})?query").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log({Uuid()})#hash").IsCanonicalQuery);

            // Hierarchical Query URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log?query").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log#hash").IsCanonicalQuery);

            // Template URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore?query").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore#hash").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log?query").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log#hash").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log({Uuid()})").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log({Uuid()})?query").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log({Uuid()})#hash").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore({uuid})/witsml20.Log").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore({uuid})/witsml20.Log?query").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore({uuid})/witsml20.Log#hash").IsCanonicalQuery);

            // Alternate URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})?query").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well#hash").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})#hash").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore#hash").IsCanonicalQuery);
        }

        [TestMethod]
        public void EtpUri_12_Alternate_Uris()
        {
            var uuid = Uuid();

            // Root URIs
            Assert.IsFalse(new EtpUri("eml:///").IsAlternate);

            // Dataspace URIs
            Assert.IsFalse(new EtpUri("eml:///dataspace(data-space)/").IsAlternate);
            Assert.IsFalse(new EtpUri("eml:///dataspace(data-space/second-level)").IsAlternate);

            // Canonical URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})").IsAlternate);

            // Canonical Query URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well").IsAlternate);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well?query").IsAlternate);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore").IsAlternate);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore?query").IsAlternate);

            // Hierarcical URIs
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})?query").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})#hash").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log({Uuid()})").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log({Uuid()})?query").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log({Uuid()})#hash").IsAlternate);

            // Hierarchical Query URIs
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log?query").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log#hash").IsAlternate);

            // Template URIs
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore?query").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore#hash").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log?query").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log#hash").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log({Uuid()})").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log({Uuid()})?query").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log({Uuid()})#hash").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore({uuid})/witsml20.Log").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore({uuid})/witsml20.Log?query").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore({uuid})/witsml20.Log#hash").IsAlternate);

            // Alternate URIs
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})?query").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well#hash").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})#hash").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore#hash").IsAlternate);
        }

        [TestMethod]
        public void EtpUri_12_Hierarchical_Uris()
        {
            var uuid = Uuid();

            // Root URIs
            Assert.IsFalse(new EtpUri("eml:///").IsHierarchical);

            // Dataspace URIs
            Assert.IsFalse(new EtpUri("eml:///dataspace(data-space)/").IsHierarchical);
            Assert.IsFalse(new EtpUri("eml:///dataspace(data-space/second-level)").IsHierarchical);

            // Canonical URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})").IsHierarchical);

            // Canonical Query URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well?query").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore?query").IsHierarchical);

            // Hierarcical URIs
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})").IsHierarchical);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})?query").IsHierarchical);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})#hash").IsHierarchical);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log({Uuid()})").IsHierarchical);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log({Uuid()})?query").IsHierarchical);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log({Uuid()})#hash").IsHierarchical);

            // Hierarchical Query URIs
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log").IsHierarchical);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log?query").IsHierarchical);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log#hash").IsHierarchical);

            // Template URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore?query").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore#hash").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log?query").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log#hash").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log({Uuid()})").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log({Uuid()})?query").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log({Uuid()})#hash").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore({uuid})/witsml20.Log").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore({uuid})/witsml20.Log?query").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore({uuid})/witsml20.Log#hash").IsHierarchical);

            // Alternate URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})?query").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well#hash").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})#hash").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore#hash").IsHierarchical);
        }

        [TestMethod]
        public void EtpUri_12_Template_Uris()
        {
            var uuid = Uuid();

            // Root URIs
            Assert.IsFalse(new EtpUri("eml:///").IsTemplate);

            // Dataspace URIs
            Assert.IsFalse(new EtpUri("eml:///dataspace(data-space)/").IsTemplate);
            Assert.IsFalse(new EtpUri("eml:///dataspace(data-space/second-level)").IsTemplate);

            // Canonical URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})").IsTemplate);

            // Canonical Query URIs
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well?query").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore?query").IsTemplate);

            // Hierarcical URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})").IsTemplate);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})?query").IsTemplate);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})#hash").IsTemplate);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log({Uuid()})").IsTemplate);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log({Uuid()})?query").IsTemplate);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log({Uuid()})#hash").IsTemplate);

            // Hierarchical Query URIs
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log?query").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore({Uuid()})/witsml20.Log#hash").IsTemplate);

            // Template URIs
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore?query").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore#hash").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log?query").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log#hash").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log({Uuid()})").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log({Uuid()})?query").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore/witsml20.Log({Uuid()})#hash").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore({uuid})/witsml20.Log").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore({uuid})/witsml20.Log?query").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml:///witsml20.Well/witsml20.Wellbore({uuid})/witsml20.Log#hash").IsTemplate);

            // Alternate URIs
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})?query").IsTemplate);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well#hash").IsTemplate);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})#hash").IsTemplate);
            Assert.IsFalse(new EtpUri($"eml:///witsml20.Well({uuid})/witsml20.Wellbore#hash").IsTemplate);
        }

        private string Uuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
