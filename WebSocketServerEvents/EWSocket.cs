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
    class EWebSocket
    {
        List<WebSocket> clients;
        HttpListener httpListener;
        WSEngine wsEngine;

        public WSEngine Listener => wsEngine;

        public EWebSocket(string httpListenerPrefix)
        {
            httpListener = new HttpListener();
            httpListener.Prefixes.Add(httpListenerPrefix);
            clients = new List<WebSocket>();
            wsEngine = new WSEngine();
        }
 
        public async void Start()
        {
            httpListener.Start();           
            while (true)
            {
                HttpListenerContext httpListenerContext = await httpListener.GetContextAsync();

                if (httpListenerContext.Request.IsWebSocketRequest)
                {
                    wsEngine.Open();
                    ProcessRequest(httpListenerContext);
                }
                else
                {                  
                    httpListenerContext.Response.Close();
                    wsEngine.Close();
                }
            }
        }
 
        public async void Broadcast(string message)
        {
            foreach (WebSocket socket in clients)
            {
                await socket.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(message), 0, message.Length),
                    WebSocketMessageType.Text, true, CancellationToken.None);
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
                    byte[] buffer = new byte[1024 * 16];
                    WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    wsEngine.Message(System.Text.Encoding.UTF8.GetString(buffer.TakeWhile(x => buffer.ToList().IndexOf(x) != receiveResult.Count).ToArray()));               
                }
            }
            catch
            {
                webSocket.Dispose();
                clients.Remove(webSocket);
                wsEngine.Close();
            }
        }
    }
}
