using Contracts;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace WebApp.Controllers
{
    public class MessageHub : Hub
    {
        public static ConcurrentDictionary<Guid, MyUserType> ConnectedUsers = new ConcurrentDictionary<Guid, MyUserType>();

        public override Task OnConnected()
        {
            var client = new MyUserType();
            client.ConnectionId = Guid.Parse(Context.ConnectionId);
            ConnectedUsers.TryAdd(client.ConnectionId, client);
            PushIdentifier(client.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            MyUserType garbage;
            ConnectedUsers.TryRemove(Guid.Parse(Context.ConnectionId), out garbage);

            return base.OnDisconnected(stopCalled);
        }

        public void PushIdentifier(Guid identifier)
        {
            Clients.Client(identifier.ToString()).identifierRecieved(identifier);
        }
        

    }

    public class HubEventNotifier
    {
        private readonly IHubContext hub;
        

        public HubEventNotifier()
        {
            hub = GlobalHost.ConnectionManager.GetHubContext<MessageHub>();
        }

        public void Handle(ClientNotification notification)
        {
            //hub.Clients.All.messageRecieved(notification);
            hub.Clients.Client(notification.ClientId.ToString()).messageRecieved(notification);
        }

        public void Handle(ItemAddedEvent @event)
        {
            hub.Clients.All.globalMessageRecieved(@event);
        }

        public void Handle(ItemUpdatedEvent @event)
        {
            hub.Clients.All.globalMessageRecieved(@event);
        }


    }

    public class MyUserType
    {
        public Guid ConnectionId { get; set; }
    }


}