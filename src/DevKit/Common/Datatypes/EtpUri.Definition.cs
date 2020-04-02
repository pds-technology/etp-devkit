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

using System.Text.RegularExpressions;

namespace Energistics.Etp.Common.Datatypes
{
    public partial struct EtpUri
    {
        public static class Definition
        {
            public static readonly string FormatParameter =                     "$format";

            // Components

            public static readonly string BaseGroup =                           "base";
            public static readonly string FamilyGroup =                         "family";
            public static readonly string ShortVersionGroup =                   "shortVersion";
            public static readonly string ObjectTypeGroup =                     "objectType";
            public static readonly string ObjectIdGroup =                       "objectId";
            public static readonly string ObjectVersionGroup =                  "objectVersion";
            public static readonly string ObjectOrFolderGroup =                 "objectOrFolder";
            public static readonly string QueryGroup =                          "query";
            public static readonly string HashGroup =                           "hash";
            public static readonly string DataspaceGroup =                      "dataspace";

            // URIs

            public static readonly string RootUriGroup =                        "rootUri";
            public static readonly string DataspaceUriGroup =                   "dataspaceUri";
            public static readonly string FamilyVersionUriGroup =               "familyVersionUri";
            public static readonly string ObjectUriGroup =                      "objectUri";
            public static readonly string HierarchicalUriGroup =                "hierarchicalUri";
            public static readonly string FolderUriGroup =                      "folderUri";
            public static readonly string QueryUriGroup =                       "queryUri";
            public static readonly string TemplateUriGroup =                    "templateUri";

            public static readonly string CanonicalUriGroup =                   "canonicalUri";
            public static readonly string AlternateUriGroup =                   "alternateUri";
            public static readonly string Etp11UriGroup =                       "etp11Uri";
            public static readonly string Etp12UriGroup =                       "etp12Uri";

            // URI Components

            private static readonly string FamilyAndShortVersion =              $@"(?:(?<{FamilyGroup}>witsml|resqml|prodml|eml)(?<{ShortVersionGroup}>\d\d))";

            private static readonly string ObjectType =                         $@"(?:(?:obj_|cs_|part_)?(?<{ObjectTypeGroup}>\w+))";
            private static readonly string ObjectId =                           $@"(?:\((?<{ObjectIdGroup}>[^ ?#),]+)\))";
            private static readonly string ObjectIdAndVersion =                 $@"(?:\((?<{ObjectIdGroup}>[^ ?#),]+)(?:,(?<{ObjectVersionGroup}>[^?#)]*))?\))";

            private static readonly string Query =                              $@"(?:(?<{QueryGroup}>\?[^#]*))";
            private static readonly string Hash =                               $@"(?:(?<{HashGroup}>#.*))";
            private static readonly string OptionalSuffix =                     $@"(?:(?:{Query})?(?:{Hash})?)";

            private static readonly string Etp11Root =                          $@"(?:eml:/)";
            private static readonly string Etp12Root =                          $@"(?:eml:|eml://)";

            private static readonly string Etp11Dataspace =                     $@"(?:(?!witsml|resqml|prodml|eml)(?<{DataspaceGroup}>[^?#)/]+(?:/(?!witsml|resqml|prodml|eml)[^?#)/]+)*))";
            private static readonly string Etp12Dataspace =                     $@"(?:dataspace\((?<{DataspaceGroup}>[^?#)]+)\))";

            private static readonly string Etp11Object =                        $@"(?<{ObjectOrFolderGroup}>{ObjectType}{ObjectId})";
            private static readonly string Etp12Object =                        $@"(?<{ObjectOrFolderGroup}>{FamilyAndShortVersion}\.{ObjectType}{ObjectIdAndVersion})";

            private static readonly string Etp11Folder =                        $@"(?<{ObjectOrFolderGroup}>{ObjectType})";
            private static readonly string Etp12Folder =                        $@"(?<{ObjectOrFolderGroup}>{FamilyAndShortVersion}\.{ObjectType})";

