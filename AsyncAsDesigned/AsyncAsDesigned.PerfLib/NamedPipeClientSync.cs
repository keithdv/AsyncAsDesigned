using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace AsyncAsDesigned.PerfLib
{
    public static class NamedPipeClientSync
    {


        public static string ClientToAppServerPipeName(int clientID, string unique)
        {
            return string.Format(_clientToAppServerPipeName, clientID, unique);
        }

        public static string AppServerToDataServerPipeName(int clientID, string unique)
        {
            return string.Format(_appServerToDataServerPipeName, clientID, unique);
        }

        public static string DataServerToAppServerPipeName(int clientID, string unique)
        {
            return string.Format(_dataServerToAppServerPipeName, clientID, unique);
        }

        public static string AppServerToClientPipeName(int clientID, string unique)
        {
            return string.Format(_appServerToClientPipeName, clientID, unique);
        }

        private const string _clientToAppServerPipeName = @"\\AsyncAsDesigned\ClientToAppServerPipe\{0}\{1}";
        private const string _appServerToDataServerPipeName = @"\\AsyncAsDesigned\AppServerToDataServerPipe\{0}\{1}";
        private const string _dataServerToAppServerPipeName = @"\\AsyncAsDesigned\DataServerToAppServerPipe\{0}\{1}";
        private const string _appServerToClientPipeName = @"\\AsyncAsDesigned\AppServerToClientPipeName\{0}\{1}";

        public static void Send(string pipeName, Token token)
        {

            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.Out, PipeOptions.None))
            {
                Exception ex = new Exception();

                var tryAgain = true;
                while (tryAgain)
                {
                    try
                    {
                        pipeClient.Connect();
                        tryAgain = false;
                    }
                    catch (FileNotFoundException)
                    {
                        Task.Delay(25).Wait();
                    }
                }

                SendToken(pipeClient, token);

                pipeClient.Flush();
                pipeClient.Close();

            }
        }

        internal static void SendToken(Stream pipeClient, Token token)
        {

            BinaryFormatter formatter = new BinaryFormatter();

            byte[] outBuffer = null;

            using (var mem = new MemoryStream())
            {
                formatter.Serialize(mem, token);
                outBuffer = mem.ToArray();
                mem.Flush();
            }

            int len = outBuffer.Length;
            if (len > UInt16.MaxValue)
            {
                len = (int)UInt16.MaxValue;
            }

            pipeClient.WriteByte((byte)(len / 256));
            pipeClient.WriteByte((byte)(len & 255));
            pipeClient.Write(outBuffer, 0, len);

        }


    }
}
