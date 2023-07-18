using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ServerApplication.Controllers
{
    public class GameHistoryController : ApiController
    {
        public List<GameData> Get(int PlayerID)
        {
            SQLManager sQLManager = new SQLManager();
            return sQLManager.GetGameHystory(PlayerID);
        }
    }
}
