using System;


namespace WebSocketServerEvents
{
    public interface IActionCommand
    {
        void Action(EventArgs e);
    }
}
