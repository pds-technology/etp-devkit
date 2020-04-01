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
    public struct EtpUri
    {
        public static class Definition
        {
            public static readonly string FormatParameter =                   "$format";

            public static readonly string FamilyGroup =                       "family";
            public static readonly string ShortVersionGroup =                 "shortVersion";
            public static readonly string ObjectTypeGroup =                   "objectType";
            public static readonly string ObjectIdGroup =                     "objectId";
            public static readonly string ObjectVersionGroup =                "objectVersion";
            public static readonly string ObjectGroup =                       "object";
            public static readonly string QueryGroup =                        "query";
            public static readonly string HashGroup =                         "hash";
            public static readonly string DataspaceGroup =                    "dataspace";
            public static readonly string ObjectUriGroup =                    "objectUri";
            public static readonly string PrefixUriGroup =                    "prefixUri";
            public static readonly string CanonicalUriGroup =                 "canonicalUri";
            public static readonly string QueryUriGroup =                     "queryUri";
            public static readonly string ObjectHierarchyUriGroup =           "objectHierarchyUri";
            public static readonly string TemplateUriGroup =                  "templateUri";
            public static readonly string AlternateUriGroup =                 "alternateUri";
            public static readonly string Etp11UriGroup =                     "etp11Uri";
            public static readonly string Etp12UriGroup =                     "etp12Uri";

            // URI Components

            private static readonly string FamilyAndShortVersion =            $@"(?:(?<{FamilyGroup}>witsml|resqml|prodml|eml)(?<{ShortVersionGroup}>\d\d))";

            private static readonly string ObjectType =                       $@"(?:(?:obj_|cs_|part_)?(?<{ObjectTypeGroup}>\w+))";
            private static readonly string ObjectId =                         $@"(?:\((?<{ObjectIdGroup}>[^ ),]+)\))";
            private static readonly string ObjectIdAndVersion =               $@"(?:\((?<{ObjectIdGroup}>[^ ),]+)(?:,(?<{ObjectVersionGroup}>[^)]*))?\))";

            private static readonly string Query =                            $@"(?:(?<{QueryGroup}>\?[^#]*))";
            private static readonly string Hash =                             $@"(?:(?<{HashGroup}>#.*))";
            private static readonly string Suffix =                           $@"(?:(?:{Query})?(?:{Hash})?)";

            private static readonly string Etp11Root =                        $@"(?:eml:/)";
            private static readonly string Etp12Root =                        $@"(?:eml:|eml://)";

            private static readonly string Etp11Dataspace =                   $@"(?:(?!witsml|resqml|prodml|eml)(?<{DataspaceGroup}>[^/]+(?:/(?!witsml|resqml|prodml|eml)[^/]+)*))";
            private static readonly string Etp12Dataspace =                   $@"(?:dataspace\((?<{DataspaceGroup}>[^)]+)\))";

            private static readonly string Etp11Object =                      $@"(?<{ObjectGroup}>{ObjectType}{ObjectId})";
            private static readonly string Etp12Object =                      $@"(?<{ObjectGroup}>{FamilyAndShortVersion}\.{ObjectType}{ObjectIdAndVersion})";

            private static readonly string Etp11Prefix =                      $@"(?:{Etp11Root}(?:/{Etp11Dataspace})?/{FamilyAndShortVersion})";
            private static readonly string Etp12Prefix =                      $@"(?:{Etp12Root}(?:/{Etp12Dataspace})?)";

            private static readonly string Etp11PrefixWithObject =            $@"(?:{Etp11Prefix}/{Etp11Object})";
            private static readonly string Etp12PrefixWithObject =            $@"(?:{Etp12Prefix}/{Etp12Object})";

            private static readonly string Etp11Folder =                      $@"(?<{ObjectGroup}>{ObjectType})";
            private static readonly string Etp12Folder =                      $@"(?<{ObjectGroup}>{FamilyAndShortVersion}\.{ObjectType})";

            private static readonly string Etp11Query =                       $@"(?:(?:{Etp11Prefix}/{Etp11Folder}|{Etp11PrefixWithObject}/{Etp11Folder}){Query}?)";
            private static readonly string Etp12Query =                       $@"(?:(?:{Etp12Prefix}/{Etp12Folder}|{Etp12PrefixWithObject}/{Etp12Folder}){Query}?)";

            private static readonly string Etp11ObjectHierarchy =             $@"(?:{Etp11PrefixWithObject}(?:/{Etp11Object})+)";
            private static readonly string Etp12ObjectHierarchy =             $@"(?:{Etp12PrefixWithObject}(?:/{Etp12Object})+)";

            private static readonly string Etp11Template =                    $@"(?:{Etp11Folder}(?:/{Etp11Folder})+)";
            private static readonly string Etp12Template =                    $@"(?:{Etp12Folder}(?:/{Etp12Folder})+)";

            private static readonly string Etp11ObjectWithTemplate =          $@"(?:{Etp11PrefixWithObject}/{Etp11Template})";
            private static readonly string Etp12ObjectWithTemplate =          $@"(?:{Etp12PrefixWithObject}/{Etp12Template})";

            private static readonly string Etp11ObjectHierarchyWithTemplate = $@"(?:{Etp11ObjectHierarchy}(?:/{Etp11Folder})+)";
            private static readonly string Etp12ObjectHierarchyWithTemplate = $@"(?:{Etp12ObjectHierarchy}(?:/{Etp12Folder})+)";

            private static readonly string Etp11TemplateOnly =                $@"(?:{Etp11Prefix}/{Etp11Template})";
            private static readonly string Etp12TemplateOnly =                $@"(?:{Etp12Prefix}/{Etp12Template})";

            private static readonly string Etp11ArbitraryTemplate =           $@"(?:{Etp11Prefix}(?:/(?:{Etp11Folder}|{Etp11Object}))+)";
            private static readonly string Etp12ArbitraryTemplate =           $@"(?:{Etp12Prefix}(?:/(?:{Etp12Folder}|{Etp12Object}))+)";

            // Root URIs

            private static readonly string Etp11RootUri =                     $@"(?:^{Etp11Root}/$)";
            private static readonly string Etp12RootUri =                     $@"(?:^{Etp12Root}/$)";

            // Canonical URIs

            private static readonly string Etp11CanonicalPrefixUri =          $@"(?<{PrefixUriGroup}>^{Etp11Root}(?:/|(?:/{Etp11Dataspace}|/{FamilyAndShortVersion}|/{Etp11Dataspace}/{FamilyAndShortVersion})/?)$)";
            private static readonly string Etp12CanonicalPrefixUri =          $@"(?<{PrefixUriGroup}>^{Etp12Root}(?:/|/{Etp12Dataspace}/?)$)";

            private static readonly string Etp11CanonicalObjectUri =          $@"(?<{ObjectUriGroup}>^{Etp11PrefixWithObject}$)";
            private static readonly string Etp12CanonicalObjectUri =          $@"(?<{ObjectUriGroup}>^{Etp12PrefixWithObject}$)";

            private static readonly string Etp11CanonicalUri =                $@"(?<{CanonicalUriGroup}>{Etp11CanonicalPrefixUri}|{Etp11CanonicalObjectUri})";
            private static readonly string Etp12CanonicalUri =                $@"(?<{CanonicalUriGroup}>{Etp12CanonicalPrefixUri}|{Etp12CanonicalObjectUri})";

            // Query URIs

            private static readonly string Etp11QueryUri =                    $@"(?<{QueryUriGroup}>^{Etp11Query}$)";
            private static readonly string Etp12QueryUri =                    $@"(?<{QueryUriGroup}>^{Etp12Query}$)";

            // Alternate URIs

            private static readonly string Etp11QueryWithSuffixUri =          $@"(?:^{Etp11Query}{Hash}$)";
            private static readonly string Etp12QueryWithSuffixUri =          $@"(?:^{Etp12Query}{Hash}$)";

            private static readonly string Etp11ObjectWithSuffixUri =         $@"(?<{ObjectUriGroup}>^(?:{Etp11PrefixWithObject}{Query}{Hash}?|{Etp11PrefixWithObject}{Hash})$)";
            private static readonly string Etp12ObjectWithSuffixUri =         $@"(?<{ObjectUriGroup}>^(?:{Etp12PrefixWithObject}{Query}{Hash}?|{Etp12PrefixWithObject}{Hash})$)";

            private static readonly string Etp11ObjectHierarchyWithSuffixUri= $@"(?<{ObjectHierarchyUriGroup}>^{Etp11ObjectHierarchy}{Suffix}?$)";
            private static readonly string Etp12ObjectHierarchyWithSuffixUri= $@"(?<{ObjectHierarchyUriGroup}>^{Etp12ObjectHierarchy}{Suffix}{Suffix}?$)";

            private static readonly string Etp11PrefixWithTemplate =          $@"(?:(?<{ObjectHierarchyUriGroup}>{Etp11ObjectHierarchyWithTemplate})|{Etp11ObjectWithTemplate}|{Etp11TemplateOnly}|{Etp11ArbitraryTemplate})";
            private static readonly string Etp12PrefixWithTemplate =          $@"(?:(?<{ObjectHierarchyUriGroup}>{Etp12ObjectHierarchyWithTemplate})|{Etp12ObjectWithTemplate}|{Etp12TemplateOnly}|{Etp12ArbitraryTemplate})";

            private static readonly string Etp11TemplateWithSuffixUri =       $@"(?<{TemplateUriGroup}>^{Etp11PrefixWithTemplate}{Suffix}$)";
            private static readonly string Etp12TemplateWithSuffixUri =       $@"(?<{TemplateUriGroup}>^{Etp12PrefixWithTemplate}{Suffix}$)";

            private static readonly string Etp11AlternateUri =                $@"(?<{AlternateUriGroup}>^(?:{Etp11QueryWithSuffixUri}|{Etp11ObjectWithSuffixUri}|{Etp11ObjectHierarchyWithSuffixUri}|{Etp11TemplateWithSuffixUri})$)";
            private static readonly string Etp12AlternateUri =                $@"(?<{AlternateUriGroup}>^(?:{Etp12QueryWithSuffixUri}|{Etp12ObjectWithSuffixUri}|{Etp12ObjectHierarchyWithSuffixUri}|{Etp12TemplateWithSuffixUri})$)";

            // Complete Patterns

            private static readonly string EtpRootUri =                       $@"(?:{Etp11RootUri}|{Etp12RootUri})";

            private static readonly string Etp11Uri =                         $@"(?<{Etp11UriGroup}>{Etp11CanonicalUri}|{Etp11QueryUri}|{Etp11AlternateUri})";
            private static readonly string Etp12Uri =                         $@"(?<{Etp12UriGroup}>{Etp12CanonicalUri}|{Etp12QueryUri}|{Etp12AlternateUri})";

            private static readonly string EtpUri =                           $@"(?:{Etp11Uri}|{Etp12Uri})";

            // Objects and Folders

            private static readonly string Etp11ObjectOrFolder =              $@"(?:^(?:{Etp11Object}|{Etp11Folder})$)";
            private static readonly string Etp12ObjectOrFolder =              $@"(?:^(?:{Etp12Object}|{Etp12Folder})$)";

            private static readonly string EtpObjectOrFolder =                $@"(?:{Etp11ObjectOrFolder}|{Etp12ObjectOrFolder})";

            // Regexes

            public static readonly Regex EtpRootUriRegex = new Regex(EtpRootUri, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            public static readonly Regex EtpUriRegex = new Regex(EtpUri, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

            public static readonly Regex EtpObjectOrFolderRegex = new Regex(EtpObjectOrFolder, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        }

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

            IsRoot = IsRootUri(uri);
            IsPrefix = !WasGroupMatched(uriMatch, Definition.ObjectGroup);
            IsCanonical = WasGroupMatched(uriMatch, Definition.CanonicalUriGroup);
            IsCanonicalQuery = WasGroupMatched(uriMatch, Definition.QueryUriGroup);
            IsAlternate = WasGroupMatched(uriMatch, Definition.AlternateUriGroup);
            IsHierarchical = WasGroupMatched(uriMatch, Definition.ObjectHierarchyUriGroup);
            IsTemplate = WasGroupMatched(uriMatch, Definition.TemplateUriGroup) || IsCanonicalQuery;

            Query = GetFirstMatch(uriMatch, Definition.QueryGroup) ?? string.Empty;
            Hash = GetFirstMatch(uriMatch, Definition.HashGroup) ?? string.Empty;

            Dataspace = GetFirstMatch(uriMatch, Definition.DataspaceGroup) ?? string.Empty;

            var family = GetFirstMatch(uriMatch, Definition.FamilyGroup);
            NamespaceFamily = EtpDataObjectType.TryGetFamily(family);

            var shortVersion = GetFirstMatch(uriMatch, Definition.ShortVersionGroup);
            NamespaceVersion = EtpDataObjectType.TryGetFamilyVersionFromShortVersion(family, shortVersion);

            _objectSegments = GetAllMatches(uriMatch, Definition.ObjectGroup);
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
        public bool IsRoot { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is a prefix URI.
        /// A prefix URI does not contain a data object or folder but may contain a supported version and/or dataspace.
        /// </summary>
        /// <value><c>true</c> if this instance is a prefix URI; otherwise, <c>false</c>.</value>
        public bool IsPrefix { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is a canonical URI.
        /// A canonical URI contains only a data object.
        /// </summary>
        /// <value><c>true</c> if this instance is a canonical URI; otherwise, <c>false</c>.</value>
        public bool IsCanonical { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is a canonical query URI.
        /// A canonical query URI contains one folder and may contain one data object and/or a query string.
        /// </summary>
        /// <value><c>true</c> if this instance is a canonical query URI; otherwise, <c>false</c>.</value>
        public bool IsCanonicalQuery { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is an alternate URI.
        /// There are several forms of supported alternate URIs:
        /// Canonical URIs with query strings and/or hashes
        /// Query URIs with hashes
        /// Hierarchical object URIs, optionally with query strings and/or hashes
        /// Template URIs (URIs with multiple folder segments), optionally with query strings and/or hashes
        /// </summary>
        /// <value><c>true</c> if this instance is an alternate URI; otherwise, <c>false</c>.</value>
        public bool IsAlternate { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is an alternate URI.
        /// Hierarchical object URIs have more than one path segment with an object defined.
        /// </summary>
        /// <value><c>true</c> if this instance is a hierarcical URI; otherwise, <c>false</c>.</value>
        public bool IsHierarchical { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is a template URI.
        /// Template object URIs have more than one path segment that is a folder.
        /// </summary>
        /// <value><c>true</c> if this instance is a template URI; otherwise, <c>false</c>.</value>
        public bool IsTemplate { get; }

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
            if (!IsValid || IsPrefix)
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
        /// Converts the URI to an ETP 1.1 URI.
        /// </summary>
        /// <returns>The converted URI.</returns>
        public EtpUri AsEtp11()
        {
            if (IsEtp11 || !IsValid)
                return this;

            var sb = new StringBuilder();
            var appendSlash = false;
            sb.Append($"{RootUri11.Uri}");
            if (!string.IsNullOrEmpty(Dataspace))
            {
                sb.Append(Dataspace);
                appendSlash = true;
            }
            if (!string.IsNullOrEmpty(NamespaceFamily) && !string.IsNullOrEmpty(NamespaceVersion))
            {
                if (appendSlash)
                    sb.Append("/");

                var shortVersion = EtpDataObjectType.TryGetFamilyShortVersionFromVersion(NamespaceFamily, NamespaceVersion);
                sb.Append($"{NamespaceFamily}{shortVersion}");
                appendSlash = true;
            }

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

            var sb = new StringBuilder();
            sb.Append(RootUri12.Uri);

            var appendSlash = false;
            if (!string.IsNullOrEmpty(Dataspace))
            {
                sb.Append($"dataspace({Dataspace})");
                appendSlash = true;
            }
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
        public static bool IsRootUri(string uri)
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
        /// Gets the last matching value in the URI for the specified capture group.
        /// </summary>
        /// <param name="match">The regex match results.</param>
        /// <param name="groupName">The capture group name.</param>
        /// <returns>The last matching value if successfully matched; <c>null</c> if not matched.</returns>
        private static string GetLastMatch(Match match, string groupName)
        {
            if (!match.Success)
                return null;
            var group = match.Groups[groupName];
            if (!group.Success)
                return null;

            return group.Captures[group.Captures.Count - 1].Value;
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

            if (objectType != null && EmlCommonObjects.Contains(objectType)) // Special case for ETP 1.1 hierarchical URIs with EML common objects
            {
                defaultFamily = "eml";
                defaultVersion = "2.1";
            }

            var family = GetFirstMatch(match, Definition.FamilyGroup) ?? defaultFamily;
            family = EtpDataObjectType.TryGetFamily(family);

            var shortVersion = GetFirstMatch(match, Definition.ShortVersionGroup);
            var version = EtpDataObjectType.TryGetFamilyVersionFromShortVersion(family, shortVersion) ?? defaultVersion;

            objectType = EtpContentType.FormatObjectType(objectType, version);

            return new Segment(family, version, objectType, objectId, objectVersion);
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
