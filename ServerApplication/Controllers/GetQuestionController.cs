using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace ServerApplication.Controllers
{
    public class GetQuestionController : ApiController
    {
        // GET: api/getquestion
        public Question Get(int GameID)
        {
            SQLManager sQLManager = new SQLManager();
            return sQLManager.GetQuestion(GameID);
        }
    }
}