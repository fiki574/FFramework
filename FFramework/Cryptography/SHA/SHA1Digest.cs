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

using System;
using FFramework.Cryptography.Other;

namespace FFramework.Cryptography.SHA
{
    public class Sha1Digest : GeneralDigest
    {
        private const int DigestLength = 20;
        private uint H1, H2, H3, H4, H5;
        private uint[] X;
        private int xOff;
        private const uint Y1 = 0x5a827999, Y2 = 0x6ed9eba1, Y3 = 0x8f1bbcdc, Y4 = 0xca62c1d6;

        public Sha1Digest()
        {
            X = new uint[80];
            Reset();
        }

        public Sha1Digest(Sha1Digest t) : base(t)
        {
            X = new uint[80];
            H1 = t.H1;
            H2 = t.H2;
            H3 = t.H3;
            H4 = t.H4;
            H5 = t.H5;
            Array.Copy(t.X, 0, X, 0, t.X.Length);
            xOff = t.xOff;
        }

        public override int DoFinal(byte[] output, int outOff)
        {
            Finish();
            ByteSwap.UInt32_To_BE(H1, output, outOff);
            ByteSwap.UInt32_To_BE(H2, output, outOff + 4);
            ByteSwap.UInt32_To_BE(H3, output, outOff + 8);
            ByteSwap.UInt32_To_BE(H4, output, outOff + 12);
            ByteSwap.UInt32_To_BE(H5, output, outOff + 0x10);
            Reset();
            return 20;
        }

        private static uint F(uint u, uint v, uint w)
        {
            return ((u & v) | (~u & w));
        }

        private static uint G(uint u, uint v, uint w)
        {
            return (((u & v) | (u & w)) | (v & w));
        }

        public override int GetDigestSize()
        {
            return 20;
        }

        private static uint H(uint u, uint v, uint w)
        {
            return ((u ^ v) ^ w);
        }

        internal override void ProcessBlock()
        {
            for (int i = 0x10; i < 80; i++)
            {
                uint num2 = ((X[i - 3] ^ X[i - 8]) ^ X[i - 14]) ^ X[i - 0x10];
                X[i] = (num2 << 1) | (num2 >> 0x1f);
            }
            uint u = H1;
            uint num4 = H2;
            uint v = H3;
            uint w = H4;
            uint num7 = H5;
            int num8 = 0;
            for (int j = 0; j < 4; j++)
            {
                num7 += ((((u << 5) | (u >> 0x1b)) + F(num4, v, w)) + X[num8++]) + 0x5a827999;
                num4 = (num4 << 30) | (num4 >> 2);
                w += ((((num7 << 5) | (num7 >> 0x1b)) + F(u, num4, v)) + X[num8++]) + 0x5a827999;
                u = (u << 30) | (u >> 2);
                v += ((((w << 5) | (w >> 0x1b)) + F(num7, u, num4)) + X[num8++]) + 0x5a827999;
                num7 = (num7 << 30) | (num7 >> 2);
                num4 += ((((v << 5) | (v >> 0x1b)) + F(w, num7, u)) + X[num8++]) + 0x5a827999;
                w = (w << 30) | (w >> 2);
                u += ((((num4 << 5) | (num4 >> 0x1b)) + F(v, w, num7)) + X[num8++]) + 0x5a827999;
                v = (v << 30) | (v >> 2);
            }
            for (int k = 0; k < 4; k++)
            {
                num7 += ((((u << 5) | (u >> 0x1b)) + H(num4, v, w)) + X[num8++]) + 0x6ed9eba1;
                num4 = (num4 << 30) | (num4 >> 2);
                w += ((((num7 << 5) | (num7 >> 0x1b)) + H(u, num4, v)) + X[num8++]) + 0x6ed9eba1;
                u = (u << 30) | (u >> 2);
                v += ((((w << 5) | (w >> 0x1b)) + H(num7, u, num4)) + X[num8++]) + 0x6ed9eba1;
                num7 = (num7 << 30) | (num7 >> 2);
                num4 += ((((v << 5) | (v >> 0x1b)) + H(w, num7, u)) + X[num8++]) + 0x6ed9eba1;
                w = (w << 30) | (w >> 2);
                u += ((((num4 << 5) | (num4 >> 0x1b)) + H(v, w, num7)) + X[num8++]) + 0x6ed9eba1;
                v = (v << 30) | (v >> 2);
            }
            for (int m = 0; m < 4; m++)
            {
                num7 += ((((u << 5) | (u >> 0x1b)) + G(num4, v, w)) + X[num8++]) + 0x8f1bbcdc;
                num4 = (num4 << 30) | (num4 >> 2);
                w += ((((num7 << 5) | (num7 >> 0x1b)) + G(u, num4, v)) + X[num8++]) + 0x8f1bbcdc;
                u = (u << 30) | (u >> 2);
                v += ((((w << 5) | (w >> 0x1b)) + G(num7, u, num4)) + X[num8++]) + 0x8f1bbcdc;
                num7 = (num7 << 30) | (num7 >> 2);
                num4 += ((((v << 5) | (v >> 0x1b)) + G(w, num7, u)) + X[num8++]) + 0x8f1bbcdc;
                w = (w << 30) | (w >> 2);
                u += ((((num4 << 5) | (num4 >> 0x1b)) + G(v, w, num7)) + X[num8++]) + 0x8f1bbcdc;
                v = (v << 30) | (v >> 2);
            }
            for (int n = 0; n < 4; n++)
            {
                num7 += ((((u << 5) | (u >> 0x1b)) + H(num4, v, w)) + X[num8++]) + 0xca62c1d6;
                num4 = (num4 << 30) | (num4 >> 2);
                w += ((((num7 << 5) | (num7 >> 0x1b)) + H(u, num4, v)) + X[num8++]) + 0xca62c1d6;
                u = (u << 30) | (u >> 2);
                v += ((((w << 5) | (w >> 0x1b)) + H(num7, u, num4)) + X[num8++]) + 0xca62c1d6;
                num7 = (num7 << 30) | (num7 >> 2);
                num4 += ((((v << 5) | (v >> 0x1b)) + H(w, num7, u)) + X[num8++]) + 0xca62c1d6;
                w = (w << 30) | (w >> 2);
                u += ((((num4 << 5) | (num4 >> 0x1b)) + H(v, w, num7)) + X[num8++]) + 0xca62c1d6;
                v = (v << 30) | (v >> 2);
            }
            H1 += u;
            H2 += num4;
            H3 += v;
            H4 += w;
            H5 += num7;
            xOff = 0;
            Array.Clear(X, 0, 0x10);
        }

        internal override void ProcessLength(long bitLength)
        {
            if (xOff > 14)
                ProcessBlock();

            X[14] = (uint)(bitLength >> 0x20);
            X[15] = (uint)bitLength;
        }

        internal override void ProcessWord(byte[] input, int inOff)
        {
            X[xOff] = ByteSwap.BE_To_UInt32(input, inOff);
            if (++xOff == 0x10)
                ProcessBlock();
        }

        public override void Reset()
        {
            base.Reset();
            H1 = 0x67452301;
            H2 = 0xefcdab89;
            H3 = 0x98badcfe;
            H4 = 0x10325476;
            H5 = 0xc3d2e1f0;
            xOff = 0;
            Array.Clear(X, 0, X.Length);
        }

        public override string AlgorithmName
        {
            get
            {
                return "SHA-1";
            }
        }
    }
}