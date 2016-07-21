//----------------------------------------------------------------------- 
// ETP DevKit, 1.0
//
// Copyright 2016 Petrotechnical Data Systems
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

using System.Text.RegularExpressions;

namespace Energistics.Datatypes
{
    /// <summary>
    /// Represents a content type supported by the Energistics Transfer Protocol (ETP).
    /// </summary>
    public struct EtpContentType
    {
        private static readonly Regex Pattern = new Regex(@"^application/x\-(witsml|resqml|prodml|energyml)\+xml;version=([0-9.]+)((;)?|(;type=((obj_|cs_)?(\w+))(;)?)?)$");
        private static readonly string BaseFormat = "application/x-{0}+xml;version={1};";
        private static readonly string TypeFormat = "type={0};";
        private readonly string _contentType;

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpContentType"/> struct.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        public EtpContentType(string contentType)
        {
            var match = Pattern.Match(contentType);

            _contentType = contentType;
            IsValid = match.Success;

            Family = GetValue(match, 1);
            Version = GetValue(match, 2);
            ObjectType = FormatObjectType(GetValue(match, 8));
            SchemaType = FormatSchemaType(GetValue(match, 7) + ObjectType, Version);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpContentType"/> struct.
        /// </summary>
        /// <param name="family">The ML family name.</param>
        /// <param name="version">The version.</param>
        public EtpContentType(string family, string version) : this(family, version, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpContentType" /> struct.
        /// </summary>
        /// <param name="family">The ML family name.</param>
        /// <param name="version">The version.</param>
        /// <param name="schemaType">The XSD type of the object.</param>
        public EtpContentType(string family, string version, string schemaType)
        {
            IsValid = true;

            Family = family;
            Version = version;
            SchemaType = FormatSchemaType(schemaType, Version);
            ObjectType = FormatObjectType(SchemaType);

            _contentType = string.Format(BaseFormat, family, version) + FormatType(SchemaType);
        }

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
        /// Gets the XSD type of the object.
        /// </summary>
        /// <value>The XSD type of the object.</value>
        public string SchemaType { get; }

        /// <summary>
        /// Gets the type of the object.
        /// </summary>
        /// <value>The type of the object.</value>
        public string ObjectType { get; }

        /// <summary>
        /// Returns true if a valid content type was specified.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public bool IsValid { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is a base content type.
        /// </summary>
        /// <value><c>true</c> if this instance is a base content type; otherwise, <c>false</c>.</value>
        public bool IsBaseType
        {
            get { return string.IsNullOrWhiteSpace(ObjectType); }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="EtpContentType"/> based on the
        /// current ML family name, version number and the specified schema type.
        /// </summary>
        /// <param name="schemaType">The XSD ype of the object.</param>
        /// <returns>The new <see cref="EtpContentType"/> instance.</returns>
        public EtpContentType For(string schemaType)
        {
            return new EtpContentType(Family, Version, schemaType);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return _contentType;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="EtpContentType"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator string(EtpContentType contentType)
        {
            return contentType.ToString();
        }

        /// <summary>
        /// Formats the schema type based on the specified version.
        /// </summary>
        /// <param name="schemaType">The XSD object type.</param>
        /// <param name="version">The data schema version.</param>
        /// <returns>The formatted schema type.</returns>
        public static string FormatSchemaType(string schemaType, string version)
        {
            if (string.IsNullOrWhiteSpace(schemaType))
                return string.Empty;

            var index = schemaType.IndexOf('_');
            var objectType = schemaType.Substring(index + 1);
            var prefix = (index > -1) ? schemaType.Substring(0, index + 1) : "obj_";

            System.Version ver = null;

            if (!version.Contains("_"))
            {
                // Capitalize object type when version >= 2.0
                objectType = (System.Version.TryParse(version, out ver) && ver.Major < 2
                    ? objectType.Substring(0, 1).ToLowerInvariant()
                    : objectType.Substring(0, 1).ToUpperInvariant())
                    + objectType.Substring(1);
            }

            schemaType = (ver != null && ver.Major < 2)
                ? prefix + objectType
                : objectType;

            return schemaType;
        }

        /// <summary>
        /// Formats the object type based on the specified schema type.
        /// </summary>
        /// <param name="schemaType">The XSD object type.</param>
        /// <returns>The formatted object type.</returns>
        public static string FormatObjectType(string schemaType)
        {
            if (string.IsNullOrWhiteSpace(schemaType))
                return string.Empty;

            var objectType = schemaType;
            var index = objectType.IndexOf('_');

            if (index > -1)
            {
                objectType = objectType.Substring(index + 1);
            }

            // Camel case object type name to match WMLTypeIn parameter
            objectType = objectType.Substring(0, 1).ToLowerInvariant()
                       + objectType.Substring(1);

            return objectType;
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
        /// Formats the specified schema type for appending to a content type.
        /// </summary>
        /// <param name="schemaType">Type of the object.</param>
        /// <returns>The formatted content type segment.</returns>
        private static string FormatType(string schemaType)
        {
            if (string.IsNullOrWhiteSpace(schemaType))
                return string.Empty;

            return string.Format(TypeFormat, schemaType);
        }
    }
}
