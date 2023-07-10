using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace ServerApplication.Controllers
{

    public class CreateGameController : ApiController
    {
        // GET: api/creategame/id
        public int Get(int id)
        {
            SQLManager sqlManager = new SQLManager();
            int gameID = sqlManager.ConnectToGame(id);
            return gameID;
        }
    }
}