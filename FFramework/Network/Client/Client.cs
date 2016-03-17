using System;
using System.Net;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace FFramework.Network.Client
{
    public class Client
    {
        public TcpClient client
        {
            get;
            private set;
        }

        private Thread clientThread;
        private NetworkStream stream;
        private IPEndPoint address;

        public bool liveConnection = false;

        private Dictionary<Byte, IPacket> _packets
        {
            get;
            set;
        }

        public Client(String ip, int Port)
        {
            address = new IPEndPoint(IPAddress.Parse(ip), Port);
            client = new TcpClient();
            _packets = new Dictionary<Byte, IPacket>();
            connect(address);
            stream = client.GetStream();
            clientThread = new Thread(new ThreadStart(handleServer));
            clientThread.Start();
        }

        public void AddReceivePacket(Byte opcode, IPacket packet)
        {
            _packets.Add(opcode, packet);
        }

        private void connect(IPEndPoint serverEndPoint)
        {
            try
            {
                client.Connect(serverEndPoint);
                liveConnection = true;
            }
            catch (SocketException)
            {
                liveConnection = false;
            }
        }

        private void handleServer()
        {
            Byte[] message = new Byte[4096];
            int bytesRead;
            while (true)
            {
                bytesRead = 0;
                try
                {
                    bytesRead = stream.Read(message, 0, 4096);
                }
                catch
                {
                    break;
                }
                if (bytesRead == 0)
                {
                    break;
                }

                IPacket packet = null;
                foreach (KeyValuePair<Byte, IPacket> p in _packets)
                {
                    if (p.Key == message[0])
                    {
                        packet = p.Value;
                        break;
                    }
                }
                packet.Write(message, 1, bytesRead - 1);
                packet.Handle();
            }
            client.Close();
            stream.Dispose();
        }

        public void Send(ClientPacket packet)
        {
            Byte[] array = packet.ToArray();
            stream.Write(array, 0, array.Length);
            stream.Flush();
        }
    }
}
