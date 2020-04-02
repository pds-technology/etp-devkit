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
    public class EtpUri11Tests : EtpUriTestBase
    {
        [TestMethod]
        public void EtpUri_11_IsRoot_Can_Detect_Root_Uri()
        {
            var uri = "eml://";
            var etpUri = new EtpUri(uri);

            Assert.IsTrue(etpUri.IsEtp11);
            Assert.IsTrue(etpUri.IsRootUri);
            Assert.IsTrue(etpUri.IsValid);
            Assert.AreEqual(EtpUri.RootUri11, etpUri);

            uri = "EML://";
            etpUri = new EtpUri(uri);

            Assert.IsTrue(etpUri.IsEtp11);
            Assert.IsTrue(etpUri.IsRootUri);
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
            Assert.IsTrue(uri.IsBaseUri);
            Assert.AreEqual(uri, uri.Parent);
            Assert.AreEqual("1.4.1.1", uri.Version);
            Assert.AreEqual("custom-database", uri.Dataspace);
        }

        [TestMethod]
        public void EtpUri_11_Can_Parse_Uri_With_Hierarchical_DataSpace()
        {
            var uri = new EtpUri("eml://custom-database/second-level/witsml14");

            Assert.IsTrue(uri.IsValid);
            Assert.IsTrue(uri.IsBaseUri);
            Assert.AreEqual(uri, uri.Parent);
            Assert.AreEqual("1.4.1.1", uri.Version);
            Assert.AreEqual("custom-database/second-level", uri.Dataspace);
        }

        [TestMethod]
        public void EtpUri_11_Can_Parse_Witsml_20_Base_Uri()
        {
            var uri = new EtpUri("eml://witsml20");

            Assert.IsTrue(uri.IsValid);
            Assert.IsTrue(uri.IsBaseUri);
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

            Assert.IsTrue(uri.IsCanonicalUri);

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
            Assert.IsTrue(uri.IsBaseUri);
            Assert.AreEqual("1.4.1.1", uri.Version);
        }

        [TestMethod]
        public void EtpUri_11_Append_Can_Append_Object_Type_To_Base_Uri()
        {
            var uri14 = new EtpUri("eml://witsml14");
            var uriWell = uri14.Append(uri14.Family, uri14.Version, "well", null);

            Assert.IsTrue(uriWell.IsValid);
            Assert.IsFalse(uriWell.IsBaseUri);
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
            Assert.IsFalse(uri.IsBaseUri);
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
        public void EtpUri_11_Uri_Base_Returns_Correct_Base()
        {
            var uuid = Uuid();

            var prefix = "eml://";
            var uri = new EtpUri("eml://");
            Assert.AreEqual(prefix, uri.UriBase);

            prefix = "eml://data-space";
            uri = new EtpUri("eml://data-space");
            Assert.AreEqual(prefix, uri.UriBase);

            prefix = "eml://data-space/level-two";
            uri = new EtpUri("eml://data-space/level-two");
            Assert.AreEqual(prefix, uri.UriBase);

            prefix = "eml://data-space/level-two/witsml14";
            uri = new EtpUri("eml://data-space/level-two/witsml14");
            Assert.AreEqual(prefix, uri.UriBase);

            prefix = "eml://witsml14";
            uri = new EtpUri("eml://witsml14");
            Assert.AreEqual(prefix, uri.UriBase);

            prefix = $"eml://data-space/level-two/witsml14/";
            uri = new EtpUri($"eml://data-space/level-two/witsml14/well({uuid})");
            Assert.AreEqual(prefix, uri.UriBase);

            prefix = $"eml://witsml14/";
            uri = new EtpUri($"eml://witsml14/well({uuid})");
            Assert.AreEqual(prefix, uri.UriBase);
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

            // Special Cases

            original = new EtpUri($"eml://some-data/space/witsml20/Well({uuid})/DataAssuranceRecord({uuid2})");
            canonical = new EtpUri($"eml://some-data/space/eml21/DataAssuranceRecord({uuid2})");
            Assert.AreEqual(canonical, original.AsCanonical());

            original = new EtpUri($"eml://some-data/space/witsml14/well({uuid})/wellbore({uuid2})");
            canonical = new EtpUri($"eml://some-data/space/witsml14/well({uuid})/wellbore({uuid2})");
            Assert.AreEqual(canonical, original.AsCanonical());
        }

        [TestMethod]
        public void EtpUri_11_Uris()
        {
            var uuid = Uuid();

            ///////////////////////////
            // Invalid URI
            ///////////////////////////
            
            var uri = new EtpUri("http://www.energistics.org");
            AssertUri(uri, IsValid: false, EtpVersion: EtpVersion.v11);

            ///////////////////////////
            // Root URIs
            ///////////////////////////

            uri = new EtpUri("eml:");
            AssertUri(uri, IsValid: false, EtpVersion: EtpVersion.v11);

            uri = new EtpUri("eml://");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsRootUri: true, IsDataspaceUri: true, IsBaseUri: true, IsCanonicalUri: true);

            uri = new EtpUri("eml://?query");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsRootUri: true, IsDataspaceUri: true, IsBaseUri: true, IsAlternateUri: true, HasQuery: true);

            uri = new EtpUri("eml://#hash");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsRootUri: true, IsDataspaceUri: true, IsBaseUri: true, IsAlternateUri: true, HasHash: true);

            ///////////////////////////
            // Dataspace URIs
            ///////////////////////////

            uri = new EtpUri("eml://data-space/");
            AssertUri(uri, IsValid: false, EtpVersion: EtpVersion.v11);

            uri = new EtpUri("eml://data-space/second-level/");
            AssertUri(uri, IsValid: false, EtpVersion: EtpVersion.v11);

            uri = new EtpUri("eml://data-space");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsDataspaceUri: true, IsBaseUri: true, IsCanonicalUri: true);

            uri = new EtpUri("eml://data-space?query");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsDataspaceUri: true, IsBaseUri: true, IsAlternateUri: true, HasQuery: true);

            uri = new EtpUri("eml://data-space#hash");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsDataspaceUri: true, IsBaseUri: true, IsAlternateUri: true, HasHash: true);

            uri = new EtpUri("eml://data-space/second-level");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsDataspaceUri: true, IsBaseUri: true, IsCanonicalUri: true);

            uri = new EtpUri("eml://data-space/second-level?query");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsDataspaceUri: true, IsBaseUri: true, IsAlternateUri: true, HasQuery: true);

            uri = new EtpUri("eml://data-space/second-level#hash");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsDataspaceUri: true, IsBaseUri: true, IsAlternateUri: true, HasHash: true);

            ///////////////////////////
            // Family Version URIs
            ///////////////////////////

            uri = new EtpUri("eml://witsml20/");
            AssertUri(uri, IsValid: false, EtpVersion: EtpVersion.v11);

            uri = new EtpUri("eml://data-space/witsml20/");
            AssertUri(uri, IsValid: false, EtpVersion: EtpVersion.v11);

            uri = new EtpUri("eml://data-space/second-level/witsml20/");
            AssertUri(uri, IsValid: false, EtpVersion: EtpVersion.v11);

            uri = new EtpUri("eml://witsml20");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsFamilyVersionUri: true, IsBaseUri: true, IsCanonicalUri: true);

            uri = new EtpUri("eml://witsml20?query");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsFamilyVersionUri: true, IsBaseUri: true, IsAlternateUri: true, HasQuery: true);

            uri = new EtpUri("eml://witsml20#hash");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsFamilyVersionUri: true, IsBaseUri: true, IsAlternateUri: true, HasHash: true);

            uri = new EtpUri("eml://data-space/witsml20");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsFamilyVersionUri: true, IsBaseUri: true, IsCanonicalUri: true);

            uri = new EtpUri("eml://data-space/witsml20?query");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsFamilyVersionUri: true, IsBaseUri: true, IsAlternateUri: true, HasQuery: true);

            uri = new EtpUri("eml://data-space/witsml20#hash");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsFamilyVersionUri: true, IsBaseUri: true, IsAlternateUri: true, HasHash: true);

            ///////////////////////////
            // Object URIs
            ///////////////////////////

            uri = new EtpUri($"eml://witsml20/Well({uuid})/");
            AssertUri(uri, IsValid: false, EtpVersion: EtpVersion.v11);

            uri = new EtpUri($"eml://witsml20/Well({uuid})");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsCanonicalUri: true, IsObjectUri: true);

            uri = new EtpUri($"eml://witsml20/Well({uuid})?query");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsAlternateUri: true, IsObjectUri: true, HasQuery: true);

            uri = new EtpUri($"eml://witsml20/Well({uuid})#hash");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsAlternateUri: true, IsObjectUri: true, HasHash: true);

            ///////////////////////////
            // Object Hierarchy URIs
            ///////////////////////////

            uri = new EtpUri($"eml://witsml20/Well({uuid})/Wellbore({Uuid()})/");
            AssertUri(uri, IsValid: false, EtpVersion: EtpVersion.v11);

            uri = new EtpUri($"eml://witsml20/Well({uuid})/Wellbore({Uuid()})/Log({Uuid()})/");
            AssertUri(uri, IsValid: false, EtpVersion: EtpVersion.v11);

            uri = new EtpUri($"eml://witsml20/Well({uuid})/Wellbore({Uuid()})");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsAlternateUri: true, IsObjectUri: true, IsHierarchicalUri: true);

            uri = new EtpUri($"eml://witsml20/Well({uuid})/Wellbore({Uuid()})?query");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsAlternateUri: true, IsObjectUri: true, IsHierarchicalUri: true, HasQuery: true);

            uri = new EtpUri($"eml://witsml20/Well({uuid})/Wellbore({Uuid()})#hash");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsAlternateUri: true, IsObjectUri: true, IsHierarchicalUri: true, HasHash: true);

            uri = new EtpUri($"eml://witsml20/Well({uuid})/Wellbore({Uuid()})/Log({Uuid()})");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsAlternateUri: true, IsObjectUri: true, IsHierarchicalUri: true);

            uri = new EtpUri($"eml://witsml20/Well({uuid})/Wellbore({Uuid()})/Log({Uuid()})?query");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsAlternateUri: true, IsObjectUri: true, IsHierarchicalUri: true, HasQuery: true);

            uri = new EtpUri($"eml://witsml20/Well({uuid})/Wellbore({Uuid()})/Log({Uuid()})#hash");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsAlternateUri: true, IsObjectUri: true, IsHierarchicalUri: true, HasHash: true);

            ///////////////////////////
            // Folder and Query URIs
            ///////////////////////////

            uri = new EtpUri($"eml://witsml20/Well/");
            AssertUri(uri, IsValid: false, EtpVersion: EtpVersion.v11);

            uri = new EtpUri($"eml://witsml20/Well({uuid})/Wellbore/");
            AssertUri(uri, IsValid: false, EtpVersion: EtpVersion.v11);

            uri = new EtpUri($"eml://witsml20/Well({uuid})/Wellbore({Uuid()})/Log/");
            AssertUri(uri, IsValid: false, EtpVersion: EtpVersion.v11);

            uri = new EtpUri($"eml://witsml20/Well");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsQueryUri: true, IsFolderUri: true, IsCanonicalUri: true, IsTemplateUri: true);

            uri = new EtpUri($"eml://witsml20/Well?query");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsQueryUri: true, IsFolderUri: true, IsCanonicalUri: true, IsTemplateUri: true, HasQuery: true);

            uri = new EtpUri($"eml://witsml20/Well#hash");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsQueryUri: true, IsFolderUri: true, IsAlternateUri: true, IsTemplateUri: true, HasHash: true);

            uri = new EtpUri($"eml://witsml20/Well({uuid})/Wellbore");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsQueryUri: true, IsFolderUri: true, IsCanonicalUri: true, IsHierarchicalUri: true, IsTemplateUri: true);

            uri = new EtpUri($"eml://witsml20/Well({uuid})/Wellbore?query");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsQueryUri: true, IsFolderUri: true, IsCanonicalUri: true, IsHierarchicalUri: true, IsTemplateUri: true, HasQuery: true);

            uri = new EtpUri($"eml://witsml20/Well({uuid})/Wellbore#hash");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsQueryUri: true, IsFolderUri: true, IsAlternateUri: true, IsHierarchicalUri: true, IsTemplateUri: true, HasHash: true);

            uri = new EtpUri($"eml://witsml20/Well({uuid})/Wellbore({Uuid()})/Log");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsFolderUri: true, IsAlternateUri: true, IsHierarchicalUri: true, IsTemplateUri: true);

            uri = new EtpUri($"eml://witsml20/Well({uuid})/Wellbore({Uuid()})/Log?query");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsFolderUri: true, IsAlternateUri: true, IsHierarchicalUri: true, IsTemplateUri: true, HasQuery: true);

            uri = new EtpUri($"eml://witsml20/Well({uuid})/Wellbore({Uuid()})/Log#hash");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsFolderUri: true, IsAlternateUri: true, IsHierarchicalUri: true, IsTemplateUri: true, HasHash: true);

            ///////////////////////////
            // Template URIs
            ///////////////////////////
            
            uri = new EtpUri($"eml://witsml20/Well/Wellbore/");
            AssertUri(uri, IsValid: false, EtpVersion: EtpVersion.v11);

            uri = new EtpUri($"eml://witsml20/Well({uuid})/Wellbore/Log/");
            AssertUri(uri, IsValid: false, EtpVersion: EtpVersion.v11);

            uri = new EtpUri($"eml://witsml20/Well({uuid})/Wellbore/Log({Uuid()})/");
            AssertUri(uri, IsValid: false, EtpVersion: EtpVersion.v11);

            uri = new EtpUri($"eml://witsml20/Well/Wellbore({uuid})/Log/");
            AssertUri(uri, IsValid: false, EtpVersion: EtpVersion.v11);

            uri = new EtpUri($"eml://witsml20/Well/Wellbore");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsFolderUri: true, IsAlternateUri: true, IsTemplateUri: true);

            uri = new EtpUri($"eml://witsml20/Well/Wellbore?query");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsFolderUri: true, IsAlternateUri: true, IsTemplateUri: true, HasQuery: true);

            uri = new EtpUri($"eml://witsml20/Well/Wellbore#hash");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsFolderUri: true, IsAlternateUri: true, IsTemplateUri: true, HasHash: true);

            uri = new EtpUri($"eml://witsml20/Well({uuid})/Wellbore/Log");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsFolderUri: true, IsAlternateUri: true, IsTemplateUri: true);

            uri = new EtpUri($"eml://witsml20/Well({uuid})/Wellbore/Log?query");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsFolderUri: true, IsAlternateUri: true, IsTemplateUri: true, HasQuery: true);

            uri = new EtpUri($"eml://witsml20/Well({uuid})/Wellbore/Log#hash");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsFolderUri: true, IsAlternateUri: true, IsTemplateUri: true, HasHash: true);

            uri = new EtpUri($"eml://witsml20/Well({uuid})/Wellbore/Log({Uuid()})");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsObjectUri: true, IsAlternateUri: true, IsTemplateUri: true);

            uri = new EtpUri($"eml://witsml20/Well({uuid})/Wellbore/Log({Uuid()})?query");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsObjectUri: true, IsAlternateUri: true, IsTemplateUri: true, HasQuery: true);

            uri = new EtpUri($"eml://witsml20/Well({uuid})/Wellbore/Log({Uuid()})#hash");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsObjectUri: true, IsAlternateUri: true, IsTemplateUri: true, HasHash: true);

            uri = new EtpUri($"eml://witsml20/Well/Wellbore({uuid})/Log");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsFolderUri: true, IsAlternateUri: true, IsTemplateUri: true);

            uri = new EtpUri($"eml://witsml20/Well/Wellbore({uuid})/Log?query");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsFolderUri: true, IsAlternateUri: true, IsTemplateUri: true, HasQuery: true);

            uri = new EtpUri($"eml://witsml20/Well/Wellbore({uuid})/Log#hash");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsFolderUri: true, IsAlternateUri: true, IsTemplateUri: true, HasHash: true);

            ///////////////////////////
            // Special Cases
            ///////////////////////////
            
            uri = new EtpUri($"eml://some-data/space/witsml14/well({uuid})/wellbore({Uuid()})");
            AssertUri(uri, IsValid: true, EtpVersion: EtpVersion.v11, IsCanonicalUri: true, IsObjectUri: true, IsHierarchicalUri: true);
        }
    }
}
