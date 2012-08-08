using System;
using System.Collections.Generic;
using System.Text;

namespace HeartData
{
    public class ConnString
    {
        /// <summary>
        /// 得到论坛Sql链接字符串
        /// </summary>
        public static string GetConString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"].ToString();
    }
}
