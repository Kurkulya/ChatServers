using System;


namespace WebSocketApi
{
    public interface IActionCommand
    {
        void Action(EventArgs e);
    }
}
