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

using System.IO;

namespace FFramework.Cryptography
{
    public class HexEncoder
    {
        internal static readonly byte[] decodingTable = new byte[0x80];
        private static readonly byte[] encodingTable = new byte[] { 0x30, 0x31, 50, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x61, 0x62, 0x63, 100, 0x65, 0x66 };

        static HexEncoder()
        {
            for (int i = 0; i < encodingTable.Length; i++) decodingTable[encodingTable[i]] = (byte)i;
            decodingTable[0x41] = decodingTable[0x61];
            decodingTable[0x42] = decodingTable[0x62];
            decodingTable[0x43] = decodingTable[0x63];
            decodingTable[0x44] = decodingTable[100];
            decodingTable[0x45] = decodingTable[0x65];
            decodingTable[70] = decodingTable[0x66];
        }

        public int Decode(byte[] data, int off, int length, Stream outStream)
        {
            int num3 = 0;
            int num4 = off + length;
            while (num4 > off)
            {
                if (!ignore((char)data[num4 - 1]))
                    break;
                num4--;
            }
            int index = off;
            while (index < num4)
            {
                while ((index < num4) && ignore((char)data[index]))
                    index++;
                byte num = decodingTable[data[index++]];
                while ((index < num4) && ignore((char)data[index]))
                    index++;
                byte num2 = decodingTable[data[index++]];
                outStream.WriteByte((byte)((num << 4) | num2));
                num3++;
            }
            return num3;
        }

        public int DecodeString(string data, Stream outStream)
        {
            int num3 = 0;
            int length = data.Length;
            while (length > 0)
            {
                if (!ignore(data[length - 1])) break;
                length--;
            }
            int num5 = 0;
            while (num5 < length)
            {
                while ((num5 < length) && ignore(data[num5]))
                    num5++;
                byte num = decodingTable[data[num5++]];
                while ((num5 < length) && ignore(data[num5]))
                    num5++;
                byte num2 = decodingTable[data[num5++]];
                outStream.WriteByte((byte)((num << 4) | num2));
                num3++;
            }
            return num3;
        }

        public int Encode(byte[] data, int off, int length, Stream outStream)
        {
            for (int i = off; i < (off + length); i++)
            {
                int num2 = data[i];
                outStream.WriteByte(encodingTable[num2 >> 4]);
                outStream.WriteByte(encodingTable[num2 & 15]);
            }
            return (length * 2);
        }

        private bool ignore(char c)
        {
            if (((c != '\n') && (c != '\r')) && (c != '\t'))
                return (c == ' ');
            return true;
        }
    }
}
