using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
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
using WebSocketSharp;

namespace WebSocketChatClientD
{
    public partial class MainWindow : Window
    {
        private WebSocket client;
        const string host = "ws://localhost:8888/ChatServer";

        public MainWindow()
        {
            InitializeComponent();

            client = new WebSocket(host);

            client.OnOpen += (s, e) => Dispatcher.Invoke(() => chatList.Items.Add("You've connected to " + host));
            client.OnError += (s, e) => Dispatcher.Invoke(() => chatList.Items.Add("Error: " +e.Message));
            client.OnMessage += (s, e) => Dispatcher.Invoke(() => chatList.Items.Add("Server: " +e.Data));
            client.OnClose += (s, e) => Dispatcher.Invoke(() => chatList.Items.Add("You've disconnected from " + host));
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
