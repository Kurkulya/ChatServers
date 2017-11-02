using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketServerEvents
{

    public class StringyEventArgs : EventArgs
    {
        public string Msg { get; }

        public StringyEventArgs(string msg)
        {
            Msg = msg;
        }        
    }

    public class WSEngine
    {
        List<IActionCommand> msgList = new List<IActionCommand>();
        List<IActionCommand> errList = new List<IActionCommand>();
        List<IActionCommand> openList = new List<IActionCommand>();
        List<IActionCommand> closeList = new List<IActionCommand>();

        public void AddMsgListener(IActionCommand cmd) => msgList.Add(cmd);
        public void AddErrListener(IActionCommand cmd) => errList.Add(cmd);
        public void AddOpenListener(IActionCommand cmd) => openList.Add(cmd);
        public void AddCloseListener(IActionCommand cmd) => closeList.Add(cmd);

        public void Message(string str)
        {
            StringyEventArgs e = new StringyEventArgs(str);
            foreach (IActionCommand c in msgList)
            {
                c.Action(e);
            }
        }

        public void Open()
        {
            EventArgs e = new EventArgs();
            foreach (IActionCommand c in openList)
            {
                c.Action(e);
            }
        }

        public void Close()
        {
            EventArgs e = new EventArgs();
            foreach (IActionCommand c in closeList)
            {
                c.Action(e);
            }
        }

        public void Error()
        {
            EventArgs e = new EventArgs();
            foreach (IActionCommand c in errList)
            {
                c.Action(e);
            }
        }
    }
}
