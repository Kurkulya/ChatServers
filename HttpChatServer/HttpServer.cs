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
        Dictionary <Cookie, bool> clients;

        public HttpServer(string uri)
        {
            clients = new Dictionary<Cookie, bool>();
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
            Dictionary<string, string> param = ParseRequest(context.Request);
            if (param.ContainsKey("message"))
            {
                lastMessage = param["message"];
                for(int i = 0; i< clients.Count; i++)
                {
                    clients[clients.Keys.ElementAt(i)] = true;
                }
                SendData(context, "received");
            }
            else if(param.ContainsKey("check"))
            {
                Cookie cookie = context.Request.Cookies["ID"];
                if (cookie == null)
                {
                    Cookie newCookie = new Cookie("ID", DateTime.Now.ToString());
                    context.Response.AppendCookie(newCookie);
                    clients.Add(newCookie, false);
                    SendData(context, "nomessage=true");                  
                }
                else
                {
                    if (clients[cookie] == true)
                    {
                        SendData(context, "message=" + lastMessage);
                        clients[cookie] = false;
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
            context.Response.AddHeader("Access-Control-Allow-Credentials", "true");
            context.Response.AddHeader("Access-Control-Expose-Headers", "Content-Type, Access-Control-Allow-Headers, Authorization"); 
            context.Response.ContentLength64 = data.Length;
            context.Response.OutputStream.Write(Encoding.ASCII.GetBytes(data), 0, data.Length);
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
    }
}
