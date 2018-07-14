using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Mysql数据库操作
{
    class Program
    {
        static void Main(string[] args)
        {
            string connStr = "Database=test1;datasource=127.0.0.1;port=3306;user=root;pwd=root";
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            #region QUERY
            //MySqlCommand cmd = new MySqlCommand("select * from user", conn);
            //MySqlDataReader reader = cmd.ExecuteReader();
            //while (reader.Read())
            //{
            //    string name = reader.GetString("name");
            //    string age = reader.GetString("age");
            //    Console.WriteLine("name:" + name + "age:" + age);
            //}
            #endregion

            #region INSERT
            string name = new Random().Next(1, 100).ToString(); int age = new Random().Next(1, 100); int id = new Random().Next(11, 100);
            //string sql = "insert into user set name='" + name + "'" + ",age='" + age + "'" + ",id = '" + id + "'";
            //MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlCommand cmd = new MySqlCommand("insert into user set name=@name,age=@age", conn);
            //cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("name", name);
            cmd.Parameters.AddWithValue("age", age);
            int count = cmd.ExecuteNonQuery();
            Console.WriteLine(count);
            #endregion

            #region DELETE
            //int id = 3;
            //MySqlCommand cmd = new MySqlCommand("delete from user where id>@id ", conn);
            //cmd.Parameters.AddWithValue("id", id);
            //int row= cmd.ExecuteNonQuery();
            //Console.WriteLine(row);
            #endregion

            #region UPDATE
            //int id = 1;string name = "bb";int age = 2;
            //MySqlCommand cmd = new MySqlCommand("update user set name=@name, age=@age where id=@id", conn);
            //cmd.Parameters.AddWithValue("id",id);
            //cmd.Parameters.AddWithValue("name", name);
            //cmd.Parameters.AddWithValue("age", age);
            //int row= cmd.ExecuteNonQuery();
            //Console.WriteLine(row);
            #endregion

            conn.Close();
            Console.ReadKey();
        }
    }
}
