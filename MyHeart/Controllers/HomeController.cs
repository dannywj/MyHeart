using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyHeart.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public void Index()
        {
            Response.Redirect("Heart/happywall.html");
        }

    }
}
