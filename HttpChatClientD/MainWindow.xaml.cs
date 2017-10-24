using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;

namespace HttpChatClientD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HttpClient client;
        string userName = "noname";
        string ip;
        Thread thread;

        public MainWindow()
        {
            ip = @"http://localhost:8888/";
            client = new HttpClient();
            thread = new Thread(StartListen);
            InitializeComponent();                      
        }



        private async void StartListen()
        {
            while(true)
            {
                try
                {
                    string response = await client.GetStringAsync(ip+"?check=" + userName);
                    string[] param = response.Split('=');
                    if (param[0] == "message")
                        Dispatcher.Invoke(() => chatList.Items.Add("Server: " + param[1]));                     
                    else if (param[0] == "name")
                        userName = param[1];
                }
                catch(Exception e)
                {
                    Dispatcher.Invoke(() => chatList.Items.Add("Cannot connect!"));
                }
                Thread.Sleep(500);
            }
        }

        private async void SendClick(object sender, RoutedEventArgs e)
        {
            //thread.Interrupt();
            try
            {
                var response = await client.PostAsync(ip, new StringContent("message=" + inputTB.Text));
                string content = await response.Content.ReadAsStringAsync();
                if(content == "received")
                    chatList.Items.Add("You: " + inputTB.Text);
            }
            catch
            {
                chatList.Items.Add("Cannot connect!");
            }
            //thread.Resume();
        }

        private void ConnectClick(object sender, RoutedEventArgs e)
        {
            if (thread.IsAlive == false)
                thread.Start();
        }
    }
}
