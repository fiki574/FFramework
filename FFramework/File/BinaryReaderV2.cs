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
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace FFramework.File
{
    public class BinaryReaderV2 : BinaryReader
    {
        private Stream baseStream;

        public long Position
        {
            get 
            { 
                return this.BaseStream.Position;
            }
            set 
            { 
                this.BaseStream.Position = value; 
            }
        }

        public BinaryReaderV2(Stream input) : base(input)
        {
            baseStream = this.BaseStream;
        }

        public BinaryReaderV2(byte[] data) : base(new MemoryStream(data))
        {
            baseStream = this.BaseStream;
        }

        public string ReadUString()
        {
            long ori = this.BaseStream.Position;
            long offset = ori;
            string res;
            this.BaseStream.Position = ori;
            short c;
            int size = 0;
            do
            {
                c = this.ReadInt16();
                size++;
            }
            while (c != 0);
            this.BaseStream.Position = offset;
            res = Encoding.Unicode.GetString(this.ReadBytes(size * 2)).Trim('\0');
            return res;
        }

        public string ReadUString(int offset)
        {
            long ori = this.BaseStream.Position;
            string res;
            this.BaseStream.Position = offset;
            short c;
            int size = 0;
            do
            {
                c = this.ReadInt16();
                size++;
            } 
            while (c != 0);
            this.BaseStream.Position = offset;
            res = Encoding.Unicode.GetString(this.ReadBytes(size * 2)).Trim('\0');
            this.BaseStream.Position = ori;
            return res;
        }

        public byte[] GetData()
        {
            return this.ReadBytes((int)this.baseStream.Length);
        }

        public void SuperClose()
        {
            try
            {
                this.Close();
                baseStream.Close();
            }
            catch (System.Exception)
            {
            }
        }
    }
}
