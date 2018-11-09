using System;
using System.Collections.Generic;

namespace FileTrackerHandler
{
    public class DemoClient
    {
        private readonly string _namePipe = @"qursaanPipe";

        public DemoClient(String pipName)
        {
            _namePipe = pipName;
        }

        ~DemoClient()
        {
            if (Client != null) Client.Stop();
        }

        
        public delegate void ReceivedMessageDelegate(object sender, ReceivedMessageEventArgs e);

        public NamedPipeClient Client { get; set; }

        public void StartClient()
        {
            Client = new NamedPipeClient(_namePipe);
            Client.OnReceivedMessage += new EventHandler<ReceivedMessageEventArgs>(Client_OnReceivedMessage);
            Client.Start();
        }

        public Boolean IsClientStarted()
        {
            return Client != null;
        }

        public Queue<string> _mFromServerQueue = new Queue<string>();

        private void Client_OnReceivedMessage(object sender, ReceivedMessageEventArgs e)
        {
            _mFromServerQueue.Enqueue(e.Message);
        }

        public void WaitClient()
        {
            Client.Wait();
        }
        public void StopClient()
        {
            if (this.Client != null)
            {
                Client.Stop();
            }
        }

    }
}