using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
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

namespace WebSocketChatClientD
{
    public partial class MainWindow : Window
    {
        ClientWebSocket client = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void SendClick(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(inputTB.Text);
                await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, false, CancellationToken.None);
                chatList.Items.Add("You: " + inputTB.Text);
            }
            catch
            {
                chatList.Items.Add("Cannot connect to server!");
            }
        }

        private async void ConnectClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (client == null)
                {
                    client = new ClientWebSocket();
                    await client.ConnectAsync(new Uri("ws://" + ipTB.Text), CancellationToken.None);
                    chatList.Items.Add("You've connected to the server!");
                    Receiver();
                }
            }
            catch (Exception ex)
            {
                chatList.Items.Add("Cannot connect to server!");
            }
        }

        private async void Receiver()
        {           
            while (client.State == WebSocketState.Open)
            {
                byte[] buffer = new byte[1024];
                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                chatList.Items.Add("Server: " + Encoding.UTF8.GetString(buffer).TrimEnd('\0'));
            }
        }
    }
}
