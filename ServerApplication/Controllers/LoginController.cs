using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace ServerApplication.Controllers
{
    public class LoginController : ApiController
    {
        // GET: api/login/<username>
        /// <summary>
        /// Returns id of player, if there is no player is found - returns -1
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public int Get(string username)
        {
            SQLManager man = new SQLManager();
            return man.Login(username);
        }
    }
}