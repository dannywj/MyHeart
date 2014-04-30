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
            DataTable dt = new DataTable();
            // 已登录用户获取全部心愿，未登录用户获取公开心愿
            if (Session["CurrentUser"] != null)
            {
                string user = Session["CurrentUser"].ToString();
                dt = DBTools.getAllHeart(user);
            }
            else
            {
                dt = DBTools.getAllHeart();
            }

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
            string joinerLoginName = Request.Form["joinerLoginName"].ToString();

            JsonResult jr = new JsonResult();
            jr.ContentType = "text/json";
            jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            if (DBTools.PubNewHeart(newHeart, joinerLoginName))
            {
                jr.Data = new { isSuccess = true };
            }
            else
            {
                jr.Data = new { isSuccess = false, errorMessage = "Pub Heart Error" };
            }
            return jr;
        }

        public JsonResult GetHeartsByLoginName(string loginName)
        {
            JsonResult jr = new JsonResult();
            jr.ContentType = "text/json";
            jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            var list = DBTools.GetHeartsByLoginName(loginName);
            jr.Data = new { isSuccess = true, heartList = list };
            return jr;
        }

        public JsonResult UpdateHeartStation(int station, int heartId)
        {
            JsonResult jr = new JsonResult();
            jr.ContentType = "text/json";
            jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            bool result = DBTools.UpdateHeartStation(station, heartId);
            jr.Data = new { isSuccess = result };
            return jr;
        }

        public JsonResult GetHeartsCount(string loginName)
        {
            JsonResult jr = new JsonResult();
            jr.ContentType = "text/json";
            jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            var resultstr = DBTools.GetHeartsCount(loginName);
            jr.Data = new { isSuccess = true, allcount = resultstr.Split('_')[0].ToString(), okcount = resultstr.Split('_')[1].ToString() };
            return jr;
        }
    }
}
