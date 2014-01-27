﻿using System;
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

        public static bool RegisterNewUser(string userName, string password, bool useEmail)
        {
            string sql = "  insert into ht_userInfo(loginName,useEmail,password)values(@userName,@use_email,@password)";
            SqlParameter[] parameters = {
					new SqlParameter("@userName", SqlDbType.NVarChar),
                    new SqlParameter("@password", SqlDbType.NVarChar),
                    new SqlParameter("@use_email", SqlDbType.Int)
                                        };
            parameters[0].Value = userName;
            parameters[1].Value = password;
            parameters[2].Value = useEmail ? "1" : "0";

            object o = SqlHelper.ExecuteNonQuery(ConnString.GetConString, CommandType.Text, sql, parameters);

            if (Convert.ToInt32(o) > 0)
            {
                if (HeartData.Common.CheckEmail(userName))
                {
                    HeartData.Common.SendMailWithTheme(userName, HeartData.MailTheme.HeartNewSignup);
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
                string sql = " select count(*) from  ht_userInfo where loginName=@userName and password=@password ";
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
                    new SqlParameter("@pubID", SqlDbType.Int),
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
                parameters[1].Value = 0;//newHeart.pubid;
                parameters[2].Value = newHeart.Puber;
                parameters[3].Value = newHeart.Joiner;
                parameters[4].Value = newHeart.Contact;
                parameters[5].Value = 1;//newHeart.bgimage;
                parameters[6].Value = newHeart.HeartContent;
                parameters[7].Value = DateTime.Now;
                parameters[8].Value = newHeart.FinishDate;
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
