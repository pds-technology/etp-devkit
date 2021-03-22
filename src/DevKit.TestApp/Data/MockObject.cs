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
using System.Linq;
using System.Text;

namespace Energistics.Etp.Data
{
    public abstract class MockObject
    {
        public static HashSet<EtpDataObjectType> ChannelTypes => new HashSet<EtpDataObjectType> { MockChannel.Type };
        public static HashSet<EtpDataObjectType> PartObjectTypes => new HashSet<EtpDataObjectType> { MockTrajectory.Type };
        public static HashSet<EtpDataObjectType> GrowingObjectTypes => new HashSet<EtpDataObjectType>(ChannelTypes.Concat(PartObjectTypes));


        public MockDataspace Dataspace { get; set; }

        public Guid Uuid { get; set; } = Guid.NewGuid();

        public string Title { get; set; }

        public EtpDataObjectType DataObjectType { get; set; }

        public bool IsDeleted { get; protected set; }

        public string ContentType => DataObjectType?.ToContentType();

        public DateTime Creation { get; set; } = 0L.ToUtcDateTime();

        public DateTime LastUpdate { get; set; } = 0L.ToUtcDateTime();

        public DateTime StoreLastWrite => ObjectLastWrite > DataLastWrite ? ObjectLastWrite : DataLastWrite;

        public DateTime ObjectLastWrite { get; private set; } = 0L.ToUtcDateTime();

        public DateTime DataLastWrite { get; private set; } = 0L.ToUtcDateTime();

        public DateTime StoreCreated { get; protected set; } = 0L.ToUtcDateTime();

        public MockObject Parent { get; set; }

        public MockObject Container { get; set; }

        public List<MockObject> Sources { get; set; } = new List<MockObject>();

        public List<MockObject> Targets { get; set; } = new List<MockObject>();

        public List<MockObject> SecondarySources { get; set; } = new List<MockObject>();

        public List<MockObject> SecondaryTargets { get; set; } = new List<MockObject>();

        public abstract HashSet<EtpDataObjectType> SupportedSourceTypes { get; }
        public abstract HashSet<EtpDataObjectType> SupportedTargetTypes { get; }
        public abstract HashSet<EtpDataObjectType> SupportedSecondarySourceTypes { get; }
        public abstract HashSet<EtpDataObjectType> SupportedSecondaryTargetTypes { get; }


        public abstract string Xml(EtpVersion version, string indentation = "", bool embedded = false);

        public EtpUri Uri(EtpVersion version) => new EtpUri(version, DataObjectType, dataspace: Dataspace?.Name, objectId: Uuid.ToString());

        public void ClearLinks()
        {
            Sources.Clear();
            SecondarySources.Clear();
            Targets.Clear();
            SecondaryTargets.Clear();
        }

        public virtual void Link()
        {
            if (IsDeleted) return;

            if (Parent != null && !Parent.IsDeleted)
            {
                Targets.Add(Parent);
                Parent.Sources.Add(this);
            }
            if (Container != null && !Container.IsDeleted)
            {
                Targets.Add(Container);
                Container.Sources.Add(this);
            }
        }

        public void Touch(DateTime touchTime)
        {
            LastUpdate = touchTime;
            UpdateObjectLastWrite(touchTime);
            if (Container != null && !Container.IsDeleted)
                Container.Touch(touchTime);
        }

        public virtual void Create(DateTime createdTime)
        {
            StoreCreated = createdTime;
            UpdateObjectLastWrite(createdTime);
            UpdateDataLastWrite(createdTime);
        }

        public void Delete(DateTime deleteTime)
        {
            if (IsDeleted)
                return;

            IsDeleted = true;
            UpdateObjectLastWrite(deleteTime);
        }

        public void Restore(DateTime createTime)
        {
            if (!IsDeleted)
                return;

            IsDeleted = false;
            Create(createTime);
        }

        public void Unjoin(DateTime unjoinTime)
        {
            if (Container == null)
                return;

            Container.UnjoinChild(this, unjoinTime);
            Container = null;
            UpdateObjectLastWrite(unjoinTime);
        }

        protected virtual void UnjoinChild(MockObject child, DateTime unjoinTime)
        {
            UpdateObjectLastWrite(unjoinTime);
        }

