using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeChatServer
{
    static class Opcode
    {
        static int Fragment = 0;
        static int Text = 1;
        static int Binary = 2;
        static int CloseConnection = 8;
        static int Ping = 9;
        static int Pong = 10;
    }
}
