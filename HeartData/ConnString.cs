using System;
using System.Collections.Generic;
using System.Text;

namespace HeartData
{
    public class ConnString
    {
        /// <summary>
        /// �õ���̳Sql�����ַ���
        /// </summary>
        public static string GetConString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"].ToString();
    }
}
