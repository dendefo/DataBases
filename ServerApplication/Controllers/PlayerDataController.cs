using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ServerApplication.Controllers
{
    public class PlayerDataController : ApiController
    {
        // GET api/<controller>/5
        public PlayerData Get(int PlayerID)
        {
            SQLManager man = new SQLManager();
            return man.GetPlayerData(PlayerID);
        }
    }
}