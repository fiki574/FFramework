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

using System;
using System.IO;

namespace FFramework.Cryptography.Other
{
    public class CRC32
    {
        public long TotalBytesRead
        {
            get
            {
                return _TotalBytesRead;
            }
        }

        public int Crc32Result
        {
            get
            {
                return unchecked((int)(~_register));
            }
        }

        public int GetCrc32(Stream input)
        {
            return GetCrc32AndCopy(input, null);
        }

        public int GetCrc32AndCopy(Stream input, Stream output)
        {
            if (input == null)
                return 0;

            unchecked
            {
                byte[] buffer = new byte[BUFFER_SIZE];
                int readSize = BUFFER_SIZE;
                _TotalBytesRead = 0;
                int count = input.Read(buffer, 0, readSize);
                if (output != null)
                    output.Write(buffer, 0, count);
                _TotalBytesRead += count;
                while (count > 0)
                {
                    SlurpBlock(buffer, 0, count);
                    count = input.Read(buffer, 0, readSize);
                    if (output != null)
                        output.Write(buffer, 0, count);
                    _TotalBytesRead += count;
                }
                return (int)(~_register);
            }
        }

        public int ComputeCrc32(int W, byte B)
        {
            return _InternalComputeCrc32((uint)W, B);
        }

        internal int _InternalComputeCrc32(uint W, byte B)
        {
            return (int)(crc32Table[(W ^ B) & 0xFF] ^ (W >> 8));
        }

        public void SlurpBlock(byte[] block, int offset, int count)
        {
            if (block == null)
                return;

            for (int i = 0; i < count; i++)
            {
                int x = offset + i;
                byte b = block[x];
                if (reverseBits)
                {
                    uint temp = (_register >> 24) ^ b;
                    _register = (_register << 8) ^ crc32Table[temp];
                }
                else
                {
                    uint temp = (_register & 0x000000FF) ^ b;
                    _register = (_register >> 8) ^ crc32Table[temp];
                }
            }
            _TotalBytesRead += count;
        }

        public void UpdateCRC(byte b)
        {
            if (reverseBits)
            {
                uint temp = (_register >> 24) ^ b;
                _register = (_register << 8) ^ crc32Table[temp];
            }
            else
            {
                uint temp = (_register & 0x000000FF) ^ b;
                _register = (_register >> 8) ^ crc32Table[temp];
            }
        }

        public void UpdateCRC(byte b, int n)
        {
            while (n-- > 0)
            {
                if (reverseBits)
                {
                    uint temp = (_register >> 24) ^ b;
                    _register = (_register << 8) ^ crc32Table[(temp >= 0) ? temp : (temp + 256)];
                }
                else
                {
                    uint temp = (_register & 0x000000FF) ^ b;
                    _register = (_register >> 8) ^ crc32Table[(temp >= 0) ? temp : (temp + 256)];
                }
            }
        }

        private static uint ReverseBits(uint data)
        {
            unchecked
            {
                uint ret = data;
                ret = (ret & 0x55555555) << 1 | (ret >> 1) & 0x55555555;
                ret = (ret & 0x33333333) << 2 | (ret >> 2) & 0x33333333;
                ret = (ret & 0x0F0F0F0F) << 4 | (ret >> 4) & 0x0F0F0F0F;
                ret = (ret << 24) | ((ret & 0xFF00) << 8) | ((ret >> 8) & 0xFF00) | (ret >> 24);
                return ret;
            }
        }

        private static byte ReverseBits(byte data)
        {
            unchecked
            {
                uint u = (uint)data * 0x00020202;
                uint m = 0x01044010;
                uint s = u & m;
                uint t = (u << 2) & (m << 1);
                return (byte)((0x01001001 * (s + t)) >> 24);
            }
        }

        private void GenerateLookupTable()
        {
            crc32Table = new uint[256];
            unchecked
            {
                uint dwCrc;
                byte i = 0;
                do
                {
                    dwCrc = i;
                    for (byte j = 8; j > 0; j--)
                    {
                        if ((dwCrc & 1) == 1)
                            dwCrc = (dwCrc >> 1) ^ dwPolynomial;
                        else dwCrc >>= 1;
                    }
                    if (reverseBits)
                        crc32Table[ReverseBits(i)] = ReverseBits(dwCrc);
                    else crc32Table[i] = dwCrc;
                    i++;
                }
                while (i != 0);
            }
        }


        private uint Gf2_matrix_times(uint[] matrix, uint vec)
        {
            uint sum = 0;
            int i = 0;
            while (vec != 0)
            {
                if ((vec & 0x01) == 0x01)
                    sum ^= matrix[i];
                vec >>= 1;
                i++;
            }
            return sum;
        }

        private void Gf2_matrix_square(uint[] square, uint[] mat)
        {
            for (int i = 0; i < 32; i++)
                square[i] = Gf2_matrix_times(mat, mat[i]);
        }

