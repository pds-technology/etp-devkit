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

using Energistics.Etp.Data;
using log4net;
using System;
using System.Collections.Generic;

namespace Energistics.Etp.Handlers
{
    public class TestDataspace : MockDataspace
    {
        private ILog Logger { get; }

        public MockWell Well01 { get; }
        public MockWell Well02 { get; }
        public MockWellbore Wellbore01 { get; }
        public MockWellbore Wellbore02 { get; }
        public MockWellbore Wellbore03 { get; }
        public MockWellbore Wellbore04 { get; }
        public MockWellbore Wellbore05 { get; }
        public MockChannelSet TimeChannelSet01 { get; }
        public MockChannel TimeChannel01 { get; }
        public MockChannel TimeChannel02 { get; }
        public MockChannel TimeChannel03 { get; }
        public MockChannel TimeChannel04 { get; }
        public MockChannelSet DepthChannelSet01 { get; }
        public MockChannel DepthChannel01 { get; }
        public MockChannel DepthChannel02 { get; }

        public MockPropertyKind RootProperty { get; }
        public MockPropertyKind Velocity { get; }
        public MockPropertyKind PenetrationRate { get; }
        public MockPropertyKind AvgPenetrationRate { get; }
        public MockPropertyKind Force { get; }
        public MockPropertyKind Load { get; }
        public MockPropertyKind HookLoad { get; }
        public MockPropertyKind Length{ get; }
        public MockPropertyKind Depth { get; }
        public MockPropertyKind MeasuredDepth { get; }
        public MockPropertyKind Time { get; }
        public MockPropertyKind AbsoluteTime { get; }
        public MockPropertyKind TimeStamp { get; }

        public MockFamily Witsml { get; }
        public MockFamily Eml { get; }

