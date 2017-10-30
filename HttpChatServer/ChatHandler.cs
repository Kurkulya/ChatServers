using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace HttpChatServer
{
    class ChatHandler : IHttpHandler
    {
        public bool IsReusable => throw new NotImplementedException();
        Dictionary<HttpSessionState, bool> clients;

        public void ProcessRequest(HttpContext context)
        {
            throw new NotImplementedException();
        }
    }
}
