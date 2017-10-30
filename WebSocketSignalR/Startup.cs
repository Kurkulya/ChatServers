using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
[assembly: OwinStartup(typeof(WebSocketSignalR.Startup))]
namespace WebSocketSignalR
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}