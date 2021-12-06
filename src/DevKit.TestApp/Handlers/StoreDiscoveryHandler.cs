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
using Energistics.Etp.Store;
using System.Collections.Generic;
using System.Linq;

namespace Energistics.Etp.Handlers
{
    public class StoreDiscoveryHandler : StoreHandlerBase
    {
        public StoreDiscoveryHandler(TestDataStore store)
            : base(store)
        {
        }

        protected override void InitializeRegistrarCore()
        {
            // Register protocol handlers
            if (Registrar.IsEtpVersionSupported(EtpVersion.v11))
            {
                Registrar.Register(new v11.Protocol.Discovery.DiscoveryStoreHandler());
            }
            if (Registrar.IsEtpVersionSupported(EtpVersion.v12))
            {
                Registrar.Register(new v12.Protocol.Discovery.DiscoveryStoreHandler());
                Registrar.Register(new v12.Protocol.Dataspace.DataspaceStoreHandler());
                Registrar.Register(new v12.Protocol.SupportedTypes.SupportedTypesStoreHandler());
            }
        }

        protected override void InitializeSessionCore()
        {
            if (Session.EtpVersion == EtpVersion.v11)
            {
                Session.Handler<v11.Protocol.Discovery.IDiscoveryStore>().OnGetResources += OnGetResources;
            }
            else if (Session.EtpVersion == EtpVersion.v12)
            {
                Session.Handler<v12.Protocol.Dataspace.IDataspaceStore>().OnGetDataspaces += OnGetDataspaces;
                Session.Handler<v12.Protocol.Discovery.IDiscoveryStore>().OnGetResources += OnGetResources;
                Session.Handler<v12.Protocol.Discovery.IDiscoveryStore>().OnGetDeletedResources += OnGetDeletedResources;
                Session.Handler<v12.Protocol.SupportedTypes.ISupportedTypesStore>().OnGetSupportedTypes += OnGetSupportedTypes;
            }
        }

        private void OnGetResources(object sender, ListRequestEventArgs<v11.Protocol.Discovery.GetResources, v11.Datatypes.Object.Resource> args)
        {
            Store.ExecuteWithLock(() =>
            {
                OnGetResourcesCore(sender, args);
            });
        }
        private void OnGetResourcesCore(object sender, ListRequestEventArgs<v11.Protocol.Discovery.GetResources, v11.Datatypes.Object.Resource> args)
        {
            var handler = (v11.Protocol.Discovery.IDiscoveryStore)sender;
            var resources = Enumerable.Empty<v11.Datatypes.Object.Resource>();

            var uri = new EtpUri(args.Request.Body.Uri);
            if (!uri.IsEtp11)
                args.FinalError = handler.ErrorInfo().InvalidUri(uri);
            else if (uri.IsRootUri && Store.Dataspaces.Count > 1) // List dataspaces
            {
                resources = Store.Dataspaces
                    .Select(d => d.Resource11);
            }
            else if (uri.IsDataspaceUri) // List family versions
            {
                resources = Store.GetFamilies(EtpVersion.v11, uri)
                    .Select(f => f.Resource11);
            }
            else if (uri.IsFolderUri) // List data objects in folder
            {
                // Work around for cross-ML ETP 1.1 URIs:
                var dataType = MockStore.TryGetCorrectedDataObjectType(uri);
                resources = Store.GetObjects(EtpVersion.v11, new MockGraphContext(uri.Parent, dataType))
                    .Select(o => o.Resource11(true));
            }
            else if (uri.IsDataRootUri || uri.IsObjectUri) // List types of data objects available
            {
                resources = Store.GetSupportedTypes(EtpVersion.v11, new MockGraphContext(uri, true, false), true)
                    .Select(st => st.Resource11);
            }
            else
                args.FinalError = handler.ErrorInfo().InvalidUri(uri);

            args.Responses.AddRange(resources);
        }

        private void OnGetDataspaces(object sender, ListRequestEventArgs<v12.Protocol.Dataspace.GetDataspaces, v12.Datatypes.Object.Dataspace> args)
        {
            Store.ExecuteWithLock(() =>
            {
                OnGetDataspacesCore(sender, args);
            });
        }

        private void OnGetDataspacesCore(object sender, ListRequestEventArgs<v12.Protocol.Dataspace.GetDataspaces, v12.Datatypes.Object.Dataspace> args)
        {
            args.Responses.AddRange(Store.Dataspaces.Select(d => d.Dataspace12));
        }

