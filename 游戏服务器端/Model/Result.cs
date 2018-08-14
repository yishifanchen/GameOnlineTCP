using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    class Result
    {
        public Result(int id ,int userID,int totalCount,int winCount)
        {
            this.ID = id;
            this.UserID = userID;
            this.TotalCount = totalCount;
            this.WinCount = winCount;
        }
        public int ID { get ; set; }
        public int UserID { get; set; }
        public int TotalCount { get; set; }
        public int WinCount { get; set;}
    }
}
