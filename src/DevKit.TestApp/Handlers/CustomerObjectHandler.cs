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
    public class CustomerObjectHandler : Handler
    {
        private Guid StoreNotificationSubscriptionGuid = Guid.NewGuid();

        protected override void InitializeRegistrarCore()
        {
            if (Registrar.IsEtpVersionSupported(EtpVersion.v11))
            {
                Registrar.Register(new v11.Protocol.Store.StoreCustomerHandler());
                Registrar.Register(new v11.Protocol.StoreNotification.StoreNotificationCustomerHandler());
            }
            if (Registrar.IsEtpVersionSupported(EtpVersion.v12))
            {
                Registrar.Register(new v12.Protocol.Store.StoreCustomerHandler());
                Registrar.Register(new v12.Protocol.StoreNotification.StoreNotificationCustomerHandler());
            }
        }

        protected override void InitializeSessionCore()
        {
            if (Session.EtpVersion == EtpVersion.v11)
            {
                Session.Handler<v11.Protocol.Store.IStoreCustomer>().OnObject += OnObject;
                Session.Handler<v11.Protocol.StoreNotification.IStoreNotificationCustomer>().OnChangeNotification += OnChangeNotification;
                Session.Handler<v11.Protocol.StoreNotification.IStoreNotificationCustomer>().OnDeleteNotification += OnDeleteNotification;
            }
            else if (Session.EtpVersion == EtpVersion.v12)
            {
                Session.Handler<v12.Protocol.Store.IStoreCustomer>().OnGetDataObjectsResponse += OnGetDataObjectsResponse;
                Session.Handler<v12.Protocol.StoreNotification.IStoreNotificationCustomer>().OnObjectChanged += OnObjectChanged;
                Session.Handler<v12.Protocol.StoreNotification.IStoreNotificationCustomer>().OnObjectDeleted += OnObjectDeleted;
            }
        }

        public override void PrintConsoleOptions()
        {
            Console.WriteLine(" O - Objects");
        }

        public override bool HandleConsoleInput(ConsoleKeyInfo info)
        {
            if (!IsKey(info, "O"))
                return false;

            var version = Session.EtpVersion;

            Console.WriteLine($" G - Store - {(version == EtpVersion.v11 ? "GetObject" : "GetDataObjects")}");
            Console.WriteLine($" N - StoreNotification - {(version == EtpVersion.v11 ? "NotificationRequest" : "SubscribeNotifications")}");
            Console.WriteLine($" U - StoreNotification - {(version == EtpVersion.v11 ? "CancelNotification" : "UnsubscribeNotifications")}");
            Console.WriteLine();

            info = Console.ReadKey();

            Console.WriteLine(" - processing...");
            Console.WriteLine();

            if (IsKey(info, "G"))
            {
                Console.WriteLine("Enter data object URI:");
                var uri = Console.ReadLine();
                Console.WriteLine();

                if (version == EtpVersion.v11)
                    Session.Handler<v11.Protocol.Store.IStoreCustomer>().GetObject(uri);
                else if (version == EtpVersion.v12)
                    Session.Handler<v12.Protocol.Store.IStoreCustomer>().GetDataObjects(new List<string> { uri });
            }
            else if (IsKey(info, "N"))
            {
                Console.WriteLine("Enter notification URI:");
                var uri = Console.ReadLine();
                Console.WriteLine();

                if (version == EtpVersion.v11)
                    Session.Handler<v11.Protocol.StoreNotification.IStoreNotificationCustomer>().NotificationRequest(new v11.Datatypes.Object.NotificationRequestRecord
                    {
                        IncludeObjectData = true,
                        ObjectTypes = new List<string>(),
                        StartTime = DateTime.UtcNow.ToEtpTimestamp(),
                        Uri = uri,
                        Uuid = StoreNotificationSubscriptionGuid.ToString(),
                    });
                else if (version == EtpVersion.v12)
                    Session.Handler<v12.Protocol.StoreNotification.IStoreNotificationCustomer>().SubscribeNotifications(new List<v12.Datatypes.Object.SubscriptionInfo>
                            {
                                new v12.Datatypes.Object.SubscriptionInfo
                                {
                                    Format = Formats.Xml,
                                    Context = new v12.Datatypes.Object.ContextInfo
                                    {
                                        Uri = uri,
                                        DataObjectTypes = new List<string>(),
                                        Depth = 999,
                                        IncludeSecondarySources = false,
                                        IncludeSecondaryTargets = false,
                                        NavigableEdges = v12.Datatypes.Object.RelationshipKind.Primary,
                                    },
                                    Scope = v12.Datatypes.Object.ContextScopeKind.sourcesOrSelf,
                                    IncludeObjectData = true,
                                    RequestUuid = StoreNotificationSubscriptionGuid.ToUuid<v12.Datatypes.Uuid>(),
                                    StartTime = 0L,
                                }
                            });
            }
            else if (IsKey(info, "U"))
            {
                Console.WriteLine($"{(version == EtpVersion.v11 ? "Cancelling notifications" : "Unsubscribing from notifications")}.");

                if (version == EtpVersion.v11)
                    Session.Handler<v11.Protocol.StoreNotification.IStoreNotificationCustomer>().CancelNotification(StoreNotificationSubscriptionGuid);
                else if (version == EtpVersion.v12)
                    Session.Handler<v12.Protocol.StoreNotification.IStoreNotificationCustomer>().UnsubscribeNotifications(StoreNotificationSubscriptionGuid);
            }

            return true;
        }

        private void OnObject(object sender, ResponseEventArgs<v11.Protocol.Store.GetObject, v11.Protocol.Store.Object> e)
        {
            Console.WriteLine(e.Response.Body.DataObject.GetString());
        }

        private void OnChangeNotification(object sender, NotificationEventArgs<v11.Datatypes.Object.NotificationRequestRecord, v11.Protocol.StoreNotification.ChangeNotification> e)
        {
            var @string = e.Notification.Body.Change.DataObject?.GetString();
            if (e.Notification.Body.Change.DataObject != null)
                e.Notification.Body.Change.DataObject.Data = null;

            Console.WriteLine(EtpExtensions.Serialize(e.Notification.Body, true));
            if (!string.IsNullOrEmpty(@string))
                Console.WriteLine(@string);
        }

        private void OnDeleteNotification(object sender, NotificationEventArgs<v11.Datatypes.Object.NotificationRequestRecord, v11.Protocol.StoreNotification.DeleteNotification> e)
        {
            Console.WriteLine(EtpExtensions.Serialize(e.Notification.Body, true));
        }

        private void OnGetDataObjectsResponse(object sender, DualResponseEventArgs<v12.Protocol.Store.GetDataObjects, v12.Protocol.Store.GetDataObjectsResponse, v12.Protocol.Store.Chunk> e)
        {
            if (e.Response1 != null)
            {
                foreach (var dataObject in e.Response1.Body.DataObjects.Values)
                    Console.WriteLine(dataObject.GetString());
            }
        }

        private void OnObjectChanged(object sender, NotificationEventArgs<v12.Datatypes.Object.SubscriptionInfo, v12.Protocol.StoreNotification.ObjectChanged> e)
        {
            var @string = e.Notification.Body.Change.DataObject?.GetString();
            if (e.Notification.Body.Change.DataObject != null)
                e.Notification.Body.Change.DataObject.Data = null;

            Console.WriteLine(EtpExtensions.Serialize(e.Notification.Body, true));
            if (!string.IsNullOrEmpty(@string))
                Console.WriteLine(@string);
        }

        private void OnObjectDeleted(object sender, NotificationEventArgs<v12.Datatypes.Object.SubscriptionInfo, v12.Protocol.StoreNotification.ObjectDeleted> e)
        {
            Console.WriteLine(EtpExtensions.Serialize(e.Notification.Body, true));
        }
    }
}
