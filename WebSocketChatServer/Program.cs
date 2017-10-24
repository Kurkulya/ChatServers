using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            WebSocketServer socketServer = new WebSocketServer("http://localhost:8888/");
            socketServer.Start();
            Console.ReadKey();
        }
    }
}
