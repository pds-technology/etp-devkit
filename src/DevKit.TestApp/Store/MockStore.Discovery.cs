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

namespace Energistics.Etp.Store
{
    public partial class MockStore
    {
        public static EtpDataObjectType TryGetCorrectedDataObjectType(EtpUri uri)
        {
            if (uri.IsEtp12)
                return uri.DataObjectType;

            var originalType = uri.DataObjectType;

            var segments = uri.GetSegments().ToList();
            if (segments.Count < 2)
                return originalType;

            var supportedTypes = new HashSet<EtpDataObjectType>();
            HashSet<EtpDataObjectType> primaryTypes, secondaryTypes;
            if (SupportedSourceTypes.TryGetValue(segments[0].DataObjectType, out primaryTypes))
                supportedTypes.UnionWith(primaryTypes);
            if (SupportedSecondarySourceTypes.TryGetValue(segments[0].DataObjectType, out secondaryTypes))
                supportedTypes.UnionWith(secondaryTypes);
            if (supportedTypes.Count == 0)
                return originalType;

            for (int i = 1; i < segments.Count; i++)
            {
                var correctedType = supportedTypes.FirstOrDefault(dt => string.Equals(segments[i].ObjectType, dt.ObjectType, StringComparison.OrdinalIgnoreCase));
                if (correctedType == null)
                    return originalType;

                if (i == segments.Count - 1)
                    return correctedType;

                supportedTypes = new HashSet<EtpDataObjectType>();
                if (SupportedSourceTypes.TryGetValue(correctedType, out primaryTypes))
                    supportedTypes.UnionWith(primaryTypes);
                if (SupportedSecondarySourceTypes.TryGetValue(correctedType, out secondaryTypes))
                    supportedTypes.UnionWith(secondaryTypes);
                if (supportedTypes.Count == 0)
                    return originalType;
            }

            return originalType;
        }

        public MockDataspace GetDataspace(EtpUri uri)
        {
            CheckLocked();

            var dataspace = uri.Dataspace ?? string.Empty;
            return Dataspaces.FirstOrDefault(d => string.Equals(d.Name, dataspace, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<MockFamily> GetFamilies(EtpVersion version, EtpUri uri)
        {
            CheckLocked();

            if (!((version == EtpVersion.v11 && uri.IsEtp11) || (version == EtpVersion.v12 && uri.IsEtp12)))
                yield break;

            var dataspace = GetDataspace(uri);
            if (dataspace == null)
                yield break;

            foreach (var family in dataspace.Families)
                yield return family;
        }

        public MockObject GetObject(EtpVersion version, EtpUri uri)
        {
            CheckLocked();

            if (!((version == EtpVersion.v11 && uri.IsEtp11) || (version == EtpVersion.v12 && uri.IsEtp12)))
                return null;

            var dataspace = GetDataspace(uri);
            if (dataspace == null)
                return null;

            MockObject @object;
            if (!ObjectsByUri[version].TryGetValue(uri, out @object))
                return null;

            return @object;
        }

        public IEnumerable<MockObject> GetObjects(EtpVersion version, MockGraphContext context, bool? activeStatusFilter = null, DateTime? storeLastWriteFilter = null)
        {
            CheckLocked();

            var uri = context.Uri;
            if (!((version == EtpVersion.v11 && uri.IsEtp11) || (version == EtpVersion.v12 && uri.IsEtp12)))
                yield break;

            IEnumerable<MockObject> objects = null;

            var dataspace = GetDataspace(uri);
            if (dataspace == null)
            {
                if (uri.IsRootUri)
                    objects = Dataspaces.SelectMany(d => d.Objects);
                else
                    yield break;
            }

            if (objects == null)
            {
                if (uri.IsBaseUri)
                {
                    if (uri.IsFamilyVersionUri)
                        objects = dataspace.Objects.FilterByFamilyAndVersion(uri.DataObjectType);
                    else
                        objects = dataspace.Objects;
                }
                else
                {
                    var @object = GetObject(version, uri);
                    if (@object == null)
                        yield break;

                    objects = @object.WalkGraph(context);
                }
            }

            var objectTypes = context.DataObjectTypes;
            foreach (var @object in objects.FilterByType(objectTypes).FilterByActiveStatus(activeStatusFilter).FilterByStoreLastWrite(storeLastWriteFilter))
                yield return @object;
        }

        public IEnumerable<MockObject> GetDeletedObjects(EtpVersion version, EtpUri uri, ISet<EtpDataObjectType> objectTypes = null, DateTime? deleteTimeFilter = null)
        {
            CheckLocked();

            if (!((version == EtpVersion.v11 && uri.IsEtp11) || (version == EtpVersion.v12 && uri.IsEtp12)))
                yield break;

            if (!uri.IsDataspaceUri)
                yield break;

            var dataspace = GetDataspace(uri);
            if (dataspace == null)
                yield break;

            foreach (var @object in dataspace.DeletedObjects.FilterByType(objectTypes).FilterByStoreLastWrite(deleteTimeFilter))
                yield return @object;
        }

        public IEnumerable<MockSupportedType> GetSupportedTypes(EtpVersion version, MockGraphContext context, bool includeEmptyTypes)
        {
            CheckLocked();

            var uri = context.Uri;
            if (!((version == EtpVersion.v11 && uri.IsEtp11) || (version == EtpVersion.v12 && uri.IsEtp12)))
                yield break;

            var dataspace = GetDataspace(uri);
            if (dataspace == null || context.IsSelfOnly)
                yield break;

            IEnumerable<IDataObjectType> supportedTypes;
            IEnumerable<MockObject> objects;
            if (uri.IsDataRootUri)
            {
                supportedTypes = SupportedDataObjects.Select(sdo => sdo.QualifiedType);
                if (uri.IsFamilyVersionUri)
                    supportedTypes = supportedTypes.FilterByFamilyAndVersion(uri.DataObjectType);

                objects = dataspace.Objects;
            }
            else
            {
                MockObject @object;
                if (!ObjectsByUri[version].TryGetValue(uri, out @object))
                    yield break;

                supportedTypes = @object.SupportedSourceAndTargetTypes(context);
                objects = @object.SourcesAndTargets(context);
            }

            foreach (var supportedType in supportedTypes)
            {
                var count = objects.Count(o => o.DataObjectType.MatchesExact(supportedType));
                if (count > 0 || includeEmptyTypes)
                    yield return new MockSupportedType { Uri = uri.Append(supportedType), DataObjectType = supportedType, ObjectCount = count };
            }
        }

    }
}
