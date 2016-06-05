/*
    C# Framework with a lot of useful functions and classes
    Copyright (C) 2016 Bruno Fištrek

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
        private Dictionary<Byte, IPacket> _packets
        {
            get;
            set;
        }

        public bool liveConnection = false;

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
            while (stream.Read(message, 0, 4096) != 0)
            {
                IPacket packet = null;
                foreach (KeyValuePair<Byte, IPacket> p in _packets)
                {
                    if (p.Key == message[0])
                    {
                        packet = p.Value;
                        break;
                    }
                }
                packet.Write(message, 1, message.Length - 1);
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
