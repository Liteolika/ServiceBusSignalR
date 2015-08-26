using Contracts;
using MassTransit;
using ReadStore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {

            IServiceBus bus = ServiceBusFactory.New(cfg =>
            {
                cfg.DisablePerformanceCounters();
                cfg.ReceiveFrom("rabbitmq://localhost/qqqq-app");
                cfg.UseRabbitMq(cf =>
                {
                    cf.ConfigureHost(new Uri("rabbitmq://localhost/qqqq-app"), hc =>
                    {
                        hc.SetUsername("petcar");
                        hc.SetPassword("?!Krone2009");
                    });
                });
            });

            EventPublisher publisher = new EventPublisher(bus);

            TheCommandHandler handler = new TheCommandHandler(publisher);
            bus.SubscribeHandler<AddItemCommand>(handler.Handle);
            bus.SubscribeHandler<UpdateItemCommand>(handler.Handle);
            bus.SubscribeHandler<DeleteItemCommand>(handler.Handle);

            Console.ReadKey();

        }
    }


    public class EventPublisher
    {
        private readonly IServiceBus _bus;
        public EventPublisher(IServiceBus bus)
        {
            _bus = bus;
        }

        public void Publish(ItemUpdatedEvent itemUpdatedEvent)
        {
            _bus.Publish(itemUpdatedEvent);
        }

        public void Publish(ItemAddedEvent itemAddedEvent)
        {
            _bus.Publish(itemAddedEvent);
        }

        public void Publish(ClientNotification clientNotification)
        {
            _bus.Publish(clientNotification);
        }

        internal void Publish(ItemDeletedEvent itemDeletedEvent)
        {
            _bus.Publish(itemDeletedEvent);
        }
    }
    public class TheCommandHandler
    {
        public readonly EventPublisher _publisher;

        public TheCommandHandler(EventPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Handle(AddItemCommand command)
        {
            try
            {
                if (command.Id == Guid.Empty)
                    throw new ArgumentNullException("command.Id");
                if (string.IsNullOrEmpty(command.Name))
                    throw new ArgumentNullException("command.Name");

                Console.WriteLine("Got command: {0}, Id: {1}, Name: {2}", command.GetType().Name, command.Id, command.Name);

                using (var db = new MyReadStore())
                {
                    if (db.SomeItems.Where(x => x.Id == command.Id).Any())
                        throw new Exception("Item does already exists");
                    db.SomeItems.Add(new SomeItem() { Id = command.Id, Name = command.Name, IsActive = false });
                    db.SaveChanges();
                }
                _publisher.Publish(new ItemAddedEvent() { Id = command.Id, Name = command.Name });
                _publisher.Publish(new ClientNotification() { Success = true, Message = "The item was added", ClientId = command.ClientId });

            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to execute command: {0}, Message: {1}", command.GetType().Name, ex.Message);
                _publisher.Publish(new ClientNotification() { Success = false, Message = ex.Message, ClientId = command.ClientId });
            }

        }

        public void Handle(DeleteItemCommand command)
        {
            try {

                if (command.Id == Guid.Empty)
                    throw new ArgumentNullException("command.Id");

                Console.WriteLine("Got command: {0}, Id: {1}", command.GetType().Name, command.Id);

                using (var db = new MyReadStore())
                {
                    var item = db.SomeItems.Where(x => x.Id == command.Id).FirstOrDefault();
                    if (item == null)
                        throw new Exception("Item with id " + command.Id.ToString() + " does not exist.");

                    db.SomeItems.Remove(item);
                    db.SaveChanges();

                    _publisher.Publish(new ItemDeletedEvent() { Id = command.Id });
                    _publisher.Publish(new ClientNotification() { Success = true, Message = "The item was updated", ClientId = command.ClientId });
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Failed to execute command: {0}, Message: {1}", command.GetType().Name, ex.Message);
                _publisher.Publish(new ClientNotification() { Success = false, Message = ex.Message, ClientId = command.ClientId });
            }
        }

        public void Handle(UpdateItemCommand command)
        {
            try
            {
                if (command.Id == Guid.Empty)
                    throw new ArgumentNullException("command.Id");
                if (string.IsNullOrEmpty(command.NewName))
                    throw new ArgumentNullException("command.Name");

                Console.WriteLine("Got command: {0}, Id: {1}, NewName: {2}", command.GetType().Name, command.Id, command.NewName);

                using (var db = new MyReadStore())
                {
                    var item = db.SomeItems.Where(x => x.Id == command.Id).FirstOrDefault();
                    if (item == null)
                        throw new Exception("Item with id " + command.Id.ToString() + " does not exist.");
                    item.Name = command.NewName;
                    item.IsActive = command.IsActive;
                    db.SaveChanges();
                }

                _publisher.Publish(new ItemUpdatedEvent() { Id = command.Id, Name = command.NewName });
                _publisher.Publish(new ClientNotification() { Success = true, Message = "The item was updated", ClientId = command.ClientId });
            }

            catch (Exception ex)
            {
                Console.WriteLine("Failed to execute command: {0}, Message: {1}", command.GetType().Name, ex.Message);
                _publisher.Publish(new ClientNotification() { Success = false, Message = ex.Message, ClientId = command.ClientId });
            }
        }
    }


}