        public TestDataspace(string name, DateTime creation)
            : base(name)
        {
            Logger = LogManager.GetLogger(GetType());

            RootProperty = new MockPropertyKind
            {
                Dataspace = this,
                Uuid = Guid.Parse("a48c9c25-1e3a-43c8-be6a-044224cc69cb"),
                Title = "property",
                Creation = DateTime.Parse("2016-12-09T17:30:47Z").ToUniversalTime(),
                LastUpdate = DateTime.Parse("2016-12-09T17:30:47Z").ToUniversalTime(),
                IsAbstract = "false",
                QuantityClass = "unitless",
            };

            Velocity = new MockPropertyKind
            {
                Dataspace = this,
                Uuid = Guid.Parse("d80b7b4d-f51d-4821-b1db-c595f18c51db"),
                Title = "velocity",
                Creation = DateTime.Parse("2016-12-09T17:30:47Z").ToUniversalTime(),
                LastUpdate = DateTime.Parse("2016-12-09T17:30:47Z").ToUniversalTime(),
                IsAbstract = "false",
                QuantityClass = "length per time",
            };

            PenetrationRate = new MockPropertyKind
            {
                Dataspace = this,
                Uuid = Guid.Parse("d2b70df2-6df3-4751-bd02-150e3fc96450"),
                Title = "penetration rate",
                Creation = DateTime.Parse("2016-12-09T17:30:47Z").ToUniversalTime(),
                LastUpdate = DateTime.Parse("2016-12-09T17:30:47Z").ToUniversalTime(),
                IsAbstract = "false",
                QuantityClass = "length per time",
            };

            AvgPenetrationRate = new MockPropertyKind
            {
                Dataspace = this,
                Uuid = Guid.Parse("7fbf2718-f45a-444e-ae3c-17f79f8817e2"),
                Title = "avg penetration rate",
                Creation = DateTime.Parse("2016-12-09T17:30:47Z").ToUniversalTime(),
                LastUpdate = DateTime.Parse("2016-12-09T17:30:47Z").ToUniversalTime(),
                IsAbstract = "false",
                QuantityClass = "length per time",
            };

            Force = new MockPropertyKind
            {
                Dataspace = this,
                Uuid = Guid.Parse("a5789b56-72a3-4561-b906-b5ba13852c23"),
                Title = "force",
                Creation = DateTime.Parse("2016-12-09T17:30:47Z").ToUniversalTime(),
                LastUpdate = DateTime.Parse("2016-12-09T17:30:47Z").ToUniversalTime(),
                IsAbstract = "false",
                QuantityClass = "force",
            };

            Load = new MockPropertyKind
            {
                Dataspace = this,
                Uuid = Guid.Parse("3bf92c0b-b712-4128-b7bc-b21e0270d9be"),
                Title = "load",
                Creation = DateTime.Parse("2016-12-09T17:30:47Z").ToUniversalTime(),
                LastUpdate = DateTime.Parse("2016-12-09T17:30:47Z").ToUniversalTime(),
                IsAbstract = "false",
                QuantityClass = "force",
            };

            HookLoad = new MockPropertyKind
            {
                Dataspace = this,
                Uuid = Guid.Parse("d0a7c1c5-73da-45b5-a762-eb49721d91e9"),
                Title = "hook load",
                Creation = DateTime.Parse("2016-12-09T17:30:47Z").ToUniversalTime(),
                LastUpdate = DateTime.Parse("2016-12-09T17:30:47Z").ToUniversalTime(),
                IsAbstract = "false",
                QuantityClass = "force",
            };

            Length = new MockPropertyKind
            {
                Dataspace = this,
                Uuid = Guid.Parse("4a305182-221e-4205-9e7c-a36b06fa5b3d"),
                Title = "length",
                Creation = DateTime.Parse("2016-12-09T17:30:47Z").ToUniversalTime(),
                LastUpdate = DateTime.Parse("2016-12-09T17:30:47Z").ToUniversalTime(),
                IsAbstract = "false",
                QuantityClass = "length",
            };

            Depth = new MockPropertyKind
            {
                Dataspace = this,
                Uuid = Guid.Parse("4364b378-899a-403a-8401-b06abd4fc0cf"),
                Title = "depth",
                Creation = DateTime.Parse("2016-12-09T17:30:47Z").ToUniversalTime(),
                LastUpdate = DateTime.Parse("2016-12-09T17:30:47Z").ToUniversalTime(),
                IsAbstract = "false",
                QuantityClass = "length",
            };

            MeasuredDepth = new MockPropertyKind
            {
                Dataspace = this,
                Uuid = Guid.Parse("c48c65d4-9680-4d10-903c-a1b2e30f66b5"),
                Title = "measured depth",
                Creation = DateTime.Parse("2016-12-09T17:30:47Z").ToUniversalTime(),
                LastUpdate = DateTime.Parse("2016-12-09T17:30:47Z").ToUniversalTime(),
                IsAbstract = "false",
                QuantityClass = "length",
            };

            Time = new MockPropertyKind
            {
                Dataspace = this,
                Uuid = Guid.Parse("3a72f130-4ebf-4ea9-a330-a610a4e4ed0e"),
                Title = "time",
                Creation = DateTime.Parse("2016-12-09T17:30:47Z").ToUniversalTime(),
                LastUpdate = DateTime.Parse("2016-12-09T17:30:47Z").ToUniversalTime(),
                IsAbstract = "false",
                QuantityClass = "time",
            };

            AbsoluteTime = new MockPropertyKind
            {
                Dataspace = this,
                Uuid = Guid.Parse("df21fba2-fe8c-4d66-9a73-fbd0e6013abb"),
                Title = "absolute time",
                Creation = DateTime.Parse("2016-12-09T17:30:47Z").ToUniversalTime(),
                LastUpdate = DateTime.Parse("2016-12-09T17:30:47Z").ToUniversalTime(),
                IsAbstract = "false",
                QuantityClass = "time",
            };

            TimeStamp = new MockPropertyKind
            {
                Dataspace = this,
                Uuid = Guid.Parse("6a2ecc9b-7561-41eb-8c6d-adf83538c085"),
                Title = "time stamp",
                Creation = DateTime.Parse("2016-12-09T17:30:47Z").ToUniversalTime(),
                LastUpdate = DateTime.Parse("2016-12-09T17:30:47Z").ToUniversalTime(),
                IsAbstract = "false",
                QuantityClass = "time",
            };

            Velocity.SetParent(RootProperty, creation);
            PenetrationRate.SetParent(Velocity, creation);
            AvgPenetrationRate.SetParent(PenetrationRate, creation);

            Force.SetParent(RootProperty, creation);
            Load.SetParent(Force, creation);
            HookLoad.SetParent(Load, creation);

            Length.SetParent(RootProperty, creation);
            Depth.SetParent(Length, creation);
            MeasuredDepth.SetParent(Depth, creation);

            Time.SetParent(RootProperty, creation);
            AbsoluteTime.SetParent(Time, creation);
            TimeStamp.SetParent(AbsoluteTime, creation);

            Well01 = new MockWell
            {
                Dataspace = this,
                Title = "Well 01",
                Creation = creation,
                LastUpdate = creation,
            };

            Well02 = new MockWell
            {
                Dataspace = this,
                Title = "Well 02",
                Creation = creation,
                LastUpdate = creation,
            };
            Wellbore01 = new MockWellbore
            {
                Dataspace = this,
                Title = "Wellbore 01",
                Creation = creation,
                LastUpdate = creation,
            };
            Wellbore01.SetWell(Well01, creation);

            Wellbore02 = new MockWellbore
            {
                Dataspace = this,
                Title = "Wellbore 02",
                Creation = creation,
                LastUpdate = creation,
            };
            Wellbore02.SetWell(Well01, creation);

            Wellbore03 = new MockWellbore
            {
                Dataspace = this,
                Title = "Wellbore 03 - Deleted Periodically",
                Creation = creation,
                LastUpdate = creation,
            };
            Wellbore03.SetWell(Well01, creation);

            Wellbore04 = new MockWellbore
            {
                Dataspace = this,
                Title = "Wellbore 04 - Permanently Deleted",
                Creation = creation,
                LastUpdate = creation,
            };
            Wellbore04.SetWell(Well01, creation);

            Wellbore05 = new MockWellbore
            {
                Dataspace = this,
                Title = "Wellbore 05 - Automatic Subscription",
                Creation = creation,
                LastUpdate = creation,
            };
            Wellbore05.SetWell(Well01, creation);

            TimeChannel01 = new MockChannel(true)
            {
                Dataspace = this,
                Title = "Average ROP",
                Creation = creation,
                LastUpdate = creation,

                ChannelPropertyKind = AvgPenetrationRate,
                Mnemonic = "ROPA",
                Uom = "m/h",
            };
            TimeChannel01.SetWellbore(Wellbore01, creation);

            TimeChannel02 = new MockChannel(true)
            {
                Dataspace = this,
                Title = "Hook Load",
                Creation = creation,
                LastUpdate = creation,

                ChannelPropertyKind = HookLoad,
                Mnemonic = "HKLD",
                Uom = "10 kN",
            };
            TimeChannel02.SetWellbore(Wellbore01, creation);

            TimeChannel03 = new MockChannel(true)
            {
                Dataspace = this,
                Title = "Bit Depth - Periodically Deleted",
                Creation = creation,
                LastUpdate = creation,

                ChannelPropertyKind = MeasuredDepth,
                Mnemonic = "BDEP",
                Uom = "m",
            };
            TimeChannel03.SetWellbore(Wellbore01, creation);

            TimeChannel04 = new MockChannel(true)
            {
                Dataspace = this,
                Title = "Hole Depth - Periodically Unjoined",
                Creation = creation,
                LastUpdate = creation,

                ChannelPropertyKind = MeasuredDepth,
                Mnemonic = "HDEP",
                Uom = "m",
            };
            TimeChannel04.SetWellbore(Wellbore01, creation);

            TimeChannelSet01 = new MockChannelSet
            {
                Dataspace = this,
                Title = "Time ChannelSet 01",
                Creation = creation,
                LastUpdate = creation,
            };
            TimeChannelSet01.SetWellbore(Wellbore01, creation);
            TimeChannel01.JoinContainer(TimeChannelSet01, creation);
            TimeChannel02.JoinContainer(TimeChannelSet01, creation);
            TimeChannel03.JoinContainer(TimeChannelSet01, creation);
            TimeChannel04.JoinContainer(TimeChannelSet01, creation);

            DepthChannel01 = new MockChannel(false)
            {
                Dataspace = this,
                Title = "Average ROP",
                Creation = creation,
                LastUpdate = creation,

                ChannelPropertyKind = AvgPenetrationRate,
                Mnemonic = "ROPA",
                Uom = "m/h",
            };
            DepthChannel01.SetWellbore(Wellbore01, creation);

            DepthChannel02 = new MockChannel(false)
            {
                Dataspace = this,
                Title = "Hook Load",
                Creation = creation,
                LastUpdate = creation,

                ChannelPropertyKind = HookLoad,
                Mnemonic = "HKLD",
                Uom = "10 kN",
            };
            DepthChannel02.SetWellbore(Wellbore01, creation);

            DepthChannelSet01 = new MockChannelSet
            {
                Dataspace = this,
                Title = "Depth ChannelSet 01",
                Creation = creation,
                LastUpdate = creation,
            };
            DepthChannelSet01.SetWellbore(Wellbore01, creation);
            DepthChannel01.JoinContainer(DepthChannelSet01, creation);
            DepthChannel02.JoinContainer(DepthChannelSet01, creation);

            var objects = new List<MockObject>
            {
                Well01,
                Well02,
                Wellbore01,
                Wellbore02,
                Wellbore03,
                Wellbore05,
                RootProperty,
                Velocity,
                PenetrationRate,
                AvgPenetrationRate,
                Force,
                Load,
                HookLoad,
                Length,
                Depth,
                MeasuredDepth,
                Time,
                AbsoluteTime,
                TimeStamp,
                TimeChannelSet01,
                TimeChannel01,
                TimeChannel02,
                TimeChannel03,
                TimeChannel04,
                DepthChannelSet01,
                DepthChannel01,
                DepthChannel02,
            };

            Objects.Clear();
            foreach (var @object in objects)
            {
                Objects[@object.Uuid] = @object;
                @object.Create(creation);
            }

            var deletedObjects = new List<MockObject>
            {
                Wellbore04,
            };

            DeletedObjects.Clear();
            foreach (var deletedObject in deletedObjects)
                DeletedObjects[deletedObject.Uuid] = deletedObject;

            Witsml = new MockFamily
            {
                Dataspace = this,
                Title = "WITSML Store (2.0)",
                Type = MockWitsmlObject.Type,
            };

            Eml = new MockFamily
            {
                Dataspace = this,
                Title = "EML Common (2.1)",
                Type = MockCommonObject.Type,
            };

            var families = new List<MockFamily> { Witsml, Eml };

            Families.Clear();
            Families.AddRange(families);
        }

        public void SetName(string name)
        {
            Name = name;
        }
    }
}
