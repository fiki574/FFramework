using System;
using System.IO;

namespace FFramework.Network.Client
{
    public class ClientPacket : MemoryStream
    {
        public ClientPacket() : base() { }

        protected void WriteS(String s)
        {
            WriteByte((Byte)s.Length);
            for (int i = 0; i < s.Length; i++) WriteByte((Byte)s[i]);
        }

        //TODO: more Write functions
    }
}
