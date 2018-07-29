using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Model;
using MySql.Data.MySqlClient;

namespace GameServer.DAO
{
    class UserDAO
    {
        public User VerifyUser(MySqlConnection mySqlConn,string username,string password)
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from User where username=@username and password=@password",mySqlConn);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("password", password);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    int id = reader.GetInt32("id");
                    User user = new User(id ,username,password);
                    return user;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("在VerifyUser的时候出现异常"+e);
            }
            finally
            {
                if (reader != null) reader.Close();
            }
            return null;
        }
        public bool GetUserByUsername(MySqlConnection mysqlConn,string username)
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from User where username=@username",mysqlConn);
                cmd.Parameters.AddWithValue("username", username);
                reader = cmd.ExecuteReader();
                return reader.HasRows;
            }
            catch(Exception e)
            {
                Console.WriteLine("在GetUserByUsername时候出现异常"+e);
            }
            finally
            {
                if (reader != null) reader.Close();
            }
            return false;
        }
        public void AddUser(MySqlConnection mySqlConn, string username,string password)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("insert into User set username=@username , password=@password",mySqlConn);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("password", password);
                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                Console.WriteLine("在AddUser的时候出现异常"+e);
            }
        }
    }
}
