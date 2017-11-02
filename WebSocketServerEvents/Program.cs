using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketServerEvents
{
    class Program
    {
        static void Main(string[] args)
        {
            WSServer ws = new WSServer();
            ws.Start();
            Console.ReadKey();
        }
    }
}
