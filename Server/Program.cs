using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Protocol;
using Protocol.Messages;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            new Net.Server();
        }

    }
}