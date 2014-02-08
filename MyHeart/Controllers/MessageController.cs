using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HeartData;

namespace MyHeart.Controllers
{
    public class MessageController : Controller
    {
        public void Index()
        {
            Response.Redirect("Message/MyMessages.html");
        }

        public JsonResult GetRandomMessage() {
            JsonResult jr = new JsonResult();
            jr.ContentType = "text/json";
            jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            var list = DBTools.GetRandomMessage();
            jr.Data = new { isSuccess = true, messageList = list };
            return jr;
        }
    }
}