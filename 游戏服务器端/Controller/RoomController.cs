using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Servers;

namespace GameServer.Controller
{
    class RoomController:BaseController
    {
        public RoomController()
        {
            requestCode = RequestCode.Room;
        }
        public string CreateRoom(string data,Client client,Server server)
        {
            server.CreateRoom(client);
            return ((int)ReturnCode.Success).ToString() + ","+((int)RoleType.Blue).ToString();
        }
    }
}
