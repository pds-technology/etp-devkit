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
    public class EtpUri11Tests
    {
        [TestMethod]
        public void EtpUri_11_IsRoot_Can_Detect_Root_Uri()
        {
            var uri = "eml://";
            var etpUri = new EtpUri(uri);

            Assert.IsTrue(etpUri.IsEtp11);
            Assert.IsTrue(etpUri.IsRoot);
            Assert.IsTrue(etpUri.IsValid);
            Assert.AreEqual(EtpUri.RootUri11, etpUri);

            uri = "EML://";
            etpUri = new EtpUri(uri);

            Assert.IsTrue(etpUri.IsEtp11);
            Assert.IsTrue(etpUri.IsRoot);
            Assert.IsTrue(etpUri.IsValid);
            Assert.AreEqual(EtpUri.RootUri11, etpUri);
        }

        [TestMethod]
        public void EtpUri_11_IsRoot_Can_Detect_Invalid_Root_Uri()
        {
            var uri = "eml:";
            var etpUri = new EtpUri(uri);

            Assert.IsFalse(etpUri.IsValid);

            uri = "eml:/";
            etpUri = new EtpUri(uri);

            Assert.IsFalse(etpUri.IsEtp11);

            uri = "eml:///";
            etpUri = new EtpUri(uri);

            Assert.IsFalse(etpUri.IsEtp11);
        }

        [TestMethod]
        public void EtpUri_11_Can_Detect_Invalid_Uri()
        {
            var expected = "eml:///witsml/well";
            var uri = new EtpUri(expected);

            Assert.IsFalse(uri.IsValid);
            Assert.IsNull(uri.Version);
            Assert.AreEqual(uri, uri.Parent);
        }

        [TestMethod]
        public void EtpUri_11_Can_Parse_Uri_With_DataSpace()
        {
            var uri = new EtpUri("eml://custom-database/witsml14");

            Assert.IsTrue(uri.IsValid);
            Assert.IsTrue(uri.IsPrefix);
            Assert.AreEqual(uri, uri.Parent);
            Assert.AreEqual("1.4.1.1", uri.Version);
            Assert.AreEqual("custom-database", uri.Dataspace);
        }

        [TestMethod]
        public void EtpUri_11_Can_Parse_Uri_With_Hierarchical_DataSpace()
        {
            var uri = new EtpUri("eml://custom-database/second-level/witsml14");

            Assert.IsTrue(uri.IsValid);
            Assert.IsTrue(uri.IsPrefix);
            Assert.AreEqual(uri, uri.Parent);
            Assert.AreEqual("1.4.1.1", uri.Version);
            Assert.AreEqual("custom-database/second-level", uri.Dataspace);
        }

        [TestMethod]
        public void EtpUri_11_Can_Parse_Witsml_20_Base_Uri()
        {
            var uri = new EtpUri("eml://witsml20");

            Assert.IsTrue(uri.IsValid);
            Assert.IsTrue(uri.IsPrefix);
            Assert.AreEqual(uri, uri.Parent);
            Assert.AreEqual("2.0", uri.Version);
        }

        [TestMethod]
        public void EtpUri_11_Can_Parse_Witsml_20_Well_Uri()
        {
            var uuid = Uuid();
            var uri = new EtpUri("eml://witsml20/Well(" + uuid + ")");
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
        public void EtpUri_11_Can_Parse_Witsml_14_Well_Uri_With_Equals_In_The_Uid()
        {
            var uuid = Uuid() + "=";
            var uri = new EtpUri("eml://witsml14/obj_well(" + uuid + ")");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual(uuid, uri.ObjectId);
            Assert.AreEqual("well", uri.ObjectType);
            Assert.AreEqual("1.4.1.1", uri.Version);
        }

        [TestMethod]
        public void EtpUri_11_Can_Parse_Witsml_20_Log_Channel_Uri()
        {
            var uuid = Uuid();
            var uri = new EtpUri("eml://witsml20/Log(" + uuid + ")/Channel(ROPA)");
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
        public void EtpUri_11_Can_Parse_Witsml_20_TrajectoryStation_Uri()
        {
            var uuid1 = Uuid();
            var uuid2 = Uuid();
            var contentType = "application/x-witsml+xml;version=2.0;type=part_TrajectoryStation";
            var uri = new EtpUri($"eml://witsml20/Trajectory({uuid1})/TrajectoryStation({uuid2})");
            var ids = uri.GetObjectIds().FirstOrDefault();

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("TrajectoryStation", uri.ObjectType);
            Assert.AreEqual(contentType, uri.ContentType);
            Assert.AreEqual(uuid2, uri.ObjectId);
            Assert.AreEqual("2.0", uri.Version);

            Assert.IsNotNull(ids);
            Assert.AreEqual("Trajectory", ids.ObjectType);
            Assert.AreEqual(uuid1, ids.ObjectId);

            uri = new EtpUri($"eml://witsml20/TrajectoryStation({uuid2})");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("TrajectoryStation", uri.ObjectType);
            Assert.AreEqual(contentType, uri.ContentType);
            Assert.AreEqual(uuid2, uri.ObjectId);
            Assert.AreEqual("2.0", uri.Version);
        }

        [TestMethod]
        public void EtpUri_11_IsRelatedTo_Can_Detect_Different_Families()
        {
            var uriResqml = new EtpUri("eml://resqml20");
            var uriWitsml = new EtpUri("eml://witsml20");

            Assert.IsTrue(uriResqml.IsValid);
            Assert.IsTrue(uriWitsml.IsValid);
            Assert.IsFalse(uriResqml.IsRelatedTo(uriWitsml));
        }

        [TestMethod]
        public void EtpUri_11_IsRelatedTo_Can_Detect_Different_Versions()
        {
            var uri14 = new EtpUri("eml://witsml14/well");
            var uri20 = new EtpUri("eml://witsml20/Well");

            Assert.IsTrue(uri14.IsValid);
            Assert.IsTrue(uri20.IsValid);
            Assert.IsFalse(uri14.IsRelatedTo(uri20));
        }

        [TestMethod]
        public void EtpUri_11_Can_Parse_Witsml_1411_Base_Uri()
        {
            var uri = new EtpUri("eml://witsml14");

            Assert.IsTrue(uri.IsValid);
            Assert.IsTrue(uri.IsPrefix);
            Assert.AreEqual("1.4.1.1", uri.Version);
        }

        [TestMethod]
        public void EtpUri_11_Append_Can_Append_Object_Type_To_Base_Uri()
        {
            var uri14 = new EtpUri("eml://witsml14");
            var uriWell = uri14.Append(uri14.Family, uri14.Version, "well", null);

            Assert.IsTrue(uriWell.IsValid);
            Assert.IsFalse(uriWell.IsPrefix);
            Assert.AreEqual("1.4.1.1", uriWell.Version);
            Assert.AreEqual("well", uriWell.ObjectType);

            Assert.AreEqual(uri14, uriWell.Parent);
            Assert.IsTrue(uri14.IsRelatedTo(uriWell));
        }

        [TestMethod]
        public void EtpUri_11_Append_Can_Append_Object_Type_And_Id_To_Base_Uri()
        {
            var uri = new EtpUri("eml://witsml14").Append("witsml", "1.4.1.1", "well", "w-01");

            Assert.IsTrue(uri.IsValid);
            Assert.IsFalse(uri.IsPrefix);
            Assert.AreEqual("1.4.1.1", uri.Version);
            Assert.AreEqual("well", uri.ObjectType);
            Assert.AreEqual("w-01", uri.ObjectId);
        }

        [TestMethod]
        public void EtpUri_11_Can_Parse_Witsml_1411_Well_Uri()
        {
            var uuid = Uuid();
            var expected = "eml://witsml14/well(" + uuid + ")";
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
        public void EtpUri_11_Can_Parse_Witsml_1411_Wellbore_Uri()
        {
            var uuid = Uuid();
            var uriWell = new EtpUri("eml://witsml14/well(" + Uuid() + ")");
            var uriWellbore = uriWell.Append(uriWell.Family, uriWell.Version, "wellbore", uuid);

            Assert.IsNotNull(uriWellbore);
            Assert.IsTrue(uriWellbore.IsValid);
            Assert.AreEqual(uuid, uriWellbore.ObjectId);
            Assert.AreEqual("wellbore", uriWellbore.ObjectType);
            Assert.AreEqual("1.4.1.1", uriWellbore.Version);

            Assert.IsTrue(uriWellbore.IsRelatedTo(uriWell));
            Assert.AreEqual(uriWell, uriWellbore.Parent);
        }

        [TestMethod]
        public void EtpUri_11_Can_Parse_Witsml_1411_Log_Uri()
        {
            var uuid = Uuid();
            var uri = new EtpUri("eml://witsml14/well(" + Uuid() + ")/wellbore(" + Uuid() + ")/log(" + uuid + ")");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual(uuid, uri.ObjectId);
            Assert.AreEqual("log", uri.ObjectType);
            Assert.AreEqual("1.4.1.1", uri.Version);
        }

        [TestMethod]
        public void EtpUri_11_Can_Parse_Full_Uri_Using_Schema_Types()
        {
            var uri = new EtpUri("eml://custom-database/witsml14/obj_well(" + Uuid() + ")/obj_wellbore(" + Uuid() + ")/obj_log(" + Uuid() + ")/cs_logCurveInfo(GR)");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("custom-database", uri.Dataspace);
            Assert.AreEqual("witsml", uri.Family);
            Assert.AreEqual("1.4.1.1", uri.Version);
            Assert.AreEqual("logCurveInfo", uri.ObjectType);
            Assert.AreEqual("GR", uri.ObjectId);
        }

        [TestMethod]
        public void EtpUri_11_Can_Parse_Full_Uri_Using_Object_Types()
        {
            var uri = new EtpUri("eml://custom-database/witsml14/well(" + Uuid() + ")/wellbore(" + Uuid() + ")/log(" + Uuid() + ")/logCurveInfo(GR)");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("custom-database", uri.Dataspace);
            Assert.AreEqual("witsml", uri.Family);
            Assert.AreEqual("1.4.1.1", uri.Version);
            Assert.AreEqual("logCurveInfo", uri.ObjectType);
            Assert.AreEqual("GR", uri.ObjectId);
        }

        [TestMethod]
        public void EtpUri_11_Can_Parse_Uri_With_No_Format_Specified()
        {
            var uri = new EtpUri("eml://witsml14/well(" + Uuid() + ")");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("1.4.1.1", uri.Version);
            Assert.AreEqual("well", uri.ObjectType);
            Assert.AreEqual("xml", uri.Format);
        }

        [TestMethod]
        public void EtpUri_11_Can_Parse_Uri_With_Xml_Format_Query_String_Specified()
        {
            var uri = new EtpUri("eml://witsml14/well(" + Uuid() + ")?$format=xml");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("1.4.1.1", uri.Version);
            Assert.AreEqual("well", uri.ObjectType);
            Assert.AreEqual("xml", uri.Format);
        }

        [TestMethod]
        public void EtpUri_11_Can_Parse_Uri_With_Json_Format_Query_String_Specified()
        {
            var uri = new EtpUri("eml://witsml14/well(" + Uuid() + ")?$format=json");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("1.4.1.1", uri.Version);
            Assert.AreEqual("well", uri.ObjectType);
            Assert.AreEqual("json", uri.Format);
        }

        [TestMethod]
        public void EtpUri_11_Can_Parse_Uri_With_Query_String()
        {
            var uri = new EtpUri("eml://witsml20/Well?name=value");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("2.0", uri.Version);
            Assert.AreEqual("Well", uri.ObjectType);
            Assert.AreEqual("?name=value", uri.Query);
            Assert.AreEqual(string.Empty, uri.Hash);
        }

        [TestMethod]
        public void EtpUri_11_Can_Parse_Uri_With_Hash_Segment()
        {
            var uri = new EtpUri("eml://witsml20/Well#/value");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("2.0", uri.Version);
            Assert.AreEqual("Well", uri.ObjectType);
            Assert.AreEqual("#/value", uri.Hash);
            Assert.AreEqual(string.Empty, uri.Query);
        }

        [TestMethod]
        public void EtpUri_11_Can_Parse_Uri_With_Query_String_And_Hash_Segment()
        {
            var uri = new EtpUri("eml://witsml20/Well?name=value#/value");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("2.0", uri.Version);
            Assert.AreEqual("Well", uri.ObjectType);
            Assert.AreEqual("?name=value", uri.Query);
            Assert.AreEqual("#/value", uri.Hash);
        }

        [TestMethod]
        public void EtpUri_11_Can_Parse_Uri_With_Url_Encoded_Values()
        {
            var uri = new EtpUri("eml://witsml20/Well(abc%20123%3D)");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("2.0", uri.Version);
            Assert.AreEqual("Well", uri.ObjectType);
            Assert.AreEqual("abc 123=", uri.ObjectId);
        }

        [TestMethod, Description("Tests object UIDs with a space are invalid")]
        public void EtpUri_11_UIDs_With_Spaces_Are_Invalid()
        {
            var wellUid = Uuid();
            wellUid = wellUid.Insert(wellUid.Length/2, " ");
            Assert.IsTrue(wellUid.Contains(" "));

            var uri = new EtpUri($"eml://custom-database/witsml14/well({wellUid})/wellbore({Uuid()})");

            Assert.IsFalse(uri.IsValid);

            uri = new EtpUri($"eml://witsml14/well({wellUid.Replace(" ", "")})/wellbore({Uuid()})");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("witsml", uri.Family);
            Assert.AreEqual("1.4.1.1", uri.Version);
        }

        [TestMethod, Description("Tests object UIDs with special characters are valid")]
        public void EtpUri_11_UIDs_With_Special_Characters_Are_Valid()
        {
            var specialCharacterUid = "UID-~!@$%=^&*_{}|(<>;:'./[]\"";
            var uri = new EtpUri($"eml://witsml20/Well({Uuid()})/Wellbore({specialCharacterUid})");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("witsml", uri.Family);
            Assert.AreEqual("2.0", uri.Version);
            Assert.AreEqual(uri.ObjectId, specialCharacterUid);
        }

        [TestMethod, Description("Tests Uris with components and must end with the component ")]
        public void EtpUri_11_Components_Without_Identifiers_Must_End_At_Component()
        {
            var uri = new EtpUri("eml://witsml20/Log/");

            Assert.IsFalse(uri.IsValid);

            uri = new EtpUri("eml://witsml20/Log");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("witsml", uri.Family);
            Assert.AreEqual("2.0", uri.Version);
            Assert.AreEqual("Log", uri.ObjectType);
        }

        [TestMethod, Description("Tests Uris with components and identifier must end with end parenthesis")]
        public void EtpUri_11_Components_With_Identifiers_Must_End_With_End_Parenthesis()
        {
            var uuid = Uuid();
            var uri = new EtpUri($"eml://witsml20/Well({uuid}))");

            Assert.IsFalse(uri.IsValid);

            uri = new EtpUri($"eml://witsml20/Well({uuid})/");

            Assert.IsFalse(uri.IsValid);

            uri = new EtpUri($"eml://witsml20/Well({uuid})");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("witsml", uri.Family);
            Assert.AreEqual("2.0", uri.Version);
            Assert.AreEqual("Well", uri.ObjectType);
        }

        [TestMethod]
        public void EtpUri_11_Family_Correct_For_Hierarchical_Eml_Common_Uris()
        {
            var uuid = Uuid();
            var uri = new EtpUri($"eml://witsml20/Well({uuid})/DataAssuranceRecord({Uuid()})");

            Assert.IsTrue(uri.IsValid);
            Assert.AreEqual("witsml", uri.NamespaceFamily);
            Assert.AreEqual("2.0", uri.NamespaceVersion);
            Assert.AreEqual("eml", uri.Family);
            Assert.AreEqual("2.1", uri.Version);
            Assert.AreEqual("DataAssuranceRecord", uri.ObjectType);
        }

        [TestMethod]
        public void EtpUri_11_Object_Version_Is_Null_Or_Uri_Is_Invalid()
        {
            var uuid = Uuid();
            var uri = new EtpUri($"eml://witsml20/Well({uuid})");

            Assert.IsTrue(uri.IsValid);
            Assert.IsNull(uri.ObjectVersion);

            uri = new EtpUri($"eml://witsml20/Well({uuid},1.0)");
            Assert.IsFalse(uri.IsValid);
        }

        [TestMethod]
        public void EtpUri_11_Can_Convert_To_12()
        {
            var uuid = Uuid();
            var uuid2 = Uuid();

            var uri11 = new EtpUri("eml://");
            var uri12 = new EtpUri("eml:///");
            Assert.AreEqual(uri12, uri11.AsEtp12());

            uri11 = new EtpUri("eml://witsml20");
            uri12 = new EtpUri("eml:///");
            Assert.AreEqual(uri12, uri11.AsEtp12());

            uri11 = new EtpUri("eml://some-data/space");
            uri12 = new EtpUri("eml:///dataspace(some-data/space)");
            Assert.AreEqual(uri12, uri11.AsEtp12());

            uri11 = new EtpUri("eml://some-data/space/witsml20/Well");
            uri12 = new EtpUri("eml:///dataspace(some-data/space)/witsml20.Well");
            Assert.AreEqual(uri12, uri11.AsEtp12());

            uri11 = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})");
            uri12 = new EtpUri($"eml:///dataspace(some-data/space)/witsml20.Well({uuid})");
            Assert.AreEqual(uri12, uri11.AsEtp12());

            uri11 = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})/Wellbore");
            uri12 = new EtpUri($"eml:///dataspace(some-data/space)/witsml20.Well({uuid})/witsml20.Wellbore");
            Assert.AreEqual(uri12, uri11.AsEtp12());

            uri11 = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})/Wellbore({uuid2})");
            uri12 = new EtpUri($"eml:///dataspace(some-data/space)/witsml20.Well({uuid})/witsml20.Wellbore({uuid2})");
            Assert.AreEqual(uri12, uri11.AsEtp12());

            uri11 = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})/DataAssuranceRecord({uuid2})");
            uri12 = new EtpUri($"eml:///dataspace(some-data/space)/witsml20.Well({uuid})/eml21.DataAssuranceRecord({uuid2})");
            Assert.AreEqual(uri12, uri11.AsEtp12());

            uri11 = new EtpUri($"eml://some-data/space/eml21/DataAssuranceRecord({uuid})");
            uri12 = new EtpUri($"eml:///dataspace(some-data/space)/eml21.DataAssuranceRecord({uuid})");
            Assert.AreEqual(uri12, uri11.AsEtp12());

            uri12 = new EtpUri($"eml:///witsml20.Well({uuid})?query#hash");
            uri11 = new EtpUri($"eml://witsml20/Well({uuid})?query#hash");
            Assert.AreEqual(uri12, uri11.AsEtp12());
        }

        [TestMethod]
        public void EtpUri_11_Template_Segments_Handled_Correctly()
        {
            var uuid = Uuid();
            var uuid2 = Uuid();

            var uri = new EtpUri($"eml://custom-database/witsml14/well/wellbore({uuid})/log/logCurveInfo({uuid2})");
            var segments = uri.GetObjectIds().ToList();

            Assert.AreEqual(null, segments[0].ObjectId);
            Assert.AreEqual(uuid, segments[1].ObjectId);
            Assert.AreEqual(null, segments[2].ObjectId);
            Assert.AreEqual(uuid2, segments[3].ObjectId);
        }

        [TestMethod]
        public void EtpUri_11_Uri_Prefix_Returns_Correct_Prefix()
        {
            var uuid = Uuid();

            var prefix = "eml://";
            var uri = new EtpUri("eml://");
            Assert.AreEqual(prefix, uri.UriPrefix);

            prefix = "eml://data-space";
            uri = new EtpUri("eml://data-space");
            Assert.AreEqual(prefix, uri.UriPrefix);

            prefix = "eml://data-space/level-two";
            uri = new EtpUri("eml://data-space/level-two");
            Assert.AreEqual(prefix, uri.UriPrefix);

            prefix = "eml://data-space/level-two/witsml14";
            uri = new EtpUri("eml://data-space/level-two/witsml14");
            Assert.AreEqual(prefix, uri.UriPrefix);

            prefix = "eml://witsml14";
            uri = new EtpUri("eml://witsml14");
            Assert.AreEqual(prefix, uri.UriPrefix);

            prefix = $"eml://data-space/level-two/witsml14/";
            uri = new EtpUri($"eml://data-space/level-two/witsml14/well({uuid})");
            Assert.AreEqual(prefix, uri.UriPrefix);

            prefix = $"eml://witsml14/";
            uri = new EtpUri($"eml://witsml14/well({uuid})");
            Assert.AreEqual(prefix, uri.UriPrefix);
        }

        [TestMethod]
        public void EtpUri_11_As_Canonical_Returns_Correct_Uri()
        {
            var uuid = Uuid();
            var uuid2 = Uuid();

            var original = new EtpUri("eml://");
            var canonical = new EtpUri("eml://");
            Assert.AreEqual(canonical, original.AsCanonical());

            original = new EtpUri("eml://some-data/space");
            canonical = new EtpUri("eml://some-data/space");
            Assert.AreEqual(canonical, original.AsCanonical());

            original = new EtpUri("eml://some-data/space?query#hash");
            canonical = new EtpUri("eml://some-data/space");
            Assert.AreEqual(canonical, original.AsCanonical());

            original = new EtpUri("eml://some-data/space/witsml20");
            canonical = new EtpUri("eml://some-data/space/witsml20");
            Assert.AreEqual(canonical, original.AsCanonical());

            original = new EtpUri("eml://witsml20");
            canonical = new EtpUri("eml://witsml20");
            Assert.AreEqual(canonical, original.AsCanonical());

            original = new EtpUri("eml://witsml20?query#hash");
            canonical = new EtpUri("eml://witsml20");
            Assert.AreEqual(canonical, original.AsCanonical());

            original = new EtpUri("eml://some-data/space/witsml20/Well");
            canonical = new EtpUri("eml://some-data/space/witsml20/Well");
            Assert.AreEqual(canonical, original.AsCanonical());

            original = new EtpUri("eml://some-data/space/witsml20/Well?query");
            canonical = new EtpUri("eml://some-data/space/witsml20/Well?query");
            Assert.AreEqual(canonical, original.AsCanonical());

            original = new EtpUri("eml://some-data/space/witsml20/Well?query#hash");
            canonical = new EtpUri("eml://some-data/space/witsml20/Well?query");
            Assert.AreEqual(canonical, original.AsCanonical());

            original = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})");
            canonical = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})");
            Assert.AreEqual(canonical, original.AsCanonical());

            original = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})?query");
            canonical = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})");
            Assert.AreEqual(canonical, original.AsCanonical());

            original = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})?query#hash");
            canonical = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})");
            Assert.AreEqual(canonical, original.AsCanonical());

            original = new EtpUri($"eml://some-data/space/witsml20/Well/Wellbore");
            canonical = new EtpUri($"eml://some-data/space/witsml20/Wellbore");
            Assert.AreEqual(canonical, original.AsCanonical());

            original = new EtpUri($"eml://some-data/space/witsml20/Well/Wellbore?query");
            canonical = new EtpUri($"eml://some-data/space/witsml20/Wellbore?query");
            Assert.AreEqual(canonical, original.AsCanonical());

            original = new EtpUri($"eml://some-data/space/witsml20/Well/Wellbore?query#hash");
            canonical = new EtpUri($"eml://some-data/space/witsml20/Wellbore?query");
            Assert.AreEqual(canonical, original.AsCanonical());

            original = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})/Wellbore");
            canonical = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})/Wellbore");
            Assert.AreEqual(canonical, original.AsCanonical());

            original = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})/Wellbore?query");
            canonical = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})/Wellbore?query");
            Assert.AreEqual(canonical, original.AsCanonical());

            original = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})/Wellbore?query#hash");
            canonical = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})/Wellbore?query");
            Assert.AreEqual(canonical, original.AsCanonical());

            original = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})/Wellbore({uuid2})");
            canonical = new EtpUri($"eml://some-data/space/witsml20/Wellbore({uuid2})");
            Assert.AreEqual(canonical, original.AsCanonical());

            original = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})/Wellbore({uuid2})?query");
            canonical = new EtpUri($"eml://some-data/space/witsml20/Wellbore({uuid2})");
            Assert.AreEqual(canonical, original.AsCanonical());

            original = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})/Wellbore({uuid2})?query#hash");
            canonical = new EtpUri($"eml://some-data/space/witsml20/Wellbore({uuid2})");
            Assert.AreEqual(canonical, original.AsCanonical());

            original = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})/Wellbore({uuid2})/Log");
            canonical = new EtpUri($"eml://some-data/space/witsml20/Wellbore({uuid2})/Log");
            Assert.AreEqual(canonical, original.AsCanonical());

            original = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})/Wellbore({uuid2})/Log?query");
            canonical = new EtpUri($"eml://some-data/space/witsml20/Wellbore({uuid2})/Log?query");
            Assert.AreEqual(canonical, original.AsCanonical());

            original = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})/Wellbore({uuid2})/Log?query#hash");
            canonical = new EtpUri($"eml://some-data/space/witsml20/Wellbore({uuid2})/Log?query");
            Assert.AreEqual(canonical, original.AsCanonical());

            original = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})/DataAssuranceRecord({uuid2})");
            canonical = new EtpUri($"eml://some-data/space/eml21/DataAssuranceRecord({uuid2})");
            Assert.AreEqual(canonical, original.AsCanonical());
        }

        [TestMethod]
        public void EtpUri_11_Valid_Uris()
        {
            var uuid = Uuid();

            // Root URIs
            Assert.IsTrue(new EtpUri("eml://").IsValid);

            // Dataspace URIs
            Assert.IsTrue(new EtpUri("eml://data-space/").IsValid);

            // Prefix URIs
            Assert.IsTrue(new EtpUri("eml://data-space/witsml14").IsValid);
            Assert.IsTrue(new EtpUri("eml://data-space/second-level/witsml14").IsValid);
            Assert.IsTrue(new EtpUri("eml://witsml14").IsValid);

            // Canonical URIs
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})").IsValid);

            // Canonical Query URIs
            Assert.IsTrue(new EtpUri($"eml://witsml14/well").IsValid);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well?query").IsValid);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore").IsValid);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore?query").IsValid);

            // Hierarcical URIs
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})").IsValid);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})?query").IsValid);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})#hash").IsValid);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})").IsValid);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})?query").IsValid);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})#hash").IsValid);

            // Hierarchical Query URIs
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log").IsValid);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log?query").IsValid);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log#hash").IsValid);

            // Template URIs
            Assert.IsTrue(new EtpUri($"eml://witsml14/well/wellbore").IsValid);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well/wellbore?query").IsValid);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well/wellbore#hash").IsValid);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log").IsValid);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log?query").IsValid);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log#hash").IsValid);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})").IsValid);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})?query").IsValid);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})#hash").IsValid);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log").IsValid);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log?query").IsValid);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log#hash").IsValid);

            // Alternate Prefix URIs
            Assert.IsTrue(new EtpUri("eml://?query").IsValid);
            Assert.IsTrue(new EtpUri("eml://data-space/?query").IsValid);
            Assert.IsTrue(new EtpUri("eml://data-space/witsml14?query").IsValid);

            // Alternate URIs
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})?query").IsValid);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well#hash").IsValid);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})#hash").IsValid);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore#hash").IsValid);
        }


        [TestMethod]
        public void EtpUri_11_Etp11_Uris()
        {
            var uuid = Uuid();

            // Root URIs
            Assert.IsTrue(new EtpUri("eml://").IsEtp11);

            // Dataspace URIs
            Assert.IsTrue(new EtpUri("eml://data-space/").IsEtp11);

            // Prefix URIs
            Assert.IsTrue(new EtpUri("eml://data-space/witsml14").IsEtp11);
            Assert.IsTrue(new EtpUri("eml://data-space/second-level/witsml14").IsEtp11);
            Assert.IsTrue(new EtpUri("eml://witsml14").IsEtp11);

            // Canonical URIs
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})").IsEtp11);

            // Canonical Query URIs
            Assert.IsTrue(new EtpUri($"eml://witsml14/well").IsEtp11);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well?query").IsEtp11);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore").IsEtp11);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore?query").IsEtp11);

            // Hierarcical URIs
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})").IsEtp11);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})?query").IsEtp11);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})#hash").IsEtp11);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})").IsEtp11);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})?query").IsEtp11);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})#hash").IsEtp11);

            // Hierarchical Query URIs
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log").IsEtp11);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log?query").IsEtp11);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log#hash").IsEtp11);

            // Template URIs
            Assert.IsTrue(new EtpUri($"eml://witsml14/well/wellbore").IsEtp11);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well/wellbore?query").IsEtp11);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well/wellbore#hash").IsEtp11);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log").IsEtp11);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log?query").IsEtp11);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log#hash").IsEtp11);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})").IsEtp11);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})?query").IsEtp11);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})#hash").IsEtp11);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log").IsEtp11);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log?query").IsEtp11);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log#hash").IsEtp11);

            // Alternate Prefix URIs
            Assert.IsTrue(new EtpUri("eml://?query").IsEtp11);
            Assert.IsTrue(new EtpUri("eml://data-space/?query").IsEtp11);
            Assert.IsTrue(new EtpUri("eml://data-space/witsml14?query").IsEtp11);

            // Alternate URIs
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})?query").IsEtp11);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well#hash").IsEtp11);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})#hash").IsEtp11);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore#hash").IsEtp11);
        }

        [TestMethod]
        public void EtpUri_11_Root_Uris()
        {
            var uuid = Uuid();

            // Root URIs
            Assert.IsTrue(new EtpUri("eml://").IsRoot);

            // Dataspace URIs
            Assert.IsFalse(new EtpUri("eml://data-space/").IsRoot);

            // Prefix URIs
            Assert.IsFalse(new EtpUri("eml://data-space/witsml14").IsRoot);
            Assert.IsFalse(new EtpUri("eml://data-space/second-level/witsml14").IsRoot);
            Assert.IsFalse(new EtpUri("eml://witsml14").IsRoot);

            // Canonical URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})").IsRoot);

            // Canonical Query URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well").IsRoot);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well?query").IsRoot);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore").IsRoot);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore?query").IsRoot);

            // Hierarcical URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})").IsRoot);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})?query").IsRoot);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})#hash").IsRoot);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})").IsRoot);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})?query").IsRoot);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})#hash").IsRoot);

            // Hierarchical Query URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log").IsRoot);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log?query").IsRoot);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log#hash").IsRoot);

            // Template URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore").IsRoot);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore?query").IsRoot);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore#hash").IsRoot);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log").IsRoot);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log?query").IsRoot);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log#hash").IsRoot);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})").IsRoot);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})?query").IsRoot);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})#hash").IsRoot);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log").IsRoot);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log?query").IsRoot);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log#hash").IsRoot);

            // Alternate Prefix URIs
            Assert.IsFalse(new EtpUri("eml://?query").IsRoot);
            Assert.IsFalse(new EtpUri("eml://data-space/?query").IsRoot);
            Assert.IsFalse(new EtpUri("eml://data-space/witsml14?query").IsRoot);

            // Alternate URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})?query").IsRoot);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well#hash").IsRoot);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})#hash").IsRoot);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore#hash").IsRoot);
        }

        [TestMethod]
        public void EtpUri_11_Prefix_Uris()
        {
            var uuid = Uuid();

            // Root URIs
            Assert.IsTrue(new EtpUri("eml://").IsPrefix);

            // Dataspace URIs
            Assert.IsTrue(new EtpUri("eml://data-space/").IsPrefix);

            // Prefix URIs
            Assert.IsTrue(new EtpUri("eml://data-space/witsml14").IsPrefix);
            Assert.IsTrue(new EtpUri("eml://data-space/second-level/witsml14").IsPrefix);
            Assert.IsTrue(new EtpUri("eml://witsml14").IsPrefix);

            // Canonical URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})").IsPrefix);

            // Canonical Query URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well?query").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore?query").IsPrefix);

            // Hierarcical URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})?query").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})#hash").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})?query").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})#hash").IsPrefix);

            // Hierarchical Query URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log?query").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log#hash").IsPrefix);

            // Template URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore?query").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore#hash").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log?query").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log#hash").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})?query").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})#hash").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log?query").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log#hash").IsPrefix);

            // Alternate Prefix URIs
            Assert.IsFalse(new EtpUri("eml://?query").IsPrefix);
            Assert.IsFalse(new EtpUri("eml://data-space/?query").IsPrefix);
            Assert.IsFalse(new EtpUri("eml://data-space/witsml14?query").IsPrefix);

            // Alternate URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})?query").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well#hash").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})#hash").IsPrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore#hash").IsPrefix);
        }

        [TestMethod]
        public void EtpUri_11_Canonical_Uris()
        {
            var uuid = Uuid();

            // Root URIs
            Assert.IsTrue(new EtpUri("eml://").IsCanonical);

            // Dataspace URIs
            Assert.IsTrue(new EtpUri("eml://data-space/").IsCanonical);

            // Prefix URIs
            Assert.IsTrue(new EtpUri("eml://data-space/witsml14").IsCanonical);
            Assert.IsTrue(new EtpUri("eml://data-space/second-level/witsml14").IsCanonical);
            Assert.IsTrue(new EtpUri("eml://witsml14").IsCanonical);

            // Canonical URIs
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})").IsCanonical);

            // Canonical Query URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well?query").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore?query").IsCanonical);

            // Hierarcical URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})?query").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})#hash").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})?query").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})#hash").IsCanonical);

            // Hierarchical Query URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log?query").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log#hash").IsCanonical);

            // Template URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore?query").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore#hash").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log?query").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log#hash").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})?query").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})#hash").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log?query").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log#hash").IsCanonical);

            // Alternate Prefix URIs
            Assert.IsFalse(new EtpUri("eml://?query").IsCanonical);
            Assert.IsFalse(new EtpUri("eml://data-space/?query").IsCanonical);
            Assert.IsFalse(new EtpUri("eml://data-space/witsml14?query").IsCanonical);

            // Alternate URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})?query").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well#hash").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})#hash").IsCanonical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore#hash").IsCanonical);
        }

        [TestMethod]
        public void EtpUri_11_Query_Uris()
        {
            var uuid = Uuid();

            // Root URIs
            Assert.IsFalse(new EtpUri("eml://").IsCanonicalQuery);

            // Dataspace URIs
            Assert.IsFalse(new EtpUri("eml://data-space/").IsCanonicalQuery);

            // Prefix URIs
            Assert.IsFalse(new EtpUri("eml://data-space/witsml14").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri("eml://data-space/second-level/witsml14").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri("eml://witsml14").IsCanonicalQuery);

            // Canonical URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})").IsCanonicalQuery);

            // Canonical Query URIs
            Assert.IsTrue(new EtpUri($"eml://witsml14/well").IsCanonicalQuery);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well?query").IsCanonicalQuery);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore").IsCanonicalQuery);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore?query").IsCanonicalQuery);

            // Hierarcical URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})?query").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})#hash").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})?query").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})#hash").IsCanonicalQuery);

            // Hierarchical Query URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log?query").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log#hash").IsCanonicalQuery);

            // Template URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore?query").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore#hash").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log?query").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log#hash").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})?query").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})#hash").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log?query").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log#hash").IsCanonicalQuery);

            // Alternate Prefix URIs
            Assert.IsFalse(new EtpUri("eml://?query").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri("eml://data-space/?query").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri("eml://data-space/witsml14?query").IsCanonicalQuery);

            // Alternate URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})?query").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well#hash").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})#hash").IsCanonicalQuery);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore#hash").IsCanonicalQuery);
        }

        [TestMethod]
        public void EtpUri_11_Alternate_Uris()
        {
            var uuid = Uuid();

            // Root URIs
            Assert.IsFalse(new EtpUri("eml://").IsAlternate);

            // Dataspace URIs
            Assert.IsFalse(new EtpUri("eml://data-space/").IsAlternate);

            // Prefix URIs
            Assert.IsFalse(new EtpUri("eml://data-space/witsml14").IsAlternate);
            Assert.IsFalse(new EtpUri("eml://data-space/second-level/witsml14").IsAlternate);
            Assert.IsFalse(new EtpUri("eml://witsml14").IsAlternate);

            // Canonical URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})").IsAlternate);

            // Canonical Query URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well").IsAlternate);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well?query").IsAlternate);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore").IsAlternate);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore?query").IsAlternate);

            // Hierarcical URIs
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})?query").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})#hash").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})?query").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})#hash").IsAlternate);

            // Hierarchical Query URIs
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log?query").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log#hash").IsAlternate);

            // Template URIs
            Assert.IsTrue(new EtpUri($"eml://witsml14/well/wellbore").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well/wellbore?query").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well/wellbore#hash").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log?query").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log#hash").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})?query").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})#hash").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log?query").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log#hash").IsAlternate);

            // Alternate Prefix URIs
            Assert.IsTrue(new EtpUri("eml://?query").IsAlternate);
            Assert.IsTrue(new EtpUri("eml://data-space/?query").IsAlternate);
            Assert.IsTrue(new EtpUri("eml://data-space/witsml14?query").IsAlternate);

            // Alternate URIs
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})?query").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well#hash").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})#hash").IsAlternate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore#hash").IsAlternate);
        }

        [TestMethod]
        public void EtpUri_11_Alternate_Prefix_Uris()
        {
            var uuid = Uuid();

            // Root URIs
            Assert.IsFalse(new EtpUri("eml://").IsAlternatePrefix);

            // Dataspace URIs
            Assert.IsFalse(new EtpUri("eml://data-space/").IsAlternatePrefix);

            // Prefix URIs
            Assert.IsFalse(new EtpUri("eml://data-space/witsml14").IsAlternatePrefix);
            Assert.IsFalse(new EtpUri("eml://data-space/second-level/witsml14").IsAlternatePrefix);
            Assert.IsFalse(new EtpUri("eml://witsml14").IsAlternatePrefix);

            // Canonical URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})").IsAlternatePrefix);

            // Canonical Query URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well").IsAlternatePrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well?query").IsAlternatePrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore").IsAlternatePrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore?query").IsAlternatePrefix);

            // Hierarcical URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})").IsAlternatePrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})?query").IsAlternatePrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})#hash").IsAlternatePrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})").IsAlternatePrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})?query").IsAlternatePrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})#hash").IsAlternatePrefix);

            // Hierarchical Query URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log").IsAlternatePrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log?query").IsAlternatePrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log#hash").IsAlternatePrefix);

            // Template URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore").IsAlternatePrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore?query").IsAlternatePrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore#hash").IsAlternatePrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log").IsAlternatePrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log?query").IsAlternatePrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log#hash").IsAlternatePrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})").IsAlternatePrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})?query").IsAlternatePrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})#hash").IsAlternatePrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log").IsAlternatePrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log?query").IsAlternatePrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log#hash").IsAlternatePrefix);

            // Alternate Prefix URIs
            Assert.IsTrue(new EtpUri("eml://?query").IsAlternatePrefix);
            Assert.IsTrue(new EtpUri("eml://data-space/?query").IsAlternatePrefix);
            Assert.IsTrue(new EtpUri("eml://data-space/witsml14?query").IsAlternatePrefix);

            // Alternate URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})?query").IsAlternatePrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well#hash").IsAlternatePrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})#hash").IsAlternatePrefix);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore#hash").IsAlternatePrefix);
        }

        [TestMethod]
        public void EtpUri_11_Hierarchical_Uris()
        {
            var uuid = Uuid();

            // Root URIs
            Assert.IsFalse(new EtpUri("eml://").IsHierarchical);

            // Dataspace URIs
            Assert.IsFalse(new EtpUri("eml://data-space/").IsHierarchical);

            // Prefix URIs
            Assert.IsFalse(new EtpUri("eml://data-space/witsml14").IsHierarchical);
            Assert.IsFalse(new EtpUri("eml://data-space/second-level/witsml14").IsHierarchical);
            Assert.IsFalse(new EtpUri("eml://witsml14").IsHierarchical);

            // Canonical URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})").IsHierarchical);

            // Canonical Query URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well?query").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore?query").IsHierarchical);

            // Hierarcical URIs
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})").IsHierarchical);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})?query").IsHierarchical);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})#hash").IsHierarchical);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})").IsHierarchical);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})?query").IsHierarchical);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})#hash").IsHierarchical);

            // Hierarchical Query URIs
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log").IsHierarchical);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log?query").IsHierarchical);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log#hash").IsHierarchical);

            // Template URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore?query").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore#hash").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log?query").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log#hash").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})?query").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})#hash").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log?query").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log#hash").IsHierarchical);

            // Alternate Prefix URIs
            Assert.IsFalse(new EtpUri("eml://?query").IsHierarchical);
            Assert.IsFalse(new EtpUri("eml://data-space/?query").IsHierarchical);
            Assert.IsFalse(new EtpUri("eml://data-space/witsml14?query").IsHierarchical);

            // Alternate URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})?query").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well#hash").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})#hash").IsHierarchical);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore#hash").IsHierarchical);
        }

        [TestMethod]
        public void EtpUri_11_Template_Uris()
        {
            var uuid = Uuid();

            // Root URIs
            Assert.IsFalse(new EtpUri("eml://").IsTemplate);

            // Dataspace URIs
            Assert.IsFalse(new EtpUri("eml://data-space/").IsTemplate);

            // Prefix URIs
            Assert.IsFalse(new EtpUri("eml://data-space/witsml14").IsTemplate);
            Assert.IsFalse(new EtpUri("eml://data-space/second-level/witsml14").IsTemplate);
            Assert.IsFalse(new EtpUri("eml://witsml14").IsTemplate);

            // Canonical URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})").IsTemplate);

            // Canonical Query URIs
            Assert.IsTrue(new EtpUri($"eml://witsml14/well").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well?query").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore?query").IsTemplate);

            // Hierarcical URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})").IsTemplate);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})?query").IsTemplate);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})#hash").IsTemplate);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})").IsTemplate);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})?query").IsTemplate);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log({Uuid()})#hash").IsTemplate);

            // Hierarchical Query URIs
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log?query").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore({Uuid()})/log#hash").IsTemplate);

            // Template URIs
            Assert.IsTrue(new EtpUri($"eml://witsml14/well/wellbore").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well/wellbore?query").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well/wellbore#hash").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log?query").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log#hash").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})?query").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well({uuid})/wellbore/log({Uuid()})#hash").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log?query").IsTemplate);
            Assert.IsTrue(new EtpUri($"eml://witsml14/well/wellbore({uuid})/log#hash").IsTemplate);

            // Alternate Prefix URIs
            Assert.IsFalse(new EtpUri("eml://?query").IsTemplate);
            Assert.IsFalse(new EtpUri("eml://data-space/?query").IsTemplate);
            Assert.IsFalse(new EtpUri("eml://data-space/witsml14?query").IsTemplate);

            // Alternate URIs
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})?query").IsTemplate);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well#hash").IsTemplate);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})#hash").IsTemplate);
            Assert.IsFalse(new EtpUri($"eml://witsml14/well({uuid})/wellbore#hash").IsTemplate);
        }

        private string Uuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