        public void Join(MockObject container, DateTime joinTime)
        {
            if (container == null || Container != null)
                return;

            UpdateObjectLastWrite(joinTime);
            Container = container;
            Container.JoinChild(this, joinTime);
        }

        protected virtual void JoinChild(MockObject child, DateTime unjoinTime)
        {
            UpdateObjectLastWrite(unjoinTime);
        }

        protected void UpdateObjectLastWrite(DateTime lastWrite)
        {
            ObjectLastWrite = lastWrite;
            if (Container != null && !Container.IsDeleted)
                Container.UpdateObjectLastWrite(lastWrite);
        }

        protected void UpdateDataLastWrite(DateTime lastWrite)
        {
            DataLastWrite = lastWrite;
            if (Container != null && !Container.IsDeleted)
                Container.UpdateDataLastWrite(lastWrite);
        }

        public IEnumerable<EtpUri> AlternateUris(EtpVersion version)
        {
            if (Parent == null && Container == null)
                return Enumerable.Empty<EtpUri>();

            return ConstructAlternateUris(this, version);
        }

        public IEnumerable<MockObject> SourcesAndTargets(MockGraphContext context)
        {
            if (context.IncludeSources)
            {
                if (context.NavigatePrimaryEdges)
                {
                    foreach (var @object in Sources)
                        yield return @object;
                }
                if (context.NavigateSecondaryEdges)
                {
                    foreach (var @object in SecondarySources)
                        yield return @object;
                }
            }
            if (context.IncludeTargets)
            {
                if (context.NavigatePrimaryEdges)
                {
                    foreach (var @object in Targets)
                        yield return @object;
                }
                if (context.NavigateSecondaryEdges)
                {
                    foreach (var @object in SecondaryTargets)
                        yield return @object;
                }
            }
        }

        public IEnumerable<MockObject> SecondarySourcesAndTargets(MockGraphContext context)
        {
            if (context.IncludeSecondarySources)
            {
                foreach (var @object in SecondarySources)
                    yield return @object;
            }
            if (context.IncludeSecondaryTargets)
            {
                foreach (var @object in SecondaryTargets)
                    yield return @object;
            }
        }

        public IEnumerable<EtpDataObjectType> SupportedSourceAndTargetTypes(MockGraphContext context)
        {
            if (context.IncludeSources)
            {
                if (context.NavigatePrimaryEdges)
                {
                    foreach (var dataObjectType in SupportedSourceTypes)
                        yield return dataObjectType;
                }
                if (context.NavigateSecondaryEdges)
                {
                    foreach (var dataObjectType in SupportedSecondarySourceTypes)
                        yield return dataObjectType;
                }
            }
            if (context.IncludeTargets)
            {
                if (context.NavigatePrimaryEdges)
                {
                    foreach (var dataObjectType in SupportedTargetTypes)
                        yield return dataObjectType;
                }
                if (context.NavigateSecondaryEdges)
                {
                    foreach (var dataObjectType in SupportedSecondaryTargetTypes)
                        yield return dataObjectType;
                }
            }
        }

        private static IEnumerable<EtpUri> ConstructAlternateUris(MockObject @object, EtpVersion version)
        {
            // Deleted objects are included to allow some checks against scopes during object deletion.
            if (@object.Parent == null && @object.Container == null)
            {
                yield return @object.Uri(version);
            }
            else
            {
                if (@object.Parent != null)
                {
                    foreach (var uri in ConstructAlternateUris(@object.Parent, version))
                        yield return uri.Append(@object.DataObjectType, objectId: @object.Uuid.ToString());
                }
                if (@object.Container != null)
                {
                    foreach (var uri in ConstructAlternateUris(@object.Container, version))
                        yield return uri.Append(@object.DataObjectType, objectId: @object.Uuid.ToString());
                }
            }
        }

        public IEnumerable<MockObject> WalkGraph(MockGraphContext context)
        {
            var visited = new Dictionary<Guid, MockObject>();
            foreach (var @object in WalkGraph(context.Depth, context.IncludeSelf, context, visited))
                yield return @object;

            if (context.IncludeSecondarySources || context.IncludeSecondaryTargets)
            {
                foreach (var @object in visited.Values.ToList())
                {
                    foreach (var secondary in @object.SecondarySourcesAndTargets(context))
                    {
                        if (!visited.ContainsKey(secondary.Uuid))
                        {
                            visited[secondary.Uuid] = secondary;
                            yield return secondary;
                        }
                    }
                }
            }
        }