        public void Combine(int crc, int length)
        {
            uint[] even = new uint[32];
            uint[] odd = new uint[32];

            if (length == 0)
                return;

            uint crc1 = ~_register;
            uint crc2 = (uint)crc;

            odd[0] = dwPolynomial;
            uint row = 1;
            for (int i = 1; i < 32; i++)
            {
                odd[i] = row;
                row <<= 1;
            }

            Gf2_matrix_square(even, odd);
            Gf2_matrix_square(odd, even);

            uint len2 = (uint)length;
            do
            {
                Gf2_matrix_square(even, odd);
                if ((len2 & 1) == 1)
                    crc1 = Gf2_matrix_times(even, crc1);
                len2 >>= 1;
                if (len2 == 0)
                    break;
                Gf2_matrix_square(odd, even);
                if ((len2 & 1) == 1)
                    crc1 = Gf2_matrix_times(odd, crc1);
                len2 >>= 1;
            }
            while (len2 != 0);

            crc1 ^= crc2;
            _register = ~crc1;
            return;
        }

        public CRC32() : this(false)
        {
        }

        public CRC32(bool reverseBits) : this(unchecked((int)0xEDB88320), reverseBits)
        {
        }

        public CRC32(int polynomial, bool reverseBits)
        {
            this.reverseBits = reverseBits;
            dwPolynomial = (uint)polynomial;
            GenerateLookupTable();
        }

        public void Reset()
        {
            _register = 0xFFFFFFFFU;
        }

        private readonly uint dwPolynomial;
        private long _TotalBytesRead;
        private readonly bool reverseBits;
        private uint[] crc32Table;
        private const int BUFFER_SIZE = 8192;
        private uint _register = 0xFFFFFFFFU;
    }

    public class CrcCalculatorStream : Stream, IDisposable
    {
        private static readonly long UnsetLengthLimit = -99;
        internal Stream _innerStream;
        private CRC32 _Crc32;
        private readonly long _lengthLimit = -99;
        private bool _leaveOpen;

        public CrcCalculatorStream(Stream stream) : this(true, UnsetLengthLimit, stream, null)
        {
        }

        public CrcCalculatorStream(Stream stream, bool leaveOpen) : this(leaveOpen, UnsetLengthLimit, stream, null)
        {
        }

        public CrcCalculatorStream(Stream stream, long length) : this(true, length, stream, null)
        {
            if (length < 0)
                return;
        }

        public CrcCalculatorStream(Stream stream, long length, bool leaveOpen) : this(leaveOpen, length, stream, null)
        {
            if (length < 0)
                return;
        }

        public CrcCalculatorStream(Stream stream, long length, bool leaveOpen, CRC32 crc32) : this(leaveOpen, length, stream, crc32)
        {
            if (length < 0)
                return;
        }

        private CrcCalculatorStream(bool leaveOpen, long length, Stream stream, CRC32 crc32) : base()
        {
            _innerStream = stream;
            _Crc32 = crc32 ?? new CRC32();
            _lengthLimit = length;
            _leaveOpen = leaveOpen;
        }

        public long TotalBytesSlurped
        {
            get
            {
                return _Crc32.TotalBytesRead;
            }
        }

        public int Crc
        {
            get
            {
                return _Crc32.Crc32Result;
            }
        }

        public bool LeaveOpen
        {
            get
            {
                return _leaveOpen;
            }
            set
            {
                _leaveOpen = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesToRead = count;
            if (_lengthLimit != UnsetLengthLimit)
            {
                if (_Crc32.TotalBytesRead >= _lengthLimit)
                    return 0;
                long bytesRemaining = _lengthLimit - _Crc32.TotalBytesRead;
                if (bytesRemaining < count)
                    bytesToRead = (int)bytesRemaining;
            }
            int n = _innerStream.Read(buffer, offset, bytesToRead);
            if (n > 0)
                _Crc32.SlurpBlock(buffer, offset, n);
            return n;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (count > 0)
                _Crc32.SlurpBlock(buffer, offset, count);
            _innerStream.Write(buffer, offset, count);
        }

        public override bool CanRead
        {
            get
            {
                return _innerStream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return _innerStream.CanWrite;
            }
        }

        public override void Flush()
        {
            _innerStream.Flush();
        }

        public override long Length
        {
            get
            {
                if (_lengthLimit == UnsetLengthLimit)
                    return _innerStream.Length;
                else return _lengthLimit;
            }
        }

        public override long Position
        {
            get
            {
                return _Crc32.TotalBytesRead;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return 0;
        }

        public override void SetLength(long value)
        {
            return;
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        public override void Close()
        {
            base.Close();
            if (!_leaveOpen)
                _innerStream.Close();
        }
    }
}