using System;
using System.IO;

namespace FFramework.Network.Server
{
    public class ServerPacket : MemoryStream
    {
        public ServerPacket() : base() { }

        protected void WriteS(String s)
        {
            WriteByte((Byte)s.Length);
            for (int i = 0; i < s.Length; i++) WriteByte((Byte)s[i]);
        }

        //TODO: more Write functions
    }
}
