using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading;
using System.Net.Http;
using System.Text;
using WebSocket.Portable;

namespace WebSocketChatClientA.Droid
{
	[Activity (Label = "WebSocketChatClientA.Android", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
        WebSocketClient client = new WebSocketClient();

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

            client.FrameReceived += data => RunOnUiThread(() => listChat.Text += data);

            btnSend.Click += SendText;
            btnConnect.Click += ConnectToServer;
        }

        private async void ConnectToServer(object sender, EventArgs e)
        {
            try
            {
                await client.OpenAsync("ws://localhost:8888/ChatServer");
            }
            catch 
            {
                listChat.Text += "\nCannot connect to server!";
            }
        }

        private async void SendText(object sender, EventArgs e)
        {
            try
            {
                await client.SendAsync(inputText.Text);
                listChat.Text += "\nYou: " + inputText.Text;
            }
            catch
            {
                listChat.Text += "\nCannot connect to server!";
            }
        }
    }
}


