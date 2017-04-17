/*
    C# Framework with a lot of useful functions and classes
    Copyright (C) 2017 Bruno Fištrek

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Net;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace FFramework.Network.Client
{
    public class Client
    {
        public TcpClient client;
        private Thread clientThread;
        private NetworkStream stream;
        private IPEndPoint address;
        private Dictionary<byte, IPacket> _packets;
        public bool liveConnection = false;

        public Client(string ip, int Port)
        {
            address = new IPEndPoint(IPAddress.Parse(ip), Port);
            client = new TcpClient();
            _packets = new Dictionary<byte, IPacket>();
            connect(address);
            stream = client.GetStream();
            clientThread = new Thread(new ThreadStart(handleServer));
            clientThread.Start();
        }

        public void AddReceivePacket(byte opcode, IPacket packet)
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
            byte[] message = new byte[4096];
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
                if (bytesRead == 0) break;

                IPacket packet = null;
                foreach (KeyValuePair<byte, IPacket> p in _packets)
                {
                    if (p.Key == message[0])
                    {
                        packet = p.Value;
                        break;
                    }
                }
                packet.EmptyData();
                packet.Write(message, 1, message.Length - 1);
                packet.Handle();
            }
            client.Close();
            stream.Dispose();
        }

        public void Send(SendPacket packet)
        {
            byte[] array = packet.ToArray();
            stream.Write(array, 0, array.Length);
            stream.Flush();
        }
    }
}
