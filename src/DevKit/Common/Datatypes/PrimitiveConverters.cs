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

namespace Energistics.Etp.Common.Datatypes
{
    /// <summary>
    /// Converts a nullable double to and from JSON.
    /// </summary>
    public class NullableDoubleConverter : NullableAttributeConverter<double?>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullableDoubleConverter"/> class.
        /// </summary>
        public NullableDoubleConverter() : base("double") { }
    }

    /// <summary>
    /// Converts a nullable integer to and from JSON.
    /// </summary>
    public class NullableIntConverter : NullableAttributeConverter<int?>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullableIntConverter"/> class.
        /// </summary>
        public NullableIntConverter() : base("int") { }
    }

    /// <summary>
    /// Converts a nullable long to and from JSON.
    /// </summary>
    public class NullableLongConverter : NullableAttributeConverter<long?>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullableLongConverter"/> class.
        /// </summary>
        public NullableLongConverter() : base("long") { }
    }

    /// <summary>
    /// Converts a nullable string to and from JSON.
    /// </summary>
    public class NullableStringConverter : NullableAttributeConverter<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullableStringConverter"/> class.
        /// </summary>
        public NullableStringConverter() : base("string") { }
    }
}
