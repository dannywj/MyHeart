using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace HeartData
{
    public class DBTools
    {
        public static DataTable getAllHeart()
        {
            DataTable dt = null;
            string sql = " select top 24 *, NewID() as random from ht_heartInfo order by random";
            try
            {
                dt = SqlHelper.ExecuteDataset(ConnString.GetConString, CommandType.Text, sql.ToString()).Tables[0];
            }
            catch
            {

            }
            return dt;
        }

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

        public static bool RegisterNewUser(string userName, string password, bool useEmail, string userNickName)
        {
            string sql = "  insert into ht_userInfo(loginName,useEmail,password,nickName)values(@userName,@use_email,@password,@nickName)";
            SqlParameter[] parameters = {
					new SqlParameter("@userName", SqlDbType.NVarChar),
                    new SqlParameter("@password", SqlDbType.NVarChar),
                    new SqlParameter("@use_email", SqlDbType.Int),
                    new SqlParameter("@nickName", SqlDbType.NVarChar),
                                        };
            parameters[0].Value = userName;
            parameters[1].Value = password;
            parameters[2].Value = useEmail ? "1" : "0";
            parameters[3].Value = userNickName;

            object o = SqlHelper.ExecuteNonQuery(ConnString.GetConString, CommandType.Text, sql, parameters);

            if (Convert.ToInt32(o) > 0)
            {
                if (HeartData.Common.CheckEmail(userName))
                {
                    string[] ParamList = { userNickName, password };
                    HeartData.Common.SendMailWithTheme(userName, HeartData.MailTheme.HeartNewSignup, ParamList);
                }
                return true;
            }
            else
            {
                return false;
            }


        }

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
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool PubNewHeart(NewHeart newHeart)
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
                       ,[station])
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
                       ,@station)");
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
                    new SqlParameter("@station", SqlDbType.Int) 
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
                object o = SqlHelper.ExecuteNonQuery(ConnString.GetConString, CommandType.Text, sql, parameters);
            }
            catch (Exception)
            {

                return false;
            }
            return true;
        }

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
        //pub tools
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
    }
}
