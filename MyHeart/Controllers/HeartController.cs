using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HeartData;

namespace MyHeart.Controllers
{
    public class HeartController : Controller
    {
        //
        // GET: /Heart/

        public void Index()
        {
            Response.Redirect("Heart/happywallnew.html");
        }

        public string GetAllHeart()
        {
            string re = string.Empty;
            DataTable dt = DBTools.getAllHeart();
            if (dt.Rows.Count > 0)
            {
                re = Common.CreateJsonParameters(dt);
            }
            else
            {
                re = "";
            }
            return re;
        }

        public JsonResult PublishNewHeart()
        {
            //发布心愿采用form方式。。。。
            NewHeart newHeart = new NewHeart();
            newHeart = Common.JosnDeserialize<NewHeart>(Request.Form["NewHeart"].ToString(), null);

            JsonResult jr = new JsonResult();
            jr.ContentType = "text/json";
            jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            if (DBTools.PubNewHeart(newHeart))
            {
                jr.Data = new { isSuccess = true };
            }
            else
            {
                jr.Data = new { isSuccess = false, errorMessage = "Pub Heart Error" };
            }
            return jr;
        }
        
    }
}
