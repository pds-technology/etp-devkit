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
using Energistics.Etp.Common.Datatypes;
using System;
using System.Collections.Generic;

namespace Energistics.Etp.Data
{
    public class MockWell : MockWitsmlObject
    {
        new public static EtpDataObjectType Type { get; } = new EtpDataObjectType(MockWitsmlObject.Type, "Well");

        public static HashSet<EtpDataObjectType> SourceTypes { get; } = new HashSet<EtpDataObjectType> { MockWellbore.Type };
        public static HashSet<EtpDataObjectType> TargetTypes { get; } = new HashSet<EtpDataObjectType> { };
        public static HashSet<EtpDataObjectType> SecondarySourceTypes { get; } = new HashSet<EtpDataObjectType> { };
        public static HashSet<EtpDataObjectType> SecondaryTargetTypes { get; } = new HashSet<EtpDataObjectType> { };

        public override HashSet<EtpDataObjectType> SupportedSourceTypes => SourceTypes;
        public override HashSet<EtpDataObjectType> SupportedTargetTypes => TargetTypes;
        public override HashSet<EtpDataObjectType> SupportedSecondarySourceTypes => SecondarySourceTypes;
        public override HashSet<EtpDataObjectType> SupportedSecondaryTargetTypes => SecondaryTargetTypes;

        public MockWell()
        {
            DataObjectType = Type;
        }

        public override string Xml(EtpVersion version, string indentation = "", bool embedded = false) =>
$@"{indentation}<Well{Namespaces(embedded)} schemaVersion=""2.0"" uuid=""{Uuid}""{DefaultNamespace(embedded)}>
{indentation}  <Citation xmlns=""http://www.energistics.org/energyml/data/commonv2"">
{indentation}    <Title>{Title}</Title>
{indentation}    <Originator>ETP DevKit</Originator>
{indentation}    <Creation>{Creation.ToUniversalTime():O}</Creation>
{indentation}    <Format>[Energistics:ETP DevKit {typeof(IEtpSession).Assembly.GetName().Version}]</Format>
{indentation}    <LastUpdate>{LastUpdate.ToUniversalTime():O}</LastUpdate>
{indentation}  </Citation>
{indentation}</Well>";

    }
}
