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
using Energistics.Etp.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Energistics.Etp.Data
{
    public static class MockExtensions
    {
        public static ISet<EtpDataObjectType> ToDataObjectTypes(this IEnumerable<string> objectTypes)
        {
            if (objectTypes == null)
                return new HashSet<EtpDataObjectType>();

            return new HashSet<EtpDataObjectType>(objectTypes.Select(ot => new EtpDataObjectType(ot)));
        }

        public static IEnumerable<EtpContentType> ToContentTypes(this IEnumerable<string> contentTypes)
        {
            if (contentTypes == null)
                return Enumerable.Empty<EtpContentType>();

            return contentTypes.Select(ot => new EtpContentType(ot));
        }

        public static IEnumerable<MockObject> FilterByType(this IEnumerable<MockObject> objects, ISet<EtpDataObjectType> objectTypes)
        {
            if (objectTypes == null || objectTypes.Count == 0)
                return objects;

            return objects.Where(o => objectTypes.Contains(o.DataObjectType));
        }

        public static IEnumerable<MockObject> FilterByFamilyAndVersion(this IEnumerable<MockObject> objects, IDataObjectType dataObjectType)
        {
            return objects.Where(o => o.DataObjectType.MatchesFamilyAndVersion(dataObjectType));
        }

        public static IEnumerable<IDataObjectType> FilterByFamilyAndVersion(this IEnumerable<IDataObjectType> dataObjectTypes, IDataObjectType dataObjectType)
        {
            return dataObjectTypes.Where(dt => dt.MatchesFamilyAndVersion(dataObjectType));
        }

        public static IEnumerable<MockObject> FilterByActiveStatus(this IEnumerable<MockObject> objects, bool? activeStatusFilter)
        {
            if (activeStatusFilter == null)
                return objects;

            return objects.Where(o => o is IMockActiveObject && ((IMockActiveObject)o).IsActive == activeStatusFilter);
        }

        public static IEnumerable<MockObject> FilterByStoreLastWrite(this IEnumerable<MockObject> objects, DateTime? lastWriteFilter)
        {
            if (lastWriteFilter == null)
                return objects;

            return objects.Where(o => o.StoreLastWrite > lastWriteFilter);
        }

        public static IEnumerable<MockObject> FilterByObjectLastWrite(this IEnumerable<MockObject> objects, DateTime? lastWriteFilter)
        {
            if (lastWriteFilter == null)
                return objects;

            return objects.Where(o => o.ObjectLastWrite > lastWriteFilter);
        }

        public static IEnumerable<MockObject> FilterByUri(this IEnumerable<MockObject> objects, EtpVersion version, EtpUri uri)
        {
            return objects.Where(o => o.Uri(version) == uri);
        }
    }
}
