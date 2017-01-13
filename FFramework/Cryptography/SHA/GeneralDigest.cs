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

namespace FFramework.Cryptography.SHA
{
    public abstract class GeneralDigest : IDigest
    {
        private const int BYTE_LENGTH = 0x40;
        private long byteCount;
        private byte[] xBuf;
        private int xBufOff;

        internal GeneralDigest()
        {
            xBuf = new byte[4];
        }

        internal GeneralDigest(GeneralDigest t)
        {
            xBuf = new byte[t.xBuf.Length];
            Array.Copy(t.xBuf, 0, xBuf, 0, t.xBuf.Length);
            xBufOff = t.xBufOff;
            byteCount = t.byteCount;
        }

        public void BlockUpdate(byte[] input, int inOff, int length)
        {
            while ((xBufOff != 0) && (length > 0))
            {
                Update(input[inOff]);
                inOff++;
                length--;
            }
            while (length > xBuf.Length)
            {
                ProcessWord(input, inOff);
                inOff += xBuf.Length;
                length -= xBuf.Length;
                byteCount += xBuf.Length;
            }
            while (length > 0)
            {
                Update(input[inOff]);
                inOff++;
                length--;
            }
        }

        public abstract int DoFinal(byte[] output, int outOff);
        public void Finish()
        {
            long bitLength = byteCount << 3;
            Update(0x80);
            while (xBufOff != 0)
            {
                Update(0);
            }
            ProcessLength(bitLength);
            ProcessBlock();
        }

        public int GetByteLength()
        {
            return 0x40;
        }

        public abstract int GetDigestSize();
        internal abstract void ProcessBlock();
        internal abstract void ProcessLength(long bitLength);
        internal abstract void ProcessWord(byte[] input, int inOff);
        public virtual void Reset()
        {
            byteCount = 0L;
            xBufOff = 0;
            Array.Clear(xBuf, 0, xBuf.Length);
        }

        public void Update(byte input)
        {
            xBuf[xBufOff++] = input;
            if (xBufOff == xBuf.Length)
            {
                ProcessWord(xBuf, 0);
                xBufOff = 0;
            }
            byteCount += 1L;
        }
        public abstract string AlgorithmName { get; }
    }
}
