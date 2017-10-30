using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace WebSocketChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var wssv = new WebSocketServer("ws://localhost:8888/");
            wssv.AddWebSocketService<ChatServer>("/ChatServer");
            wssv.Start();
            Console.ReadKey(true);
            wssv.Stop();
        }
    }
}
