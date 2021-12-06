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

        public bool IsActive { get; private set; }

        public DateTime LastActivatedTime { get; private set; } = 0L.ToUtcDateTime();

        public DateTime ActiveChangeTime { get; private set; } = 0L.ToUtcDateTime();

        public DateTime LastJoinedTime { get; private set; } = 0L.ToUtcDateTime();

        public DateTime LastUnjoinedTime { get; private set; } = 0L.ToUtcDateTime();

        public Dictionary<Guid, MockObject> Parents { get; private set; } = new Dictionary<Guid, MockObject>();

        public Dictionary<Guid, MockObject> Children { get; private set; } = new Dictionary<Guid, MockObject>();

        public Dictionary<Guid, MockObject> Containers { get; private set; } = new Dictionary<Guid, MockObject>();

        public Dictionary<Guid, MockObject> Containees { get; private set; } = new Dictionary<Guid, MockObject>();

        public Dictionary<Guid, MockObject> PrimarySources { get; private set; } = new Dictionary<Guid, MockObject>();

        public Dictionary<Guid, MockObject> PrimaryTargets { get; private set; } = new Dictionary<Guid, MockObject>();

        public Dictionary<Guid, MockObject> SecondarySources { get; private set; } = new Dictionary<Guid, MockObject>();

        public Dictionary<Guid, MockObject> SecondaryTargets { get; private set; } = new Dictionary<Guid, MockObject>();

        public abstract HashSet<EtpDataObjectType> SupportedSourceTypes { get; }
        public abstract HashSet<EtpDataObjectType> SupportedTargetTypes { get; }
        public abstract HashSet<EtpDataObjectType> SupportedSecondarySourceTypes { get; }
        public abstract HashSet<EtpDataObjectType> SupportedSecondaryTargetTypes { get; }


        public abstract string Xml(EtpVersion version, string indentation = "", bool embedded = false);

        public EtpUri Uri(EtpVersion version) => new EtpUri(version, DataObjectType, dataspace: Dataspace?.Name, objectId: Uuid.ToString());

        public void ClearLinks()
        {
            PrimarySources.Clear();
            SecondarySources.Clear();
            PrimaryTargets.Clear();
            SecondaryTargets.Clear();
        }

        public virtual void Link()
        {
            if (IsDeleted) return;

            foreach (var parent in Parents.Values)
            {
                if (parent.IsDeleted)
                    continue;
                PrimaryTargets[parent.Uuid] = parent;
                parent.PrimarySources[Uuid] = this;
            }
            foreach (var child in Children.Values)
            {
                if (child.IsDeleted)
                    continue;
                PrimarySources[child.Uuid] = child;
                child.PrimaryTargets[Uuid] = this;
            }
            foreach (var container in Containers.Values)
            {
                if (container.IsDeleted)
                    continue;
                PrimaryTargets[container.Uuid] = container;
                container.PrimaryTargets[Uuid] = this;
            }
            foreach (var containee in Containees.Values)
            {
                if (containee.IsDeleted)
                    continue;
                PrimarySources[containee.Uuid] = containee;
                containee.PrimarySources[Uuid] = this;
            }
        }

        public void Touch(DateTime touchTime)
        {
            LastUpdate = touchTime;
            UpdateObjectLastWrite(touchTime);
            foreach (var container in Containers.Values)
                container.Touch(touchTime);
        }

        public virtual void Create(DateTime createdTime)
        {
            StoreCreated = createdTime;
            IsActive = false;
            LastJoinedTime = LastUnjoinedTime = ActiveChangeTime = LastActivatedTime = StoreCreated;
            UpdateObjectLastWrite(createdTime);
            UpdateDataLastWrite(createdTime);
        }

        public Dictionary<Guid, MockObject> Delete(DateTime deleteTime, bool pruneContainedDataObjects = false)
        {
            if (IsDeleted)
                return new Dictionary<Guid, MockObject>();

            var deleted = new Dictionary<Guid, MockObject>() { [Uuid] = this };

            IsDeleted = true;
            UpdateObjectLastWrite(deleteTime);
            var containers = Containers.Values.ToList();
            foreach (var container in containers)
            {
                UnjoinContainer(container, deleteTime);
            }
            var containees = Containees.Values.ToList();
            foreach (var containee in containees)
            {
                UnjoinContainee(containee, deleteTime);
                if (pruneContainedDataObjects && containee.Containers.Count == 0)
                {
                    var d = containee.Delete(deleteTime, pruneContainedDataObjects: pruneContainedDataObjects);
                    foreach (var kvp in d)
                        deleted[kvp.Key] = kvp.Value;
                }
            }

            return deleted;
        }

        public Dictionary<Guid, MockObject> Restore(DateTime createTime)
        {
            if (!IsDeleted)
                return new Dictionary<Guid, MockObject>();

            var restored = new Dictionary<Guid, MockObject>() { [Uuid] = this };

            IsDeleted = false;
            Create(createTime);
            foreach (var containee in Containees.Values)
            {
                var r = containee.Restore(createTime);

                foreach (var kvp in r)
                    restored[kvp.Key] = kvp.Value;
            }

            return restored;
        }

        public void UnjoinContainer(MockObject container, DateTime unjoinTime)
        {
            if (container == null || !Containers.ContainsKey(container.Uuid))
                return;

            Containers.Remove(container.Uuid);
            container.UnjoinContainee(this, unjoinTime);
            LastUnjoinedTime = unjoinTime;
            UpdateObjectLastWrite(unjoinTime);
        }

        protected virtual void UnjoinContainee(MockObject containee, DateTime unjoinTime)
        {
            if (containee == null || !Containees.ContainsKey(containee.Uuid))
                return;

            Containees.Remove(containee.Uuid);
            containee.UnjoinContainer(this, unjoinTime);
            UpdateObjectLastWrite(unjoinTime);
        }

        public void JoinContainer(MockObject container, DateTime joinTime)
        {
            if (container == null || Containers.ContainsKey(container.Uuid))
                return;

            Containers[container.Uuid] = container;
            container.JoinContainee(this, joinTime);
            LastJoinedTime = joinTime;
            UpdateObjectLastWrite(joinTime);
        }

        protected virtual void JoinContainee(MockObject containee, DateTime joinTime)
        {
            if (containee == null || Containees.ContainsKey(containee.Uuid))
                return;

            Containees[containee.Uuid] = containee;
            containee.JoinContainer(this, joinTime);
            UpdateObjectLastWrite(joinTime);
        }

        protected virtual void AddChild(MockObject child, DateTime updateTime, bool relationshipOnChild)
        {
            if (child == null || Children.ContainsKey(child.Uuid))
                return;

            Children[child.Uuid] = child;
            child.AddParent(this, updateTime, relationshipOnChild);
            if (!relationshipOnChild)
                UpdateObjectLastWrite(updateTime);
        }

        protected virtual void AddParent(MockObject parent, DateTime updateTime, bool relationshipOnChild)
        {
            if (parent == null || Parents.ContainsKey(parent.Uuid))
                return;

            Parents[parent.Uuid] = parent;
            parent.AddChild(this, updateTime, relationshipOnChild);
            if (relationshipOnChild)
                UpdateObjectLastWrite(updateTime);
        }

        protected virtual void RemoveChild(MockObject child, DateTime updateTime, bool relationshipOnChild)
        {
            if (child == null || !Children.ContainsKey(child.Uuid))
                return;

            Children.Remove(child.Uuid);
            child.RemoveParent(this, updateTime, relationshipOnChild);
            if (!relationshipOnChild)
                UpdateObjectLastWrite(updateTime);
        }

        protected virtual void RemoveParent(MockObject parent, DateTime updateTime, bool relationshipOnChild)
        {
            if (parent == null || !Parents.ContainsKey(parent.Uuid))
                return;

            Parents.Remove(parent.Uuid);
            parent.RemoveChild(this, updateTime, relationshipOnChild);
            if (relationshipOnChild)
                UpdateObjectLastWrite(updateTime);
        }

        protected void UpdateObjectLastWrite(DateTime lastWrite)
        {
            ObjectLastWrite = lastWrite;
            foreach (var container in Containers.Values)
                container.UpdateObjectLastWrite(lastWrite);
        }

        protected virtual void UpdateDataLastWrite(DateTime lastWrite)
        {
            DataLastWrite = lastWrite;
            foreach (var container in Containers.Values)
                container.UpdateDataLastWrite(lastWrite);
        }

        public void SetActive(bool active, DateTime activeChangeTime)
        {
            LastActivatedTime = activeChangeTime;
            if (IsActive == active)
                return;

            IsActive = active;
            ActiveChangeTime = activeChangeTime;
            UpdateObjectLastWrite(activeChangeTime);
        }

        public IEnumerable<EtpUri> AlternateUris(EtpVersion version)
        {
            if (Parents.Count == 0 && Containers.Count == 0)
                return Enumerable.Empty<EtpUri>();

            return ConstructAlternateUris(this, version);
        }

        public IEnumerable<MockObject> SourcesAndTargets(MockGraphContext context)
        {
            if (context.IncludeSources)
            {
                if (context.NavigatePrimaryEdges)
                {
                    foreach (var @object in PrimarySources.Values)
                        yield return @object;
                }
                if (context.NavigateSecondaryEdges)
                {
                    foreach (var @object in SecondarySources.Values)
                        yield return @object;
                }
            }
            if (context.IncludeTargets)
            {
                if (context.NavigatePrimaryEdges)
                {
                    foreach (var @object in PrimaryTargets.Values)
                        yield return @object;
                }
                if (context.NavigateSecondaryEdges)
                {
                    foreach (var @object in SecondaryTargets.Values)
                        yield return @object;
                }
            }
        }

        public IEnumerable<MockObject> SecondarySourcesAndTargets(MockGraphContext context)
        {
            if (context.IncludeSecondarySources)
            {
                foreach (var @object in SecondarySources.Values)
                    yield return @object;
            }
            if (context.IncludeSecondaryTargets)
            {
                foreach (var @object in SecondaryTargets.Values)
                    yield return @object;
            }
        }

        public IEnumerable<EtpDataObjectType> SupportedPrimarySourceAndTargetTypes(MockGraphContext context)
        {
            if (context.IncludeSources)
            {
                if (context.NavigatePrimaryEdges)
                {
                    foreach (var dataObjectType in SupportedSourceTypes)
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
            }
        }

        public IEnumerable<EtpDataObjectType> SupportedSecondarySourceAndTargetTypes(MockGraphContext context)
        {
            if (context.IncludeSources)
            {
                if (context.NavigateSecondaryEdges)
                {
                    foreach (var dataObjectType in SupportedSecondarySourceTypes)
                        yield return dataObjectType;
                }
            }
            if (context.IncludeTargets)
            {
                if (context.NavigateSecondaryEdges)
                {
                    foreach (var dataObjectType in SupportedSecondaryTargetTypes)
                        yield return dataObjectType;
                }
            }
        }
        private static IEnumerable<EtpUri> ConstructAlternateUris(MockObject @object, EtpVersion version)
        {
            if (@object.Parents.Count == 0 && @object.Containers.Count == 0)
            {
                yield return @object.Uri(version);
            }
            else
            {
                if (@object.Parents.Count != 0)
                {
                    foreach (var parent in @object.Parents.Values)
                    {
                        foreach (var uri in ConstructAlternateUris(parent, version))
                            yield return uri.Append(@object.DataObjectType, objectId: @object.Uuid.ToString());
                    }
                }
                if (@object.Containers.Count != 0)
                {
                    foreach (var container in @object.Containers.Values)
                    {
                        foreach (var uri in ConstructAlternateUris(container, version))
                            yield return uri.Append(@object.DataObjectType, objectId: @object.Uuid.ToString());
                    }
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

        public v11.Datatypes.Object.Resource Resource11(bool includeCounts) => new v11.Datatypes.Object.Resource
        {
            Uuid = Uuid,
            Uri = Uri(EtpVersion.v11),
            Name = Title,
            HasChildren = (includeCounts && (SupportedSourceTypes.Count + SupportedSecondarySourceTypes.Count) > 0) ? -1 : 0,
            ContentType = ContentType,
            ResourceType = ResourceTypes.DataObject.ToString(),
            CustomData = new Dictionary<string, string>(),
            ChannelSubscribable = true,
            ObjectNotifiable = true,
            LastChanged = LastUpdate,
        };

        public v12.Datatypes.Object.Resource Resource12(bool includeCounts) => new v12.Datatypes.Object.Resource
        {
            Uri = Uri(EtpVersion.v12),
            AlternateUris = AlternateUris(EtpVersion.v12).Select(uri => uri.Uri).ToList(),
            Name = Title,
            SourceCount = includeCounts ? PrimarySources.Count + SecondarySources.Count : null,
            TargetCount = includeCounts ? PrimaryTargets.Count + SecondaryTargets.Count : null,
            LastChanged = LastUpdate,
            StoreCreated = StoreCreated,
            StoreLastWrite = StoreLastWrite,
            ActiveStatus = IsActive ? v12.Datatypes.Object.ActiveStatusKind.Active : v12.Datatypes.Object.ActiveStatusKind.Inactive,
            CustomData = new Dictionary<string, v12.Datatypes.DataValue>(),
        };

        public v12.Datatypes.Object.DeletedResource DeletedResource12 => new v12.Datatypes.Object.DeletedResource
        {
            Uri = Uri(EtpVersion.v12),
            DeletedTime = StoreLastWrite,
            CustomData = new Dictionary<string, v12.Datatypes.DataValue>(),
        };

        public v11.Datatypes.Object.DataObject DataObject11(bool includeData) => new v11.Datatypes.Object.DataObject
        {
            Data = includeData ? Encoding.UTF8.GetBytes(Xml(EtpVersion.v11)) : new byte[0],
            Resource = Resource11(false),
            ContentEncoding = ContentEncodings.Empty,
        };

        public v12.Datatypes.Object.DataObject DataObject12(bool includeData) => new v12.Datatypes.Object.DataObject
        {
            Data = includeData ? Encoding.UTF8.GetBytes(Xml(EtpVersion.v11)) : new byte[0],
            Resource = Resource12(false),
            Format = Formats.Xml,
            BlobId = null,
        };

        public v11.Datatypes.Object.ObjectChange ObjectChange11(bool includeData, v11.Datatypes.Object.ObjectChangeTypes changeType) => new v11.Datatypes.Object.ObjectChange
        {
            ChangeType = changeType,
            ChangeTime = StoreLastWrite,
            DataObject = DataObject11(includeData),
        };

        public v12.Datatypes.Object.ObjectChange ObjectChange12(bool includeData, v12.Datatypes.Object.ObjectChangeKind changeKind) => new v12.Datatypes.Object.ObjectChange
        {
            ChangeKind = changeKind,
            ChangeTime = StoreLastWrite,
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
