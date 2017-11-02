using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebSocketApi;

namespace WebSocketClientDEvent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EWebSocket client;
        const string host = "ws://localhost:8888/";

        public MainWindow()
        {
            InitializeComponent();

            client = new EWebSocket(host);

            client.OnOpen += (s, e) => Dispatcher.Invoke(() => chatList.Items.Add("You've connected to " + host));
            client.OnError += (s, e) => Dispatcher.Invoke(() => chatList.Items.Add("Error: " + e.Message));
            client.OnMessage += (s, e) => Dispatcher.Invoke(() => chatList.Items.Add("Server: " + e.Data));
            client.OnClose += (s, e) => Dispatcher.Invoke(() => chatList.Items.Add("You've disconnected from " + host));
        }
        socket.Listener.AddMsgListener(new ActionMessage());
            socket.Listener.AddErrListener(new ActionError());
            socket.Listener.AddOpenListener(new ActionOpen());
            socket.Listener.AddCloseListener(new ActionClose());
        }

    public void Start()
    {
        socket.Start();
    }

    class ActionOpen : IActionCommand
    {
        public void Action(EventArgs e)
        {
            Console.WriteLine("Client has connected");
        }
    }

    class ActionMessage : IActionCommand
    {
        public void Action(EventArgs e)
        {
            string msg = (e as StringyEventArgs).Msg;
            socket.Broadcast(msg);
        }
    }

    class ActionError : IActionCommand
    {
        public void Action(EventArgs e)
        {
            Console.WriteLine("Some Error!");
        }
    }

    class ActionClose : IActionCommand
    {
        public void Action(EventArgs e)
        {
            Console.WriteLine("Client has disconnected");
        }
    }

    private void SendClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var content = inputTB.Text;
                if (!string.IsNullOrEmpty(content))
                    client.Send(content);
            }
            catch
            {
                chatList.Items.Add("Cannot connect to server!");
            }
        }

        private void ConnectClick(object sender, RoutedEventArgs e)
        {
            try
            {
                client.Connect();
            }
            catch
            {
                chatList.Items.Add("Cannot connect to server!");
            }
        }
    }
}
