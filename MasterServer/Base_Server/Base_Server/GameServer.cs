using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Base_Server
{
    class GameServer
    {
        public string serverName = "";
        public EndPoint owner;
        public int maxCount = 0;
        public int currentCount = 0;
    }
}
