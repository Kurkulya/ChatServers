using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace WebSocketSignalR
{
    public class ChatHub : Hub
    {
        static List<string> Sessions = new List<string>();

        // Отправка сообщений
        public void Send(string message)
        {
            Clients.All.addMessage(message);
        }

        // Подключение нового пользователя
        public void Connect()
        {
            var id = Context.ConnectionId;

            if (!Sessions.Any(x => x == id))
            {
                Sessions.Add(id);
            }
        }

        // Отключение пользователя
        public override Task OnDisconnected(bool stopCalled)
        {
            var item = Sessions.FirstOrDefault(x => x == Context.ConnectionId);
            if (item != null)
            {
                Sessions.Remove(item);
                var id = Context.ConnectionId;
                Clients.All.onUserDisconnected(id);
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}