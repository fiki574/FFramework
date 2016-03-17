using System;
using System.IO;
using System.Text;

namespace FFramework.Network.Client
{
    public class ServerPacket : MemoryStream
    {
        public ServerPacket() : base() { }

        protected String ReadS()
        {
            int length = ReadByte();
            if (length > Length - Position) return "";
            Byte[] str = new Byte[length];
            Read(str, 0, length);
            return Encoding.ASCII.GetString(str);
        }

        //TODO: more Read functions
    }
}
