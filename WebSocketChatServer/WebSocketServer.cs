
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace WebSocketChatServer
{
    class ChatServer : WebSocketBehavior
    {
        protected override void OnOpen()
        {
            Console.WriteLine("Listening...");
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            Sessions.Broadcast(e.Data);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            base.OnError(e);
        }
    }
}
