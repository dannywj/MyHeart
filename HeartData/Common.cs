using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Xml;
using System.Data;
using System.Web;
using System.Web.Script.Serialization;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace HeartData
{
    public class Common
    {
        public static bool SendMail(string To, string Subject, string Body)
        {
            try
            {
                MailAddress from = new MailAddress(HeartData.CnfgConst.ALertRegisterFrom);
                MailAddress to = new MailAddress(To);
                MailMessage message = new MailMessage(from, to);
                message.Subject = Subject;
                message.Body = Body;
                message.Priority = MailPriority.Normal;
                message.IsBodyHtml = true;
                SmtpClient client = new SmtpClient(HeartData.CnfgConst.server);
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(HeartData.CnfgConst.SysUserName, HeartData.CnfgConst.SysPwd);
                client.Send(message);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SendMailWithTheme(string ToMailAddress, string ThemeName, string[] ParamList)
        {
            bool result = false;

            if (ThemeName == HeartData.MailTheme.HeartNewSignup)
            {
                string subject = "欢迎来到许愿墙的世界";
                string content = string.Format(@"{0} 欢迎来到许愿墙的世界，这里将是梦想开始的地方！
                <br/>请牢记您的登录密码：{1}<br/>期待我们与您一起实现一个又一个大大小小的愿望！", ParamList[0], ParamList[1]);

                if (SendMail(ToMailAddress, subject, content))
                    return true;
            }
            return result;
        }

        public static bool CheckEmail(string strEmail)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(strEmail, @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
        }

        /// <summary>  
        /// 将对象转换为 JSON 字符串。  
        /// </summary>  
        /// <param name="obj">要序列化的对象。</param>  
        /// <returns>序列化的 JSON 字符串。</returns>  
        public static string JsonSerialize(object obj)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Serialize(obj);
        }
        /// <summary>  
        /// 将指定的 JSON 字符串转换为 T 类型的对象。  
        /// </summary>  
        /// <typeparam name="T">所生成对象的类型。</typeparam>  
        /// <param name="input">要进行反序列化的 JSON 字符串。</param>  
        /// <param name="def">反序列化失败时返回的默认值。</param>  
        /// <returns>反序列化的对象。</returns>  
        public static T JosnDeserialize<T>(string input, T def)
        {
            if (string.IsNullOrEmpty(input))
                return def;
            try
            {
                JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                return jsSerializer.Deserialize<T>(input);
            }
            catch (InvalidOperationException)
            {
                return def;
            }
        }

        #region dataTable转换成Json格式

        public static string CreateJsonParameters(DataTable dt)
        {
            /**/
            /**/
            /**/
            /* /****************************************************************************
      * Without goingin to the depth of the functioning of this Method, i will try to give an overview
      * As soon as this method gets a DataTable it starts to convert it into JSON String,
      * it takes each row and in each row it grabs the cell name and its data.
      * This kind of JSON is very usefull when developer have to have Column name of the .
      * Values Can be Access on clien in this way. OBJ.HEAD[0].<ColumnName>
      * NOTE: One negative point. by this method user will not be able to call any cell by its index.
     * *************************************************************************/
            StringBuilder JsonString = new StringBuilder();
            //Exception Handling        
            if (dt != null && dt.Rows.Count > 0)
            {
                JsonString.Append("{ ");
                JsonString.Append("\"table\":[ ");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    JsonString.Append("{ ");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j < dt.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + "\"" + dt.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == dt.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + "\"" + dt.Rows[i][j].ToString() + "\"");
                        }
                    }
                    /**/
                    /**/
                    /**/
                    /*end Of String*/
                    if (i == dt.Rows.Count - 1)
                    {
                        JsonString.Append("} ");
                    }
                    else
                    {
                        JsonString.Append("}, ");
                    }
                }
                JsonString.Append("]}");
                return JsonString.ToString();
            }
            else
            {
                return null;
            }
        }
        #endregion dataTable转换成Json格式
    }

    //public class JSONModelBinder<T> : IModelBinder
    //{
    //    public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    //    {
    //        // 从请求中获取提交的参数数据 
    //        var json = controllerContext.HttpContext.Request.Form[bindingContext.ModelName] as string;
    //        // 霍力强替换后台自动创建的ExtensionData
    //        json = json.Replace("\"ExtensionData\":{},", string.Empty);
    //        // 提交参数是对象 )
    //        if (json.StartsWith("{") && json.EndsWith("}"))
    //        {
    //            JavaScriptSerializer js = new JavaScriptSerializer();
    //            object obj = js.Deserialize<T>(json);
    //            return obj;
    //        }

    //        // 提交参数是数组 
    //        if (json.StartsWith("[") && json.EndsWith("]"))
    //        {
    //            JavaScriptSerializer js = new JavaScriptSerializer();
    //            List<T> obj = js.Deserialize<List<T>>(json);

    //            return obj;
    //        }

    //        return null;
    //    }
    //}
}
