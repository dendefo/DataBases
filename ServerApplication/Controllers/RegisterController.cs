using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ServerApplication.Controllers
{
    public class RegisterController: ApiController
    {

        public int Get(string username)
        {
            SQLManager sQLManager = new SQLManager();
            return sQLManager.Register(username);

        }
    }
}