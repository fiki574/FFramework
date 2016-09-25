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
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using FFramework.Utilities;

namespace FFramework.Network.Server
{
    public class Listener
    {
        private TcpListener tcpListener;
        private Thread listenThread;
        private String IP;
        private int Port;
        private ThreadSafeList<Client> _clients = new ThreadSafeList<Client>();
        private Dictionary<Byte, IPacket> _packets;

        public Listener(String ip, Int32 port)
        {
            IPAddress address = IPAddress.Any;
            _packets = new Dictionary<Byte, IPacket>();
            try
            {
                address = IPAddress.Parse(ip);
            }
            catch
            {
                throw new Exception("Failed to start listener on chosen address");
            }
            IP = address.ToString();
            Port = port;
            tcpListener = new TcpListener(address, port);
            listenThread = new Thread(new ThreadStart(listenForClients));
            listenThread.Start();
        }

        public void AddReceivePacket(Byte opcode, IPacket packet)
        {
            _packets.Add(opcode, packet);
        }

        private void listenForClients()
        {
            try
            {
                this.tcpListener.Start();
            }
            catch (SocketException)
            {
                listenForClients();
            }
            while (true)
            {
                TcpClient client = this.tcpListener.AcceptTcpClient();
                Thread clientThread = new Thread(new ParameterizedThreadStart(handleClient));
                clientThread.Start(client);
            }
        }

        private void handleClient(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            Client cclient = new Client(tcpClient);
            _clients.Add(cclient);
            NetworkStream clientStream = tcpClient.GetStream();
            Byte[] message = new Byte[4096];
            while (clientStream.Read(message, 0, 4096) != 0)
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
                packet.Handle(cclient);
            }
            lock (_clients)
            {
                _clients.Remove(cclient);
            }
            tcpClient.Close();
        }
    }
}
