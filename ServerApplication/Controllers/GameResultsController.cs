using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ServerApplication.Controllers
{
    public class GameResultsController: ApiController
    {

        public int[] Get(int GameID)
        {
            SQLManager sQLManager = new SQLManager();
            return sQLManager.GetGameResults(GameID);
            //return sQLManager.Register(username);

        }
    }
}