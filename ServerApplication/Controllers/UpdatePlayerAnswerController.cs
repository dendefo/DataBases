using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ServerApplication.Controllers
{
    public class UpdatePlayerAnswerController : ApiController
    {
        public void Get(int GameID, int PlayerID, float AnswerTime, bool IsAnswerRight)
        {
            SQLManager manager = new SQLManager();
            manager.UpdatePlayerQuestion(GameID, PlayerID, AnswerTime, IsAnswerRight);
        }
    }
}
