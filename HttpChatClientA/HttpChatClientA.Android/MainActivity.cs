using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Net.Http;
using System.Threading;

namespace HttpChatClientA.Droid
{
	[Activity (Label = "HttpChatClientA.Android", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
        HttpClient client;
        string userName = "noname";
        Thread thread;

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

            client = new HttpClient();
            thread = new Thread(StartListen);
        }

        private void ConnectToServer(object sender, EventArgs e)
        {
            if (thread.IsAlive == false)
                thread.Start();
            listChat.Text += "\nYou've connected to server!";
        }

        private async void SendText(object sender, EventArgs e)
        {
            try
            {
                var response = await client.PostAsync(inputIp.Text, new StringContent("message=" + inputText.Text));
                string content = await response.Content.ReadAsStringAsync();
                if (content == "received")
                    listChat.Text += "\nYou: " + inputText.Text;
            }
            catch
            {
                listChat.Text += "\nCannot connect!";
            }
        }

       
        private async void StartListen()
        {
            while (true)
            {
                try
                {
                    string response = await client.GetStringAsync(inputIp.Text + "?check=" + userName);
                    string[] param = response.Split('=');
                    if (param[0] == "message")
                        RunOnUiThread(() => listChat.Text += "\nServer: " + param[1]);
                    else if (param[0] == "name")
                        userName = param[1];
                }
                catch (Exception e)
                {
                    RunOnUiThread(() => listChat.Text += "\nCannot connect!");
                }
                Thread.Sleep(500);
            }
        }
    }
}


