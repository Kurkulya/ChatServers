using System.Net.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;

namespace WebSocketChatServer
{
    class WebSocketServer
    {
        List<WebSocket> clients;
        HttpListener httpListener;

        public WebSocketServer(string httpListenerPrefix)
        {
            httpListener = new HttpListener();
            httpListener.Prefixes.Add(httpListenerPrefix);
            clients = new List<WebSocket>();
        }

        public async void Start()
        {
            httpListener.Start();
            Console.WriteLine("Listening...");

            while (true)
            {
                HttpListenerContext httpListenerContext = await httpListener.GetContextAsync();

                if (httpListenerContext.Request.IsWebSocketRequest)
                    ProcessRequest(httpListenerContext);
                else
                    httpListenerContext.Response.Close();
            }
        }


        private async void ProcessRequest(HttpListenerContext context)
        {
            WebSocketContext webSocketContext = await context.AcceptWebSocketAsync(subProtocol: null);
            WebSocket webSocket = webSocketContext.WebSocket;

            if (clients.Contains(webSocket) == false)
                clients.Add(webSocket);
          
            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    byte[] buffer = new byte[1024];
                    WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer),
                        CancellationToken.None);

                    foreach (WebSocket socket in clients)
                    {
                        await socket.SendAsync(new ArraySegment<byte>(buffer, 0, receiveResult.Count), 
                            WebSocketMessageType.Text, receiveResult.EndOfMessage, CancellationToken.None);
                    }
                }
            }
            catch
            {
                webSocket.Dispose();
                clients.Remove(webSocket);
            }
        }
    }
}
