using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common;
using MySql.Data.MySqlClient;
using GameServer.Tool;
using GameServer.Model;

namespace GameServer.Servers
{
    class Client
    {
        private Socket clientSocket;
        private Server server;
        private Message msg = new Message();
        private MySqlConnection mysqlConn;
        private User user;
        private Result result;
        public MySqlConnection MysqlConn
        {
            get { return mysqlConn; }
        }
        public Client() { }
        public Client(Socket clientSocket,Server server)
        {
            this.clientSocket = clientSocket;
            this.server = server;
            mysqlConn = ConnHelper.Connect();
        }
        public void Start()
        {
            if (clientSocket == null || clientSocket.Connected == false) return;
            clientSocket.BeginReceive(msg.Data,msg.StartIndex,msg.RemainSize,SocketFlags.None,ReceiveCallback,null);
        }
        private void ReceiveCallback(IAsyncResult ar)
        {
            try{
                if (clientSocket == null || clientSocket.Connected == false) return;
                int count = clientSocket.EndReceive(ar);
                if (count == 0)
                {
                    Close();
                }
                //Console.WriteLine(count);
                msg.ReadMessage(count,OnProcessMessage);
                Start();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                Close();
            }
        }
        private void OnProcessMessage(RequestCode requestCode, ActionCode actionCode, string data)
        {
            Console.WriteLine(requestCode.ToString()+actionCode.ToString()+data);
            server.HandleRequest(requestCode, actionCode, data,this);
        }
        private void Close()
        {
            ConnHelper.CloseConnection(mysqlConn);
            if (clientSocket!=null)
            {
                clientSocket.Close();
            }
            server.RemoveClient(this);
            Console.WriteLine("当前客户端数量：{0}", server.clientList.Count);
        }
        public void Send(ActionCode actionCode, string data)
        {
            byte[] bytes = Message.PackData(actionCode,data);
            clientSocket.Send(bytes);
        }
        public void SetUserData(User user,Result result)
        {
            this.user = user;
            this.result = result;
        }
        public string GetUserData()
        {
            return user.ID + "," + user.Username + "," + result.TotalCount + "," + result.WinCount;
        }
    }
}
