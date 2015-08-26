using Contracts;
using MassTransit;
using ReadStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;

namespace WebApp.Controllers
{
    public class ClientIDManagementAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var clientid = GetClientId(actionContext);
            actionContext.ActionArguments.Add("CLIENTID", clientid);
            base.OnActionExecuting(actionContext);
        }

        private Guid GetClientId(HttpActionContext context)
        {
            Guid clientId = Guid.Empty;

            IEnumerable<string> hvals;
            if (context.Request.Headers.TryGetValues("X-ClientID", out hvals))
            {
                Guid.TryParse(hvals.First(), out clientId);
            }

            return clientId;
        }

    }

    [ClientIDManagement]
    public abstract class ApiControllerBase : ApiController
    {

        //protected Guid ClientId()
        //{
        //    object clientId;
        //    ActionContext.ActionArguments.TryGetValue("CLIENTID", out clientId);
        //    if (clientId != null)
        //        return Guid.Parse(clientId.ToString());
        //    return Guid.Empty;
        //}

        protected Guid CallerClientId
        {
            get
            {
                object clientId;
                ActionContext.ActionArguments.TryGetValue("CLIENTID", out clientId);
                if (clientId != null)
                    return Guid.Parse(clientId.ToString());
                return Guid.Empty;
            }
        }

    }

    
    public class TestController : ApiControllerBase
    {

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
            _bus.Publish(new AddItemCommand() { Id = Guid.NewGuid(), Name = model.Name, ClientId = CallerClientId });
            return Ok();
        }

        public IHttpActionResult Put(MyModel model)
        {
            _bus.Publish(new UpdateItemCommand() { Id = model.Id, NewName = model.Name, ClientId = CallerClientId, IsActive = model.IsActive });
            return Ok();
        }

        public IHttpActionResult Delete(Guid id)
        {
            _bus.Publish(new DeleteItemCommand() { Id = id, ClientId = CallerClientId });
            return Ok();
        }

        

    }

    public class MyModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

}
