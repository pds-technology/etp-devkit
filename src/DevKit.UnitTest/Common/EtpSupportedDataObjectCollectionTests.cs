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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Energistics.Etp.Common
{
    [TestClass]
    public class EtpSupportedDataObjectCollectionTests
    {
        [TestMethod]
        public void EtpSupportedDataObjectCollection_Intersection_Nothing_In_Common()
        {
            var instanceDataObjects = new List<EtpSupportedDataObject>
            {
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "*"), new EtpDataObjectCapabilities { ActiveTimeoutPeriod = int.MaxValue }),
            };
            var counterpartDataObjects = new List<EtpSupportedDataObject>
            {
                new EtpSupportedDataObject(new EtpDataObjectType("resqml", "2.0.1", "*"), new EtpDataObjectCapabilities { MaxDataObjectSize = 999999999 }),
            };

            var collection = EtpSupportedDataObjectCollection.GetSupportedDataObjectCollection(instanceDataObjects, counterpartDataObjects, true);

            Assert.AreEqual(0, collection.SupportedTypes.Count());
            Assert.AreEqual(0, collection.SupportedFamilies.Count());
        }

        [TestMethod]
        public void EtpSupportedDataObjectCollection_Intersection_Family_In_Common()
        {
            var instanceDataObjects = new List<EtpSupportedDataObject>
            {
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "*"), new EtpDataObjectCapabilities { ActiveTimeoutPeriod = int.MaxValue }),
                new EtpSupportedDataObject(new EtpDataObjectType("resqml", "2.0.1", "*"), new EtpDataObjectCapabilities { MaxDataObjectSize = 999999999 }),
            };
            var counterpartDataObjects = new List<EtpSupportedDataObject>
            {
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "*"), new EtpDataObjectCapabilities { SupportsPut = false }),
                new EtpSupportedDataObject(new EtpDataObjectType("prodml", "2.0", "*"), new EtpDataObjectCapabilities { SupportsDelete = true }),
            };

            var collection = EtpSupportedDataObjectCollection.GetSupportedDataObjectCollection(instanceDataObjects, counterpartDataObjects, true);

            Assert.AreEqual(0, collection.SupportedTypes.Count());
            Assert.AreEqual(1, collection.SupportedFamilies.Count());
        }

        [TestMethod]
        public void EtpSupportedDataObjectCollection_Intersection_DataObject_In_Common()
        {
            var instanceDataObjects = new List<EtpSupportedDataObject>
            {
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "Well"), new EtpDataObjectCapabilities { ActiveTimeoutPeriod = int.MaxValue }),
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "Wellbore"), new EtpDataObjectCapabilities { MaxDataObjectSize = 999999999 }),
            };
            var counterpartDataObjects = new List<EtpSupportedDataObject>
            {
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "Well"), new EtpDataObjectCapabilities { SupportsPut = false }),
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "ChannelSet"), new EtpDataObjectCapabilities { SupportsDelete = true }),
            };

            var collection = EtpSupportedDataObjectCollection.GetSupportedDataObjectCollection(instanceDataObjects, counterpartDataObjects, true);

            Assert.AreEqual(1, collection.SupportedTypes.Count());
            Assert.AreEqual(int.MaxValue, collection.SupportedTypes.First().Capabilities.ActiveTimeoutPeriod);
            Assert.AreEqual(false, collection.SupportedTypes.First().CounterpartCapabilities.SupportsPut);
            Assert.AreEqual(0, collection.SupportedFamilies.Count());
        }

        [TestMethod]
        public void EtpSupportedDataObjectCollection_Intersection_Instance_WildCard_And_Counterpart_DataObjects()
        {
            var instanceDataObjects = new List<EtpSupportedDataObject>
            {
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "*"), new EtpDataObjectCapabilities { ActiveTimeoutPeriod = int.MaxValue }),
            };
            var counterpartDataObjects = new List<EtpSupportedDataObject>
            {
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "Well"), new EtpDataObjectCapabilities { SupportsPut = false }),
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "Wellbore"), new EtpDataObjectCapabilities { MaxDataObjectSize = 999999999 }),
                new EtpSupportedDataObject(new EtpDataObjectType("prodml", "2.0", "*"), new EtpDataObjectCapabilities { SupportsDelete = true }),
            };

            var collection = EtpSupportedDataObjectCollection.GetSupportedDataObjectCollection(instanceDataObjects, counterpartDataObjects, true);

            Assert.AreEqual(2, collection.SupportedTypes.Count());
            Assert.AreEqual("Well", collection.SupportedTypes.ToList()[0].QualifiedType.ObjectType);
            Assert.AreEqual("Wellbore", collection.SupportedTypes.ToList()[1].QualifiedType.ObjectType);
            Assert.AreEqual(int.MaxValue, collection.SupportedTypes.First().Capabilities.ActiveTimeoutPeriod);
            Assert.AreEqual(false, collection.SupportedTypes.First().CounterpartCapabilities.SupportsPut);
            Assert.AreEqual(0, collection.SupportedFamilies.Count());
        }

        [TestMethod]
        public void EtpSupportedDataObjectCollection_Intersection_Instance_DataObjects_And_Counterpart_WildCard()
        {
            var instanceDataObjects = new List<EtpSupportedDataObject>
            {
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "Well"), new EtpDataObjectCapabilities { ActiveTimeoutPeriod = int.MaxValue }),
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "Wellbore"), new EtpDataObjectCapabilities { MaxDataObjectSize = 999999999 }),
                new EtpSupportedDataObject(new EtpDataObjectType("prodml", "2.0", "*"), new EtpDataObjectCapabilities { SupportsDelete = true }),
            };
            var counterpartDataObjects = new List<EtpSupportedDataObject>
            {
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "*"), new EtpDataObjectCapabilities { SupportsPut = false }),
            };

            var collection = EtpSupportedDataObjectCollection.GetSupportedDataObjectCollection(instanceDataObjects, counterpartDataObjects, true);

            Assert.AreEqual(2, collection.SupportedTypes.Count());
            Assert.AreEqual("Well", collection.SupportedTypes.ToList()[0].QualifiedType.ObjectType);
            Assert.AreEqual("Wellbore", collection.SupportedTypes.ToList()[1].QualifiedType.ObjectType);
            Assert.AreEqual(int.MaxValue, collection.SupportedTypes.First().Capabilities.ActiveTimeoutPeriod);
            Assert.AreEqual(false, collection.SupportedTypes.First().CounterpartCapabilities.SupportsPut);
            Assert.AreEqual(0, collection.SupportedFamilies.Count());
        }

        [TestMethod]
        public void EtpSupportedDataObjectCollection_Intersection_Instance_Empty_And_Counterpart_DataObjects()
        {
            var instanceDataObjects = new List<EtpSupportedDataObject>
            {
            };
            var counterpartDataObjects = new List<EtpSupportedDataObject>
            {
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "Well"), new EtpDataObjectCapabilities { SupportsPut = false }),
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "Wellbore"), new EtpDataObjectCapabilities { MaxDataObjectSize = 999999999 }),
                new EtpSupportedDataObject(new EtpDataObjectType("prodml", "2.0", "*"), new EtpDataObjectCapabilities { SupportsDelete = true }),
            };

            var collection = EtpSupportedDataObjectCollection.GetSupportedDataObjectCollection(instanceDataObjects, counterpartDataObjects, true);

            Assert.AreEqual(0, collection.SupportedTypes.Count());
            Assert.AreEqual(0, collection.SupportedFamilies.Count());
        }

        [TestMethod]
        public void EtpSupportedDataObjectCollection_Intersection_Instance_DataObjects_And_Counterpart_Empty()
        {
            var instanceDataObjects = new List<EtpSupportedDataObject>
            {
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "Well"), new EtpDataObjectCapabilities { ActiveTimeoutPeriod = int.MaxValue }),
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "Wellbore"), new EtpDataObjectCapabilities { MaxDataObjectSize = 999999999 }),
                new EtpSupportedDataObject(new EtpDataObjectType("prodml", "2.0", "*"), new EtpDataObjectCapabilities { SupportsDelete = true }),
            };
            var counterpartDataObjects = new List<EtpSupportedDataObject>
            {
            };

            var collection = EtpSupportedDataObjectCollection.GetSupportedDataObjectCollection(instanceDataObjects, counterpartDataObjects, true);

            Assert.AreEqual(0, collection.SupportedTypes.Count());
            Assert.AreEqual(0, collection.SupportedFamilies.Count());
        }

        [TestMethod]
        public void EtpSupportedDataObjectCollection_Union_Nothing_In_Common()
        {
            var instanceDataObjects = new List<EtpSupportedDataObject>
            {
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "*")),
            };
            var counterpartDataObjects = new List<EtpSupportedDataObject>
            {
                new EtpSupportedDataObject(new EtpDataObjectType("resqml", "2.0.1", "*")),
            };

            var collection = EtpSupportedDataObjectCollection.GetSupportedDataObjectCollection(instanceDataObjects, counterpartDataObjects, false);

            Assert.AreEqual(0, collection.SupportedTypes.Count());
            Assert.AreEqual(2, collection.SupportedFamilies.Count());
        }

        [TestMethod]
        public void EtpSupportedDataObjectCollection_Union_Family_In_Common()
        {
            var instanceDataObjects = new List<EtpSupportedDataObject>
            {
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "*")),
                new EtpSupportedDataObject(new EtpDataObjectType("resqml", "2.0.1", "*")),
            };
            var counterpartDataObjects = new List<EtpSupportedDataObject>
            {
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "*")),
                new EtpSupportedDataObject(new EtpDataObjectType("prodml", "2.0", "*")),
            };

            var collection = EtpSupportedDataObjectCollection.GetSupportedDataObjectCollection(instanceDataObjects, counterpartDataObjects, false);

            Assert.AreEqual(0, collection.SupportedTypes.Count());
            Assert.AreEqual(3, collection.SupportedFamilies.Count());
        }

        [TestMethod]
        public void EtpSupportedDataObjectCollection_Union_DataObject_In_Common()
        {
            var instanceDataObjects = new List<EtpSupportedDataObject>
            {
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "Well")),
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "Wellbore")),
            };
            var counterpartDataObjects = new List<EtpSupportedDataObject>
            {
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "Well")),
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "ChannelSet")),
            };

            var collection = EtpSupportedDataObjectCollection.GetSupportedDataObjectCollection(instanceDataObjects, counterpartDataObjects, false);

            Assert.AreEqual(3, collection.SupportedTypes.Count());
            Assert.AreEqual(0, collection.SupportedFamilies.Count());
        }

        [TestMethod]
        public void EtpSupportedDataObjectCollection_Union_Instance_WildCard_And_Counterpart_DataObjects()
        {
            var instanceDataObjects = new List<EtpSupportedDataObject>
            {
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "*")),
            };
            var counterpartDataObjects = new List<EtpSupportedDataObject>
            {
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "Well")),
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "Wellbore")),
                new EtpSupportedDataObject(new EtpDataObjectType("prodml", "2.0", "*")),
            };

            var collection = EtpSupportedDataObjectCollection.GetSupportedDataObjectCollection(instanceDataObjects, counterpartDataObjects, false);

            Assert.AreEqual(2, collection.SupportedTypes.Count());
            Assert.AreEqual("Well", collection.SupportedTypes.ToList()[0].QualifiedType.ObjectType);
            Assert.AreEqual("Wellbore", collection.SupportedTypes.ToList()[1].QualifiedType.ObjectType);
            Assert.AreEqual(2, collection.SupportedFamilies.Count());
        }

        [TestMethod]
        public void EtpSupportedDataObjectCollection_Union_Instance_DataObjects_And_Counterpart_WildCard()
        {
            var instanceDataObjects = new List<EtpSupportedDataObject>
            {
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "Well")),
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "Wellbore")),
                new EtpSupportedDataObject(new EtpDataObjectType("prodml", "2.0", "*")),
            };
            var counterpartDataObjects = new List<EtpSupportedDataObject>
            {
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "*")),
            };

            var collection = EtpSupportedDataObjectCollection.GetSupportedDataObjectCollection(instanceDataObjects, counterpartDataObjects, false);

            Assert.AreEqual(2, collection.SupportedTypes.Count());
            Assert.AreEqual("Well", collection.SupportedTypes.ToList()[0].QualifiedType.ObjectType);
            Assert.AreEqual("Wellbore", collection.SupportedTypes.ToList()[1].QualifiedType.ObjectType);
            Assert.AreEqual(2, collection.SupportedFamilies.Count());
        }

        [TestMethod]
        public void EtpSupportedDataObjectCollection_Union_Instance_Empty_And_Counterpart_DataObjects()
        {
            var instanceDataObjects = new List<EtpSupportedDataObject>
            {
            };
            var counterpartDataObjects = new List<EtpSupportedDataObject>
            {
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "Well")),
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "Wellbore")),
                new EtpSupportedDataObject(new EtpDataObjectType("prodml", "2.0", "*")),
            };

            var collection = EtpSupportedDataObjectCollection.GetSupportedDataObjectCollection(instanceDataObjects, counterpartDataObjects, false);

            Assert.AreEqual(2, collection.SupportedTypes.Count());
            Assert.AreEqual("Well", collection.SupportedTypes.ToList()[0].QualifiedType.ObjectType);
            Assert.AreEqual("Wellbore", collection.SupportedTypes.ToList()[1].QualifiedType.ObjectType);
            Assert.AreEqual(1, collection.SupportedFamilies.Count());
        }

        [TestMethod]
        public void EtpSupportedDataObjectCollection_Union_Instance_DataObjects_And_Counterpart_Empty()
        {
            var instanceDataObjects = new List<EtpSupportedDataObject>
            {
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "Well")),
                new EtpSupportedDataObject(new EtpDataObjectType("witsml", "2.0", "Wellbore")),
                new EtpSupportedDataObject(new EtpDataObjectType("prodml", "2.0", "*")),
            };
            var counterpartDataObjects = new List<EtpSupportedDataObject>
            {
            };

            var collection = EtpSupportedDataObjectCollection.GetSupportedDataObjectCollection(instanceDataObjects, counterpartDataObjects, false);

            Assert.AreEqual(2, collection.SupportedTypes.Count());
            Assert.AreEqual("Well", collection.SupportedTypes.ToList()[0].QualifiedType.ObjectType);
            Assert.AreEqual("Wellbore", collection.SupportedTypes.ToList()[1].QualifiedType.ObjectType);
            Assert.AreEqual(1, collection.SupportedFamilies.Count());
        }
    }
}
