using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace client
{
    public class TelnetClient
    {
        private TcpClient client;
        private StreamReader reader;
        private StreamWriter writer;

        public TelnetClient(string ip, int port = 23)
        {
            client = new TcpClient(ip, port);
            reader = new StreamReader(client.GetStream());
            writer = new StreamWriter(client.GetStream());
        }

        public string Read()
        {
            return reader.ReadLine();
        }

        public void Write(string line)
        {
            writer.WriteLine(line);
            writer.Flush();

        }
    }
}
