using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Net.WebSockets;
using System.Threading;
using System.Net.Http;
using System.Text;

namespace WebSocketChatClientA.Droid
{
	[Activity (Label = "WebSocketChatClientA.Android", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
        ClientWebSocket client = null;

        Button btnSend;
        Button btnConnect;
        TextView listChat;
        EditText inputText;
        EditText inputIp;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            btnSend = FindViewById<Button>(Resource.Id.btnSend);
            btnConnect = FindViewById<Button>(Resource.Id.btnConnect);
            listChat = FindViewById<TextView>(Resource.Id.listChat);
            inputText = FindViewById<EditText>(Resource.Id.inputText);
            inputIp = FindViewById<EditText>(Resource.Id.ipInput);

            btnSend.Click += SendText;
            btnConnect.Click += ConnectToServer;
        }

        private async void ConnectToServer(object sender, EventArgs e)
        {
            try
            {
                if (client == null)
                {
                    client = new ClientWebSocket();
                    await client.ConnectAsync(new Uri("ws://" + inputIp.Text), CancellationToken.None);
                    listChat.Text += "\nYou've connected to the server!";
                    Receiver();
                }
            }
            catch (Exception ex)
            {
                listChat.Text += "\nCannot connect to server!";
            }
        }

        private async void SendText(object sender, EventArgs e)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(inputText.Text);
                await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, false, CancellationToken.None);
                listChat.Text += "\nYou: " + inputText.Text;
            }
            catch
            {
                listChat.Text += "\nCannot connect to server!";
            }
        }

        private async void Receiver()
        {
            while (client.State == WebSocketState.Open)
            {
                byte[] buffer = new byte[1024];
                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                listChat.Text += "\nServer: " + Encoding.UTF8.GetString(buffer).TrimEnd('\0');
            }
        }
    }
}


