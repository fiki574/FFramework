/*
    C# Framework with a lot of useful functions and classes
    Copyright (C) 2018/2019 Bruno Fištrek

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
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using FFramework.Memory;
using FFramework.Extensions;

namespace FFramework.Network
{
    public partial class Session
    {
        private bool m_closed = false;
        private int m_expectedLength = 0, m_offset = 0;
        private NetworkStream m_nStream;
        private TcpClient m_tcpClient;
        private VariableBuffer m_variableBuffer = new VariableBuffer(512, 512);
        private static Dictionary<InterfaceType, Dictionary<int, PacketHandler>> _handlerMap = new Dictionary<InterfaceType, Dictionary<int, PacketHandler>>();
        private delegate void PacketHandler(Session session, BinaryReader reader);

        public Session(TcpClient tcpClient)
        {
            m_tcpClient = tcpClient;
            m_nStream = m_tcpClient.GetStream();
            m_nStream.BeginRead(m_variableBuffer.Data, 0, 4, OnReceiveLength, null);
        }

        public static void MapHandlers()
        {
            Type t = typeof(Session);
            foreach (var m in t.GetMethods(BindingFlags.NonPublic | BindingFlags.Static))
            {
                object[] o = m.GetCustomAttributes(typeof(Handler), false);
                if (o.Length > 0)
                {
                    Handler h = (Handler)o[0];
                    Dictionary<int, PacketHandler> d;
                    if (_handlerMap.ContainsKey(h.OpcodeHandler))
                        d = _handlerMap[h.OpcodeHandler];
                    else
                    {
                        d = new Dictionary<int, PacketHandler>();
                        _handlerMap.Add(h.OpcodeHandler, d);
                    }

                    if (d.ContainsKey(h.Type))
                        continue;

                    d.Add(h.Type, (PacketHandler)Delegate.CreateDelegate(typeof(PacketHandler), m));
                }
            }
        }

        public void OnReceiveData(IAsyncResult result)
        {
            if (m_closed)
                return;

            try
            {
                int bytesReceived = m_nStream.EndRead(result);
                if (bytesReceived == 0)
                {
                    Close();
                    return;
                }
                else if (m_offset + bytesReceived < m_expectedLength)
                {
                    m_offset += bytesReceived;
                    m_nStream.BeginRead(m_variableBuffer.Data, m_offset, m_expectedLength - m_offset, OnReceiveData, null);
                    return;
                }

                byte[] packet = new byte[m_expectedLength];
                Array.Copy(m_variableBuffer.Data, packet, m_expectedLength);
                ThreadPool.QueueUserWorkItem(HandlePacket, packet);
                m_nStream.BeginRead(m_variableBuffer.Data, 0, 4, OnReceiveLength, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Close();
            }
        }

        public void OnReceiveLength(IAsyncResult result)
        {
            if (m_closed)
                return;

            try
            {
                int bytesReceived = m_nStream.EndRead(result);
                if (bytesReceived == 0)
                {
                    Close();
                    return;
                }
                else if (bytesReceived != 4)
                {
                    m_nStream.BeginRead(m_variableBuffer.Data, 0, 4, OnReceiveLength, null);
                    return;
                }

                m_expectedLength = BitConverter.ToInt32(m_variableBuffer.Data, 0);
                m_variableBuffer.EnsureLength(m_expectedLength);
                m_offset = 0;
                m_nStream.BeginRead(m_variableBuffer.Data, m_offset, m_expectedLength, OnReceiveData, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Close();
            }
        }

        public void HandlePacket(object oPacket)
        {
            if (m_closed)
                return;

            try
            {
                byte[] packet = (byte[])oPacket;
                using (BinaryReader reader = new BinaryReader(new MemoryStream(packet)))
                {
                    int iface = reader.ReadInt32();
                    int pid = reader.ReadInt32();
                    Dictionary<int, PacketHandler> handlerMap;
                    if (!_handlerMap.TryGetValue((InterfaceType)iface, out handlerMap))
                    {
                        Close();
                        return;
                    }

                    PacketHandler h;
                    if (!handlerMap.TryGetValue(pid, out h))
                    {
                        Close();
                        return;
                    }

                    h(this, reader);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Close();
            }
        }

        public void Send(byte[] response, int size)
        {
            try
            {
                if (m_closed)
                    return;

                lock (m_nStream)
                {
                    m_nStream.Write(BitConverter.GetBytes(size), 0, 4);
                    m_nStream.Write(response, 0, size);
                }
            }
            catch
            {
                Close();
            }
        }

        public void Send(Action<BinaryWriter> writerAction)
        {
            if (m_closed)
                return;

            using (ExpandableMemoryStream responseStream = new ExpandableMemoryStream())
            using (BinaryWriter writer = new BinaryWriter(responseStream))
            {
                writerAction(writer);
                var size = responseStream.Position;
                Send(responseStream.ToArray(), (int)size);
            }
        }

        public virtual void Send(object structure)
        {
            Send(w => w.WriteStructure(structure));
        }

        public void Close()
        {
            if (m_closed)
                return;

            m_closed = true;

            try
            {
                if (m_tcpClient != null)
                    m_tcpClient.Close();
            }
            finally
            {
                m_tcpClient = null;
            }
        }
    }
}
