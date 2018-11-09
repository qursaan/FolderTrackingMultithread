using System;

namespace FileTrackerHandler
{
    public class ReceivedMessageEventArgs : EventArgs
    {

        public ReceivedMessageEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}