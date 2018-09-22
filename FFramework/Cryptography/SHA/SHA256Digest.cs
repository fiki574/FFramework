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

namespace FFramework.Cryptography.SHA
{
    public class SHA256Digest : GeneralDigest
    {
        private const int DigestLength = 0x20;
        private uint H1, H2, H3, H4, H5, H6, H7, H8;
        private static readonly uint[] K = new uint[] { 0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5, 0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5, 0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3, 0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174, 0xe49b69c1, 0xefbe4786, 0xfc19dc6, 0x240ca1cc, 0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da, 0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7, 0xc6e00bf3, 0xd5a79147, 0x6ca6351, 0x14292967, 0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13, 0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85, 0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3, 0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070, 0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5, 0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3, 0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208, 0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2 };
        private uint[] X;
        private int xOff;

        public SHA256Digest()
        {
            X = new uint[0x40];
            initHs();
        }

        public SHA256Digest(SHA256Digest t) : base(t)
        {
            X = new uint[0x40];
            H1 = t.H1;
            H2 = t.H2;
            H3 = t.H3;
            H4 = t.H4;
            H5 = t.H5;
            H6 = t.H6;
            H7 = t.H7;
            H8 = t.H8;
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
            ByteSwap.UInt32_To_BE(H6, output, outOff + 20);
            ByteSwap.UInt32_To_BE(H7, output, outOff + 0x18);
            ByteSwap.UInt32_To_BE(H8, output, outOff + 0x1c);
            Reset();
            return 0x20;
        }

        public override int GetDigestSize()
        {
            return 0x20;
        }

        private void initHs()
        {
            H1 = 0x6a09e667;
            H2 = 0xbb67ae85;
            H3 = 0x3c6ef372;
            H4 = 0xa54ff53a;
            H5 = 0x510e527f;
            H6 = 0x9b05688c;
            H7 = 0x1f83d9ab;
            H8 = 0x5be0cd19;
        }

        internal override void ProcessBlock()
        {
            for (int i = 0x10; i <= 0x3f; i++)
                X[i] = ((Theta1(X[i - 2]) + X[i - 7]) + Theta0(X[i - 15])) + X[i - 0x10];

            uint x = H1;
            uint y = H2;
            uint z = H3;
            uint num5 = H4;
            uint num6 = H5;
            uint num7 = H6;
            uint num8 = H7;
            uint num9 = H8;
            int index = 0;
            for (int j = 0; j < 8; j++)
            {
                num9 += (Sum1Ch(num6, num7, num8) + K[index]) + X[index];
                num5 += num9;
                num9 += Sum0Maj(x, y, z);
                index++;
                num8 += (Sum1Ch(num5, num6, num7) + K[index]) + X[index];
                z += num8;
                num8 += Sum0Maj(num9, x, y);
                index++;
                num7 += (Sum1Ch(z, num5, num6) + K[index]) + X[index];
                y += num7;
                num7 += Sum0Maj(num8, num9, x);
                index++;
                num6 += (Sum1Ch(y, z, num5) + K[index]) + X[index];
                x += num6;
                num6 += Sum0Maj(num7, num8, num9);
                index++;
                num5 += (Sum1Ch(x, y, z) + K[index]) + X[index];
                num9 += num5;
                num5 += Sum0Maj(num6, num7, num8);
                index++;
                z += (Sum1Ch(num9, x, y) + K[index]) + X[index];
                num8 += z;
                z += Sum0Maj(num5, num6, num7);
                index++;
                y += (Sum1Ch(num8, num9, x) + K[index]) + X[index];
                num7 += y;
                y += Sum0Maj(z, num5, num6);
                index++;
                x += (Sum1Ch(num7, num8, num9) + K[index]) + X[index];
                num6 += x;
                x += Sum0Maj(y, z, num5);
                index++;
            }
            H1 += x;
            H2 += y;
            H3 += z;
            H4 += num5;
            H5 += num6;
            H6 += num7;
            H7 += num8;
            H8 += num9;
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
            Reset();
            initHs();
            xOff = 0;
            Array.Clear(X, 0, X.Length);
        }

        private static uint Sum0Maj(uint x, uint y, uint z)
        {
            return (((((x >> 2) | (x << 30)) ^ ((x >> 13) | (x << 0x13))) ^ ((x >> 0x16) | (x << 10))) + (((x & y) ^ (x & z)) ^ (y & z)));
        }

        private static uint Sum1Ch(uint x, uint y, uint z)
        {
            return (((((x >> 6) | (x << 0x1a)) ^ ((x >> 11) | (x << 0x15))) ^ ((x >> 0x19) | (x << 7))) + ((x & y) ^ (~x & z)));
        }

        private static uint Theta0(uint x)
        {
            return ((((x >> 7) | (x << 0x19)) ^ ((x >> 0x12) | (x << 14))) ^ (x >> 3));
        }

        private static uint Theta1(uint x)
        {
            return ((((x >> 0x11) | (x << 15)) ^ ((x >> 0x13) | (x << 13))) ^ (x >> 10));
        }

        public override string AlgorithmName
        {
            get
            {
                return "SHA-256";
            }
        }
    }
}