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
using Org.BouncyCastle.Math;

namespace FFramework.Cryptography.Other
{ 
    public class DiffieHellman
    {
        private static readonly BigInteger Prime =
            new BigInteger(1, new byte[]
                               {
                                   0xEB, 0x74, 0x42, 0x1E, 0x8C, 0x7F, 0xEE, 0xB7, 0xD8, 0x0A, 0xEF, 0x07, 0x7B, 0x94,
                                   0x1D, 0x6D, 0xFA, 0xD3, 0x96, 0x59, 0x1C, 0x9C, 0xC0, 0x7A, 0x45, 0x3E, 0x7E, 0xC9,
                                   0x0C, 0xA5, 0xA1, 0xD7, 0x90, 0x5D, 0x29, 0x8E, 0x4D, 0x6D, 0xA4, 0xF4, 0x91, 0x61,
                                   0xBD, 0x3B, 0xDB, 0x8C, 0xD6, 0x5C, 0x42, 0x55, 0x99, 0x99, 0x20, 0xD8, 0xDD, 0xA7,
                                   0x78, 0x1B, 0x07, 0xD4, 0x97, 0xC0, 0xF6, 0xBB
                               });

        private static readonly BigInteger PrivateKey =
            new BigInteger(1, new byte[]
                               {
                                   0x5C, 0x52, 0x54, 0x6D, 0xA9, 0x11, 0xB0, 0x7E, 0x33, 0xFB, 0xAB, 0xD0, 0x8C, 0xA5,
                                   0xE6, 0x05, 0x67, 0x3D, 0xAE, 0xF8, 0x0F, 0xAF, 0x96, 0xF7, 0xC7, 0x3F, 0xF4, 0xAD,
                                   0x43, 0xFE, 0xC7, 0x98, 0xE0, 0xA2, 0x57, 0x2F, 0x5B, 0xD3, 0x41, 0xFF, 0x79, 0xD9,
                                   0x60, 0x8E, 0x31, 0x2C, 0x59, 0xE7, 0xE1, 0x49, 0x55, 0x82, 0xC5, 0x5E, 0x7D, 0x6B,
                                   0x6B, 0xD8, 0xBF, 0xEF, 0xCD, 0xA4, 0x31, 0x84
                               });

        public static byte[] GenerateSharedKey(byte[] clientSeed)
        {
            Array.Reverse(clientSeed);
            byte[] result = new BigInteger(1, clientSeed).ModPow(PrivateKey, Prime).ToByteArrayUnsigned();
            Array.Reverse(result);
            return result;
        }
    }
}