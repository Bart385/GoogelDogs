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
            TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 1337);
            listener.Start();

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                TestMessaging(client);
            }
        }

        private static async void TestMessaging(TcpClient client)
        {
            await MessagingUtil.SendMessage(client.GetStream(), new OkMessage());
        }
    }
}