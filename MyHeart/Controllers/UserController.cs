using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HeartData;
using NetDimension.Weibo;

namespace MyHeart.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult CheckNewUser(string userName)
        {
            JsonResult jr = new JsonResult();
            jr.ContentType = "text/json";
            jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            if (DBTools.CheckNewUser(userName))
            {
                jr.Data = new { isSuccess = true };
            }
            else
            {
                jr.Data = new { isSuccess = false, errorMessage = "User Exist" };
            }
            return jr;
        }

        public JsonResult RegisterNewUser(string userName, string password, string userNickName)
        {
            JsonResult jr = new JsonResult();
            jr.ContentType = "text/json";
            jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            bool isEmail = false;
            if (Common.CheckEmail(userName))
            {
                isEmail = true;
            }
            if (DBTools.RegisterNewUser(userName, password, isEmail, userNickName))
            {
                //Session["CurrentUser"] = userName;
                jr.Data = new { isSuccess = true };
            }
            else
            {
                jr.Data = new { isSuccess = false, errorMessage = "Register Error" };
            }
            return jr;
        }

        public JsonResult UserLogin(string userName, string password)
        {
            JsonResult jr = new JsonResult();
            jr.ContentType = "text/json";
            jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            if (DBTools.UserLogin(userName, password))
            {
                Session["CurrentUser"] = userName;
                var userinfo = DBTools.GetUserInfoByLoginName(userName);
                jr.Data = new { isSuccess = true, nickName = userinfo.NickName };
            }
            else
            {
                jr.Data = new { isSuccess = false, errorMessage = "User not find" };
            }
            return jr;
        }

        public JsonResult OAuthInit()
        {
            JsonResult jr = new JsonResult();
            jr.ContentType = "text/json";
            jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            try
            {
                Client Sina = null;
                OAuth oauth = new OAuth(CnfgConst.Sina_AppKey, CnfgConst.Sina_AppSecret, CnfgConst.Sina_CallbackUrl);

                Sina = new Client(oauth); //用cookie里的accesstoken来实例化OAuth，这样OAuth就有操作权限了
                //if (!string.IsNullOrEmpty(code))//暂时无法获取到传回的URL
                //{
                //    var token = oauth.GetAccessTokenByAuthorizationCode(code);
                //    string accessToken = token.Token;

                //    Response.Cookies["AccessToken"].Value = accessToken;
                //    jr.Data = new { isSuccess = true };
                //    //Response.Redirect("Default.aspx");
                //}
                //else
                {
                    string url = oauth.GetAuthorizeURL();
                    jr.Data = new { isSuccess = false, url = url };
                }
                return jr;

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public JsonResult OAuthLogin(string code)
        {
            JsonResult jr = new JsonResult();
            jr.ContentType = "text/json";
            jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            try
            {
                Client Sina = null;
                OAuth oauth = new OAuth(CnfgConst.Sina_AppKey, CnfgConst.Sina_AppSecret, CnfgConst.Sina_CallbackUrl);

                Sina = new Client(oauth); //用cookie里的accesstoken来实例化OAuth，这样OAuth就有操作权限了
                if (!string.IsNullOrEmpty(code))//暂时无法获取到传回的URL
                {
                    var token = oauth.GetAccessTokenByAuthorizationCode(code);
                    string accessToken = token.Token;

                    Response.Cookies["AccessToken"].Value = accessToken;

                    var userinfo = Sina.API.Dynamic.Users.Show(token.UID.ToString(), "");

                    jr.Data = new { isSuccess = true, Data = userinfo.ToString() };
                    //Response.Redirect("Default.aspx");
                }
                else
                {
                    jr.Data = new { isSuccess = false };
                }
                return jr;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public JsonResult GetUserLoginStatus()
        {
            JsonResult jr = new JsonResult();
            jr.ContentType = "text/json";
            jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            if (Session["CurrentUser"] != null)
            {
                var user = DBTools.GetUserInfoByLoginName(Session["CurrentUser"].ToString());
                jr.Data = new { isSuccess = true, CurrentUser = Session["CurrentUser"].ToString(), NickName = user.NickName };
            }
            else
            {
                jr.Data = new { isSuccess = false, errorMessage = "User not Login" };
            }
            return jr;
        }

        public JsonResult UserLogout()
        {
            JsonResult jr = new JsonResult();
            jr.ContentType = "text/json";
            jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            Session["CurrentUser"] = null;
            jr.Data = new { isSuccess = true };
            return jr;
        }
    }
}
