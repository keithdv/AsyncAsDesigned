using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAsDesigned.PerfLib
{
    public class NamedPipeServerSync
    {

        string pipeName;
        static BinaryFormatter formatter = new BinaryFormatter();


        public delegate Task TokenReceived(Token token);
        public event TokenReceived TokenReceivedEvent;

        public NamedPipeServerSync(string pipeName)
        {
            this.pipeName = pipeName;
        }

        public void Start(bool oneMessage = false)
        {
            
            Token token = null;
            List<Task> tokenReceivedEventTasks = new List<Task>();

            do
            {
                using (var pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.None))
                {

                    pipeServer.WaitForConnection();

                    token = Received(pipeServer);

                    pipeServer.Close();

                }

                if (token != null && !token.End)
                {
                    tokenReceivedEventTasks.Add(TokenReceivedEvent?.Invoke(token));
                }
            }
            while (!(token?.End ?? false) && !oneMessage);

            // Ensure that we wait for the DataServer to respond for any pending Tasks
            Task.WaitAll(tokenReceivedEventTasks.ToArray());

        }

        private static Token Received(NamedPipeServerStream namedPipeServerStream)
        {
            int len = 0;

            len = namedPipeServerStream.ReadByte() * 256;
            len += namedPipeServerStream.ReadByte();
            byte[] inBuffer = new byte[len];
            namedPipeServerStream.Read(inBuffer, 0, len);

            Token token;

            using (var mem = new MemoryStream(inBuffer))
            {
                token = (Token)formatter.Deserialize(mem);
            }

            return token;


        }

    }
}
