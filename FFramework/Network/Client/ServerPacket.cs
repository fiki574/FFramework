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
using System.IO;
using System.Text;

namespace FFramework.Network.Client
{
    public class ServerPacket : MemoryStream
    {
        public ServerPacket() : base() { }

        protected string ReadS()
        {
            int length = ReadByte();
            if (length > Length - Position) return "";
            byte[] str = new byte[length];
            Read(str, 0, length);
            return Encoding.ASCII.GetString(str);
        }

        protected object ReadValue(Type t)
        {
            if (t == typeof(short)) return Convert.ToInt16(ReadS());
            else if (t == typeof(int)) return Convert.ToInt32(ReadS());
            else if (t == typeof(long)) return Convert.ToInt64(ReadS());
            else if (t == typeof(double)) return Convert.ToDouble(ReadS());
            else if (t == typeof(bool)) return Convert.ToBoolean(ReadS());
            else if (t == typeof(byte)) return Convert.ToByte(ReadS());
            else if (t == typeof(char)) return Convert.ToChar(ReadS());
            else if (t == typeof(decimal)) return Convert.ToDecimal(ReadS());
            else if (t == typeof(ushort)) return Convert.ToUInt16(ReadS());
            else if (t == typeof(uint)) return Convert.ToUInt32(ReadS());
            else if (t == typeof(ulong)) return Convert.ToUInt64(ReadS());
            else return null;
        }
    }
}
