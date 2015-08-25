using Contracts;
using Magnum.Pipeline;
using MassTransit;
using Microsoft.Owin;
using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebApp.Controllers;

[assembly: OwinStartup(typeof(WebApp.Startup))]
namespace WebApp
{
    public class Startup
    {

        public static IServiceBus Bus;

        public void Configuration(IAppBuilder app)
        {

            HttpConfiguration config = new HttpConfiguration();

            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            app.UseWebApi(config);

            app.MapSignalR();

            Bus = ServiceBusFactory.New(cfg =>
            {
                cfg.DisablePerformanceCounters();
                cfg.ReceiveFrom("rabbitmq://localhost/qqqq-web");
                cfg.UseRabbitMq(cf =>
                {
                    cf.ConfigureHost(new Uri("rabbitmq://localhost/qqqq-web"), hc =>
                    {
                        hc.SetUsername("petcar");
                        hc.SetPassword("?!Krone2009");
                    });
                });
            });

            HubEventNotifier notifier = new HubEventNotifier();
            Bus.SubscribeHandler<ClientNotification>(notifier.Handle);
            Bus.SubscribeHandler<ItemAddedEvent>(notifier.Handle);
            Bus.SubscribeHandler<ItemUpdatedEvent>(notifier.Handle);


        }

    }


    

}