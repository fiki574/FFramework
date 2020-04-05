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

using System.Text;
using System.IO;

namespace FFramework_Core.IO.Memory
{
    public class StringStream : MemoryStream
    {
        private int count;

        public StringStream() : base()
        {
            count = 0;
        }

        public void Write(object data)
        {
            if (!data.GetType().IsPrimitive)
                return;

            string stringData = data.ToString();
            WriteByte((byte)stringData.Length);
            for (int i = 0; i < stringData.Length; i++)
                WriteByte((byte)stringData[i]);

            count++;
        }

        public object Read()
        {
            int length = ReadByte();
            if (length > Length - Position)
                return null;

            byte[] str = new byte[length];
            Read(str, 0, length);
            return Encoding.ASCII.GetString(str);
        }

        public void ResetPosition()
        {
            Position = 0;
        }

        public void CloseAndDispose()
        {
            Close();
            Dispose();
        }

        public int GetObjectCount()
        {
            return count;
        }
    }
}