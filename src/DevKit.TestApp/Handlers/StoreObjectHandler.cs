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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Energistics.Etp.Handlers
{
    public class StoreObjectHandler : StoreHandlerBase
    {
        private Random Random { get; } = new Random();

        private BackgroundLoop ObjectUpdateLoop { get; } = new BackgroundLoop();

        public StoreObjectHandler(TestDataStore store)
            : base(store)
        {
        }

        protected override void InitializeRegistrarCore()
        {
            // Register protocol handlers
            if (Registrar.IsEtpVersionSupported(EtpVersion.v11))
            {
                Registrar.Register(new v11.Protocol.Store.StoreStoreHandler());
                Registrar.Register(new v11.Protocol.StoreNotification.StoreNotificationStoreHandler());
            }
            if (Registrar.IsEtpVersionSupported(EtpVersion.v12))
            {
                Registrar.Register(new v12.Protocol.Store.StoreStoreHandler());
                Registrar.Register(new v12.Protocol.StoreNotification.StoreNotificationStoreHandler());
            }
        }

        protected override void InitializeSessionCore()
        {
            Session.SessionClosed += OnServerSessionClosed;
            if (Session.EtpVersion == EtpVersion.v11)
            {
                Session.Handler<v11.Protocol.Store.IStoreStore>().OnGetObject += OnGetObject;
                Session.Handler<v11.Protocol.StoreNotification.IStoreNotificationStore>().OnNotificationRequest += OnNotificationRequest;
                Session.Handler<v11.Protocol.StoreNotification.IStoreNotificationStore>().OnCancelNotification += OnCancelNotification;
            }
            else if (Session.EtpVersion == EtpVersion.v12)
            {
                Session.Handler<v12.Protocol.Store.IStoreStore>().OnGetDataObjects += OnGetDataObjects;
                Session.Handler<v12.Protocol.StoreNotification.IStoreNotificationStore>().OnStarted += OnStoreNotificationStoreStarted;
                Session.Handler<v12.Protocol.StoreNotification.IStoreNotificationStore>().OnSubscribeNotifications += OnSubscribeNotifications;
                Session.Handler<v12.Protocol.StoreNotification.IStoreNotificationStore>().OnUnsubscribeNotifications += OnUnsubscribeNotifications;
            }
        }

        public override void PrintConsoleOptions()
        {
            Console.WriteLine(" U - Start / stop background store object updates");
        }

        public override bool HandleConsoleInput(ConsoleKeyInfo info)
        {
            if (IsKey(info, "U"))
            {
                if (ObjectUpdateLoop.IsStarted)
                {
                    Console.WriteLine("Stopping object updates.");
                    ObjectUpdateLoop.Stop();
                }
                else
                {
                    Console.WriteLine("Starting object updates.");
                    ObjectUpdateLoop.Start(UpdateDataObjects, TimeSpan.FromSeconds(5));
                }

                return true;
            }

            return false;
        }

        private void UpdateDataObjects(CancellationToken token)
        {
            Store.ExecuteWithLock(() =>
            {
                RandomlyUpdateDataObjects();
                Store.RefreshAll();
            });
        }

        private void RandomlyUpdateDataObjects()
        {
            foreach (var @object in Dataspace.Objects.Values)
            {
                if (Random.NextDouble() > 0.75 && !(@object is MockPropertyKind))
                {
                    Store.TouchObject(@object);
                }
            }

            if (Random.NextDouble() > 0.9)
            {
                if (Dataspace.Wellbore03.IsDeleted)
                    Store.RestoreObject(Dataspace.Wellbore03);
                else
                    Store.DeleteObject(Dataspace.Wellbore03);
            }

            if (Random.NextDouble() > 0.9)
            {
                if (Dataspace.TimeChannel03.IsDeleted)
                {
                    Store.RestoreObject(Dataspace.TimeChannel03);
                    Store.JoinObject(Dataspace.TimeChannelSet01, Dataspace.TimeChannel03);
                }
                else
                    Store.DeleteObject(Dataspace.TimeChannel03);
            }

            if (Random.NextDouble() > 0.9)
            {
                if (Dataspace.TimeChannel04.Containers.Count == 0)
                    Store.JoinObject(Dataspace.TimeChannelSet01, Dataspace.TimeChannel04);
                else
                    Store.UnjoinObject(Dataspace.TimeChannelSet01, Dataspace.TimeChannel04);
            }
        }

        private void OnServerSessionClosed(object sender, SessionClosedEventArgs args)
        {
            var server = (IEtpSession)sender;

            Store.ExecuteWithLock(() =>
            {
                Store.CancelAllObjectNotifications(server.SessionId);
            });
        }

        private void OnGetObject(object sender, RequestEventArgs<v11.Protocol.Store.GetObject, v11.Datatypes.Object.DataObject> args)
        {
            Store.ExecuteWithLock(() =>
            {
                OnGetObjectCore(sender, args);
            });
        }
        private void OnGetObjectCore(object sender, RequestEventArgs<v11.Protocol.Store.GetObject, v11.Datatypes.Object.DataObject> args)
        {
            var handler = (v11.Protocol.Store.IStoreStore)sender;

            args.Response = Store.GetObject(EtpVersion.v11, new EtpUri(args.Request.Body.Uri))?.DataObject11(true);
            if (args.Response == null)
                args.FinalError = handler.ErrorInfo().NotFound(args.Request.Body.Uri);
        }

        private void OnNotificationRequest(object sender, VoidRequestEventArgs<v11.Protocol.StoreNotification.NotificationRequest> args)
        {
            Store.ExecuteWithLock(() =>
            {
                OnNotificationRequestCore(sender, args);
            });
        }

        private void OnNotificationRequestCore(object sender, VoidRequestEventArgs<v11.Protocol.StoreNotification.NotificationRequest> args)
        {
            var handler = (v11.Protocol.StoreNotification.IStoreNotificationStore)sender;

            var request = args.Request.Body.Request;
            var subscriptionInfo = new MockSubscriptionInfo(request);
            var callbacks = CreateStoreNotificationCallbacks(handler);

            var startTime = request.StartTime;
            if (!Store.SubscribeObjectNotifications(EtpVersion.v11, startTime < Store.StoreLastWrite, startTime, false, handler.Session.SessionId, subscriptionInfo, callbacks))
                args.FinalError = handler.ErrorInfo().RequestUuidRejected(args.Request.Body.Request);
        }

        private void OnCancelNotification(object sender, VoidRequestEventArgs<v11.Protocol.StoreNotification.CancelNotification> args)
        {
            Store.ExecuteWithLock(() =>
            {
                OnCancelNotificationCore(sender, args);
            });
        }

        private void OnCancelNotificationCore(object sender, VoidRequestEventArgs<v11.Protocol.StoreNotification.CancelNotification> args)
        {
            var handler = (v11.Protocol.StoreNotification.IStoreNotificationStore)sender;

            if (!Store.UnsubscribeObjectNotifications(args.Request.Body.RequestUuid))
                args.FinalError = handler.ErrorInfo().NotFound(args.Request.Body);
        }

        private void OnGetDataObjects(object sender, MapAndListRequestEventArgs<v12.Protocol.Store.GetDataObjects, v12.Datatypes.Object.DataObject, v12.Protocol.Store.Chunk> args)
        {
            Store.ExecuteWithLock(() =>
            {
                OnGetDataObjectsCore(sender, args);
            });
        }

        private void OnGetDataObjectsCore(object sender, MapAndListRequestEventArgs<v12.Protocol.Store.GetDataObjects, v12.Datatypes.Object.DataObject, v12.Protocol.Store.Chunk> args)
        {
            var handler = (v12.Protocol.Store.IStoreStore)sender;
            if (!string.Equals(args.Request.Body.Format, Formats.Xml, StringComparison.OrdinalIgnoreCase))
            {
                args.FinalError = handler.ErrorInfo().InvalidArgument("Format", args.Request.Body.Format);
            }
            else
            {
                args.Response1Map = new Dictionary<string, v12.Datatypes.Object.DataObject>();
                foreach (var kvp in args.Request.Body.Uris)
                {
                    var uri = new EtpUri(kvp.Value);
                    var @object = Store.GetObject(EtpVersion.v12, uri);
                    if (@object != null)
                        args.Response1Map[kvp.Key] = @object.DataObject12(true);
                    else
                        args.ErrorMap[kvp.Key] = handler.ErrorInfo().NotFound(args.Request.Body.Uris[kvp.Key]);
                }
            }
        }

        private void OnStoreNotificationStoreStarted(object sender, EventArgs args)
        {
            var handler = (v12.Protocol.StoreNotification.IStoreNotificationStore)sender;

            if (!handler.Session.SessionSupportedDataObjects.IsSupported(Dataspace.Wellbore05.DataObjectType))
                return;

            Store.ExecuteWithLock(() =>
            {
                var callbacks = CreateStoreNotificationCallbacks(handler);
                var uuid = Guid.NewGuid();
                var subscriptionInfo = new MockSubscriptionInfo(EtpVersion.v12, Dataspace.Wellbore05, uuid);
                if (Store.SubscribeObjectNotifications(EtpVersion.v12, false, Store.StoreLastWrite, true, handler.Session.SessionId, subscriptionInfo, callbacks))
                    handler.UnsolicitedStoreNotifications(new List<v12.Datatypes.Object.SubscriptionInfo> { subscriptionInfo.SubsriptionInfo12 });
            });
        }

        private void OnSubscribeNotifications(object sender, MapRequestEventArgs<v12.Protocol.StoreNotification.SubscribeNotifications, string> args)
        {
            Store.ExecuteWithLock(() =>
            {
                OnSubscribeNotificationsCore(sender, args);
            });
        }

        private void OnSubscribeNotificationsCore(object sender, MapRequestEventArgs<v12.Protocol.StoreNotification.SubscribeNotifications, string> args)
        {
            var handler = (v12.Protocol.StoreNotification.IStoreNotificationStore)sender;

            foreach (var kvp in args.Request.Body.Request)
            {
                var callbacks = CreateStoreNotificationCallbacks(handler);
                var subscriptionInfo = new MockSubscriptionInfo(kvp.Value);
                if (Store.SubscribeObjectNotifications(EtpVersion.v12, false, Store.StoreLastWrite, true, handler.Session.SessionId, subscriptionInfo, callbacks))
                    args.ResponseMap[kvp.Key] = string.Empty;
                else
                    args.ErrorMap[kvp.Key] = handler.ErrorInfo().RequestUuidRejected(kvp.Value);
            }
        }

        private void OnUnsubscribeNotifications(object sender, RequestEventArgs<v12.Protocol.StoreNotification.UnsubscribeNotifications, Guid> args)
        {
            Store.ExecuteWithLock(() =>
            {
                OnUnsubscribeNotificationsCore(sender, args);
            });
        }

        private void OnUnsubscribeNotificationsCore(object sender, RequestEventArgs<v12.Protocol.StoreNotification.UnsubscribeNotifications, Guid> args)
        {
            var handler = (v12.Protocol.StoreNotification.IStoreNotificationStore)sender;

            if (!Store.UnsubscribeObjectNotifications(args.Request.Body.RequestUuid))
                args.Response = args.Request.Body.RequestUuid;
            else
                args.FinalError = handler.ErrorInfo().NotFound(args.Request.Body);
        }

        private MockObjectCallbacks CreateStoreNotificationCallbacks(v11.Protocol.StoreNotification.IStoreNotificationStore handler)
        {
            return new MockObjectCallbacks
            {
                JoinedSubscription = null,
                Created = (subscriptionUuid, @object, includeData) =>
                {
                    handler.ChangeNotification(subscriptionUuid, @object.ObjectChange11(includeData, v11.Datatypes.Object.ObjectChangeTypes.Upsert));
                },
                Updated = (subscriptionUuid, @object, includeData) =>
                {
                    handler.ChangeNotification(subscriptionUuid, @object.ObjectChange11(includeData, v11.Datatypes.Object.ObjectChangeTypes.Upsert));
                },
                Joined = null,
                Unjoined = null,
                ActiveStatusChanged = null,
                Deleted = (subscriptionUuid, @object) =>
                {
                    handler.DeleteNotification(subscriptionUuid, @object.ObjectChange11(false, v11.Datatypes.Object.ObjectChangeTypes.Delete));
                },
                UnjoinedSubscription = null,
                SubscriptionEnded = null,
            };
        }

        private MockObjectCallbacks CreateStoreNotificationCallbacks(v12.Protocol.StoreNotification.IStoreNotificationStore handler)
        {
            return new MockObjectCallbacks
            {
                JoinedSubscription = (subscriptionUuid, @object, includeData) =>
                {
                    handler.ObjectChanged(subscriptionUuid, @object.ObjectChange12(includeData, v12.Datatypes.Object.ObjectChangeKind.joinedSubscription));
                },
                Created = (subscriptionUuid, @object, includeData) =>
                {
                    handler.ObjectChanged(subscriptionUuid, @object.ObjectChange12(includeData, v12.Datatypes.Object.ObjectChangeKind.insert));
                },
                Updated = (subscriptionUuid, @object, includeData) =>
                {
                    handler.ObjectChanged(subscriptionUuid, @object.ObjectChange12(includeData, v12.Datatypes.Object.ObjectChangeKind.update));
                },
                Joined = (subscriptionUuid, @object, includeData) =>
                {
                    handler.ObjectChanged(subscriptionUuid, @object.ObjectChange12(includeData, v12.Datatypes.Object.ObjectChangeKind.joined));
                },
                Unjoined = (subscriptionUuid, @object, includeData) =>
                {
                    handler.ObjectChanged(subscriptionUuid, @object.ObjectChange12(includeData, v12.Datatypes.Object.ObjectChangeKind.unjoined));
                },
                ActiveStatusChanged = (subscriptionUuid, @object, isActive) =>
                {
                    handler.ObjectActiveStatusChanged(subscriptionUuid, isActive ? v12.Datatypes.Object.ActiveStatusKind.Active : v12.Datatypes.Object.ActiveStatusKind.Inactive, @object.StoreLastWrite, @object.Resource12(false));
                },
                Deleted = (subscriptionUuid, @object) =>
                {
                    handler.ObjectDeleted(subscriptionUuid, @object.Uri(EtpVersion.v12), @object.StoreLastWrite);
                },
                UnjoinedSubscription = (subscriptionUuid, @object, includeData) =>
                {
                    handler.ObjectChanged(subscriptionUuid, @object.ObjectChange12(includeData, v12.Datatypes.Object.ObjectChangeKind.unjoinedSubscription));
                },
                SubscriptionEnded = (subscriptionUuid, reason) =>
                {
                    handler.NotificationSubscriptionEnded(subscriptionUuid, reason);
                },
            };
        }
    }
}
