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
using GameServer.DAO;

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
        private Room room;
        private ResultDAO resultDAO = new ResultDAO();
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
        public int HP { get; set; }
        public bool TakeDamage(int damage)
        {
            HP -= damage;
            HP = Math.Max(HP, 0);
            Console.WriteLine("玩家："+user.Username+"剩余血量："+HP);
            if (HP <= 0) return true;
            return false;
        }
        public bool IsDie()
        {
            return HP <= 0;
        }
        public Room Room
        {
            get { return room; }
            set { room = value; }
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
            //Console.WriteLine("请求类型："+requestCode.ToString()+"方法类型："+actionCode.ToString()+"数据:"+data);
            server.HandleRequest(requestCode, actionCode, data,this);
        }
        private void Close()
        {
            ConnHelper.CloseConnection(mysqlConn);
            if (clientSocket!=null)
            {
                clientSocket.Close();
            }
            if (room != null)
            {
                room.QuitRoom(this);
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
        public int GetUserId()
        {
            return user.ID;
        }
        public bool IsHouseOwner()
        {
            return room.IsHouseOwner(this);
        }
        public void UpdateResult(bool isVictory)
        {
            UpdateResultToDB(isVictory);
            UpdateResultToClient();
        }
        private void UpdateResultToDB(bool isVictory)
        {
            result.TotalCount++;
            if (isVictory) result.WinCount++;
            resultDAO.UpdateOrAddResult(mysqlConn,result);
        }
        private void UpdateResultToClient()
        {
            Send(ActionCode.UpdateResult, string.Format("{0},{1}", result.TotalCount, result.WinCount));
        }
    }
}
