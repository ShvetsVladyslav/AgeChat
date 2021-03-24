using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AgeChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server();
            server.Run();
        }
    }
}
