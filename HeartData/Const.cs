using System;
using System.Collections.Generic;
using System.Text;

namespace HeartData
{
    public static class CnfgConst
    {
        public static string server = System.Configuration.ConfigurationManager.AppSettings["server"].ToString();
        public static string ALertRegisterTo = System.Configuration.ConfigurationManager.AppSettings["ALertRegisterTo"].ToString();
        public static string ALertRegisterFrom = System.Configuration.ConfigurationManager.AppSettings["ALertRegisterFrom"].ToString();
        public static string ALertSubjectJoinTo = System.Configuration.ConfigurationManager.AppSettings["ALertSubjectJoinTo"].ToString();
        public static string ALertSubjectJoinFrom = System.Configuration.ConfigurationManager.AppSettings["ALertSubjectJoinFrom"].ToString();
        public static string ALertRegisterSubject = "网络学堂新用户注册审核";
        public static string ALertRegisterBody = "网络学堂有用户注册，需要您以管理员身份登陆系统处理新注册用户！  登陆地址：" + System.Configuration.ConfigurationManager.AppSettings["WebSite"].ToString();

        public static string ALertSubjectJoinSubject = "网络学堂学生课程审批";
        public static string ALertSubjectJoinBody = "网络学堂有学生注册您的课程，需要您以教师身份登陆系统处理课程审批请求！  登陆地址：" + System.Configuration.ConfigurationManager.AppSettings["WebSite"].ToString();

        public static string SysUserName = System.Configuration.ConfigurationManager.AppSettings["MailUserName"].ToString();
        public static string SysPwd = System.Configuration.ConfigurationManager.AppSettings["MailPWD"].ToString();

        public static string Sina_AppKey = System.Configuration.ConfigurationManager.AppSettings["Sina_AppKey"].ToString();
        public static string Sina_AppSecret = System.Configuration.ConfigurationManager.AppSettings["Sina_AppSecret"].ToString();
        public static string Sina_CallbackUrl = System.Configuration.ConfigurationManager.AppSettings["Sina_CallbackUrl"].ToString();

    }

    public static class MailTheme
    {
        public static string HeartNewSignup = "HeartNewSignup";
    }
}
