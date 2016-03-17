using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFramework.Utilities
{
    ///<summary>Used in exact same way as normal memory stream, though this extension allows run-time expansion of default stream size of 2048/4096 bytes</summary>
    public class ExpandableMemoryStream : Stream
    {
        private byte[] m_data;

        public byte[] Data
        {
            get
            {
                return m_data;
            }
        }

        private int m_initalSize;
        private int m_blockSize;
        private int m_position = 0;
        private int m_length = 0;

        public ExpandableMemoryStream(int initialSize = 2048, int blockSize = 4096)
        {
            m_initalSize = initialSize;
            m_blockSize = blockSize;
            m_data = new byte[m_initalSize];
        }

        public void EnsureLength(int length)
        {
            if (length > m_data.Length)
            {
                int blocks = (int)Math.Ceiling(length / (double)m_blockSize);
                byte[] newData = new byte[blocks * m_blockSize];
                Array.Copy(m_data, newData, m_length);
                m_data = newData;
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
                if (value > m_data.Length) EnsureLength((int)value);
                m_position = (int)value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (m_position + count > m_data.Length) throw new IndexOutOfRangeException();
            Array.Copy(m_data, m_position, buffer, offset, count);
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
            Array.Copy(buffer, offset, m_data, m_position, count);
            if (m_position + count > m_length) m_length = m_position + count;
            m_position += count;
        }

        public byte[] ToArray()
        {
            byte[] data = new byte[m_length];
            Array.Copy(m_data, data, m_length);
            return data;
        }
    }
}
