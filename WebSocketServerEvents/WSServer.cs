using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketServerEvents
{
    public class WSServer
    {
        static EWebSocket socket;

        public WSServer()
        {
            socket = new EWebSocket("http://localhost:8888/");

            socket.Listener.AddMsgListener(new ActionMessage());
            socket.Listener.AddErrListener(new ActionError());
            socket.Listener.AddOpenListener(new ActionOpen());
            socket.Listener.AddCloseListener(new ActionClose());
        }

        public void Start()
        {
            socket.Start();
        }

        class ActionOpen : IActionCommand
        {
            public void Action(EventArgs e)
            {
                Console.WriteLine("Client has connected");
            }
        }

        class ActionMessage : IActionCommand
        {
            public void Action(EventArgs e)
            {
                string msg = (e as StringyEventArgs).Msg;
                socket.Broadcast(msg);
            }
        }

        class ActionError : IActionCommand
        {
            public void Action(EventArgs e)
            {
                Console.WriteLine("Some Error!");
            }
        }

        class ActionClose : IActionCommand
        {
            public void Action(EventArgs e)
            {
                Console.WriteLine("Client has disconnected");
            }
        }
    }
}
