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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Energistics.Etp.Common.Datatypes
{
    /// <summary>
    /// Represents a URI supported by the Energistics Transfer Protocol (ETP).
    /// </summary>
    public partial struct EtpUri
    {
        private static readonly HashSet<string> EmlCommonObjects = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Activity",
            "ActivityTemplate",
            "DataAssuranceRecord",
            "EpcExternalPartReference",
            "GeodeticCrs",
            "GraphicalInformationSet",
            "ProjectedCrs",
            "PropertyKind",
            "PropertyKindDictionary",
            "TimeSeries",
            "VerticalCrs"
        };

        private readonly EtpUri?[] _parent;
        private readonly string[] _objectSegments;

        /// <summary>
        /// The root ETP 1.1 URI supported by the Discovery protocol.
        /// </summary>
        public static readonly EtpUri RootUri11 = new EtpUri("eml://");

        /// <summary>
        /// The root ETP 1.2 URI supported by the Discovery protocol.
        /// </summary>
        public static readonly EtpUri RootUri12 = new EtpUri("eml:///");

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpUri"/> struct.
        /// </summary>
        /// <param name="uri">The URI string.</param>
        public EtpUri(string uri)
            : this()
        {
            var uriMatch = Definition.EtpUriRegex.Match(uri);
            _parent = new EtpUri?[] { null };

            Uri = uri;
            
            IsValid = uriMatch.Success;

            if (!IsValid)
                return;

            if (WasGroupMatched(uriMatch, Definition.Etp11UriGroup))
                EtpVersion = EtpVersion.v11;
            else
                EtpVersion = EtpVersion.v12;

            Query = GetFirstMatch(uriMatch, Definition.QueryGroup) ?? string.Empty;
            Hash = GetFirstMatch(uriMatch, Definition.HashGroup) ?? string.Empty;

            Dataspace = GetFirstMatch(uriMatch, Definition.DataspaceGroup) ?? string.Empty;

            var family = GetFirstMatch(uriMatch, Definition.FamilyGroup);
            NamespaceFamily = EtpDataObjectType.TryGetFamily(family);

            var shortVersion = GetFirstMatch(uriMatch, Definition.ShortVersionGroup);
            NamespaceVersion = EtpDataObjectType.TryGetFamilyVersionFromShortVersion(family, shortVersion);

            _objectSegments = GetAllMatches(uriMatch, Definition.ObjectOrFolderGroup);
            if (_objectSegments != null)
            {
                var segment = CreateSegment(_objectSegments[_objectSegments.Length - 1], NamespaceFamily, NamespaceVersion);

                Family = segment.Family;
                Version = segment.Version;

                ObjectType = segment.ObjectType;
                ObjectId = segment.ObjectId;
                ObjectVersion = segment.ObjectVersion;
            }
            else
            {
                Family = NamespaceFamily;
                Version = NamespaceVersion;
            }

            DataObjectType = new EtpDataObjectType(Family, Version, ObjectType);

            Format = GetQueryStringFormat(Query, EtpContentType.Xml);

            ContentType = new EtpContentType(Family, Version, ObjectType, Format);

            IsRootUri = WasGroupMatched(uriMatch, Definition.RootUriGroup);
            IsDataspaceUri = IsRootUri /* Default Dataspace */ || WasGroupMatched(uriMatch, Definition.DataspaceUriGroup);
            IsFamilyVersionUri = WasGroupMatched(uriMatch, Definition.FamilyVersionUriGroup);

            IsBaseUri = IsRootUri || IsDataspaceUri || IsFamilyVersionUri;

            IsQueryUri = WasGroupMatched(uriMatch, Definition.QueryUriGroup);
            IsObjectUri = WasGroupMatched(uriMatch, Definition.ObjectUriGroup);
            IsFolderUri = WasGroupMatched(uriMatch, Definition.FolderUriGroup);
            IsHierarchicalUri = WasGroupMatched(uriMatch, Definition.HierarchicalUriGroup) || WasGroupMatched(uriMatch, Definition.TemplateUriGroup);

            HasEmptyObjectId = WasGroupMatched(uriMatch, Definition.EmptyIdGroup);

            var canonical = WasGroupMatched(uriMatch, Definition.CanonicalUriGroup)
                                || (IsObjectUri && NamespaceVersion.StartsWith("1")); // Special case for 1.x URIs.

            if (WasGroupMatched(uriMatch, Definition.DataspaceGroup) && string.IsNullOrEmpty(Dataspace)) // Disallow eml:///dataspace()
                canonical = false;
            if (HasEmptyObjectId || WasGroupMatched(uriMatch, Definition.EmptyVersionGroup)) // Disallow empty object IDs or empty object versions
                canonical = false;

            IsCanonicalUri = canonical;

            IsAlternateUri = !IsCanonicalUri; // Allow for special cases.
            IsHierarchicalUri = WasGroupMatched(uriMatch, Definition.HierarchicalUriGroup);
            IsFolderTemplateUri = WasGroupMatched(uriMatch, Definition.FolderUriGroup) || WasGroupMatched(uriMatch, Definition.FolderTemplateUriGroup);
            IsObjectTemplateUri = WasGroupMatched(uriMatch, Definition.ObjectTemplateUriGroup);
            IsTemplateUri = IsFolderTemplateUri || IsObjectTemplateUri;

            if (IsBaseUri)
                UriBase = UriWithoutSuffix;
            else
                UriBase = GetFirstMatch(uriMatch, Definition.BaseGroup) + "/";
        }

        /// <summary>
        /// The ETP version for this URI.
        /// </summary>
        public EtpVersion EtpVersion { get; }

        /// <summary>
        /// Wheher or not this is an ETP 1.1 URI.
        /// </summary>
        public bool IsEtp11 => EtpVersion == EtpVersion.v11;

        /// <summary>
        /// Wheher or not this is an ETP 1.2 URI.
        /// </summary>
        public bool IsEtp12 => EtpVersion == EtpVersion.v12;

        /// <summary>
        /// Gets the original URI string.
        /// </summary>
        /// <value>The URI.</value>
        public string Uri { get; }

        /// <summary>
        /// Gets the URI's Base.
        /// </summary>
        public string UriBase { get; }

        /// <summary>
        /// Gets the URI without any query or hash suffix.
        /// </summary>
        public string UriWithoutSuffix
        {
            get
            {
                if (string.IsNullOrEmpty(Uri) || (string.IsNullOrEmpty(Query) && string.IsNullOrEmpty(Hash)))
                    return Uri;

                var suffixLength = (Query?.Length ?? 0) + (Hash?.Length ?? 0);
                return Uri.Substring(0, Uri.Length - suffixLength);
            }
        }

        /// <summary>
        /// Gets the data space.
        /// </summary>
        /// <value>The data space.</value>
        public string Dataspace { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is in the default dataspace.
        /// </summary>
        /// <value><c>true</c> if this instance is in the default dataspace; otherwise, <c>false</c>.</value>
        public bool IsDefaultDataspace => string.IsNullOrEmpty(Dataspace);

        /// <summary>
        /// Gets the ML family of the ETP 1.1 URI namespace
        /// </summary>
        public string NamespaceFamily { get; }

        /// <summary>
        /// Gets the ML family version of the ETP 1.1 URI namespace
        /// </summary>
        public string NamespaceVersion { get; }

        /// <summary>
        /// Gets the ML family name for the object specified by the URI.
        /// </summary>
        /// <value>The ML family.</value>
        public string Family { get; }

        /// <summary>
        /// Gets the family version for the object specified by the URI.
        /// </summary>
        /// <value>The version.</value>
        public string Version { get; }

        /// <summary>
        /// Gets the format.
        /// </summary>
        /// <value>The format.</value>
        public string Format { get; }

        /// <summary>
        /// Gets the type of the object.
        /// </summary>
        /// <value>The type of the object.</value>
        public string ObjectType { get; }

        /// <summary>
        /// Gets the object identifier.
        /// </summary>
        /// <value>The object identifier.</value>
        public string ObjectId { get; }

        /// <summary>
        /// Gets the object version.
        /// </summary>
        /// <value>The object version.</value>
        public string ObjectVersion { get; }

        /// <summary>
        /// Gets the query string.
        /// </summary>
        /// <value>The query string.</value>
        public string Query { get; }

        /// <summary>
        /// Gets the hash segment.
        /// </summary>
        /// <value>The hash segment.</value>
        public string Hash { get; }

        /// <summary>
        /// Gets the content type.
        /// </summary>
        /// <value>The type of the content.</value>
        public EtpContentType ContentType { get; }

        /// <summary>
        /// Gets the data object type.
        /// </summary>
        public EtpDataObjectType DataObjectType { get; }

        /// <summary>
        /// Returns true if a valid URI was specified.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public bool IsValid { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is the root URI.
        /// For ETP 1.1: eml://
        /// For ETP 1.2: eml:/ or eml:///
        /// </summary>
        /// <value><c>true</c> if this instance is the root URI; otherwise, <c>false</c>.</value>
        public bool IsRootUri { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is the a dataspace URI.
        /// For ETP 1.1: the root URI or eml://some/data/space
        /// For ETP 1.2: the root URI or eml:///dataspace(uid)
        /// </summary>
        /// <value><c>true</c> if this instance is the root URI; otherwise, <c>false</c>.</value>
        public bool IsDataspaceUri { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is an ETP 1.1 family version.
        /// For ETP 1.1: eml://witsml20 or eml://some/data/space/witsml20
        /// </summary>
        /// <value><c>true</c> if this instance is a family version URI; otherwise, <c>false</c>.</value>
        public bool IsFamilyVersionUri { get; }

        /// <summary>
        /// Gets a value indicating whether this instance uniquely identifies an object.
        /// For ETP 1.1: eml://witsml20/Well(UUID) or eml://witsml20/Well(UUID)/Wellbore(UUID)
        /// For ETP 1.2: eml://witsml20.Well(UUID) or eml://witsml20.Well(UUID)/witsml20.Wellbore(UUID)
        /// </summary>
        /// <value><c>true</c> if this instance uniquely identifies an object; otherwise, <c>false</c>.</value>
        public bool IsObjectUri { get; }

        /// <summary>
        /// A query URI has special meaning in ETP 1.2.  It is a folder URI at the Base or top-level object level.
        /// For ETP 1.1: eml://witsml20/Well or eml://witsml20/Well(UUID)/Wellbore
        /// For ETP 1.2: eml://witsml20.Well or eml://witsml20.Well(UUID)/witsml20.Wellbore
        /// </summary>
        /// <value><c>true</c> if this instance is a query URI; otherwise, <c>false</c>.</value>
        public bool IsQueryUri { get; }

        /// <summary>
        /// Gets a value indicating whether this instance represents a folder.  A folder is a Base,
        /// object or object hierarchy URI that ends with an object type.
        /// For ETP 1.1: eml://witsml20/Well or eml://witsml20/Well(UUID)/Wellbore(UUID)/Log
        /// For ETP 1.2: eml://witsml20.Well or eml://witsml20.Well(UUID)/witsml20.Wellbore(UUID)/Log
        /// </summary>
        /// <value><c>true</c> if this instance uniquely identifies an object; otherwise, <c>false</c>.</value>
        public bool IsFolderUri { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is a Base URI.
        /// A Base URI is a root URI, a dataspace URI or a family version URI.
        /// </summary>
        /// <value><c>true</c> if this instance is a Base URI; otherwise, <c>false</c>.</value>
        public bool IsBaseUri { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is a canonical URI.
        /// A canonical object URI contains only a data object.
        /// A canonical query URI contains one folder and may contain one data object and/or a query string.
        /// </summary>
        /// <value><c>true</c> if this instance is a canonical URI; otherwise, <c>false</c>.</value>
        public bool IsCanonicalUri { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is an alternate URI.
        /// There are several forms of supported alternate URIs:
        /// Canonical URIs with query strings and/or hashes
        /// Query URIs with hashes
        /// Hierarchical object URIs, optionally with query strings and/or hashes
        /// Template URIs (URIs with multiple folder segments), optionally with query strings and/or hashes
        /// </summary>
        /// <value><c>true</c> if this instance is an alternate URI; otherwise, <c>false</c>.</value>
        public bool IsAlternateUri { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is an alternate URI.
        /// Hierarchical object URIs have more than one path segment with an object defined.
        /// </summary>
        /// <value><c>true</c> if this instance is a hierarcical URI; otherwise, <c>false</c>.</value>
        public bool IsHierarchicalUri { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is a template URI.
        /// Template URIs have more than one path segment that is a folder.
        /// </summary>
        /// <value><c>true</c> if this instance is a template URI; otherwise, <c>false</c>.</value>
        public bool IsTemplateUri { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is an object template URI.
        /// Object template URIs are template URIs that end with an object.
        /// </summary>
        /// <value><c>true</c> if this instance is an object template URI; otherwise, <c>false</c>.</value>
        public bool IsObjectTemplateUri { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is an folder template URI.
        /// Folder template URIs are template URIs that end with a folder.
        /// </summary>
        /// <value><c>true</c> if this instance is a folder template URI; otherwise, <c>false</c>.</value>
        public bool IsFolderTemplateUri { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has a query.
        /// </summary>
        /// <value><c>true</c> if this instance has a query; <c>false</c> otherwise.</value>
        public bool HasQuery => !string.IsNullOrEmpty(Query);

        /// <summary>
        /// Gets a value indicating whether this instance has a hash.
        /// </summary>
        /// <value><c>true</c> if this instance has a hash; <c>false</c> otherwise.</value>
        public bool HasHash => !string.IsNullOrEmpty(Hash);

        /// <summary>
        /// Gets a value indicating whether this instance has a query or a hash.
        /// </summary>
        /// <value><c>true</c> if this instance has a query or a hash; <c>false</c> otherwise.</value>
        public bool HasQueryOrHash => HasQuery || HasHash;

        /// <summary>
        /// Gets a value indicating whether this instance has one or more empty object IDs.
        /// ETP 1.1 example: eml://witsml20/Well()
        /// </summary>
        /// <value><c>true</c> if this instance has one or more empty object IDs; <c>false</c> otherwise.</value>
        public bool HasEmptyObjectId { get; }

        /// <summary>
        /// Gets the parent URI.
        /// </summary>
        /// <value>The parent URI.</value>
        public EtpUri Parent
        {
            get
            {
                if (_parent[0] == null)
                    _parent[0] = CreateParent();

                return _parent[0].Value;
            }
        }

        /// <summary>
        /// Creates the parent URI.
        /// </summary>
        /// <returns>The parent URI.</returns>
        private EtpUri? CreateParent()
        {
            if (!IsValid || IsBaseUri)
                return this;

            var uri = UriWithoutSuffix;

            var rootLength = IsEtp11 ? RootUri11.Uri.Length : RootUri12.Uri.Length;

            var index = uri.LastIndexOf('/', uri.Length - 1, uri.Length - rootLength);
            if (index < 0)
                return IsEtp11 ? RootUri11 : RootUri12;
            else
                return new EtpUri(uri.Substring(0, index));
        }

        /// <summary>
        /// Determines whether this instance is related to the specified <see cref="EtpUri"/>.
        /// </summary>
        /// <param name="other">The other URI.</param>
        /// <returns>
        ///   <c>true</c> if the two <see cref="EtpUri"/> instances share the same family and
        ///   version; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRelatedTo(EtpUri other)
        {
            return string.Equals(Family, other.Family, StringComparison.InvariantCultureIgnoreCase)
                && string.Equals(Version, other.Version, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Gets a collection of <see cref="Segment"/> items.
        /// </summary>
        /// <returns>A collection of <see cref="Segment"/> items.</returns>
        public IEnumerable<Segment> GetObjectIds()
        {
            if (_objectSegments == null)
                yield break;

            for (int i = 0; i < _objectSegments.Length; i++)
            {
                var last = i == _objectSegments.Length - 1;
                yield return CreateSegment(_objectSegments[i], last ? Family : NamespaceFamily, last ? Version : NamespaceVersion);
            }
        }

        /// <summary>
        /// Creates a case-insensitive dictionary of object types and object identifiers.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyDictionary<string, string> GetObjectIdMap()
        {
            return GetObjectIds()
                .ToDictionary(x => x.ObjectType, x => x.ObjectId, StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Appends the specified object type and optional object identifier to the <see cref="EtpUri" />.
        /// </summary>
        /// <param name="family">The object family.</param>
        /// <param name="version">The object version.</param>
        /// <param name="objectType">The object type.</param>
        /// <param name="objectId">The object identifier.</param>
        /// <param name="objectVersion">The object version.</param>
        /// <param name="encode">if set to <c>true</c> encode the object identifier value.</param>
        /// <returns>A new <see cref="EtpUri" /> instance.</returns>
        public EtpUri Append(string family, string version, string objectType, string objectId = null, string objectVersion = null, bool encode = false)
        {
            objectType = EtpContentType.FormatObjectType(objectType, version);
            CorrectCommonObjectFamilyAndVersion(objectType, ref family, ref version);

            if (encode && objectId != null)
                objectId = WebUtility.UrlEncode(objectId);

            var uri = UriWithoutSuffix ?? string.Empty;
            string slash = null;
            if (uri.Length == 0 || uri[uri.Length - 1] != '/')
                slash = "/";

            if (IsEtp11)
            {
                if (objectId != null)
                    objectId = $"({objectId})";

                return new EtpUri($"{uri}{slash}{objectType}{objectId}{Query}{Hash}");
            }
            else
            {
                family = family ?? Family;
                version = version ?? Version;
                var shortVersion = EtpDataObjectType.TryGetFamilyShortVersionFromVersion(family, version);

                string objectIdAndVersion = null;
                if (objectId != null && objectVersion != null)
                    objectIdAndVersion = $"({objectId},{objectVersion})";
                else if (objectId != null)
                    objectIdAndVersion = $"({objectId})";

                return new EtpUri($"{uri}{slash}{family}{shortVersion}.{objectType}{objectIdAndVersion}{Query}{Hash}");
            }
        }

        /// <summary>
        /// Creates a Base URI for the specified ETP version with the specified family, version and dataspace.
        /// </summary>
        /// <param name="etpVersion">The ETP version.</param>
        /// <param name="family">The family.</param>
        /// <param name="version">The version.</param>
        /// <param name="dataspace">The dataspace.</param>
        /// <returns>The ETP URI Base.</returns>
        private static string CreateBaseUri(EtpVersion etpVersion, string family, string version, string dataspace)
        {
            var sb = new StringBuilder();
            var appendSlash = false;
            if (etpVersion == EtpVersion.v11)
                sb.Append($"{RootUri11.Uri}");
            else
                sb.Append($"{RootUri12.Uri}");

            if (!string.IsNullOrEmpty(dataspace))
            {
                if (etpVersion == EtpVersion.v11)
                    sb.Append(dataspace);
                else
                    sb.Append($"dataspace({dataspace})");

                appendSlash = true;
            }

            if (etpVersion == EtpVersion.v11 && !string.IsNullOrEmpty(family) && !string.IsNullOrEmpty(version))
            {
                if (appendSlash)
                    sb.Append("/");

                var shortVersion = EtpDataObjectType.TryGetFamilyShortVersionFromVersion(family, version);
                sb.Append($"{family}{shortVersion}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts the URI to an ETP 1.1 URI.
        /// </summary>
        /// <returns>The converted URI.</returns>
        public EtpUri AsEtp11()
        {
            if (IsEtp11 || !IsValid)
                return this;

            var Base = CreateBaseUri(EtpVersion.v11, NamespaceFamily, NamespaceVersion, Dataspace);

            var sb = new StringBuilder(Base);
            var appendSlash = Base[Base.Length - 1] != '/';

            foreach (var segment in GetObjectIds())
            {
                if (appendSlash)
                    sb.Append("/");
                sb.Append($"{segment.ObjectType}");

                if (segment.ObjectId != null)
                    sb.Append($"({segment.ObjectId})");
            }

            sb.Append(Query);
            sb.Append(Hash);

            return new EtpUri(sb.ToString());
        }

        /// <summary>
        /// Converts the URI to an ETP 1.2 URI.
        /// </summary>
        /// <returns>The converted URI.</returns>
        public EtpUri AsEtp12()
        {
            if (IsEtp12 || !IsValid)
                return this;

            var Base = CreateBaseUri(EtpVersion.v12, NamespaceFamily, NamespaceVersion, Dataspace);

            var sb = new StringBuilder(Base);
            var appendSlash = Base[Base.Length - 1] != '/';

            foreach (var segment in GetObjectIds())
            {
                if (appendSlash)
                    sb.Append("/");
                var shortVersion = EtpDataObjectType.TryGetFamilyShortVersionFromVersion(segment.Family, segment.Version);
                sb.Append($"{segment.Family}{shortVersion}.{segment.ObjectType}");
                if (!string.IsNullOrEmpty(segment.ObjectId))
                {
                    if (!string.IsNullOrEmpty(segment.ObjectVersion))
                        sb.Append($"({segment.ObjectId},{segment.ObjectVersion})");
                    else
                        sb.Append($"({segment.ObjectId})");
                }

                appendSlash = true;
            }

            sb.Append(Query);
            sb.Append(Hash);

            return new EtpUri(sb.ToString());
        }

        /// <summary>
        /// Converts the URI to a canonical URI or canonical query URI.
        /// </summary>
        /// <param name="encode">Whether or not to encode the object ID.</param>
        /// <returns>The URI as a canonical URI or canonical query URI.</returns>
        public EtpUri AsCanonical(bool encode = false)
        {
            if (!IsValid || IsCanonicalUri)
                return this;


            if (IsBaseUri) // Alternate Base URI
            {
                var @base = CreateBaseUri(EtpVersion, NamespaceFamily, NamespaceVersion, Dataspace);
                return new EtpUri(@base);
            }
            else if (ObjectId == null) // Query or Template URI
            {
                EtpUri canonical;
                var segments = GetObjectIds().ToList();
                if (segments.Count > 1 && segments[segments.Count - 2].ObjectId != null)
                {
                    var s = segments[segments.Count - 2];
                    var @base = CreateBaseUri(EtpVersion, s.Family, s.Version, Dataspace);
                    canonical = new EtpUri($"{@base}{Query}");
                    var objectVersion = string.IsNullOrEmpty(s.ObjectVersion) ? null : s.ObjectVersion;
                    canonical = canonical.Append(s.Family, s.Version, s.ObjectType, s.ObjectId, objectVersion, encode);
                }
                else
                {
                    var @base = CreateBaseUri(EtpVersion, Family, Version, Dataspace);
                    canonical = new EtpUri($"{@base}{Query}");
                }
                return canonical.Append(Family, Version, ObjectType, null, null, encode);
            }
            else // Object URI
            {
                var @base = CreateBaseUri(EtpVersion, Family, Version, Dataspace);
                var objectVersion = string.IsNullOrEmpty(ObjectVersion) ? null : ObjectVersion;
                return new EtpUri(@base).Append(Family, Version, ObjectType, ObjectId, objectVersion, encode);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Uri;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is EtpUri))
                return false;

            return Equals((EtpUri)obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="EtpUri" />, is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="EtpUri" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="EtpUri" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(EtpUri other)
        {
            return string.Equals(other, this, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Uri.ToLowerInvariant().GetHashCode();
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="EtpUri"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator string(EtpUri uri)
        {
            return uri.ToString();
        }

        /// <summary>
        /// Determines whether the specified URI is a root URI.
        /// </summary>
        /// <param name="uri">The URI string.</param>
        /// <returns><c>true</c> if the URI is a root URI; otherwise, <c>false</c>.</returns>
        public static bool IsRoot(string uri)
        {
            return Definition.EtpRootUriRegex.IsMatch(uri);
        }

        /// <summary>
        /// Gets the first matching value in the URI for the specified capture group.
        /// </summary>
        /// <param name="match">The regex match results.</param>
        /// <param name="groupName">The capture group name.</param>
        /// <returns>The first matching value if successfully matched; <c>null</c> if not matched.</returns>
        private static string GetFirstMatch(Match match, string groupName)
        {
            if (!match.Success)
                return null;
            var group = match.Groups[groupName];
            if (!group.Success)
                return null;

            return group.Captures[0].Value;
        }

        /// <summary>
        /// Gets an array of all matches in the URI for the specified capture group.
        /// </summary>
        /// <param name="match">The regex match results.</param>
        /// <param name="groupName">The capture group name.</param>
        /// <returns>The array of all matching value if successfully matched; <c>null</c> if not matched.</returns>
        private static string[] GetAllMatches(Match match, string groupName)
        {
            if (!match.Success)
                return null;
            var group = match.Groups[groupName];
            if (!group.Success)
                return null;

            var matches = new string[group.Captures.Count];
            for (int i = 0; i < group.Captures.Count; i++)
                matches[i] = group.Captures[i].Value;

            return matches;
        }

        /// <summary>
        /// Checks if the specified capture group was matched.
        /// </summary>
        /// <param name="match">The regex match results.</param>
        /// <param name="groupName">The capture group name.</param>
        /// <returns><c>true</c> if the group was matched; <c>false</c> otherwise.</returns>
        private static bool WasGroupMatched(Match match, string groupName)
        {
            if (!match.Success)
                return false;
            var group = match.Groups[groupName];
            return group.Success;
        }

        /// <summary>
        /// Gets the $format from the query string.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The query string $format parameter value.</returns>
        private static string GetQueryStringFormat(string queryString, string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(queryString))
                return defaultValue;

            try
            {
                var values = HttpUtility.ParseQueryString(queryString);
                var format = values[Definition.FormatParameter]?.Trim();

                return string.IsNullOrWhiteSpace(format) ? defaultValue : format;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Creates an ETP URI segment from the specified object string.
        /// </summary>
        /// <param name="object">The object string.</param>
        /// <param name="defaultFamily">The default family if the object does not specify it.</param>
        /// <param name="defaultVersion">The default family version if the object string does not specify it.</param>
        /// <returns></returns>
        private static Segment CreateSegment(string @object, string defaultFamily, string defaultVersion)
        {
            var match = Definition.EtpObjectOrFolderRegex.Match(@object);

            var objectType = GetFirstMatch(match, Definition.ObjectTypeGroup);
            var objectId = GetFirstMatch(match, Definition.ObjectIdGroup);
            if (objectId != null)
                objectId = WebUtility.UrlDecode(objectId);
            var objectVersion = GetFirstMatch(match, Definition.ObjectVersionGroup);

            CorrectCommonObjectFamilyAndVersion(objectType, ref defaultFamily, ref defaultVersion);

            var family = GetFirstMatch(match, Definition.FamilyGroup) ?? defaultFamily;
            family = EtpDataObjectType.TryGetFamily(family);

            var shortVersion = GetFirstMatch(match, Definition.ShortVersionGroup);
            var version = EtpDataObjectType.TryGetFamilyVersionFromShortVersion(family, shortVersion) ?? defaultVersion;

            objectType = EtpContentType.FormatObjectType(objectType, version);

            return new Segment(family, version, objectType, objectId, objectVersion);
        }

        /// <summary>
        /// Ensures that EML Common objects have the correct family and version.
        /// </summary>
        /// <param name="family">The input and output family.</param>
        /// <param name="version">The input and output version.</param>
        private static void CorrectCommonObjectFamilyAndVersion(string objectType, ref string family, ref string version)
        {
            if (string.IsNullOrEmpty(objectType))
                return;

            if (!EmlCommonObjects.Contains(objectType))
                return;

            if (string.Equals(family, "eml", StringComparison.OrdinalIgnoreCase))
                return;

            family = "eml";
            version = "2.1";
        }

        /// <summary>
        /// Represents an <see cref="EtpUri"/> path segment containing an
        /// object type and an optional object identifier (e.g. UUID or UID).
        /// </summary>
        public struct Segment
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Segment"/> struct.
            /// </summary>
            /// <param name="family"> The domain/family.</param>
            /// <param name="version">The domain/family version.</param>
            /// <param name="objectType">The object type.</param>
            /// <param name="objectId">The object identifier.</param>
            /// <param name="objectVersion">The object version.</param>
            public Segment(string family, string version, string objectType, string objectId, string objectVersion)
            {
                Family = family;
                Version = version;
                ObjectType = objectType;
                ObjectId = objectId;
                ObjectVersion = objectVersion;
            }

            /// <summary>
            /// Gets the ML domain name.
            /// </summary>
            /// <value>The ML family.</value>
            public string Family { get; }

            /// <summary>
            /// Gets the family version.
            /// </summary>
            /// <value>The version.</value>
            public string Version { get; }

            /// <summary>
            /// Gets the type of the object.
            /// </summary>
            /// <value>The type of the object.</value>
            public string ObjectType { get; }

            /// <summary>
            /// Gets the object identifier.
            /// </summary>
            /// <value>The object identifier.</value>
            public string ObjectId { get; }

            /// <summary>
            /// Gets the object version.
            /// </summary>
            /// <value>The version.</value>
            public string ObjectVersion { get; }
        }
    }
}
