using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Model;
using MySql.Data.MySqlClient;

namespace GameServer.DAO
{
    class ResultDAO
    {
        public Result GetResultByUserId(MySqlConnection mysqlConn,int userId)
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from result where userid= @userid", mysqlConn);
                cmd.Parameters.AddWithValue("userid", userId);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    int id = reader.GetInt32("id");
                    int totalCount = reader.GetInt32("totalcount");
                    int winCount = reader.GetInt32("wincount");

                    Result res = new Result(id, userId, totalCount, winCount);
                    return res;
                } else
                {
                    Result res = new Result(-1, userId, 0, 0);
                    return res;
                } }
            catch (Exception e)
            {
                Console.WriteLine("在GetResultByUserId的时候出现异常："+e);
            }
            finally
            {
                if (reader != null) reader.Close();
            }
            return null;
        }
        public void UpdateOrAddResult(MySqlConnection conn,Result res)
        {
            try
            {
                MySqlCommand cmd = null;
                if (res.ID <= -1)
                {
                    cmd = new MySqlCommand("insert into result set totalcount=@totalcount,wincount=@wincount,userid=@userid", conn);
                }
                else
                {
                    cmd = new MySqlCommand("update result set totalcount=@totalcount,wincount=@wincount where userid=@userid", conn);
                }
                cmd.Parameters.AddWithValue("totalcount", res.TotalCount);
                cmd.Parameters.AddWithValue("wincount", res.WinCount);
                cmd.Parameters.AddWithValue("userid", res.UserID);
                cmd.ExecuteNonQuery();
                if (res.ID <= -1)
                {
                    Result tempRes = GetResultByUserId(conn,res.UserID);
                    res.ID = tempRes.ID;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("在UpdateOrAddResult的时候出现异常：" + e);
            }
        }
    }
}
