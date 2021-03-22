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
using Energistics.Etp.Common;
using System.Collections.Generic;

namespace Energistics.Etp.Handlers
{
    public class CustomerDiscoveryHandler : Handler
    {
        protected override void InitializeRegistrarCore()
        {
            if (Registrar.IsEtpVersionSupported(EtpVersion.v11))
            {
                Registrar.Register(new v11.Protocol.Discovery.DiscoveryCustomerHandler());
            }
            if (Registrar.IsEtpVersionSupported(EtpVersion.v12))
            {
                Registrar.Register(new v12.Protocol.Dataspace.DataspaceCustomerHandler());
                Registrar.Register(new v12.Protocol.Discovery.DiscoveryCustomerHandler());
                Registrar.Register(new v12.Protocol.SupportedTypes.SupportedTypesCustomerHandler());
                Registrar.Register(new v12.Protocol.Store.StoreCustomerHandler());
            }
        }

        protected override void InitializeSessionCore()
        {
            if (Session.EtpVersion == EtpVersion.v11)
            {
                Session.Handler<v11.Protocol.Discovery.IDiscoveryCustomer>().OnGetResourcesResponse += OnGetResourcesResponse;
            }
            else if (Session.EtpVersion == EtpVersion.v12)
            {
                Session.Handler<v12.Protocol.Dataspace.IDataspaceCustomer>().OnGetDataspacesResponse += OnGetDataspacesResponse;
                Session.Handler<v12.Protocol.Discovery.IDiscoveryCustomer>().OnGetResourcesResponse += OnGetResourcesResponse;
                Session.Handler<v12.Protocol.Discovery.IDiscoveryCustomer>().OnGetDeletedResourcesResponse += OnGetDeletedResourcesResponse;
                Session.Handler<v12.Protocol.SupportedTypes.ISupportedTypesCustomer>().OnGetSupportedTypesResponse += OnGetSupportedTypesResponse;
            }
        }


        public override void PrintConsoleOptions()
        {
            Console.WriteLine(" D - Discovery");
        }

        public override bool HandleConsoleInput(ConsoleKeyInfo info)
        {
            if (!IsKey(info, "D"))
                return false;

            var version = Session.EtpVersion;

            if (version == EtpVersion.v12)
                Console.WriteLine(" D - Dataspace - GetDataspaces");
            Console.WriteLine(" R - Discovery - GetResources");
            if (version == EtpVersion.v12)
                Console.WriteLine(" Q - Discovery - GetDeletedResources");
            if (version == EtpVersion.v12)
                Console.WriteLine(" S - SupportedTypes - GetSupportedTypes");
            Console.WriteLine();

            info = Console.ReadKey();

            Console.WriteLine(" - processing...");
            Console.WriteLine();

            if (IsKey(info, "D"))
            {
                if (version == EtpVersion.v12)
                {
                    Console.WriteLine("Getting dataspaces...");

                    Session.Handler<v12.Protocol.Dataspace.IDataspaceCustomer>().GetDataspaces();
                }
            }
            else if (IsKey(info, "R"))
            {
                Console.WriteLine("Enter resource URI:");
                var uri = Console.ReadLine();
                Console.WriteLine();

                if (version == EtpVersion.v11)
                    Session.Handler<v11.Protocol.Discovery.IDiscoveryCustomer>().GetResources(uri);
                else if (version == EtpVersion.v12)
                    Session.Handler<v12.Protocol.Discovery.IDiscoveryCustomer>().GetResources(GetContextInfo(uri), v12.Datatypes.Object.ContextScopeKind.sources, countObjects: true);
            }
            else if (IsKey(info, "Q"))
            {
                if (version == EtpVersion.v12)
                {
                    Console.WriteLine("Enter deleted resources URI:");
                    var uri = Console.ReadLine();
                    Console.WriteLine();

                    Session.Handler<v12.Protocol.Discovery.IDiscoveryCustomer>().GetDeletedResources(uri);
                }
            }
            else if (IsKey(info, "S"))
            {
                if (version == EtpVersion.v12)
                {
                    Console.WriteLine("Enter supported type URI:");
                    var uri = Console.ReadLine();
                    Console.WriteLine();

                    Session.Handler<v12.Protocol.SupportedTypes.ISupportedTypesCustomer>().GetSupportedTypes(uri, v12.Datatypes.Object.ContextScopeKind.sources, returnEmptyTypes: true, countObjects: true);
                }
            }

            return true;
        }

        private void OnGetDataspacesResponse(object sender, ResponseEventArgs<v12.Protocol.Dataspace.GetDataspaces, v12.Protocol.Dataspace.GetDataspacesResponse> e)
        {
            if (e.Response != null)
                Console.WriteLine(EtpExtensions.Serialize(e.Response.Body.Dataspaces, true));
        }

        private void OnGetResourcesResponse(object sender, ResponseEventArgs<v11.Protocol.Discovery.GetResources, v11.Protocol.Discovery.GetResourcesResponse> e)
        {
            Console.WriteLine(EtpExtensions.Serialize(e.Response.Body.Resource, true));
        }

        private void OnGetResourcesResponse(object sender, DualResponseEventArgs<v12.Protocol.Discovery.GetResources, v12.Protocol.Discovery.GetResourcesResponse, v12.Protocol.Discovery.GetResourcesEdgesResponse> e)
        {
            if (e.Response1 != null)
                Console.WriteLine(EtpExtensions.Serialize(e.Response1.Body.Resources, true));
            if (e.Response2 != null)
                Console.WriteLine(EtpExtensions.Serialize(e.Response2.Body.Edges, true));
        }

        private void OnGetDeletedResourcesResponse(object sender, ResponseEventArgs<v12.Protocol.Discovery.GetDeletedResources, v12.Protocol.Discovery.GetDeletedResourcesResponse> e)
        {
            if (e.Response != null)
                Console.WriteLine(EtpExtensions.Serialize(e.Response.Body.DeletedResources, true));
        }

        private void OnGetSupportedTypesResponse(object sender, ResponseEventArgs<v12.Protocol.SupportedTypes.GetSupportedTypes, v12.Protocol.SupportedTypes.GetSupportedTypesResponse> e)
        {
            if (e.Response != null)
                Console.WriteLine(EtpExtensions.Serialize(e.Response.Body.SupportedTypes, true));
        }

        private v12.Datatypes.Object.ContextInfo GetContextInfo(string uri)
        {
            return new v12.Datatypes.Object.ContextInfo
            {
                Uri = uri,
                DataObjectTypes = new List<string>(),
                Depth = 1,
                IncludeSecondarySources = false,
                IncludeSecondaryTargets = false,
                NavigableEdges = v12.Datatypes.Object.RelationshipKind.Primary,
            };
        }
    }
}
