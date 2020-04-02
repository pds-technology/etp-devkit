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
    public class EtpUriTestBase
    {
        public void AssertUri(EtpUri uri, bool IsValid, EtpVersion EtpVersion, bool IsRootUri = false, bool IsDataspaceUri = false, bool IsFamilyVersionUri = false, bool IsObjectUri = false, bool IsQueryUri = false, bool IsFolderUri = false, bool IsBaseUri = false, bool IsCanonicalUri = false, bool IsAlternateUri = false, bool IsHierarchicalUri = false, bool IsTemplateUri = false, bool HasQuery = false, bool HasHash = false)
        {
            Assert.AreEqual(IsValid, uri.IsValid, $"IsValid: {uri}");
            if (!uri.IsValid)
                return;

            Assert.AreEqual(EtpVersion, uri.EtpVersion, $"EtpVersion: {uri}");
            if (uri.EtpVersion == EtpVersion.v11)
                Assert.IsTrue(uri.IsEtp11, $"Etp11: {uri}");
            else
                Assert.IsTrue(uri.IsEtp12, $"Etp12: {uri}");

            Assert.AreEqual(IsRootUri, uri.IsRootUri, $"IsRootUri: {uri}");
            Assert.AreEqual(IsDataspaceUri, uri.IsDataspaceUri, $"IsDataspaceUri: {uri}");
            Assert.AreEqual(IsFamilyVersionUri, uri.IsFamilyVersionUri, $"IsFamilyVersionUri: {uri}");
            Assert.AreEqual(IsObjectUri, uri.IsObjectUri, $"IsObjectUri: {uri}");
            Assert.AreEqual(IsQueryUri, uri.IsQueryUri, $"IsQueryUri: {uri}");
            Assert.AreEqual(IsFolderUri, uri.IsFolderUri, $"IsFolderUri: {uri}");
            Assert.AreEqual(IsBaseUri, uri.IsBaseUri, $"IsBaseUri: {uri}");
            Assert.AreEqual(IsCanonicalUri, uri.IsCanonicalUri, $"IsCanonicalUri: {uri}");
            Assert.AreEqual(IsAlternateUri, uri.IsAlternateUri, $"IsAlternateUri: {uri}");
            Assert.AreEqual(IsHierarchicalUri, uri.IsHierarchicalUri, $"IsHierarchicalUri: {uri}");
            Assert.AreEqual(IsTemplateUri, uri.IsTemplateUri, $"IsTemplateUri: {uri}");
            Assert.AreEqual(HasQuery, uri.HasQuery, $"HasQuery: {uri}");
            Assert.AreEqual(HasHash, uri.HasHash, $"HasHash: {uri}");
        }

        protected string Uuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