        private void OnGetResources(object sender, DualListRequestEventArgs<v12.Protocol.Discovery.GetResources, v12.Datatypes.Object.Resource, v12.Datatypes.Object.Edge> args)
        {
            Store.ExecuteWithLock(() =>
            {
                OnGetResourcesCore(sender, args);
            });
        }

        private void OnGetResourcesCore(object sender, DualListRequestEventArgs<v12.Protocol.Discovery.GetResources, v12.Datatypes.Object.Resource, v12.Datatypes.Object.Edge> args)
        {
            var handler = (v12.Protocol.Discovery.IDiscoveryStore)sender;
            var resources = Enumerable.Empty<v12.Datatypes.Object.Resource>();

            bool? activeStatusFilter = null;
            if (args.Request.Body.ActiveStatusFilter != null)
                activeStatusFilter = args.Request.Body.ActiveStatusFilter == v12.Datatypes.Object.ActiveStatusKind.Active ? true : false;

            var uri = new EtpUri(args.Request.Body.Context.Uri);
            if (!uri.IsEtp12)
                args.FinalError = handler.ErrorInfo().InvalidUri(uri);
            else if (uri.IsDataRootUri || uri.IsObjectUri) // List data objects
            {
                resources = Store.GetObjects(EtpVersion.v12, new MockGraphContext(args.Request.Body), activeStatusFilter, args.Request.Body.StoreLastWriteFilter)
                    .Where(o => handler.Session.SessionSupportedDataObjects.IsSupported(o.DataObjectType))
                    .Select(o => o.Resource12(true));
            }
            else
                args.FinalError = handler.ErrorInfo().InvalidUri(uri);

            args.Responses1.AddRange(resources);
        }

        private void OnGetDeletedResources(object sender, ListRequestEventArgs<v12.Protocol.Discovery.GetDeletedResources, v12.Datatypes.Object.DeletedResource> args)
        {
            Store.ExecuteWithLock(() =>
            {
                OnGetDeletedResourcesCore(sender, args);
            });
        }

        private void OnGetDeletedResourcesCore(object sender, ListRequestEventArgs<v12.Protocol.Discovery.GetDeletedResources, v12.Datatypes.Object.DeletedResource> args)
        {
            var handler = (v12.Protocol.Discovery.IDiscoveryStore)sender;
            var resources = Enumerable.Empty<v12.Datatypes.Object.DeletedResource>();

            var uri = new EtpUri(args.Request.Body.DataspaceUri);
            if (!uri.IsEtp12 || !uri.IsDataspaceUri)
                args.FinalError = handler.ErrorInfo().InvalidUri(uri);
            else
            {
                resources = Store.GetDeletedObjects(EtpVersion.v12, uri, objectTypes: args.Request.Body.DataObjectTypes.ToDataObjectTypes(), deleteTimeFilter: args.Request.Body.DeleteTimeFilter)
                    .Where(o => handler.Session.SessionSupportedDataObjects.IsSupported(o.DataObjectType))
                    .Select(@do => @do.DeletedResource12);
            }

            args.Responses.AddRange(resources);
        }

        private void OnGetSupportedTypes(object sender, ListRequestEventArgs<v12.Protocol.SupportedTypes.GetSupportedTypes, v12.Datatypes.Object.SupportedType> args)
        {
            Store.ExecuteWithLock(() =>
            {
                OnGetSupportedTypesCore(sender, args);
            });
        }

        private void OnGetSupportedTypesCore(object sender, ListRequestEventArgs<v12.Protocol.SupportedTypes.GetSupportedTypes, v12.Datatypes.Object.SupportedType> args)
        {
            var handler = (v12.Protocol.SupportedTypes.ISupportedTypesStore)sender;
            var supportedTypes = Enumerable.Empty<v12.Datatypes.Object.SupportedType>();

            var uri = new EtpUri(args.Request.Body.Uri);
            if (!uri.IsEtp12)
                args.FinalError = handler.ErrorInfo().InvalidUri(uri);
            else if (uri.IsDataRootUri || uri.IsObjectUri) // List types of data objects available
            {
                supportedTypes = Store.GetSupportedTypes(EtpVersion.v12, new MockGraphContext(args.Request.Body), args.Request.Body.ReturnEmptyTypes)
                    .Where(st => handler.Session.SessionSupportedDataObjects.IsSupported(st.DataObjectType))
                    .Select(st => st.SupportedType12);
            }
            else
                args.FinalError = handler.ErrorInfo().InvalidUri(uri);

            args.Responses.AddRange(supportedTypes);
        }
    }
}
