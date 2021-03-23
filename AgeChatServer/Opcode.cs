using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeChatServer
{
    static class Opcode
    {
        public static int Fragment = 0;
        public static int Text = 1;
        public static int Binary = 2;
        public static int CloseConnection = 8;
        public static int Ping = 9;
        public static int Pong = 10;
    }
}
