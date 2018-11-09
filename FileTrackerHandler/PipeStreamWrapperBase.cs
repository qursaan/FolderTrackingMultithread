using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;

namespace FileTrackerHandler
{
    public abstract class PipeStreamWrapperBase<T> where T : PipeStream
    {
        protected const int BUFFER_SIZE = 4096;
        public event EventHandler<ReceivedMessageEventArgs> OnReceivedMessage;

        public PipeStreamWrapperBase(string pipeName)
        {
            if (pipeName == null)
            {
                throw new ArgumentNullException("pipeName", "Argument cannot be null.");
            }
            PipeName = pipeName;
        }

        protected T Pipe { get; set; }
        protected StreamWriter PipeWriter { get; set; }
        protected abstract T CreateStream();
        protected abstract bool AutoFlushPipeWriter { get; }
        protected abstract void ReadFromPipe(object state);
        public string PipeName { get; private set; }


        public void Start()
        {
            Pipe = CreateStream();
            ThreadPool.QueueUserWorkItem(new WaitCallback(ReadFromPipe));
        }

        public void Stop()
        {
            try
            {
                m_stopRequested = true;
                Pipe.Close();
                Pipe.Dispose();
            }
            catch (Exception e)
            {
                
            }
        }

        public void Wait()
        {
            Pipe.WaitForPipeDrain();
        }

        protected bool m_stopRequested = false;

        protected static byte[] ReadMessage(PipeStream stream)
        {
            MemoryStream memoryStream = new MemoryStream();

            byte[] buffer = new byte[BUFFER_SIZE];

            do
            {
                memoryStream.Write(buffer, 0, stream.Read(buffer, 0, buffer.Length));

            } while (stream.IsMessageComplete == false);

            return memoryStream.ToArray();
        }

        public void Write(string message)
        {
            if (Pipe.IsConnected == true && Pipe.CanWrite == true)
            {
                if (PipeWriter == null)
                {
                    PipeWriter = new StreamWriter(Pipe);

                    PipeWriter.AutoFlush = AutoFlushPipeWriter;
                }

                WriteToStream(message);
            }
        }

        protected virtual void WriteToStream(string message)
        {
            PipeWriter.WriteLine(message);
        }

        protected void ThrowOnReceivedMessage(byte[] message)
        {
            if (OnReceivedMessage != null)
            {
                OnReceivedMessage(this, new ReceivedMessageEventArgs(Encoding.Default.GetString(message)));
            }
        }
    }
}