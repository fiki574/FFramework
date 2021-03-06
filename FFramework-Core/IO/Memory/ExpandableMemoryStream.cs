﻿/*
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
    
    Credits: https://github.com/usertoroot
*/

using System;
using System.IO;

namespace FFramework_Core.IO.Memory
{
    public class ExpandableMemoryStream : Stream
    {
        public byte[] Data
        {
            get;
            private set;
        }

        private int m_initalSize, m_blockSize, m_position = 0, m_length = 0;

        public ExpandableMemoryStream(int initialSize = 2048, int blockSize = 4096)
        {
            m_initalSize = initialSize;
            m_blockSize = blockSize;
            Data = new byte[m_initalSize];
        }

        public void EnsureLength(int length)
        {
            if (length > Data.Length)
            {
                int blocks = (int)Math.Ceiling(length / (double)m_blockSize);
                byte[] newData = new byte[blocks * m_blockSize];
                Array.Copy(Data, newData, m_length);
                Data = newData;
            }
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override void Flush()
        {
        }

        public override long Length
        {
            get
            {
                return m_length;
            }
        }

        public override long Position
        {
            get
            {
                return m_position;
            }
            set
            {
                if (value > Data.Length) EnsureLength((int)value);
                m_position = (int)value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (m_position + count > Data.Length)
                return -1;

            Array.Copy(Data, m_position, buffer, offset, count);
            m_position += count;
            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = m_length - offset;
                    break;
            }
            return m_position;
        }

        public override void SetLength(long value)
        {
            EnsureLength((int)value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            EnsureLength(m_position + count);
            Array.Copy(buffer, offset, Data, m_position, count);
            if (m_position + count > m_length)
                m_length = m_position + count;
            m_position += count;
        }

        public byte[] ToArray()
        {
            byte[] data = new byte[m_length];
            Array.Copy(Data, data, m_length);
            return data;
        }
    }
}