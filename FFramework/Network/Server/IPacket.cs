using System;
using System.Net.Sockets;

namespace FFramework.Network.Server
{
    public interface IPacket
    {
        void Write(byte[] buffer, int offset, int count);
        void Handle(Client client);
    }
}
