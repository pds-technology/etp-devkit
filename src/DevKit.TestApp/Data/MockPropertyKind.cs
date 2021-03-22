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
using System.Collections.Generic;

namespace Energistics.Etp.Data
{
    public class MockPropertyKind : MockCommonObject
    {
        new public static EtpDataObjectType Type { get; } = new EtpDataObjectType(MockCommonObject.Type, "PropertyKind");

        public static HashSet<EtpDataObjectType> SourceTypes { get; } = new HashSet<EtpDataObjectType> { Type };
        public static HashSet<EtpDataObjectType> TargetTypes { get; } = new HashSet<EtpDataObjectType> { };
        public static HashSet<EtpDataObjectType> SecondarySourceTypes { get; } = new HashSet<EtpDataObjectType> { MockChannel.Type };
        public static HashSet<EtpDataObjectType> SecondaryTargetTypes { get; } = new HashSet<EtpDataObjectType> { Type };

        public override HashSet<EtpDataObjectType> SupportedSourceTypes => SourceTypes;
        public override HashSet<EtpDataObjectType> SupportedTargetTypes => TargetTypes;
        public override HashSet<EtpDataObjectType> SupportedSecondarySourceTypes => SecondarySourceTypes;
        public override HashSet<EtpDataObjectType> SupportedSecondaryTargetTypes => SecondaryTargetTypes;


        public MockPropertyKind()
        {
            DataObjectType = Type;
        }

        public string IsAbstract { get; set; }

        public string QuantityClass { get; set; }

        public override string Xml(EtpVersion version, string indentation = "", bool embedded = false) =>
$@"{indentation}<PropertyKind{Namespaces(embedded)} schemaVersion=""2.0"" uuid=""{Uuid}""{DefaultNamespace(embedded)}>
{indentation}  <Citation>
{indentation}    <Title>{Title}</Title>
{indentation}    <Originator>Energistics</Originator>
{indentation}    <Creation>{Creation.ToUniversalTime():O}</Creation>
{indentation}    <Format>Excel:xml</Format>
{indentation}  </Citation>
{indentation}  <IsAbstract>{IsAbstract}</IsAbstract>
{indentation}  <QuantityClass>{QuantityClass}</QuantityClass>
{indentation}{(Parent == null ? string.Empty : $@"  <Parent>
{indentation}    <ContentType>{Parent.ContentType}</ContentType>
{indentation}    <Title>{Parent.Title}</Title>
{indentation}    <Uuid>{Parent.Uuid}</Uuid>
{indentation}    <UuidAuthority>Energistics</UuidAuthority>
{indentation}  </Parent>
{indentation}")}</PropertyKind>";
    }
}
