using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Servers
{
    enum RoomState
    {
        WaitingJoin,
        WaitingBattle,
        Battle,
        End
    }
    class Room
    {
        private List<Client> clientRoom = new List<Client>();
        private RoomState state = RoomState.WaitingJoin;
        private Server server;

        public Room(Server server)
        {
            this.server = server;
        }
        public bool IsWaitingJoin()
        {
            return state == RoomState.WaitingJoin;
        }
        public void AddClient(Client client)
        {
            clientRoom.Add(client);
            if (clientRoom.Count >= 2)
            {
                state = RoomState.WaitingBattle;
            }
        }
        public string GetHouseOwnerData()
        {
            return clientRoom[0].GetUserData();
        }
    }
}
