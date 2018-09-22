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
    
    Credits: https://github.com/usertoroot
*/

using System.IO;
using System.Text;

namespace FFramework.Memory
{
    public class ByteBuffer
    {
        long mReadPos, mWritePos;
        private MemoryStream mData;
        private BinaryReader mBinRead;
        private BinaryWriter mBinWrite;

        public ByteBuffer()
        {
            mData = new MemoryStream();
            mBinRead = new BinaryReader(mData);
            mBinWrite = new BinaryWriter(mData);
            mReadPos = 0;
            mWritePos = 0;
        }

        public int GetSize()
        {
            return (int)mData.Length;
        }

        public int GetBytesLeft()
        {
            return (int)(mWritePos - mReadPos);
        }

        public void PopEndBytes(int size)
        {
            mData.SetLength(mData.Length - size);
            mWritePos -= size;
        }

        public byte[] getBuffer()
        {
            return mData.ToArray();
        }

        public byte[] getBufferLeft()
        {
            byte[] returnArray = new byte[GetBytesLeft()];
            mData.Read(returnArray, 0, GetBytesLeft());
            mData.Position = mReadPos;
            return returnArray;
        }

        public void Clear()
        {
            mReadPos = 0;
            mWritePos = 0;
            mData.Dispose();
            mData = new MemoryStream();
            mBinRead = new BinaryReader(mData);
            mBinWrite = new BinaryWriter(mData);
        }

        public byte ReadByte()
        {
            mData.Position = mReadPos;
            byte data = (byte)mData.ReadByte();
            mReadPos = mData.Position;
            return data;
        }

        public ushort ReadShort()
        {
            mData.Position = mReadPos;
            ushort data = mBinRead.ReadUInt16();
            mReadPos = mData.Position;
            return data;
        }

        public uint ReadInt()
        {
            mData.Position = mReadPos;
            uint data = mBinRead.ReadUInt32();
            mReadPos = mData.Position;
            return data;
        }

        public ulong ReadLong()
        {
            mData.Position = mReadPos;
            ulong data = mBinRead.ReadUInt64();
            mReadPos = mData.Position;
            return data;
        }

        public float ReadFloat()
        {
            mData.Position = mReadPos;
            float data = mBinRead.ReadSingle();
            mReadPos = mData.Position;
            return data;
        }

        public virtual string ReadAString()
        {
            mData.Position = mReadPos;
            StringBuilder retStr = new StringBuilder();
            byte strLen = (byte)mData.ReadByte();
            strLen -= 128;
            for (byte i = 0; i < strLen; ++i) retStr.Append((char)mData.ReadByte());
            mReadPos = mData.Position;
            return retStr.ToString();
        }

        public virtual string ReadUString()
        {
            mData.Position = mReadPos;
            StringBuilder retStr = new StringBuilder();
            byte strLen = (byte)mData.ReadByte();
            strLen -= 128;
            for (byte i = 0; i < strLen; ++i)
                retStr.Append((char)mBinRead.ReadUInt16());
            mReadPos = mData.Position;
            return retStr.ToString();
        }

        public byte[] ReadData(int length)
        {
            mData.Position = mReadPos;
            byte[] returnArray = new byte[length];
            mData.Read(returnArray, 0, length);
            mReadPos = mData.Position;
            return returnArray;
        }

        public void WriteByte(byte data)
        {
            mData.Position = mWritePos;
            mBinWrite.Write(data);
            mWritePos = mData.Position;
        }

        public void WriteShort(ushort data)
        {
            mData.Position = mWritePos;
            mBinWrite.Write(data);
            mWritePos = mData.Position;
        }

        public void WriteInt(uint data)
        {
            mData.Position = mWritePos;
            mBinWrite.Write(data);
            mWritePos = mData.Position;
        }

        public void WriteLong(ulong data)
        {
            mData.Position = mWritePos;
            mBinWrite.Write(data);
            mWritePos = mData.Position;
        }

        public void WriteFloat(float data)
        {
            mData.Position = mWritePos;
            mBinWrite.Write(data);
            mWritePos = mData.Position;
        }

        public virtual void WriteAString(string data)
        {
            mData.Position = mWritePos;
            byte strLen = (byte)data.Length;
            mData.WriteByte((byte)(strLen + 128));
            for (int i = 0; i < strLen; ++i)
                mData.WriteByte((byte)data[i]);
            mWritePos = mData.Position;
        }

        public virtual void WriteUString(string data)
        {
            mData.Position = mWritePos;
            byte strLen = (byte)data.Length;
            mData.WriteByte((byte)(strLen + 128));
            for (int i = 0; i < strLen; ++i)
                mBinWrite.Write((ushort)data[i]);
            mWritePos = mData.Position;
        }

        public void WriteData(byte[] data)
        {
            mData.Position = mWritePos;
            mData.Write(data, 0, data.Length);
            mWritePos = mData.Position;
        }

        public void WriteBuffer(ByteBuffer data)
        {
            mData.Position = mWritePos;
            byte[] dataArray = data.getBuffer();
            mData.Write(dataArray, (int)mData.Length, data.GetSize());
            mWritePos = mData.Position;
        }

        public byte PeekByte()
        {
            long myPos = mReadPos;
            byte data = ReadByte();
            mReadPos = myPos;
            return data;
        }

        public ushort PeekShort()
        {
            long myPos = mReadPos;
            ushort data = ReadShort();
            mReadPos = myPos;
            return data;
        }

        public uint PeekInt()
        {
            long myPos = mReadPos;
            uint data = ReadInt();
            mReadPos = myPos;
            return data;
        }

        public ulong PeekLong()
        {
            long myPos = mReadPos;
            ulong data = ReadLong();
            mReadPos = myPos;
            return data;
        }

        public float PeekFloat()
        {
            long myPos = mReadPos;
            float data = ReadFloat();
            mReadPos = myPos;
            return data;
        }

        public byte this[int index]
        {
            get
            {
                long pos = mData.Position;
                mData.Seek(index, SeekOrigin.Begin);
                byte data = (byte)mData.ReadByte();
                mData.Position = pos;
                return data;
            }
            set
            {
                byte[] data = mData.GetBuffer();
                data[index] = value;
            }
        }
    }
}