using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApp.Controllers
{
    public class GuidController : ApiController
    {

        public IHttpActionResult Get()
        {
            Guid guid = Guid.NewGuid();
            return Ok(guid);
        }

    }
}
