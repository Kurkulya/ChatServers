using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using static Android.Bluetooth.BluetoothClass;

namespace AndroidClient.Droid
{
	[Activity (Label = "AndroidClient.Android", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
        TcpClient client;
        BinaryReader reader;
        BinaryWriter writer;

        Button btnSend;
        Button btnConnect;
        TextView listChat;
        EditText inputText;
        EditText inputIp;

        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);

			btnSend = FindViewById<Button> (Resource.Id.btnSend);
            btnConnect = FindViewById<Button>(Resource.Id.btnConnect);
            listChat = FindViewById<TextView>(Resource.Id.listChat);
            inputText = FindViewById<EditText>(Resource.Id.inputText);
            inputIp = FindViewById<EditText>(Resource.Id.ipInput);

            btnSend.Click += SendText;
            btnConnect.Click += ConnectToServer;
        }

        private void ConnectToServer(object sender, EventArgs e)
        {
            client = new TcpClient();
            try
            {
                string[] ipport = inputIp.Text.Split(':');
                client.Connect(ipport[0], Convert.ToInt32(ipport[1]));

                NetworkStream stream = client.GetStream();
                reader = new BinaryReader(stream);
                writer = new BinaryWriter(stream);
                Thread th = new Thread(WaitingAnswer);
                th.Start();
            }
            catch
            {
               listChat.Text += "\nCannot connect to server!";
            }
        }

        private void SendText(object sender, EventArgs e)
        {
            try
            {
                if (writer != null)
                {
                    writer.Write(inputText.Text);

                    listChat.Text += "\nYou: " + inputText.Text;
                }
            }
            catch
            {
                listChat.Text += "\nConnection lost!";
            }
        }


        private void WaitingAnswer()
        {
            try
            {
                while (true)
                {
                    string data = reader.ReadString();
                    if (data.Length != 0)
                    {
                        RunOnUiThread(() =>
                        {
                            listChat.Text += "\nServer:" + data;
                        });
                    }
                }
            }
            catch
            {
                RunOnUiThread(() =>
                {
                    listChat.Text += "Connection lost!";
                });
                
            }
        }

        protected override void OnDestroy()
        {
            client.Close();
            base.OnDestroy();
        }
    }
}


