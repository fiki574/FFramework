/*
    C# Framework with a lot of useful functions and classes
    Copyright (C) 2019/2020 Bruno Fištrek

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

using System.IO;

namespace FFramework_Crypto.Other
{
    public sealed class Hex
    {
        private static readonly HexEncoder encoder = new HexEncoder();

        private Hex()
        {
        }

        public static byte[] Decode(string data)
        {
            MemoryStream outStream = new MemoryStream((data.Length + 1) / 2);
            encoder.DecodeString(data, outStream);
            return outStream.ToArray();
        }

        public static byte[] Decode(byte[] data)
        {
            MemoryStream outStream = new MemoryStream((data.Length + 1) / 2);
            encoder.Decode(data, 0, data.Length, outStream);
            return outStream.ToArray();
        }

        public static int Decode(string data, Stream outStream)
        {
            return encoder.DecodeString(data, outStream);
        }

        public static byte[] Encode(byte[] data)
        {
            return Encode(data, 0, data.Length);
        }

        public static int Encode(byte[] data, Stream outStream)
        {
            return encoder.Encode(data, 0, data.Length, outStream);
        }

        public static byte[] Encode(byte[] data, int off, int length)
        {
            MemoryStream outStream = new MemoryStream(length * 2);
            encoder.Encode(data, off, length, outStream);
            return outStream.ToArray();
        }

        public static int Encode(byte[] data, int off, int length, Stream outStream)
        {
            return encoder.Encode(data, off, length, outStream);
        }
    }
}