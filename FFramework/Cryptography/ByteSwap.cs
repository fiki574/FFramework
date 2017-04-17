/*
    C# Framework with a lot of useful functions and classes
    Copyright (C) 2017 Bruno Fištrek

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
using System.Linq;

namespace FFramework.Cryptography
{
    public class ByteSwap
    {
        public static uint BE_To_UInt32(byte[] bs)
        {
            uint num = (uint)(bs[0] << 0x18);
            num |= (uint)(bs[1] << 0x10);
            num |= (uint)(bs[2] << 8);
            return (num | bs[3]);
        }

        public static uint BE_To_UInt32(byte[] bs, int off)
        {
            uint num = (uint)(bs[off] << 0x18);
            num |= (uint)(bs[++off] << 0x10);
            num |= (uint)(bs[++off] << 8);
            return (num | bs[++off]);
        }

        public static ulong BE_To_UInt64(byte[] bs)
        {
            uint num = BE_To_UInt32(bs);
            uint num2 = BE_To_UInt32(bs, 4);
            return ((num << 0x20) | num2);
        }

        public static ulong BE_To_UInt64(byte[] bs, int off)
        {
            uint num = BE_To_UInt32(bs, off);
            uint num2 = BE_To_UInt32(bs, off + 4);
            return ((num << 0x20) | num2);
        }

        public static uint LE_To_UInt32(byte[] bs)
        {
            uint num = bs[0];
            num |= (uint)(bs[1] << 8);
            num |= (uint)(bs[2] << 0x10);
            return (num | ((uint)(bs[3] << 0x18)));
        }

        public static uint LE_To_UInt32(byte[] bs, int off)
        {
            uint num = bs[off];
            num |= (uint)(bs[++off] << 8);
            num |= (uint)(bs[++off] << 0x10);
            return (num | ((uint)(bs[++off] << 0x18)));
        }

        public static ulong LE_To_UInt64(byte[] bs)
        {
            uint num = LE_To_UInt32(bs);
            return ((LE_To_UInt32(bs, 4) << 0x20) | num);
        }

        public static ulong LE_To_UInt64(byte[] bs, int off)
        {
            uint num = LE_To_UInt32(bs, off);
            return ((LE_To_UInt32(bs, off + 4) << 0x20) | num);
        }

        public static double Swap(double input)
        {
            byte[] bytes = BitConverter.GetBytes(input);
            return BitConverter.ToSingle(new byte[] { bytes[7], bytes[6], bytes[5], bytes[4], bytes[3], bytes[2], bytes[1], bytes[0] }, 0);
        }

        public static float Swap(float input)
        {
            byte[] bytes = BitConverter.GetBytes(input);
            return BitConverter.ToSingle(new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] }, 0);
        }

        public static ushort Swap(ushort input)
        {
            return (ushort)(((0xff00 & input) >> 8) | ((0xff & input) << 8));
        }

        public static uint Swap(uint input)
        {
            return (uint)(((((-16777216 & input) >> 0x18) | ((0xff0000 & input) >> 8)) | ((0xff00 & input) << 8)) | ((0xff & input) << 0x18));
        }

        public static ulong Swap(ulong input)
        {
            byte[] bytes = BitConverter.GetBytes(input);
            byte[] buffer2 = new byte[BitConverter.GetBytes(input).Count()];
            for (int i = 0; i < BitConverter.GetBytes(input).Count(); i++) buffer2[i] = bytes[(BitConverter.GetBytes(input).Count() - 1) - i];
            return BitConverter.ToUInt64(buffer2, 0);
        }

        public static void UInt32_To_BE(uint n, byte[] bs)
        {
            bs[0] = (byte)(n >> 0x18);
            bs[1] = (byte)(n >> 0x10);
            bs[2] = (byte)(n >> 8);
            bs[3] = (byte)n;
        }

        public static void UInt32_To_BE(uint n, byte[] bs, int off)
        {
            bs[off] = (byte)(n >> 0x18);
            bs[++off] = (byte)(n >> 0x10);
            bs[++off] = (byte)(n >> 8);
            bs[++off] = (byte)n;
        }

        public static void UInt32_To_LE(uint n, byte[] bs)
        {
            bs[0] = (byte)n;
            bs[1] = (byte)(n >> 8);
            bs[2] = (byte)(n >> 0x10);
            bs[3] = (byte)(n >> 0x18);
        }

        public static void UInt32_To_LE(uint n, byte[] bs, int off)
        {
            bs[off] = (byte)n;
            bs[++off] = (byte)(n >> 8);
            bs[++off] = (byte)(n >> 0x10);
            bs[++off] = (byte)(n >> 0x18);
        }

        public static void UInt64_To_BE(ulong n, byte[] bs)
        {
            UInt32_To_BE((uint)(n >> 0x20), bs);
            UInt32_To_BE((uint)n, bs, 4);
        }

        public static void UInt64_To_BE(ulong n, byte[] bs, int off)
        {
            UInt32_To_BE((uint)(n >> 0x20), bs, off);
            UInt32_To_BE((uint)n, bs, off + 4);
        }

        public static void UInt64_To_LE(ulong n, byte[] bs)
        {
            UInt32_To_LE((uint)n, bs);
            UInt32_To_LE((uint)(n >> 0x20), bs, 4);
        }

        public static void UInt64_To_LE(ulong n, byte[] bs, int off)
        {
            UInt32_To_LE((uint)n, bs, off);
            UInt32_To_LE((uint)(n >> 0x20), bs, off + 4);
        }
    }
}
