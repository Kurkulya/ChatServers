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
        Dictionary <string, bool> clients;
        int count = 0;

        public HttpServer(string uri)
        {
            clients = new Dictionary<string, bool>();
            listener = new HttpListener();
            listener.Prefixes.Add(uri);
        }

        public void Start()
        {
            listener.Start();
            Console.WriteLine("Listening...");

            while (true)
            {
                try
                {
                    HttpListenerContext context = listener.GetContext();
                    Receiver(context);
                }
                catch(Exception e)
                {

                }
            }

        }

        private void Receiver(HttpListenerContext context)
        {
            string message = "";
            string check = "";

            if (context.Request.HttpMethod == "POST")
            {
                var body = new StreamReader(context.Request.InputStream).ReadToEnd();
                string[] param = body.Split('=');
                if (param[0] == "message")
                    message = param[1];
                else if (param[0] == "check")
                    check = param[1];              
            }
            else
            {
                string[] param = context.Request.QueryString.AllKeys;
                if (param.Contains("message"))
                    message = context.Request.QueryString["message"];
                else if (param.Contains("check"))
                    check = context.Request.QueryString["check"];
            }

            if (message != "")
            {
                lastMessage = message;
                for(int i = 0; i< clients.Count; i++)
                {
                    clients[clients.Keys.ElementAt(i)] = true;
                }
                SendData(context, "received");
            }
            else if(check != "")
            {
                if (check == "noname")
                {
                    string userName = "user" + count++;
                    clients.Add(userName, false);
                    SendData(context, "name=" + userName);                  
                }
                else
                {
                    if (clients.ContainsKey(check) == false)
                        clients.Add(check, false);

                    if (clients[check] == true)
                    {
                        SendData(context, "message=" + lastMessage);
                        clients[check] = false;
                    }
                    else
                    {
                        SendData(context, "nomessage=true");
                    }
                }
            }
        }

        private void SendData(HttpListenerContext context, string data)
        {
            context.Response.ContentType = "text/plain";
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            context.Response.ContentLength64 = data.Length;
            context.Response.OutputStream.Write(Encoding.ASCII.GetBytes(data), 0, data.Length);
            context.Response.OutputStream.Close();
        }
    }
}
