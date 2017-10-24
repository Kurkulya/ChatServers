using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientChat
{
    public partial class Form1 : Form
    {
        TcpClient client;
        BinaryReader reader;
        BinaryWriter writer;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (writer != null)
                {
                    writer.Write(textBox1.Text);
                    listBox1.Items.Add("You: " + textBox1.Text);
                }
            }
            catch
            {
                listBox1.Items.Add("Connection lost!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            client = new TcpClient();
            try
            {
                client.Connect("127.0.0.1", 9050);

                NetworkStream stream = client.GetStream();
                reader = new BinaryReader(stream);
                writer = new BinaryWriter(stream);
                Thread th = new Thread(WaitingAnswer);
                th.Start();
            }
            catch
            {
                listBox1.Items.Add("Cannot connect to server!");
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
                        if (listBox1.InvokeRequired)
                        {
                            listBox1.BeginInvoke(new Action(delegate
                            {
                                listBox1.Items.Add("Server:" + data);
                            }));
                        }
                    }
                }
            }
            catch
            {
                if (listBox1.InvokeRequired)
                {
                    listBox1.BeginInvoke(new Action(delegate
                    {
                        listBox1.Items.Add("Connection lost!");
                    }));
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            client.Close();
            base.OnClosed(e);
        }
    }
}