            private static readonly string Etp11ObjectOrFolder =                $@"(?:{Etp11Object}|{Etp11Folder})";
            private static readonly string Etp12ObjectOrFolder =                $@"(?:{Etp12Object}|{Etp12Folder})";

            private static readonly string Etp11ObjectHierarchy =               $@"(?:{Etp11Object}(?:/{Etp11Object})+)";
            private static readonly string Etp12ObjectHierarchy =               $@"(?:{Etp12Object}(?:/{Etp12Object})+)";

            private static readonly string Etp11Base =                          $@"(?<{BaseGroup}>{Etp11Root}(?:/{Etp11Dataspace})?/{FamilyAndShortVersion})";
            private static readonly string Etp12Base =                          $@"(?<{BaseGroup}>{Etp12Root}(?:/{Etp12Dataspace})?)";

            // Root URIs

            private static readonly string Etp11RootUri =                       $@"(?<{RootUriGroup}>{Etp11Root}/)";
            private static readonly string Etp12RootUri =                       $@"(?<{RootUriGroup}>{Etp12Root}/)";

            // Dataspace URIs

            private static readonly string Etp11DataspaceUri =                  $@"(?<{DataspaceUriGroup}>{Etp11Root}/{Etp11Dataspace})";
            private static readonly string Etp12DataspaceUri =                  $@"(?<{DataspaceUriGroup}>{Etp12Root}/{Etp12Dataspace})";

            // FamilyVersion URIs

            private static readonly string Etp11FamilyVersionUri =              $@"(?<{FamilyVersionUriGroup}>{Etp11Root}/(?:{FamilyAndShortVersion}|{Etp11Dataspace}/{FamilyAndShortVersion}))";

            // Base URIs

            private static readonly string Etp11BaseUri =                       $@"(?:{Etp11RootUri}|{Etp11DataspaceUri}|{Etp11FamilyVersionUri})";
            private static readonly string Etp12BaseUri =                       $@"(?:{Etp12RootUri}|{Etp12DataspaceUri})";

            // Object URIs

            private static readonly string Etp11ObjectUri =                     $@"(?<{ObjectUriGroup}>{Etp11Base}/{Etp11Object})";
            private static readonly string Etp12ObjectUri =                     $@"(?<{ObjectUriGroup}>{Etp12Base}/{Etp12Object})";

            // Object Hierarchy URIs

            private static readonly string Etp11ObjectHierarchyUri =            $@"(?<{ObjectUriGroup}>(?<{HierarchicalUriGroup}>{Etp11Base}/{Etp11ObjectHierarchy}))";
            private static readonly string Etp12ObjectHierarchyUri =            $@"(?<{ObjectUriGroup}>(?<{HierarchicalUriGroup}>{Etp12Base}/{Etp12ObjectHierarchy}))";

            // Query and Folder URIs

            private static readonly string Etp11BaseFolderUri =                 $@"(?<{FolderUriGroup}>{Etp11Base}/{Etp11Folder})";
            private static readonly string Etp12BaseFolderUri =                 $@"(?<{FolderUriGroup}>{Etp12Base}/{Etp12Folder})";

            private static readonly string Etp11ObjectFolderUri =               $@"(?<{FolderUriGroup}>(?<{HierarchicalUriGroup}>{Etp11Base}/{Etp11Object}/{Etp11Folder}))";
            private static readonly string Etp12ObjectFolderUri =               $@"(?<{FolderUriGroup}>(?<{HierarchicalUriGroup}>{Etp12Base}/{Etp12Object}/{Etp12Folder}))";

            private static readonly string Etp11ObjectHierarchyFolderUri =      $@"(?<{FolderUriGroup}>(?<{HierarchicalUriGroup}>{Etp11Base}/{Etp11Object}(?:/{Etp11Object})+/{Etp11Folder}))";
            private static readonly string Etp12ObjectHierarchyFolderUri =      $@"(?<{FolderUriGroup}>(?<{HierarchicalUriGroup}>{Etp12Base}/{Etp12Object}(?:/{Etp12Object})+/{Etp12Folder}))";

