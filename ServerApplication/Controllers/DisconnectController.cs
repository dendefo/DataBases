using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ServerApplication.Controllers
{
    public class DisconnectController : ApiController
    {
        public void Get(int PlayerID)
        {
            SQLManager sQLManager = new SQLManager();
            sQLManager.Disconnect(PlayerID);
        }
    }
}
