using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Servers;
using GameServer.Model;
using GameServer.DAO;

namespace GameServer.Controller
{
    class UserController:BaseController
    {
        private ResultDAO resultDAO=new ResultDAO();
        private UserDAO userDAO = new UserDAO();
        public UserController()
        {
            requestCode = RequestCode.User;
        }
        public string Login(string data,Client client)
        {
            string[] strs = data.Split(',');
            User user = userDAO.VerifyUser(client.MysqlConn,strs[0],strs[1]);
            if (user == null)
            {
                return ((int)ReturnCode.Fail).ToString();
            }
            else
            {
                Result res = resultDAO.GetResultByUserId(client.MysqlConn,user.ID);

                return string.Format("{0},{1},{2},{3}",((int)ReturnCode.Success).ToString(),user.ID,res.TotalCount,res.WinCount);
            }
        }
        public string Register(string data,Client client)
        {
            string[] strs = data.Split(',');
            string username = strs[0];string password = strs[1];
            bool res = userDAO.GetUserByUsername(client.MysqlConn, username);
            if (res)
            {
                return ((int)ReturnCode.Fail).ToString();
            }
            userDAO.AddUser(client.MysqlConn, username,password);
            return ((int)ReturnCode.Success).ToString();
        }
    }
}