        private IEnumerable<MockObject> WalkGraph(int depth, bool includeSelf, MockGraphContext context, Dictionary<Guid, MockObject> visited)
        {
            // Deleted objects are included to allow some checks against scopes during object deletion.
            if (includeSelf)
            {
                if (!visited.ContainsKey(Uuid))
                {
                    visited[Uuid] = this;
                    yield return this;
                }
            }
            if (depth <= 0)
                yield break;

            foreach (var @object in SourcesAndTargets(context))
            {
                if (!visited.ContainsKey(@object.Uuid))
                {
                    visited[@object.Uuid] = @object;
                    yield return @object;
                    if (depth > 1)
                    {
                        foreach (var node in @object.WalkGraph(depth - 1, false, context, visited))
                            yield return node;
                    }
                }
            }
        }

        public v11.Datatypes.Object.Resource Resource11 => new v11.Datatypes.Object.Resource
        {
            Uuid = Uuid.ToString(),
            Uri = Uri(EtpVersion.v11),
            Name = Title,
            HasChildren = (SupportedSourceTypes.Count + SupportedSecondarySourceTypes.Count) > 0 ? -1 : 0,
            ContentType = ContentType,
            ResourceType = ResourceTypes.DataObject.ToString(),
            CustomData = new Dictionary<string, string>(),
            ChannelSubscribable = true,
            ObjectNotifiable = true,
            LastChanged = LastUpdate.ToEtpTimestamp(),
        };

        public v12.Datatypes.Object.Resource Resource12 => new v12.Datatypes.Object.Resource
        {
            Uri = Uri(EtpVersion.v12),
            AlternateUris = AlternateUris(EtpVersion.v12).Select(uri => uri.Uri).ToList(),
            Name = Title,
            DataObjectType = DataObjectType,
            SourceCount = Sources.Count + SecondarySources.Count,
            TargetCount = Targets.Count + SecondaryTargets.Count,
            LastChanged = LastUpdate.ToEtpTimestamp(),
            StoreLastWrite = StoreLastWrite.ToEtpTimestamp(),
            CustomData = new Dictionary<string, v12.Datatypes.DataValue>(),
        };

        public v12.Datatypes.Object.DeletedResource DeletedResource12 => new v12.Datatypes.Object.DeletedResource
        {
            Uri = Uri(EtpVersion.v12),
            DeletedTime = StoreLastWrite.ToEtpTimestamp(),
            DataObjectType = DataObjectType,
            CustomData = new Dictionary<string, v12.Datatypes.DataValue>(),
        };

        public v11.Datatypes.Object.DataObject DataObject11(bool includeData) => new v11.Datatypes.Object.DataObject
        {
            Data = includeData ? Encoding.UTF8.GetBytes(Xml(EtpVersion.v11)) : new byte[0],
            Resource = Resource11,
            ContentEncoding = ContentEncodings.Empty,
        };

        public v12.Datatypes.Object.DataObject DataObject12(bool includeData) => new v12.Datatypes.Object.DataObject
        {
            Data = includeData ? Encoding.UTF8.GetBytes(Xml(EtpVersion.v11)) : new byte[0],
            Resource = Resource12,
            Format = Formats.Xml,
            BlobId = null,
        };

        public v11.Datatypes.Object.ObjectChange ObjectChange11(bool includeData, v11.Datatypes.Object.ObjectChangeTypes changeType) => new v11.Datatypes.Object.ObjectChange
        {
            ChangeType = changeType,
            ChangeTime = StoreLastWrite.ToEtpTimestamp(),
            DataObject = DataObject11(includeData),
        };

        public v12.Datatypes.Object.ObjectChange ObjectChange12(bool includeData, v12.Datatypes.Object.ObjectChangeKind changeKind) => new v12.Datatypes.Object.ObjectChange
        {
            ChangeKind = changeKind,
            ChangeTime = StoreLastWrite.ToEtpTimestamp(),
            DataObject = DataObject12(includeData),
        };

        protected string Namespaces(bool embedded)
        {
            if (embedded)
                return string.Empty;

            return @" xmlns:gml=""http://www.opengis.net/gml/3.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xlink=""http://www.w3.org/1999/xlink"" xmlns:dc=""http://purl.org/dc/terms/""";
        }

        protected abstract string DefaultNamespace(bool embedded);
    }
}
