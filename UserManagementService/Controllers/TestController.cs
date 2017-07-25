using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Web.Http;

namespace UserManagementService.Controllers
{
    [CustomAuthorizeAttribute]
    public class TestController : ApiController
    {
        public string GetHelloWorld()
        {
            var username = Request.Properties[ClaimTypes.Name];
            return "Hello World";
        }
    }
}
