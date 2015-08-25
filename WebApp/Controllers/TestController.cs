using Contracts;
using MassTransit;
using ReadStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApp.Controllers
{
    public class TestController : ApiController
    {

        //public static List<MyModel> Items = new List<MyModel>()
        //{
        //    new MyModel() { Id = Guid.NewGuid(), Name = "Peter" },
        //    new MyModel() { Id = Guid.NewGuid(), Name = "Anna" }
        //};

        private readonly IServiceBus _bus;
        public TestController()
        {
            _bus = Startup.Bus;
        }


        public IHttpActionResult Get()
        {
            using (var db = new MyReadStore())
            {
                return Ok(db.SomeItems.ToList());
            }
        }

        public IHttpActionResult Post(MyModel model)
        {
            var clientId = GetClientId();
            _bus.Publish(new AddItemCommand() { Id = Guid.NewGuid(), Name = model.Name, ClientId = clientId });
            return Ok();
        }

        public IHttpActionResult Put(MyModel model)
        {
            var clientId = GetClientId();
            _bus.Publish(new UpdateItemCommand() { Id = model.Id, NewName = model.Name, ClientId = clientId, IsActive = model.IsActive });
            return Ok();
        }

        private Guid GetClientId()
        {
            Guid clientId = Guid.Empty;

            IEnumerable<string> hvals;
            if (Request.Headers.TryGetValues("X-ClientID", out hvals))
            {
                Guid.TryParse(hvals.First(), out clientId);
            }

            return clientId;
        }

    }

    public class MyModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

}
