using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common;
using MySql.Data.MySqlClient;
using GameServer.Tool;

namespace GameServer.Servers
{
    class Client
    {
        private Socket clientSocket;
        private Server server;
        private Message msg = new Message();
        private MySqlConnection mysqlConn;

        public Client() { }
        public Client(Socket clientSocket,Server server)
        {
            this.clientSocket = clientSocket;
            this.server = server;
            mysqlConn = ConnHelper.Connect();
        }
        public void Start()
        {
            clientSocket.BeginReceive(msg.Data,msg.StartIndex,msg.RemainSize,SocketFlags.None,ReceiveCallback,null);
        }
        private void ReceiveCallback(IAsyncResult ar)
        {
            try{
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
            Console.WriteLine(data);
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
        }
        public void Send(RequestCode requestCode, string data)
        {
            byte[] bytes = Message.PackData(requestCode,data);
            clientSocket.Send(bytes);
        }
    }
}
