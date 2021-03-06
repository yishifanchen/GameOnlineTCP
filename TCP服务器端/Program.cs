﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace TCP服务器端
{
    class Program
    {
        static void Main(string[] args)
        {
            StartServerAsync();
            Console.ReadKey();
        }
        static void StartServerAsync()
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //192.168.0.112    127.0.0.1
            //IpAddress xxx.xxx.xxx.xxx IPEndPoint xxx.xxx.xxx.xxx:port
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 6688);
            serverSocket.Bind(ipEndPoint);//绑定ip和端口号
            serverSocket.Listen(0);//开始监听端口号
            Console.WriteLine("服务器已启动");
            //Socket clientSocket = serverSocket.Accept();//接受一个客户端的连接
            serverSocket.BeginAccept(AcceptCallBack, serverSocket);

            
        }
        void StartServerSync()
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //192.168.0.112    127.0.0.1
            //IpAddress xxx.xxx.xxx.xxx IPEndPoint xxx.xxx.xxx.xxx:port
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 88);
            serverSocket.Bind(ipEndPoint);//绑定ip和端口号
            serverSocket.Listen(0);//开始监听端口号
            Socket clientSocket = serverSocket.Accept();//接受一个客户端的连接
            //向客户端发送一条消息
            string msg = "Hello client! 你好...";
            byte[] data = System.Text.Encoding.UTF8.GetBytes(msg);
            clientSocket.Send(data);
            //接收一个客户端的消息
            byte[] dataBuffer = new byte[1024];
            int count = clientSocket.Receive(dataBuffer);
            string msgReceive = System.Text.Encoding.UTF8.GetString(dataBuffer, 0, count);
            //Console.WriteLine(msgReceive);
            Console.ReadKey();
            clientSocket.Close();
            serverSocket.Close();
        }
        static Message msg = new Message();
        static void AcceptCallBack(IAsyncResult ar)
        {
            Socket serverSocket = ar.AsyncState as Socket;
            Socket clientSocket = serverSocket.EndAccept(ar);
            Console.WriteLine("有一个客户端连接");
            //向客户端发送一条消息
            string msgStr = "Hello client! 你好...";
            for (int i=0;i<1000;i++)
            {
                byte[] data = Message.GetBytes(msgStr);
                clientSocket.Send(data);
            }
            clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None, ReceiveCallBack, clientSocket);

            serverSocket.BeginAccept(AcceptCallBack, serverSocket);
        }

        static byte[] dataBuffer = new byte[1024];
        static void ReceiveCallBack(IAsyncResult ar)
        {
            Socket clientSocket = null;
            try
            {
                clientSocket = ar.AsyncState as Socket;
                int count = clientSocket.EndReceive(ar);
                if (count == 0)
                {
                    clientSocket.Close();
                    return;
                }
                msg.AddCount(count);
                //string msgStr = Encoding.UTF8.GetString(dataBuffer, 0, count);
                //Console.WriteLine("从客户端接受到消息：" + msgStr);

                msg.ReadMessage();

                //clientSocket.BeginReceive(dataBuffer, 0, 1024, SocketFlags.None, ReceiveCallBack, clientSocket);

                clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None, ReceiveCallBack, clientSocket);
            }
            catch (Exception e)
            {
                Console.Write(e);
                if (clientSocket != null)
                    clientSocket.Close();
            }
            finally
            {
                
            }
        }
    }
}
