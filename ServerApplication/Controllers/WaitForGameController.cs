using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace ServerApplication.Controllers
{
    public class WaitForGameController : ApiController
    {
        // GET: api/waitforgame/
        public bool Get(int id)
        {
            SQLManager sQLManager = new SQLManager();
            return sQLManager.CheckIfGameIsReady(id);
        }
    }
}