using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace HeartData
{
    public class DBTools
    {
        //===================================================
        //=======================Heart=======================
        //===================================================
        /// <summary>
        /// 获取公开心愿
        /// </summary>
        /// <returns></returns>
        public static DataTable getAllHeart()
        {
            DataTable dt = null;
            string sql = " select top 24 *, NewID() as random from ht_heartInfo where isPrivate=0 and (station=0 or( station=1 and addDate>'" + DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd") + "') )  order by random";
            try
            {
                dt = SqlHelper.ExecuteDataset(ConnString.GetConString, CommandType.Text, sql.ToString()).Tables[0];
            }
            catch
            {

            }
            return dt;
        }
        /// <summary>
        /// 获取全部心愿，包含自己的隐私心愿
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public static DataTable getAllHeart(string currentUser)
        {
            DataTable dt = null;
            string sql = " select top 24 *, NewID() as random from ht_heartInfo where (isPrivate=0 OR (isPrivate=1 AND pubId='" + currentUser + "') )and (station=0 or( station=1 and addDate>'" + DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd") + "') ) order by random";
            try
            {
                dt = SqlHelper.ExecuteDataset(ConnString.GetConString, CommandType.Text, sql.ToString()).Tables[0];
            }
            catch
            {

            }
            return dt;
        }

        /// <summary>
        /// 发布心愿
        /// </summary>
        /// <param name="newHeart">心愿内容</param>
        /// <param name="joinerLoginName">参与者登录名（可选）</param>
        /// <returns></returns>
        public static bool PubNewHeart(NewHeart newHeart, string joinerLoginName = "")
        {
            try
            {
                string sql = string.Format(@"INSERT INTO ht_heartInfo
                       ([title]
                       ,[pubID]
                       ,[pubName]
                       ,[participator]
                       ,[contact]
                       ,[bgImage]
                       ,[content]
                       ,[beginDate]
                       ,[endDate]
                       ,[heartLevel]
                       ,[station]
                       ,[isPrivate])
                 VALUES
                       (@title
                       ,@pubID
                       ,@pubName 
                       ,@participator
                       ,@contact 
                       ,@bgImage 
                       ,@content 
                       ,@beginDate 
                       ,@endDate 
                       ,@heartLevel 
                       ,@station
                       ,@isPrivate)");
                SqlParameter[] parameters = {
					new SqlParameter("@title", SqlDbType.NVarChar),
                    new SqlParameter("@pubID", SqlDbType.NVarChar),
                    new SqlParameter("@pubName", SqlDbType.NVarChar),
                    new SqlParameter("@participator", SqlDbType.NVarChar),
                    new SqlParameter("@contact", SqlDbType.NVarChar),
                    new SqlParameter("@bgImage", SqlDbType.NVarChar),
                    new SqlParameter("@content", SqlDbType.NVarChar),
                    new SqlParameter("@beginDate", SqlDbType.DateTime),
                    new SqlParameter("@endDate", SqlDbType.DateTime),
                    new SqlParameter("@heartLevel", SqlDbType.Int),
                    new SqlParameter("@station", SqlDbType.Int) ,
                    new SqlParameter("@isPrivate", SqlDbType.Int) 
                                        };
                parameters[0].Value = newHeart.Title;
                parameters[1].Value = newHeart.PubId;
                parameters[2].Value = newHeart.Puber;
                parameters[3].Value = newHeart.Joiner;
                parameters[4].Value = newHeart.Contact;
                parameters[5].Value = 1;//newHeart.bgimage;
                parameters[6].Value = newHeart.HeartContent;
                parameters[7].Value = DateTime.Now;
                parameters[8].Value = newHeart.FinishDate == "" ? null : newHeart.FinishDate;
                parameters[9].Value = newHeart.HeartLevel;
                parameters[10].Value = 0;//newHeart.station;
                parameters[11].Value = newHeart.IsPrivate;
                object o = SqlHelper.ExecuteNonQuery(ConnString.GetConString, CommandType.Text, sql, parameters);

                //获取heartId
                string sql_maxId = "select max( heartId) from ht_heartInfo ";
                object id = SqlHelper.ExecuteScalar(ConnString.GetConString, CommandType.Text, sql_maxId);

                //绑定参与关系
                BindUserJoin(int.Parse(id.ToString()), newHeart.PubId, newHeart.Joiner, joinerLoginName);
            }
            catch (Exception)
            {

                return false;
            }
            return true;
        }

        /// <summary>
        /// 绑定参与者 
        /// </summary>
        /// <param name="heartId">心愿id</param>
        /// <param name="pubLoginName">发布者登录名</param>
        /// <param name="joinerNickName">参与者昵称</param>
        /// <param name="joinerLoginName">参与者登录名</param>
        public static void BindUserJoin(int heartId, string pubLoginName, string joinerNickName, string joinerLoginName)
        {
            try
            {
                string sql = string.Format(@"insert into ht_userJoin
                (heartId, pubLoginName,joinerNickName,joinerLoginName,isRead)
                 VALUES
                       ({0}
                       ,'{1}'
                       ,'{2}'
                       ,'{3}'
                       ,'false' )", heartId, pubLoginName, joinerNickName, joinerLoginName);
                object o = SqlHelper.ExecuteNonQuery(ConnString.GetConString, CommandType.Text, sql);
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 根据登录名获取心愿信息
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <returns></returns>
        public static List<NewHeart> GetHeartsByLoginName(string loginName)
        {
            DataTable dt = null;
            List<NewHeart> list = new List<NewHeart>();

            string sql = " select * from ht_heartInfo where pubId='" + loginName + "' order by heartId desc";
            try
            {
                dt = SqlHelper.ExecuteDataset(ConnString.GetConString, CommandType.Text, sql.ToString()).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        NewHeart nh = new NewHeart();
                        nh.Contact = dt.Rows[i]["contact"].ToString();
                        if (string.IsNullOrEmpty(dt.Rows[i]["endDate"].ToString()))
                        {
                            nh.FinishDate = string.Empty;
                        }
                        else
                        {
                            nh.FinishDate = Convert.ToDateTime(dt.Rows[i]["endDate"].ToString()).ToString("yyyy-MM-dd");
                        }
                        nh.HeartContent = dt.Rows[i]["content"].ToString();
                        nh.HeartLevel = int.Parse(dt.Rows[i]["heartLevel"].ToString());
                        nh.Joiner = dt.Rows[i]["participator"].ToString();
                        nh.Puber = dt.Rows[i]["pubName"].ToString();
                        nh.Title = dt.Rows[i]["title"].ToString();
                        nh.Station = int.Parse(dt.Rows[i]["station"].ToString());
                        nh.HeartId = int.Parse(dt.Rows[i]["heartId"].ToString());
                        list.Add(nh);
                    }
                }
            }
            catch
            {
                return null;
            }
            return list;
        }

        /// <summary>
        /// 更新心愿状态为已完成
        /// </summary>
        /// <param name="station">状态</param>
        /// <param name="heartid">心愿id</param>
        /// <returns></returns>
        public static bool UpdateHeartStation(int station, int heartid)
        {
            try
            {
                string sql = string.Format(@"update ht_heartInfo set station={0} where heartId={1}", station, heartid);

                object o = SqlHelper.ExecuteNonQuery(ConnString.GetConString, CommandType.Text, sql);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取心愿条数
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public static string GetHeartsCount(string loginName)
        {
            DataTable dt = new DataTable();
            string sql = string.Format(@"
            select (
	            select count(*) from ht_heartInfo
	            where pubId='{0}'
            ) as allcount,
            (
	            select count(*) from ht_heartInfo
	            where pubId='{0}' and station=1
            ) as okcount", loginName);
            string allcount = string.Empty;
            string okcount = string.Empty;
            try
            {
                dt = SqlHelper.ExecuteDataset(ConnString.GetConString, CommandType.Text, sql.ToString()).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    allcount = dt.Rows[0]["allcount"].ToString();
                    okcount = dt.Rows[0]["okcount"].ToString();
                }
            }
            catch
            {
                return null;
            }
            return allcount + "_" + okcount;
        }

        /// <summary>
        /// 获取个人未读新动态条数
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public static int GetNewMessageCount(string loginName)
        {
            string sql = string.Format(@"
            select count(*) as newcount from ht_userJoin where joinerLoginName='{0}' and isRead='false' ", loginName);
            int allcount = 0;
            try
            {
                object o = SqlHelper.ExecuteScalar(ConnString.GetConString, CommandType.Text, sql.ToString());
                allcount = Convert.ToInt32(o);
            }
            catch
            {
                return 0;
            }
            return allcount;
        }

        public static List<NewHeart> GetNewMessage(string loginName)
        {
            DataTable dt = null;
            List<NewHeart> list = new List<NewHeart>();

            string sql = string.Format(@"select *  from ht_userJoin j
                    inner join ht_heartInfo h on j.heartId=h.heartId
                    where joinerLoginName='{0}' and isRead='false' ", loginName);
            try
            {
                dt = SqlHelper.ExecuteDataset(ConnString.GetConString, CommandType.Text, sql.ToString()).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        NewHeart nh = new NewHeart();
                        nh.Contact = dt.Rows[i]["contact"].ToString();
                        if (string.IsNullOrEmpty(dt.Rows[i]["endDate"].ToString()))
                        {
                            nh.FinishDate = string.Empty;
                        }
                        else
                        {
                            nh.FinishDate = Convert.ToDateTime(dt.Rows[i]["endDate"].ToString()).ToString("yyyy-MM-dd");
                        }
                        nh.HeartContent = dt.Rows[i]["content"].ToString();
                        nh.HeartLevel = int.Parse(dt.Rows[i]["heartLevel"].ToString());
                        nh.Joiner = dt.Rows[i]["participator"].ToString();
                        nh.Puber = dt.Rows[i]["pubName"].ToString();
                        nh.Title = dt.Rows[i]["title"].ToString();
                        nh.Station = int.Parse(dt.Rows[i]["station"].ToString());
                        nh.HeartId = int.Parse(dt.Rows[i]["heartId"].ToString());
                        list.Add(nh);
                    }
                }
            }
            catch
            {
                return null;
            }
            return list;
        }

        public static bool UpdateNewMessageStatus(string loginName)
        {
            try
            {
                string sql = string.Format(@"update ht_userJoin set isRead='true' where joinerLoginName='{0}' and isRead='false'", loginName);

                object o = SqlHelper.ExecuteNonQuery(ConnString.GetConString, CommandType.Text, sql);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        //===================================================
        //=======================User========================
        //===================================================
        /// <summary>
        /// 检查新用户是否可以使用
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool CheckNewUser(string userName)
        {
            string sql = "  select count(*) from ht_userInfo where loginName=@userName";
            SqlParameter[] parameters = {
					new SqlParameter("@userName", SqlDbType.NVarChar)
                                        };
            parameters[0].Value = userName;
            DataSet ds = SqlHelper.ExecuteDataset(ConnString.GetConString, CommandType.Text, sql, parameters);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                if (int.Parse(ds.Tables[0].Rows[0][0].ToString()) > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 注册新用户
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="useEmail"></param>
        /// <param name="userNickName"></param>
        /// <returns></returns>
        public static bool RegisterNewUser(string userName, string password, bool useEmail, string userNickName, string nickNamePinYin = "")
        {
            string sql = "  insert into ht_userInfo(loginName,useEmail,password,nickName,nickNamePy)values(@userName,@use_email,@password,@nickName,@nickNamePy)";
            SqlParameter[] parameters = {
					new SqlParameter("@userName", SqlDbType.NVarChar),
                    new SqlParameter("@password", SqlDbType.NVarChar),
                    new SqlParameter("@use_email", SqlDbType.Int),
                    new SqlParameter("@nickName", SqlDbType.NVarChar),
                    new SqlParameter("@nickNamePy", SqlDbType.VarChar),
                                        };
            parameters[0].Value = userName;
            parameters[1].Value = password;
            parameters[2].Value = useEmail ? "1" : "0";
            parameters[3].Value = userNickName;
            parameters[4].Value = nickNamePinYin;

            object o = SqlHelper.ExecuteNonQuery(ConnString.GetConString, CommandType.Text, sql, parameters);

            if (Convert.ToInt32(o) > 0)
            {
                //if (HeartData.Common.CheckEmail(userName))
                //{
                //    string[] ParamList = { userNickName, password };
                //HeartData.Common.SendMailWithTheme(userName, HeartData.MailTheme.HeartNewSignup, ParamList);
                // 在新线程中运行
                if (HeartData.Common.CheckEmail(userName))
                {
                    string[] ParamList = { userNickName, password };
                    ThreadStart starter = delegate { SendMailWithTheme(userName, ParamList); };
                    Thread t = new Thread(starter);
                    t.Start();
                }
                //}
                return true;
            }
            else
            {
                return false;
            }


        }

        /// <summary>
        /// 发送用户通知邮件
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="ParamList"></param>
        public static void SendMailWithTheme(string userName, string[] ParamList)
        {
            HeartData.Common.SendMailWithTheme(userName, HeartData.MailTheme.HeartNewSignup, ParamList);
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool UserLogin(string userName, string password)
        {
            try
            {
                string sql = " select * from  ht_userInfo where loginName=@userName and password=@password ";
                SqlParameter[] parameters = {
					new SqlParameter("@userName", SqlDbType.NVarChar),
                    new SqlParameter("@password", SqlDbType.NVarChar)
                                        };
                parameters[0].Value = userName;
                parameters[1].Value = password;
                object o = SqlHelper.ExecuteScalar(ConnString.GetConString, CommandType.Text, sql, parameters);
                if (Convert.ToInt32(o) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public static User GetUserInfoByLoginName(string loginName)
        {
            User u = new User();
            string sql = " select * from  ht_userInfo where loginName='" + loginName + "'";
            DataSet ds = SqlHelper.ExecuteDataset(ConnString.GetConString, CommandType.Text, sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                u.NickName = ds.Tables[0].Rows[0]["nickName"].ToString();
            }
            return u;
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        public static List<User> getAllUser()
        {
            List<User> list = new List<User>();
            DataTable dt = null;
            string sql = "select * from ht_userInfo where status=0 ";
            try
            {
                dt = SqlHelper.ExecuteDataset(ConnString.GetConString, CommandType.Text, sql.ToString()).Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    User u = new User();
                    u.LoginName = dt.Rows[i]["loginName"].ToString();
                    u.NickName = dt.Rows[i]["nickName"].ToString();
                    u.Status = int.Parse(dt.Rows[i]["status"].ToString());
                    list.Add(u);
                }
            }
            catch
            {

            }
            return list;
        }

        /// <summary>
        /// 获取所有用户by key
        /// </summary>
        /// <returns></returns>
        public static List<User> getAllUser(string key, string currentUserLoginName)
        {
            List<User> list = new List<User>();
            DataTable dt = null;
            string sql = string.Format("select top 10 * from ht_userInfo where (nickName like '%{0}%' or nickNamePy like '%{0}%') and loginName<>'{1}'  and status=0 ", key, currentUserLoginName);
            try
            {
                dt = SqlHelper.ExecuteDataset(ConnString.GetConString, CommandType.Text, sql.ToString()).Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    User u = new User();
                    u.LoginName = dt.Rows[i]["loginName"].ToString();
                    u.NickName = dt.Rows[i]["nickName"].ToString();
                    u.Status = int.Parse(dt.Rows[i]["status"].ToString());
                    list.Add(u);
                }
            }
            catch
            {

            }
            return list;
        }

        public static bool UpdateUserInfo(string loginName, string userNickName, string nickNamePinYin)
        {
            string sql = "update ht_userInfo set nickName=@nickName,nickNamePy=@nickNamePy where loginName='" + loginName + "'";
            SqlParameter[] parameters = {
                    new SqlParameter("@nickName", SqlDbType.NVarChar),
                    new SqlParameter("@nickNamePy", SqlDbType.VarChar),
                                        };
            parameters[0].Value = userNickName;
            parameters[1].Value = nickNamePinYin;

            try
            {
                object o = SqlHelper.ExecuteNonQuery(ConnString.GetConString, CommandType.Text, sql, parameters);
                return true;
            }
            catch (Exception)
            {

            }
            return false;
        }
        //===================================================
        //=======================Others======================
        //===================================================
        #region Message Pub Tools
        public static bool PubNewMessage(string date, string content, string writer)
        {
            try
            {
                string sql = string.Format(@"INSERT INTO ht_message ( writer, content, pub_date )
                VALUES  ( '{0}', N'{1}',  '{2}')", writer, content, date);

                object o = SqlHelper.ExecuteNonQuery(ConnString.GetConString, CommandType.Text, sql);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static int GetMessageCount()
        {
            try
            {
                string sql = string.Format(@"select count(*) from ht_message");
                object o = SqlHelper.ExecuteScalar(ConnString.GetConString, CommandType.Text, sql);
                return Convert.ToInt32(o);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static string GetLastMessageDate()
        {
            try
            {
                string sql = string.Format(@"SELECT TOP 1 Convert(varchar(10),[pub_date],120) AS pub_date from ht_message ORDER BY pub_date desc");
                object o = SqlHelper.ExecuteScalar(ConnString.GetConString, CommandType.Text, sql);
                return o.ToString();
            }
            catch (Exception)
            {
                return "";
            }

        }

        //======Message======
        public static List<MessageItem> GetRandomMessage()
        {
            DataTable dt = null;
            List<MessageItem> list = new List<MessageItem>();

            string sql = @" SELECT top 3 NewID() as random, Convert(varchar(10),[pub_date],120) as pubdate,[writer],[content]
                        FROM [ht_message]
                        where writer='juejue'
                        ORDER BY random desc";
            try
            {
                dt = SqlHelper.ExecuteDataset(ConnString.GetConString, CommandType.Text, sql.ToString()).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MessageItem item = new MessageItem();
                        item.Content = dt.Rows[i]["content"].ToString().Length > 100 ? (dt.Rows[i]["content"].ToString().Substring(0, 100) + "...") : (dt.Rows[i]["content"].ToString());
                        item.PubDate = dt.Rows[i]["PubDate"].ToString();
                        item.Writer = dt.Rows[i]["Writer"].ToString();
                        list.Add(item);
                    }
                }
            }
            catch
            {
                return null;
            }
            return list;
        }

        public static List<MessageItem> GetMessageByDate(string date)
        {
            DataTable dt = null;
            List<MessageItem> list = new List<MessageItem>();

            string sql = @"SELECT [writer],[content],Convert(varchar(10),[pub_date],120) as pubdate FROM ht_message WHERE pub_date='" + date + "' order by writer desc";
            try
            {
                dt = SqlHelper.ExecuteDataset(ConnString.GetConString, CommandType.Text, sql.ToString()).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MessageItem item = new MessageItem();
                        item.Content = Common.ChangeTxtFace(dt.Rows[i]["content"].ToString());
                        item.PubDate = dt.Rows[i]["PubDate"].ToString();
                        item.Writer = dt.Rows[i]["Writer"].ToString();
                        list.Add(item);
                    }
                }
            }
            catch
            {
                return null;
            }
            return list;
        }
        #endregion
    }
}
