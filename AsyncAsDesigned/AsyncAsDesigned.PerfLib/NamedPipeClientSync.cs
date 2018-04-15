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


        public static string AppServerListenPipe(int clientID, string uniqueID)
        {
            return string.Format(_appServerListenPipe, uniqueID, clientID);
        }

        public static string DataServerListenPipe(int clientID, string uniqueID)
        {
            return string.Format(_dataServerListenPipe, uniqueID, clientID);
        }

        private const string _appServerListenPipe = @"\\AsyncAsDesigned\AppServerListenPipeSync\{0}\{1}";
        private const string _dataServerListenPipe = @"\\AsyncAsDesigned\DataServerListenPipeSync\{0}\{1}";

        public static void Send(string pipeName, Token token)
        {

            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.Out, PipeOptions.None))
            {

                pipeClient.Connect();

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
