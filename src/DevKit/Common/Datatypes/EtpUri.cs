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
using System.Text.RegularExpressions;
using System.Web;

namespace Energistics.Etp.Common.Datatypes
{
    /// <summary>
    /// Represents a URI supported by the Energistics Transfer Protocol (ETP).
    /// </summary>
    public struct EtpUri
    {
        private static readonly Regex Pattern = new Regex(@"^eml:(\/\/|\/\/\/|\/\/(([_\w\-]+)?\/)?((witsml|resqml|prodml|eml)([0-9]+)(\+(xml|json))?)(\/((obj_|cs_|part_)?(\w+))(\(([^\s\)]+)\))?)*?(\?[^#]*)?(#.*)?)$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private const string FormatParameterName = "$format";
        private readonly EtpUri?[] _parent;
        private readonly Match _match;

        /// <summary>
        /// The root URI supported by the Discovery protocol.
        /// </summary>
        public static readonly EtpUri RootUri = new EtpUri("eml://");

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpUri"/> struct.
        /// </summary>
        /// <param name="uri">The URI string.</param>
        public EtpUri(string uri)
        {
            _match = Pattern.Match(uri);
            _parent = new EtpUri?[] { null };

            Uri = uri;
            IsValid = _match.Success || IsRoot(uri);


            DataSpace = GetValue(_match, 3);
            Family = GetValue(_match, 5);
            Version = FormatVersion(GetValue(_match, 6), Family);
            ObjectType = null;
            ObjectId = null;
            Query = GetValue(_match, 15);
            Hash = GetValue(_match, 16);

            var format = GetQueryStringFormat(Query, GetValue(_match, 8));
            Format = string.IsNullOrWhiteSpace(format) ? EtpContentType.Xml : format;
            ContentType = new EtpContentType(Family, Version, null, Format);

            if (!HasRepeatValues(_match)) return;

            var last = GetObjectIds().Last();
            ObjectType = last.ObjectType;
            ObjectId = last.ObjectId;
            ContentType = new EtpContentType(Family, Version, ObjectType, Format);
        }

        /// <summary>
        /// Gets the original URI string.
        /// </summary>
        /// <value>The URI.</value>
        public string Uri { get; }

        /// <summary>
        /// Gets the data space.
        /// </summary>
        /// <value>The data space.</value>
        public string DataSpace { get; }

        /// <summary>
        /// Gets the ML family name.
        /// </summary>
        /// <value>The ML family.</value>
        public string Family { get; }

        /// <summary>
        /// Gets the version.
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
        /// Returns true if a valid URI was specified.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public bool IsValid { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is a base URI.
        /// </summary>
        /// <value><c>true</c> if this instance is a base URI; otherwise, <c>false</c>.</value>
        public bool IsBaseUri => string.IsNullOrWhiteSpace(ObjectType) && string.IsNullOrWhiteSpace(ObjectId);

        /// <summary>
        /// Gets a value indicating whether this instance is a the root URI.
        /// </summary>
        /// <value><c>true</c> if this instance is the root URI; otherwise, <c>false</c>.</value>
        public bool IsRootUri => string.Equals(RootUri, Uri, StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        /// Gets the parent URI.
        /// </summary>
        /// <value>The parent URI.</value>
        public EtpUri Parent
        {
            get
            {
                if (!IsValid || IsBaseUri)
                    return this;

                if (_parent[0].HasValue)
                    return _parent[0].Value;

                var uri = Uri;

                if (!string.IsNullOrWhiteSpace(Query))
                    uri = uri.Substring(0, uri.IndexOf('?'));

                else if (!string.IsNullOrWhiteSpace(Hash))
                    uri = uri.Substring(0, uri.IndexOf('#'));

                var index = uri.LastIndexOf('/');
                _parent[0] = new EtpUri(uri.Substring(0, index));

                return _parent[0].Value;
            }
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
            if (HasRepeatValues(_match))
            {
                var objectPathGroup = _match.Groups[9];
                var objectTypeGroup = _match.Groups[12];
                var objectIdGroup = _match.Groups[14];

                var groupIndex = 0;
                for (int i=0; i<objectTypeGroup.Captures.Count; i++)
                {
                    var objectPath = objectPathGroup.Captures[i].Value;
                    var objectType = objectTypeGroup.Captures[i].Value;

                    var objectId = objectIdGroup.Captures.Count > groupIndex && objectPath.EndsWith(")")
                        ? WebUtility.UrlDecode(objectIdGroup.Captures[groupIndex++].Value)
                        : null;

                    yield return new Segment(
                        EtpContentType.FormatObjectType(objectType, Version),
                        objectId);
                }
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
        /// <param name="objectType">The object type.</param>
        /// <param name="objectId">The object identifier.</param>
        /// <param name="encode">if set to <c>true</c> encode the object identifier value.</param>
        /// <returns>A new <see cref="EtpUri" /> instance.</returns>
        public EtpUri Append(string objectType, string objectId = null, bool encode = false)
        {
            objectType = EtpContentType.FormatObjectType(objectType, Version);

            if (encode)
                objectId = WebUtility.UrlEncode(objectId);

            return string.IsNullOrWhiteSpace(objectId) ?
                new EtpUri(Uri + "/" + objectType) :
                new EtpUri(Uri + $"/{ objectType }({ objectId })");
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
            return Uri.ToLower().GetHashCode();
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
            return string.Equals(RootUri, uri, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Gets the value contained within the specified match at the specified index.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <param name="index">The index.</param>
        /// <returns>The matched value found at the specified index.</returns>
        private static string GetValue(Match match, int index)
        {
            return match.Success && match.Groups.Count > index
                ? match.Groups[index].Value
                : null;
        }

        /// <summary>
        /// Determines whether the specified match contains repeating values.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns><c>true</c> if any repeating groups were matched; otherwise, <c>false</c>.</returns>
        private static bool HasRepeatValues(Match match)
        {
            return match.Success && match.Groups[12].Captures.Count > 0;
        }

        /// <summary>
        /// Formats the version number.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="family">The ML family.</param>
        /// <returns>A dot delimited version number.</returns>
        private static string FormatVersion(string version, string family)
        {
            if (string.IsNullOrWhiteSpace(version))
                return null;

            // Force WITSML versions 13* and 14* to be formatted as 1.3.1.1 and 1.4.1.1, respectively
            if ("WITSML".Equals(family, StringComparison.InvariantCultureIgnoreCase))
            {
                if (version == "13" || version == "131")
                    version = "1311";
                else if (version == "14" || version == "141")
                    version = "1411";
            }

            return string.Join(".", version.Trim().Select(x => x));
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
                var format = values[FormatParameterName]?.Trim();

                return string.IsNullOrWhiteSpace(format) ? defaultValue : format;
            }
            catch
            {
                return defaultValue;
            }
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
            /// <param name="objectType">The object type.</param>
            /// <param name="objectId">The object identifier.</param>
            public Segment(string objectType, string objectId)
            {
                ObjectType = objectType;
                ObjectId = objectId;
            }

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
        }
    }
}
