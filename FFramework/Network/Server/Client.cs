using System;
using System.Net.Sockets;

namespace FFramework.Network.Server
{
    public class Client
    {
        public TcpClient tcp
        {
            get;
            private set;
        }

        public Client(TcpClient client)
        {
            tcp = client;
        }

        public void Send(ServerPacket packet)
        {
            Byte[] array = packet.ToArray();
            tcp.GetStream().Write(array, 0, array.Length);
            tcp.GetStream().Flush();
        }
    }
}
