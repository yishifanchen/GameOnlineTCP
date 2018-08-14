using Common;
using GameServer.Controller;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace GameServer.Servers
{
    class Server
    {
        private IPEndPoint ipEndPoint;
        private Socket serverSocket;
        public List<Client> clientList = new List<Client>();
        private List<Room> roomList = new List<Room>();
        private ControllerManager controllerManager;

        public Server() { }
        public Server(int port)
        {
            controllerManager = new ControllerManager(this);
            SetIpAndPort(IPAddress.Any, port);
        }
        public void SetIpAndPort(IPAddress iPAddress, int port)
        {
            ipEndPoint = new IPEndPoint(iPAddress, port);
        }
        public void Start()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(ipEndPoint);
            serverSocket.Listen(0);
            serverSocket.BeginAccept(AcceptCallBack,null);
        }
        private void AcceptCallBack(IAsyncResult ar)
        {
            Socket clientSocket = serverSocket.EndAccept(ar);
            Client client = new Client(clientSocket,this);
            client.Start();
            clientList.Add(client);
            Console.WriteLine("有客户端连接，当前客户端数量：{0}",clientList.Count);
            serverSocket.BeginAccept(AcceptCallBack, null);
        }
        public void RemoveClient(Client client)
        {
            lock (clientList)
            {
                clientList.Remove(client);
            }
        }
        public void SendResponse(Client client,ActionCode actionCode,string data)
        {
            client.Send(actionCode, data);
        }
        public void HandleRequest(RequestCode requestCode, ActionCode actionCode, string data, Client client)
        {
            controllerManager.HandleRequest(requestCode, actionCode, data, client);
        }
        public void CreateRoom(Client client)
        {
            Room room = new Room(this);
            room.AddClient(client);
            roomList.Add(room);
        }
        public void RemoveRoom(Room room)
        {
            if (roomList != null && room != null)
            {
                roomList.Remove(room);
            }
        }
        public List<Room> GetRoomList()
        {
            return roomList;
        }
        public Room GetRoomById(int id)
        {
            foreach(Room room in roomList)
            {
                if (room.GetId() == id) return room;
            }
            return null;
        }
    }
}