            private static readonly string Etp11QueryUri =                      $@"(?<{QueryUriGroup}>{Etp11BaseFolderUri}|{Etp11ObjectFolderUri})";
            private static readonly string Etp12QueryUri =                      $@"(?<{QueryUriGroup}>{Etp12BaseFolderUri}|{Etp12ObjectFolderUri})";

            private static readonly string Etp11FolderUri =                     $@"(?:{Etp11QueryUri}|{Etp11ObjectHierarchyFolderUri})";
            private static readonly string Etp12FolderUri =                     $@"(?:{Etp12QueryUri}|{Etp12ObjectHierarchyFolderUri})";

            // Template URIs

            private static readonly string Etp11FolderTemplateUri =             $@"(?<{FolderUriGroup}>{Etp11Base}(?:/{Etp11ObjectOrFolder})+/{Etp11Folder})";
            private static readonly string Etp12FolderTemplateUri =             $@"(?<{FolderUriGroup}>{Etp12Base}(?:/{Etp12ObjectOrFolder})+/{Etp12Folder})";

            private static readonly string Etp11ObjectTemplateUri =             $@"(?<{ObjectUriGroup}>{Etp11Base}(?:/{Etp11ObjectOrFolder})+/{Etp11Object})";
            private static readonly string Etp12ObjectTemplateUri =             $@"(?<{ObjectUriGroup}>{Etp12Base}(?:/{Etp12ObjectOrFolder})+/{Etp12Object})";

            private static readonly string Etp11TemplateUri =                   $@"(?<{TemplateUriGroup}>{Etp11FolderTemplateUri}|{Etp11ObjectTemplateUri})";
            private static readonly string Etp12TemplateUri =                   $@"(?<{TemplateUriGroup}>{Etp12FolderTemplateUri}|{Etp12ObjectTemplateUri})";

            // Canonical URIs

            private static readonly string Etp11CanonicalUri =                  $@"(?<{CanonicalUriGroup}>^(?:{Etp11BaseUri}|{Etp11ObjectUri}|{Etp11QueryUri}{Query}?)$)";
            private static readonly string Etp12CanonicalUri =                  $@"(?<{CanonicalUriGroup}>^(?:{Etp12BaseUri}|{Etp12ObjectUri}|{Etp12QueryUri}{Query}?)$)";

            // Alternate URIs

            private static readonly string Etp11AlternateUri =                  $@"(?<{AlternateUriGroup}>^(?:{Etp11BaseUri}|{Etp11ObjectUri}|{Etp11FolderUri}|{Etp11ObjectHierarchyUri}|{Etp11TemplateUri}){OptionalSuffix}$)";
            private static readonly string Etp12AlternateUri =                  $@"(?<{AlternateUriGroup}>^(?:{Etp12BaseUri}|{Etp12ObjectUri}|{Etp12FolderUri}|{Etp12ObjectHierarchyUri}|{Etp12TemplateUri}){OptionalSuffix}$)";

            // Complete Patterns

            private static readonly string EtpRootUri =                         $@"(?:{Etp11RootUri}|{Etp12RootUri})";

            private static readonly string Etp11Uri =                           $@"(?<{Etp11UriGroup}>{Etp11CanonicalUri}|{Etp11AlternateUri})";
            private static readonly string Etp12Uri =                           $@"(?<{Etp12UriGroup}>{Etp12CanonicalUri}|{Etp12AlternateUri})";

            private static readonly string EtpUri =                             $@"(?:{Etp11Uri}|{Etp12Uri})";

            // Objects and Folders

            private static readonly string EtpObjectOrFolder =                  $@"(?:^(?:{Etp11ObjectOrFolder}|{Etp12ObjectOrFolder})$)";

            // Regexes

            public static readonly Regex EtpRootUriRegex =          new Regex(EtpRootUri, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            public static readonly Regex EtpUriRegex =              new Regex(EtpUri, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

            public static readonly Regex EtpObjectOrFolderRegex =   new Regex(EtpObjectOrFolder, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        }
    }
}
