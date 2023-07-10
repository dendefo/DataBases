using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ServerApplication.Controllers
{
    public class GetUsernameController : ApiController
    {
        // GET api/<controller>/5
        public string Get(int usernameID)
        {
            SQLManager man = new SQLManager();
            return man.GetUsername(usernameID);
        }
    }
}