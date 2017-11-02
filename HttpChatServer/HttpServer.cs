using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpChatServer
{
    class HttpServer
    {
        HttpListener listener;
        string lastMessage = "";
        Dictionary<Cookie, bool> clients;

        Queue<string> receiveQueue = new Queue<string>();

        public event Func<HttpListenerContext, Task> MessageReceived;
        public event Action ServerStarted;
        public event Action<Exception> ErrorRaised;

        public HttpServer(string uri)
        {
            clients = new Dictionary<Cookie, bool>();
            listener = new HttpListener();
            listener.Prefixes.Add(uri);
            this.MessageReceived += OnMessage;
            this.ServerStarted += OnStart;
            this.ErrorRaised += OnError;
        }

        private void OnError(Exception e)
        {
            Console.WriteLine(e.Message);
        }

        private async void OnStart()
        {
            Console.WriteLine("Listening...");
            while (true)
            {
                try
                {
                    HttpListenerContext context = await listener.GetContextAsync();
                    await MessageReceived(context);
                }
                catch (Exception e)
                {
                    ErrorRaised(e);
                }
            }
        }

        private async Task OnMessage(HttpListenerContext context)
        {
            Dictionary<string, string> param = ParseRequest(context.Request);
            if (param.ContainsKey("message"))
            {
                if (clients.Any(x => x.Value == true))
                    receiveQueue.Enqueue(param["message"]);
                else
                    SetMessage(param["message"]);

                await SendDataAsync(context, "received");
            }
            else if(param.ContainsKey("check"))
            {
                Cookie cookie = context.Request.Cookies["ID"];
                if (cookie == null)
                {
                    Cookie newCookie = new Cookie("ID", DateTime.Now.ToString());
                    context.Response.AppendCookie(newCookie);
                    if(!clients.ContainsKey(newCookie))
                        clients.Add(newCookie, false);
                    await SendDataAsync(context, "nomessage=true");
                }
                else
                {
                    if (clients[cookie] == true)
                    {
                        await SendDataAsync(context, "message=" + lastMessage);
                        clients[cookie] = false;

                        if (!clients.Any(x => x.Value == true) && receiveQueue.Count != 0)
                            SetMessage(receiveQueue.Dequeue());
                    }
                    else
                    {
                        await SendDataAsync(context, "nomessage=true");
                    }
                }
            }
        }


        public void Start()
        {
            listener.Start();
            ServerStarted();
            Console.ReadKey();
        }

        private async Task SendDataAsync(HttpListenerContext context, string data)
        {
            context.Response.ContentType = "text/plain";
            context.Response.AddHeader("Access-Control-Allow-Origin", "*"); 
            context.Response.ContentLength64 = data.Length;
            await context.Response.OutputStream.WriteAsync(Encoding.ASCII.GetBytes(data), 0, data.Length);
            context.Response.OutputStream.Close();
        }

        private Dictionary<string,string> ParseRequest(HttpListenerRequest request)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();

            if (request.HttpMethod == "POST")
            {
                string body = new StreamReader(request.InputStream).ReadToEnd();
                string[] temp = body.Split('=', '&');
                for (int i = 0; i < temp.Length; i+=2)
                {
                    param.Add(temp[i], temp[i + 1]);
                }              
            }
            else
            {
                string[] temp = request.QueryString.AllKeys;
                foreach(string key in temp)
                {
                    param.Add(key, request.QueryString[key]);
                }
            }

            return param;
        }

        private void SetMessage(string message)
        {
            lastMessage = message;
            for (int i = 0; i < clients.Count; i++)
            {
                clients[clients.Keys.ElementAt(i)] = true;
            }          
        }
    }
}
