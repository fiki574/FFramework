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
using System.Collections;
using System.Text;
using System.Globalization;

namespace FFramework_Crypto.SRP6
{
    [Serializable]
    public class BigInteger
    {
        private static readonly byte[] bitCounts = new byte[] {
        0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4,
        1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
        1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
        2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
        1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
        2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
        2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
        3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
        1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
        2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
        2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
        3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
        2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
        3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
        3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
        4, 5, 5, 6, 5, 6, 6, 7, 5, 6, 6, 7, 6, 7, 7, 8
        };

        private const int BitsPerByte = 8;
        private const int BitsPerInt = 0x20;
        private const int BytesPerInt = 4;
        private static readonly int chunk10 = 0x13;
        private static readonly int chunk16 = 0x10;
        private static readonly int chunk2 = 1;
        private const long IMASK = 0xffffffffL;
        private int[] magnitude;
        private long mQuote;
        private int nBitLength;
        private int nBits;
        public static readonly BigInteger One = CreateUValueOf(1L);
        private static readonly int[][] primeLists = new int[][] {
        new int[] { 3, 5, 7, 11, 13, 0x11, 0x13, 0x17 }, new int[] { 0x1d, 0x1f, 0x25, 0x29, 0x2b }, new int[] { 0x2f, 0x35, 0x3b, 0x3d, 0x43 }, new int[] { 0x47, 0x49, 0x4f, 0x53 }, new int[] { 0x59, 0x61, 0x65, 0x67 }, new int[] { 0x6b, 0x6d, 0x71, 0x7f }, new int[] { 0x83, 0x89, 0x8b, 0x95 }, new int[] { 0x97, 0x9d, 0xa3, 0xa7 }, new int[] { 0xad, 0xb3, 0xb5, 0xbf }, new int[] { 0xc1, 0xc5, 0xc7, 0xd3 }, new int[] { 0xdf, 0xe3, 0xe5 }, new int[] { 0xe9, 0xef, 0xf1 }, new int[] { 0xfb, 0x101, 0x107 }, new int[] { 0x10d, 0x10f, 0x115 }, new int[] { 0x119, 0x11b, 0x125 }, new int[] { 0x133, 0x137, 0x139 },
        new int[] { 0x13d, 0x14b, 0x151 }, new int[] { 0x15b, 0x15d, 0x161 }, new int[] { 0x167, 0x16f, 0x175 }, new int[] { 0x17b, 0x17f, 0x185 }, new int[] { 0x18d, 0x191, 0x199 }, new int[] { 0x1a3, 0x1a5, 0x1af }, new int[] { 0x1b1, 0x1b7, 0x1bb }, new int[] { 0x1c1, 0x1c9, 0x1cd }, new int[] { 0x1cf, 0x1d3, 0x1df }, new int[] { 0x1e7, 0x1eb, 0x1f3 }, new int[] { 0x1f7, 0x1fd, 0x209 }, new int[] { 0x20b, 0x21d, 0x223 }, new int[] { 0x22d, 0x233, 0x239 }, new int[] { 0x23b, 0x241, 0x24b }, new int[] { 0x251, 0x257, 0x259 }, new int[] { 0x25f, 0x265, 0x269 },
        new int[] { 0x26b, 0x277, 0x281 }, new int[] { 0x283, 0x287, 0x28d }, new int[] { 0x293, 0x295, 0x2a1 }, new int[] { 0x2a5, 0x2ab, 0x2b3 }, new int[] { 0x2bd, 0x2c5, 0x2cf }, new int[] { 0x2d7, 0x2dd, 0x2e3 }, new int[] { 0x2e7, 0x2ef, 0x2f5 }, new int[] { 0x2f9, 0x301, 0x305 }, new int[] { 0x313, 0x31d, 0x329 }, new int[] { 0x32b, 0x335, 0x337 }, new int[] { 0x33b, 0x33d, 0x347 }, new int[] { 0x355, 0x359, 0x35b }, new int[] { 0x35f, 0x36d, 0x371 }, new int[] { 0x373, 0x377, 0x38b }, new int[] { 0x38f, 0x397, 0x3a1 }, new int[] { 0x3a9, 0x3ad, 0x3b3 },
        new int[] { 0x3b9, 0x3c7, 0x3cb }, new int[] { 0x3d1, 0x3d7, 0x3df }, new int[] { 0x3e5, 0x3f1, 0x3f5 }, new int[] { 0x3fb, 0x3fd, 0x407 } };
        private static readonly int[] primeProducts = new int[primeLists.Length];
        private static readonly BigInteger radix10 = ValueOf(10L);
        private static readonly BigInteger radix10E = radix10.Pow(chunk10);
        private static readonly BigInteger radix16 = ValueOf(0x10L);
        private static readonly BigInteger radix16E = radix16.Pow(chunk16);
        private static readonly BigInteger radix2 = ValueOf(2L);
        private static readonly BigInteger radix2E = radix2.Pow(chunk2);
        private static readonly Random RandomSource = new Random();
        private static readonly byte[] rndMask = new byte[] { 0xff, 0x7f, 0x3f, 0x1f, 15, 7, 3, 1 };
        private int sign;
        public static readonly BigInteger Ten = CreateUValueOf(10L);
        public static readonly BigInteger Three = CreateUValueOf(3L);
        public static readonly BigInteger Two = CreateUValueOf(2L);
        private static readonly ulong UIMASK = 0xffffffffL;
        public static readonly BigInteger Zero = new BigInteger(0, ZeroMagnitude, false);
        private static readonly byte[] ZeroEncoding = new byte[0];
        private static readonly int[] ZeroMagnitude = new int[0];

        static BigInteger()
        {
            for (int i = 0; i < primeLists.Length; i++)
            {
                int[] numArray = primeLists[i];
                int num2 = 1;
                for (int j = 0; j < numArray.Length; j++) num2 *= numArray[j];
                primeProducts[i] = num2;
            }
        }

        private BigInteger()
        {
            nBits = -1;
            nBitLength = -1;
            mQuote = -1L;
        }

        public BigInteger(byte[] bytes) : this(bytes, 0, bytes.Length)
        {
        }

        public BigInteger(string value) : this(value, 10)
        {
        }

        public BigInteger(int sizeInBits, Random random)
        {
            nBits = -1;
            nBitLength = -1;
            mQuote = -1L;
            if (sizeInBits < 0) throw new ArgumentException("sizeInBits must be positive");
            nBits = -1;
            nBitLength = -1;
            if (sizeInBits == 0) magnitude = ZeroMagnitude;
            else
            {
                int byteLength = GetByteLength(sizeInBits);
                byte[] buffer = new byte[byteLength];
                random.NextBytes(buffer);
                buffer[0] = (byte)(buffer[0] & rndMask[(8 * byteLength) - sizeInBits]);
                magnitude = MakeMagnitude(buffer, 0, buffer.Length);
                sign = (magnitude.Length < 1) ? 0 : 1;
            }
        }

        public BigInteger(int sign, byte[] bytes) : this(sign, bytes, 0, bytes.Length)
        {
        }

        public BigInteger(string str, int radix)
        {
            NumberStyles allowHexSpecifier;
            int num;
            BigInteger integer;
            BigInteger integer2;
            nBits = -1;
            nBitLength = -1;
            mQuote = -1L;
            if (str.Length == 0) throw new FormatException("Zero length BigInteger");
            switch (radix)
            {
                case 2:
                    allowHexSpecifier = NumberStyles.Integer;
                    num = chunk2;
                    integer = radix2;
                    integer2 = radix2E;
                    break;

                case 10:
                    allowHexSpecifier = NumberStyles.Integer;
                    num = chunk10;
                    integer = radix10;
                    integer2 = radix10E;
                    break;

                case 0x10:
                    allowHexSpecifier = NumberStyles.AllowHexSpecifier;
                    num = chunk16;
                    integer = radix16;
                    integer2 = radix16E;
                    break;

                default:
                    throw new FormatException("Only bases 2, 10, or 16 allowed");
            }
            int startIndex = 0;
            sign = 1;
            if (str[0] == '-')
            {
                if (str.Length == 1) throw new FormatException("Zero length BigInteger");
                sign = -1;
                startIndex = 1;
            }
        Label_00D1:
            if (startIndex < str.Length)
            {
                char ch = str[startIndex];
                if (int.Parse(ch.ToString(), allowHexSpecifier) == 0)
                {
                    startIndex++;
                    goto Label_00D1;
                }
            }
            if (startIndex >= str.Length)
            {
                sign = 0;
                magnitude = ZeroMagnitude;
            }
            else
            {
                BigInteger zero = Zero;
                int num3 = startIndex + num;
                if (num3 <= str.Length)
                {
                    do
                    {
                        string s = str.Substring(startIndex, num);
                        ulong num4 = ulong.Parse(s, allowHexSpecifier);
                        BigInteger integer4 = CreateUValueOf(num4);
                        switch (radix)
                        {
                            case 2:
                                if (num4 > 1L) throw new FormatException("Bad character in radix 2 string: " + s);
                                zero = zero.ShiftLeft(1);
                                break;

                            case 0x10:
                                zero = zero.ShiftLeft(0x40);
                                break;

                            default:
                                zero = zero.Multiply(integer2);
                                break;
                        }
                        zero = zero.Add(integer4);
                        startIndex = num3;
                        num3 += num;
                    }
                    while (num3 <= str.Length);
                }
                if (startIndex < str.Length)
                {
                    string str3 = str.Substring(startIndex);
                    BigInteger integer5 = CreateUValueOf(ulong.Parse(str3, allowHexSpecifier));
                    if (zero.sign > 0)
                    {
                        if (radix != 2)
                        {
                            if (radix == 0x10) zero = zero.ShiftLeft(str3.Length << 2);
                            else zero = zero.Multiply(integer.Pow(str3.Length));
                        }
                        zero = zero.Add(integer5);
                    }
                    else
                    {
                        zero = integer5;
                    }
                }
                magnitude = zero.magnitude;
            }
        }

        public BigInteger(byte[] bytes, int offset, int length)
        {
            nBits = -1;
            nBitLength = -1;
            mQuote = -1L;
            if (length == 0) throw new FormatException("Zero length BigInteger");
            if (((sbyte)bytes[offset]) < 0)
            {
                sign = -1;
                int num = offset + length;
                int index = offset;
                while ((index < num) && (((sbyte)bytes[index]) == -1)) index++;
                if (index >= num) magnitude = One.magnitude;
                else
                {
                    int num3 = num - index;
                    byte[] buffer = new byte[num3];
                    int num4 = 0;
                    while (num4 < num3) buffer[num4++] = (byte)~bytes[index++];
                    while (buffer[--num4] == 0xff) buffer[num4] = 0;
                    buffer[num4] = (byte)(buffer[num4] + 1);
                    magnitude = MakeMagnitude(buffer, 0, buffer.Length);
                }
            }
            else
            {
                magnitude = MakeMagnitude(bytes, offset, length);
                sign = (magnitude.Length > 0) ? 1 : 0;
            }
        }

        private BigInteger(int signum, int[] mag, bool checkMag)
        {
            nBits = -1;
            nBitLength = -1;
            mQuote = -1L;
            if (checkMag)
            {
                int index = 0;
                while ((index < mag.Length) && (mag[index] == 0)) index++;
                if (index == mag.Length) magnitude = ZeroMagnitude;
                else
                {
                    sign = signum;
                    if (index == 0)
                        magnitude = mag;
                    else
                    {
                        magnitude = new int[mag.Length - index];
                        Array.Copy(mag, index, magnitude, 0, magnitude.Length);
                    }
                }
            }
            else
            {
                sign = signum;
                magnitude = mag;
            }
        }

        public BigInteger(int bitLength, int certainty, Random random)
        {
            nBits = -1;
            nBitLength = -1;
            mQuote = -1L;
            if (bitLength < 2) throw new ArithmeticException("bitLength < 2");
            sign = 1;
            nBitLength = bitLength;
            if (bitLength == 2)
            {
                magnitude = (random.Next(2) == 0) ? Two.magnitude : Three.magnitude;
                return;
            }
            int byteLength = GetByteLength(bitLength);
            byte[] buffer = new byte[byteLength];
            int index = (8 * byteLength) - bitLength;
            byte num3 = rndMask[index];
            while (true)
            {
                random.NextBytes(buffer);
                buffer[0] = (byte)(buffer[0] & num3);
                buffer[0] = (byte)(buffer[0] | ((byte)(1 << (7 - index))));
                buffer[byteLength - 1] = (byte)(buffer[byteLength - 1] | 1);
                magnitude = MakeMagnitude(buffer, 0, buffer.Length);
                nBits = -1;
                mQuote = -1L;
                if ((certainty < 1) || CheckProbablePrime(certainty, random)) return;
                if (bitLength > 0x20)
                {
                    for (int i = 0; i < 0x2710; i++)
                    {
                        int num5 = 0x21 + random.Next(bitLength - 2);
                        magnitude[magnitude.Length - (num5 >> 5)] ^= (1) << num5;
                        magnitude[magnitude.Length - 1] ^= (random.Next() + 1) << 1;
                        mQuote = -1L;
                        if (CheckProbablePrime(certainty, random)) return;
                    }
                }
            }
        }

        public BigInteger(int sign, byte[] bytes, int offset, int length)
        {
            nBits = -1;
            nBitLength = -1;
            mQuote = -1L;
            if ((sign < -1) || (sign > 1)) throw new FormatException("Invalid sign value");
            if (sign == 0) magnitude = ZeroMagnitude;
            else
            {
                magnitude = MakeMagnitude(bytes, offset, length);
                this.sign = (magnitude.Length < 1) ? 0 : sign;
            }
        }

        public BigInteger Abs()
        {
            if (sign < 0) return Negate();
            return this;
        }

        public BigInteger Add(BigInteger value)
        {
            if (sign == 0) return value;
            if (sign == value.sign) return AddToMagnitude(value.magnitude);
            if (value.sign == 0) return this;
            if (value.sign < 0) return Subtract(value.Negate());
            return value.Subtract(Negate());
        }

        private static int[] AddMagnitudes(int[] a, int[] b)
        {
            int index = a.Length - 1;
            int num2 = b.Length - 1;
            long num3 = 0L;
            while (num2 >= 0)
            {
                num3 += (long)(((ulong)a[index]) + ((ulong)b[num2--]));
                a[index--] = (int)num3;
                num3 = num3 >> 0x20;
            }
            if (num3 != 0L) while ((index >= 0) && (++a[index--] == 0)) { }
            return a;
        }

        private BigInteger AddToMagnitude(int[] magToAdd)
        {
            int[] numArray;
            int[] magnitude;
            int[] numArray3;
            if (this.magnitude.Length < magToAdd.Length)
            {
                numArray = magToAdd;
                magnitude = this.magnitude;
            }
            else
            {
                numArray = this.magnitude;
                magnitude = magToAdd;
            }
            uint maxValue = uint.MaxValue;
            if (numArray.Length == magnitude.Length)
            {
                maxValue -= (uint)magnitude[0];
            }
            bool checkMag = numArray[0] >= maxValue;
            if (checkMag)
            {
                numArray3 = new int[numArray.Length + 1];
                numArray.CopyTo(numArray3, 1);
            }
            else numArray3 = (int[])numArray.Clone();
            return new BigInteger(sign, AddMagnitudes(numArray3, magnitude), checkMag);
        }

        public BigInteger And(BigInteger value)
        {
            if ((sign == 0) || (value.sign == 0)) return Zero;
            int[] numArray = (sign > 0) ? magnitude : Add(One).magnitude;
            int[] numArray2 = (value.sign > 0) ? value.magnitude : value.Add(One).magnitude;
            bool flag = (sign < 0) && (value.sign < 0);
            int[] mag = new int[Math.Max(numArray.Length, numArray2.Length)];
            int num2 = mag.Length - numArray.Length;
            int num3 = mag.Length - numArray2.Length;
            for (int i = 0; i < mag.Length; i++)
            {
                int num5 = (i >= num2) ? numArray[i - num2] : 0;
                int num6 = (i >= num3) ? numArray2[i - num3] : 0;
                if (sign < 0) num5 = ~num5;
                if (value.sign < 0) num6 = ~num6;
                mag[i] = num5 & num6;
                if (flag) mag[i] = ~mag[i];
            }
            BigInteger integer = new BigInteger(1, mag, true);
            if (flag) integer = integer.Not();
            return integer;
        }

        public BigInteger AndNot(BigInteger val)
        {
            return And(val.Not());
        }

        private static int BitLen(int w)
        {
            if (w >= 0x8000)
            {
                if (w >= 0x800000)
                {
                    if (w >= 0x8000000)
                    {
                        if (w >= 0x20000000)
                        {
                            if (w >= 0x40000000) return 0x1f;
                            return 30;
                        }
                        if (w >= 0x10000000) return 0x1d;
                        return 0x1c;
                    }
                    if (w >= 0x2000000)
                    {
                        if (w >= 0x4000000) return 0x1b;
                        return 0x1a;
                    }
                    if (w >= 0x1000000) return 0x19;
                    return 0x18;
                }
                if (w >= 0x80000)
                {
                    if (w >= 0x200000)
                    {
                        if (w >= 0x400000) return 0x17;
                        return 0x16;
                    }
                    if (w >= 0x100000) return 0x15;
                    return 20;
                }
                if (w >= 0x20000)
                {
                    if (w >= 0x40000) return 0x13;
                    return 0x12;
                }
                if (w >= 0x10000) return 0x11;
                return 0x10;
            }
            if (w >= 0x80)
            {
                if (w >= 0x800)
                {
                    if (w >= 0x2000)
                    {
                        if (w >= 0x4000) return 15;
                        return 14;
                    }
                    if (w >= 0x1000) return 13;
                    return 12;
                }
                if (w >= 0x200)
                {
                    if (w >= 0x400) return 11;
                    return 10;
                }
                if (w >= 0x100) return 9;
                return 8;
            }
            if (w >= 8)
            {
                if (w >= 0x20)
                {
                    if (w >= 0x40) return 7;
                    return 6;
                }
                if (w >= 0x10) return 5;
                return 4;
            }
            if (w >= 2)
            {
                if (w >= 4) return 3;
                return 2;
            }
            if (w >= 1) return 1; 
            if (w >= 0) return 0;
            return 0x20;
        }

        private int CalcBitLength(int indx, int[] mag)
        {
        Label_0000:
            if (indx >= mag.Length) return 0;
            if (mag[indx] == 0)
            {
                indx++;
                goto Label_0000;
            }
            int num = 0x20 * ((mag.Length - indx) - 1);
            int w = mag[indx];
            num += BitLen(w);
            if ((sign < 0) && ((w & -w) == w))
            {
                do
                {
                    if (++indx >= mag.Length)
                    {
                        num--;
                        return num;
                    }
                }
                while (mag[indx] == 0);
            }
            return num;
        }

        private bool CheckProbablePrime(int certainty, Random random)
        {
            int num = Math.Min(BitLength - 1, primeLists.Length);
            for (int i = 0; i < num; i++)
            {
                int num3 = Remainder(primeProducts[i]);
                foreach (int num5 in primeLists[i]) if ((num3 % num5) == 0) return ((BitLength < 0x10) && (IntValue == num5));
            }
            return RabinMillerTest(certainty, random);
        }

        public BigInteger ClearBit(int n)
        {
            if (n < 0) throw new ArithmeticException("Bit address less than zero");
            if (!TestBit(n)) return this;
            if ((sign > 0) && (n < (BitLength - 1))) return FlipExistingBit(n);
            return AndNot(One.ShiftLeft(n));
        }

        private static int CompareNoLeadingZeroes(int xIndx, int[] x, int yIndx, int[] y)
        {
            int num = (x.Length - y.Length) - (xIndx - yIndx);
            if (num == 0)
            {
                while (xIndx < x.Length)
                {
                    uint num2 = (uint)x[xIndx++];
                    uint num3 = (uint)y[yIndx++];
                    if (num2 != num3)
                    {
                        if (num2 >= num3) return 1;
                        return -1;
                    }
                }
                return 0;
            }
            if (num >= 0) return 1;
            return -1;
        }

        public int CompareTo(BigInteger value)
        {
            if (sign < value.sign) return -1;
            if (sign > value.sign) return 1;
            if (sign != 0) return (sign * CompareNoLeadingZeroes(0, magnitude, 0, value.magnitude));
            return 0;
        }

        public int CompareTo(object obj)
        {
            return CompareTo((BigInteger)obj);
        }

        private static int CompareTo(int xIndx, int[] x, int yIndx, int[] y)
        {
            while ((xIndx != x.Length) && (x[xIndx] == 0)) xIndx++;
            while ((yIndx != y.Length) && (y[yIndx] == 0)) yIndx++;
            return CompareNoLeadingZeroes(xIndx, x, yIndx, y);
        }

        private static BigInteger CreateUValueOf(ulong value)
        {
            int num = (int)(value >> 0x20);
            int num2 = (int)value;
            if (num != 0) return new BigInteger(1, new int[] { num, num2 }, false);
            if (num2 == 0) return Zero;
            BigInteger integer = new BigInteger(1, new int[] { num2 }, false);
            if ((num2 & -num2) == num2) integer.nBits = 1;
            return integer;
        }

        private static BigInteger CreateValueOf(long value)
        {
            if (value >= 0L)  return CreateUValueOf((ulong)value);
            if (value == -9223372036854775808L) return CreateValueOf(~value).Not();
            return CreateValueOf(-value).Negate();
        }

        public BigInteger Divide(BigInteger val)
        {
            if (val.sign == 0) throw new ArithmeticException("Division by zero error");
            if (sign == 0) return Zero;
            if (val.QuickPow2Check())
            {
                BigInteger integer = Abs().ShiftRight(val.Abs().BitLength - 1);
                if (val.sign != sign) return integer.Negate();
                return integer;
            }
            int[] x = (int[])magnitude.Clone();
            return new BigInteger(sign * val.sign, Divide(x, val.magnitude), true);
        }

        private int[] Divide(int[] x, int[] y)
        {
            int[] numArray;
            int index = 0;
            while ((index < x.Length) && (x[index] == 0)) index++;
            int num2 = 0;
            while ((num2 < y.Length) && (y[num2] == 0)) num2++;
            int num3 = CompareNoLeadingZeroes(index, x, num2, y);
            if (num3 > 0)
            {
                int[] numArray2;
                int[] numArray3;
                int num4 = CalcBitLength(num2, y);
                int num5 = CalcBitLength(index, x);
                int n = num5 - num4;
                int start = 0;
                int yIndx = 0;
                int num9 = num4;
                if (n > 0)
                {
                    numArray2 = new int[(n >> 5) + 1];
                    numArray2[0] = (1) << (n % 0x20);
                    numArray3 = ShiftLeft(y, n);
                    num9 += n;
                }
                else
                {
                    numArray2 = new int[] { 1 };
                    int length = y.Length - num2;
                    numArray3 = new int[length];
                    Array.Copy(y, num2, numArray3, 0, length);
                }
                numArray = new int[numArray2.Length];
                while (true)
                {
                    if ((num9 < num5) || (CompareNoLeadingZeroes(index, x, yIndx, numArray3) >= 0))
                    {
                        Subtract(index, x, yIndx, numArray3);
                        AddMagnitudes(numArray, numArray2);
                        while (x[index] == 0) if (++index == x.Length) return numArray;
                        num5 = (0x20 * ((x.Length - index) - 1)) + BitLen(x[index]);
                        if (num5 <= num4)
                        {
                            if (num5 < num4) return numArray;
                            num3 = CompareNoLeadingZeroes(index, x, num2, y);
                            if (num3 <= 0) goto Label_01C2;
                        }
                    }
                    n = num9 - num5;
                    if (n == 1)
                    {
                        uint num11 = (uint)(numArray3[yIndx] >> 1);
                        uint num12 = (uint)x[index];
                        if (num11 > num12) n++;
                    }
                    if (n < 2)
                    {
                        ShiftRightOneInPlace(yIndx, numArray3);
                        num9--;
                        ShiftRightOneInPlace(start, numArray2);
                    }
                    else
                    {
                        ShiftRightInPlace(yIndx, numArray3, n);
                        num9 -= n;
                        ShiftRightInPlace(start, numArray2, n);
                    }
                    while (numArray3[yIndx] == 0) yIndx++;
                    while (numArray2[start] == 0) start++;
                }
            }
            numArray = new int[1];
        Label_01C2:
            if (num3 == 0)
            {
                AddMagnitudes(numArray, One.magnitude);
                Array.Clear(x, index, x.Length - index);
            }
            return numArray;
        }

        public BigInteger[] DivideAndRemainder(BigInteger val)
        {
            if (val.sign == 0) throw new ArithmeticException("Division by zero error");
            BigInteger[] integerArray = new BigInteger[2];
            if (sign == 0)
            {
                integerArray[0] = Zero;
                integerArray[1] = Zero;
                return integerArray;
            }
            if (val.QuickPow2Check())
            {
                int n = val.Abs().BitLength - 1;
                BigInteger integer = Abs().ShiftRight(n);
                int[] numArray = LastNBits(n);
                integerArray[0] = (val.sign == sign) ? integer : integer.Negate();
                integerArray[1] = new BigInteger(sign, numArray, true);
                return integerArray;
            }
            int[] x = (int[])magnitude.Clone();
            int[] mag = Divide(x, val.magnitude);
            integerArray[0] = new BigInteger(sign * val.sign, mag, true);
            integerArray[1] = new BigInteger(sign, x, true);
            return integerArray;
        }

        private static int[] DoSubBigLil(int[] bigMag, int[] lilMag)
        {
            int[] x = (int[])bigMag.Clone();
            return Subtract(0, x, 0, lilMag);
        }

        public override bool Equals(object obj)
        {
            if (obj != this)
            {
                if (!(obj is BigInteger integer)) return false;
                if ((integer.sign != sign) || (integer.magnitude.Length != magnitude.Length)) return false;
                for (int i = 0; i < this.magnitude.Length; i++) if (integer.magnitude[i] != magnitude[i]) return false;
            }
            return true;
        }

        private static BigInteger ExtEuclid(BigInteger a, BigInteger b, BigInteger u1Out, BigInteger u2Out)
        {
            BigInteger[] integerArray;
            BigInteger one = One;
            BigInteger integer2 = a;
            BigInteger zero = Zero;
            for (BigInteger integer4 = b; integer4.sign > 0; integer4 = integerArray[1])
            {
                integerArray = integer2.DivideAndRemainder(integer4);
                BigInteger n = zero.Multiply(integerArray[0]);
                BigInteger integer6 = one.Subtract(n);
                one = zero;
                zero = integer6;
                integer2 = integer4;
            }
            if (u1Out != null)
            {
                u1Out.sign = one.sign;
                u1Out.magnitude = one.magnitude;
            }
            if (u2Out != null)
            {
                BigInteger integer7 = one.Multiply(a);
                BigInteger integer8 = integer2.Subtract(integer7).Divide(b);
                u2Out.sign = integer8.sign;
                u2Out.magnitude = integer8.magnitude;
            }
            return integer2;
        }

        private static long FastExtEuclid(long a, long b, long[] uOut)
        {
            long num6;
            long num = 1L;
            long num2 = a;
            long num3 = 0L;
            for (long i = b; i > 0L; i = num6)
            {
                long num5 = num2 / i;
                num6 = num - (num3 * num5);
                num = num3;
                num3 = num6;
                num6 = num2 - (i * num5);
                num2 = i;
            }
            uOut[0] = num;
            uOut[1] = (num2 - (num * a)) / b;
            return num2;
        }

        private static long FastModInverse(long v, long m)
        {
            if (m < 1L) throw new ArithmeticException("Modulus must be positive");
            long[] uOut = new long[2];
            if (FastExtEuclid(v, m, uOut) != 1L) throw new ArithmeticException("Numbers not relatively prime");
            if (uOut[0] < 0L) uOut[0] += m;
            return uOut[0];
        }

        public BigInteger FlipBit(int n)
        {
            if (n < 0) throw new ArithmeticException("Bit address less than zero");
            if ((sign > 0) && (n < (BitLength - 1))) return FlipExistingBit(n);
            return Xor(One.ShiftLeft(n));
        }

        private BigInteger FlipExistingBit(int n)
        {
            int[] mag = (int[])magnitude.Clone();
            mag[(mag.Length - 1) - (n >> 5)] ^= (1) << n;
            return new BigInteger(sign, mag, false);
        }

        public BigInteger Gcd(BigInteger value)
        {
            BigInteger integer;
            if (value.sign == 0) return Abs();
            if (sign == 0) return value.Abs();
            BigInteger integer2 = this;
            for (BigInteger integer3 = value; integer3.sign != 0; integer3 = integer)
            {
                integer = integer2.Mod(integer3);
                integer2 = integer3;
            }
            return integer2;
        }

        private static int GetByteLength(int nBits)
        {
            return (((nBits + 8) - 1) / 8);
        }

        public override int GetHashCode()
        {
            int length = magnitude.Length;
            if (magnitude.Length > 0)
            {
                length ^= magnitude[0];
                if (magnitude.Length > 1) length ^= magnitude[magnitude.Length - 1];
            }
            if (sign >= 0) return length;
            return ~length;
        }

        public int GetLowestSetBit()
        {
            if (sign == 0) return -1;
            int length = magnitude.Length;
            while (--length > 0) if (magnitude[length] != 0) break;
            int num2 = magnitude[length];
            int num3 = ((num2 & 0xffff) == 0) ? (((num2 & 0xff0000) == 0) ? 7 : 15) : (((num2 & 0xff) == 0) ? 0x17 : 0x1f);
            while (num3 > 0)
            {
                if ((num2 << num3) == -2147483648) break;
                num3--;
            }
            return (((magnitude.Length - length) * 0x20) - (num3 + 1));
        }

        private long GetMQuote()
        {
            if (mQuote == -1L)
            {
                if ((magnitude.Length == 0) || ((magnitude[magnitude.Length - 1] & 1) == 0)) return -1L;
                long v = (~magnitude[magnitude.Length - 1] | 1) & (0xffffffffL);
                mQuote = FastModInverse(v, 0x100000000L);
            }
            return mQuote;
        }

        private BigInteger Inc()
        {
            if (sign == 0) return One;
            if (sign < 0) return new BigInteger(-1, DoSubBigLil(magnitude, One.magnitude), true);
            return AddToMagnitude(One.magnitude);
        }

        public bool IsProbablePrime(int certainty)
        {
            if (certainty <= 0) return true;
            BigInteger integer = Abs();
            if (!integer.TestBit(0)) return integer.Equals(Two);
            if (integer.Equals(One)) return false;
            return integer.CheckProbablePrime(certainty, RandomSource);
        }

        private int[] LastNBits(int n)
        {
            if (n < 1) return ZeroMagnitude;
            int num = ((n + 0x20) - 1) / 0x20;
            num = Math.Min(num, magnitude.Length);
            int[] destinationArray = new int[num];
            Array.Copy(magnitude, magnitude.Length - num, destinationArray, 0, num);
            int num2 = n % 0x20;
            if (num2 != 0) destinationArray[0] &= ~(((-1)) << num2);
            return destinationArray;
        }

        private static int[] MakeMagnitude(byte[] bytes, int offset, int length)
        {
            int num = offset + length;
            int index = offset;
            while ((index < num) && (bytes[index] == 0)) index++;
            if (index >= num) return ZeroMagnitude;
            int num3 = ((num - index) + 3) / 4;
            int num4 = (num - index) % 4;
            if (num4 == 0) num4 = 4;
            if (num3 < 1) return ZeroMagnitude;
            int[] numArray = new int[num3];
            int num5 = 0;
            int num6 = 0;
            for (int i = index; i < num; i++)
            {
                num5 = num5 << 8;
                num5 |= bytes[i] & 0xff;
                num4--;
                if (num4 <= 0)
                {
                    numArray[num6] = num5;
                    num6++;
                    num4 = 4;
                    num5 = 0;
                }
            }
            if (num6 < numArray.Length) numArray[num6] = num5;
            return numArray;
        }

        public BigInteger Max(BigInteger value)
        {
            if (CompareTo(value) <= 0) return value;
            return this;
        }

        public BigInteger Min(BigInteger value)
        {
            if (CompareTo(value) >= 0) return value;
            return this;
        }

        public BigInteger Mod(BigInteger m)
        {
            if (m.sign < 1) throw new ArithmeticException("Modulus must be positive");
            BigInteger integer = Remainder(m);
            if (integer.sign < 0) return integer.Add(m);
            return integer;
        }

        public BigInteger ModInverse(BigInteger m)
        {
            if (m.sign < 1) throw new ArithmeticException("Modulus must be positive");
            BigInteger integer = new BigInteger();
            if (!ExtEuclid(Mod(m), m, integer, null).Equals(One)) throw new ArithmeticException("Numbers not relatively prime.");
            if (integer.sign < 0)
            {
                integer.sign = 1;
                integer.magnitude = DoSubBigLil(m.magnitude, integer.magnitude);
            }
            return integer;
        }

        public BigInteger ModPow(BigInteger exponent, BigInteger m)
        {
            if (m.sign < 1) throw new ArithmeticException("Modulus must be positive");
            if (m.Equals(One)) return Zero;
            if (exponent.sign == 0) return One;
            if (sign == 0) return Zero;
            int[] array = null;
            int[] a = null;
            bool flag = (m.magnitude[m.magnitude.Length - 1] & 1) == 1;
            long mQuote = 0L;
            if (flag)
            {
                mQuote = m.GetMQuote();
                array = ShiftLeft(0x20 * m.magnitude.Length).Mod(m).magnitude;
                flag = array.Length <= m.magnitude.Length;
                if (flag)
                {
                    a = new int[m.magnitude.Length + 1];
                    if (array.Length < m.magnitude.Length)
                    {
                        int[] numArray4 = new int[m.magnitude.Length];
                        array.CopyTo(numArray4, (numArray4.Length - array.Length));
                        array = numArray4;
                    }
                }
            }
            if (!flag)
            {
                if (magnitude.Length <= m.magnitude.Length)
                {
                    array = new int[m.magnitude.Length];
                    magnitude.CopyTo(array, (array.Length - magnitude.Length));
                }
                else
                {
                    BigInteger integer2 = Remainder(m);
                    array = new int[m.magnitude.Length];
                    integer2.magnitude.CopyTo(array, (array.Length - integer2.magnitude.Length));
                }
                a = new int[m.magnitude.Length * 2];
            }
            int[] numArray3 = new int[m.magnitude.Length];
            for (int i = 0; i < exponent.magnitude.Length; i++)
            {
                int num3 = exponent.magnitude[i];
                int num4 = 0;
                if (i == 0)
                {
                    while (num3 > 0)
                    {
                        num3 = num3 << 1;
                        num4++;
                    }
                    array.CopyTo(numArray3, 0);
                    num3 = num3 << 1;
                    num4++;
                }
                while (num3 != 0)
                {
                    if (flag) MultiplyMonty(a, numArray3, numArray3, m.magnitude, mQuote);
                    else
                    {
                        Square(a, numArray3);
                        Remainder(a, m.magnitude);
                        Array.Copy(a, a.Length - numArray3.Length, numArray3, 0, numArray3.Length);
                        ZeroOut(a);
                    }
                    num4++;
                    if (num3 < 0)
                    {
                        if (flag) MultiplyMonty(a, numArray3, array, m.magnitude, mQuote);
                        else
                        {
                            Multiply(a, numArray3, array);
                            Remainder(a, m.magnitude);
                            Array.Copy(a, a.Length - numArray3.Length, numArray3, 0, numArray3.Length);
                            ZeroOut(a);
                        }
                    }
                    num3 = num3 << 1;
                }
                while (num4 < 0x20)
                {
                    if (flag)  MultiplyMonty(a, numArray3, numArray3, m.magnitude, mQuote);
                    else
                    {
                        Square(a, numArray3);
                        Remainder(a, m.magnitude);
                        Array.Copy(a, a.Length - numArray3.Length, numArray3, 0, numArray3.Length);
                        ZeroOut(a);
                    }
                    num4++;
                }
            }
            if (flag)
            {
                ZeroOut(array);
                array[array.Length - 1] = 1;
                MultiplyMonty(a, numArray3, array, m.magnitude, mQuote);
            }
            BigInteger integer3 = new BigInteger(1, numArray3, true);
            if (exponent.sign <= 0) return integer3.ModInverse(m);
            return integer3;
        }

        public BigInteger Multiply(BigInteger val)
        {
            if ((sign == 0) || (val.sign == 0)) return Zero;
            if (val.QuickPow2Check())
            {
                BigInteger integer = ShiftLeft(val.Abs().BitLength - 1);
                if (val.sign <= 0) return integer.Negate();
                return integer;
            }
            if (QuickPow2Check())
            {
                BigInteger integer2 = val.ShiftLeft(Abs().BitLength - 1);
                if (sign <= 0) return integer2.Negate();
                return integer2;
            }
            int num = ((BitLength + val.BitLength) / 0x20) + 1;
            int[] w = new int[num];
            if (val == this) Square(w, magnitude);
            else Multiply(w, magnitude, val.magnitude);
            return new BigInteger(sign * val.sign, w, true);
        }

        private static int[] Multiply(int[] x, int[] y, int[] z)
        {
            int length = z.Length;
            if (length < 1) return x;
            int index = x.Length - y.Length;
            while (true)
            {
                long num3 = z[--length] & (0xffffffffL);
                long num4 = 0L;
                for (int i = y.Length - 1; i >= 0; i--)
                {
                    num4 += (((num3) * (y[i] & 0xffffffffL)) + (x[index + i] & 0xffffffffL));
                    x[index + i] = (int)num4;
                    num4 = num4 >> 0x20;
                }
                index--;
                if (length < 1)
                {
                    if (index >= 0) x[index] = (int)num4;
                    return x;
                }
                x[index] = (int)num4;
            }
        }

        private static void MultiplyMonty(int[] a, int[] x, int[] y, int[] m, long mQuote)
        {
            if (m.Length == 1) x[0] = (int)MultiplyMontyNIsOne((uint)x[0], (uint)y[0], (uint)m[0], (ulong)mQuote);
            else
            {
                int length = m.Length;
                int index = length - 1;
                long num3 = y[index] & (0xffffffffL);
                Array.Clear(a, 0, length + 1);
                for (int i = length; i > 0; i--)
                {
                    long num5 = x[i - 1] & (0xffffffffL);
                    long num6 = (((((a[length] & 0xffffffffL) + (((num5 * num3)) & 0xffffffffL)) & 0xffffffffL) * (mQuote)) & 0xffffffffL);
                    long num7 = num5 * num3;
                    long num8 = ((num6) * (m[index] & 0xffffffffL));
                    long num9 = (((a[length] & 0xffffffffL) + ((num7) & 0xffffffffL)) + ((num8) & 0xffffffffL));
                    long num10 = ((num7 >> 0x20) + (num8 >> 0x20)) + (num9 >> 0x20);
                    for (int j = index; j > 0; j--)
                    {
                        num7 = ((num5) * (y[j - 1] & 0xffffffffL));
                        num8 = ((num6) * (m[j - 1] & 0xffffffffL));
                        num9 = ((((a[j] & 0xffffffffL) + ((num7) & 0xffffffffL)) + ((num8) & 0xffffffffL)) + ((num10) & 0xffffffffL));
                        num10 = (((num10 >> 0x20) + (num7 >> 0x20)) + (num8 >> 0x20)) + (num9 >> 0x20);
                        a[j + 1] = (int)num9;
                    }
                    num10 += a[0] & (0xffffffffL);
                    a[1] = (int)num10;
                    a[0] = (int)(num10 >> 0x20);
                }
                if (CompareTo(0, a, 0, m) >= 0) Subtract(0, a, 0, m);
                Array.Copy(a, 1, x, 0, length);
            }
        }

        private static uint MultiplyMontyNIsOne(uint x, uint y, uint m, ulong mQuote)
        {
            ulong num = m;
            ulong num2 = x * y;
            ulong num3 = (num2 * mQuote) & UIMASK;
            ulong num4 = num3 * num;
            ulong num5 = (num2 & UIMASK) + (num4 & UIMASK);
            ulong num6 = ((num2 >> 0x20) + (num4 >> 0x20)) + (num5 >> 0x20);
            if (num6 > num) num6 -= num;
            return (uint)(num6 & UIMASK);
        }

        public BigInteger Negate()
        {
            if (sign == 0) return this;
            return new BigInteger(-this.sign, this.magnitude, false);
        }

        public BigInteger NextProbablePrime()
        {
            if (sign < 0) throw new ArithmeticException("Cannot be called on value < 0");
            if (CompareTo(Two) < 0) return Two;
            BigInteger integer = Inc().SetBit(0);
            while (!integer.CheckProbablePrime(100, RandomSource)) integer = integer.Add(Two);
            return integer;
        }

        public BigInteger Not()
        {
            return Inc().Negate();
        }

        public BigInteger Or(BigInteger value)
        {
            if (sign == 0) return value;
            if (value.sign == 0) return this;
            int[] numArray = (sign > 0) ? magnitude : Add(One).magnitude;
            int[] numArray2 = (value.sign > 0) ? value.magnitude : value.Add(One).magnitude;
            bool flag = (sign < 0) || (value.sign < 0);
            int[] mag = new int[Math.Max(numArray.Length, numArray2.Length)];
            int num2 = mag.Length - numArray.Length;
            int num3 = mag.Length - numArray2.Length;
            for (int i = 0; i < mag.Length; i++)
            {
                int num5 = (i >= num2) ? numArray[i - num2] : 0;
                int num6 = (i >= num3) ? numArray2[i - num3] : 0;
                if (sign < 0) num5 = ~num5;
                if (value.sign < 0) num6 = ~num6;
                mag[i] = num5 | num6;
                if (flag) mag[i] = ~mag[i];
            }
            BigInteger integer = new BigInteger(1, mag, true);
            if (flag) integer = integer.Not();
            return integer;
        }

        public BigInteger Pow(int exp)
        {
            if (exp < 0) throw new ArithmeticException("Negative exponent");
            if (exp == 0) return One;
            if ((sign == 0) || Equals(One)) return this;
            BigInteger one = One;
            BigInteger val = this;
        Label_0037:
            if ((exp & 1) == 1) one = one.Multiply(val);
            exp = exp >> 1;
            if (exp != 0)
            {
                val = val.Multiply(val);
                goto Label_0037;
            }
            return one;
        }

        public static BigInteger ProbablePrime(int bitLength, Random random)
        {
            return new BigInteger(bitLength, 100, random);
        }

        private bool QuickPow2Check()
        {
            return ((sign > 0) && (nBits == 1));
        }

        internal bool RabinMillerTest(int certainty, Random random)
        {
            BigInteger integer4;
            BigInteger m = this;
            BigInteger integer2 = m.Subtract(One);
            int lowestSetBit = integer2.GetLowestSetBit();
            BigInteger exponent = integer2.ShiftRight(lowestSetBit);
        Label_001D:
            integer4 = new BigInteger(m.BitLength, random);
            if ((integer4.CompareTo(One) <= 0) || (integer4.CompareTo(integer2) >= 0)) goto Label_001D;
            BigInteger integer5 = integer4.ModPow(exponent, m);
            if (!integer5.Equals(One))
            {
                int num2 = 0;
                while (!integer5.Equals(integer2))
                {
                    if (++num2 == lowestSetBit) return false;
                    integer5 = integer5.ModPow(Two, m);
                    if (integer5.Equals(One)) return false;
                }
            }
            certainty -= 2;
            if (certainty > 0) goto Label_001D;
            return true;
        }

        public BigInteger Remainder(BigInteger n)
        {
            int[] numArray;
            if (n.sign == 0) throw new ArithmeticException("Division by zero error");
            if (sign == 0) return Zero;
            if (n.magnitude.Length == 1)
            {
                int m = n.magnitude[0];
                if (m > 0)
                {
                    if (m != 1)
                    {
                        int num2 = Remainder(m);
                        if (num2 != 0) return new BigInteger(sign, new int[] { num2 }, false);
                    }
                    return Zero;
                }
            }
            if (CompareNoLeadingZeroes(0, magnitude, 0, n.magnitude) < 0) return this;
            if (n.QuickPow2Check()) numArray = LastNBits(n.Abs().BitLength - 1);
            else
            {
                numArray = (int[])magnitude.Clone();
                numArray = Remainder(numArray, n.magnitude);
            }
            return new BigInteger(sign, numArray, true);
        }

        private int Remainder(int m)
        {
            long num = 0L;
            for (int i = 0; i < magnitude.Length; i++)
            {
                long num3 = (long)((ulong)magnitude[i]);
                num = ((num << 0x20) | num3) % (m);
            }
            return (int)num;
        }

        private int[] Remainder(int[] x, int[] y)
        {
            int index = 0;
            while ((index < x.Length) && (x[index] == 0)) index++;
            int num2 = 0;
            while ((num2 < y.Length) && (y[num2] == 0)) num2++; 
            int num3 = CompareNoLeadingZeroes(index, x, num2, y);
            if (num3 > 0)
            {
                int[] numArray;
                int num4 = CalcBitLength(num2, y);
                int num5 = CalcBitLength(index, x);
                int n = num5 - num4;
                int yIndx = 0;
                int num8 = num4;
                if (n > 0)
                {
                    numArray = ShiftLeft(y, n);
                    num8 += n;
                }
                else
                {
                    int length = y.Length - num2;
                    numArray = new int[length];
                    Array.Copy(y, num2, numArray, 0, length);
                }
                while (true)
                {
                    if ((num8 < num5) || (CompareNoLeadingZeroes(index, x, yIndx, numArray) >= 0))
                    {
                        Subtract(index, x, yIndx, numArray);
                        while (x[index] == 0) if (++index == x.Length) return x;
                        num5 = (0x20 * ((x.Length - index) - 1)) + BitLen(x[index]);
                        if (num5 <= num4)
                        {
                            if (num5 < num4) return x; 
                            num3 = CompareNoLeadingZeroes(index, x, num2, y);
                            if (num3 <= 0) break;
                        }
                    }
                    n = num8 - num5;
                    if (n == 1)
                    {
                        uint num10 = (uint)(numArray[yIndx] >> 1);
                        uint num11 = (uint)x[index];
                        if (num10 > num11) n++;
                    }
                    if (n < 2)
                    {
                        ShiftRightOneInPlace(yIndx, numArray);
                        num8--;
                    }
                    else
                    {
                        ShiftRightInPlace(yIndx, numArray, n);
                        num8 -= n;
                    }
                    while (numArray[yIndx] == 0) yIndx++;
                }
            }
            if (num3 == 0) Array.Clear(x, index, x.Length - index);
            return x;
        }

        public BigInteger SetBit(int n)
        {
            if (n < 0)  new ArithmeticException("Bit address less than zero");
            if (TestBit(n)) return this;
            if ((sign > 0) && (n < (BitLength - 1))) return FlipExistingBit(n);
            return Or(One.ShiftLeft(n));
        }

        public BigInteger ShiftLeft(int n)
        {
            if ((sign == 0) || (magnitude.Length == 0)) return Zero;
            if (n == 0) return this;
            if (n < 0)  return ShiftRight(-n);
            BigInteger integer = new BigInteger(sign, ShiftLeft(magnitude, n), true);
            if (nBits != -1) integer.nBits = (sign > 0) ? nBits : (nBits + n);
            if (nBitLength != -1) integer.nBitLength = nBitLength + n;
            return integer;
        }

        private static int[] ShiftLeft(int[] mag, int n)
        {
            int[] numArray;
            int num = n >> 5;
            int num2 = n & 0x1f;
            int length = mag.Length;
            if (num2 == 0)
            {
                numArray = new int[length + num];
                mag.CopyTo(numArray, 0);
                return numArray;
            }
            int index = 0;
            int num5 = 0x20 - num2;
            int num6 = mag[0] >> num5;
            if (num6 != 0)
            {
                numArray = new int[(length + num) + 1];
                numArray[index++] = num6;
            }
            else numArray = new int[length + num];
            int num7 = mag[0];
            for (int i = 0; i < (length - 1); i++)
            {
                int num9 = mag[i + 1];
                numArray[index++] = (num7 << num2) | (num9 >> num5);
                num7 = num9;
            }
            numArray[index] = mag[length - 1] << num2;
            return numArray;
        }

        public BigInteger ShiftRight(int n)
        {
            if (n == 0) return this;
            if (n < 0) return ShiftLeft(-n);
            if (n >= BitLength)
            {
                if (sign >= 0) return Zero;
                return One.Negate();
            }
            int num = ((BitLength - n) + 0x1f) >> 5;
            int[] destinationArray = new int[num];
            int num2 = n >> 5;
            int num3 = n & 0x1f;
            if (num3 == 0) Array.Copy(magnitude, 0, destinationArray, 0, destinationArray.Length);
            else
            {
                int num4 = 0x20 - num3;
                int index = (magnitude.Length - 1) - num2;
                for (int i = num - 1; i >= 0; i--)
                {
                    destinationArray[i] = magnitude[index--] >> num3;
                    if (index >= 0) destinationArray[i] |= magnitude[index] << num4;
                }
            }
            return new BigInteger(sign, destinationArray, false);
        }

        private static void ShiftRightInPlace(int start, int[] mag, int n)
        {
            int index = (n >> 5) + start;
            int num2 = n & 0x1f;
            int num3 = mag.Length - 1;
            if (index != start)
            {
                int num4 = index - start;
                for (int i = num3; i >= index; i--) mag[i] = mag[i - num4];
                for (int j = index - 1; j >= start; j--) mag[j] = 0;
            }
            if (num2 != 0)
            {
                int num7 = 0x20 - num2;
                int num8 = mag[num3];
                for (int k = num3; k > index; k--)
                {
                    int num10 = mag[k - 1];
                    mag[k] = (num8 >> num2) | (num10 << num7);
                    num8 = num10;
                }
                mag[index] = mag[index] >> num2;
            }
        }

        private static void ShiftRightOneInPlace(int start, int[] mag)
        {
            int num3;
            int length = mag.Length;
            for (int i = mag[length - 1]; --length > start; i = num3)
            {
                num3 = mag[length - 1];
                mag[length] = (i >> 1) | (num3 << 0x1f);
            }
            mag[start] = mag[start] >> 1;
        }

        private static int[] Square(int[] w, int[] x)
        {
            ulong num;
            ulong num2;
            int index = w.Length - 1;
            for (int i = x.Length - 1; i != 0; i--)
            {
                ulong num6 = (ulong)x[i];
                num = num6 * num6;
                num2 = num >> 0x20;
                num = (uint)num;
                num += (ulong)w[index];
                w[index] = (int)((uint)num);
                ulong num3 = num2 + (num >> 0x20);
                for (int j = i - 1; j >= 0; j--)
                {
                    index--;
                    num = num6 * ((ulong)x[j]);
                    num2 = num >> 0x1f;
                    num = (uint)(num << 1);
                    num += num3 + ((ulong)w[index]);
                    w[index] = (int)((uint)num);
                    num3 = num2 + (num >> 0x20);
                }
                num3 += (ulong)w[--index];
                w[index] = (int)((uint)num3);
                if (--index >= 0) w[index] = (int)((uint)(num3 >> 0x20));
                index += i;
            }
            num = (ulong)x[0];
            num *= num;
            num2 = num >> 0x20;
            num &= 0xffffffffL;
            num += (ulong)w[index];
            w[index] = (int)((uint)num);
            if (--index >= 0) w[index] = (int)((uint)((num2 + (num >> 0x20)) + ((ulong)w[index])));
            return w;
        }

        public BigInteger Subtract(BigInteger n)
        {
            BigInteger integer;
            BigInteger integer2;
            if (n.sign == 0) return this;
            if (sign == 0) return n.Negate();
            if (sign != n.sign) return Add(n.Negate());
            int num = CompareNoLeadingZeroes(0, magnitude, 0, n.magnitude);
            if (num == 0) return Zero;
            if (num < 0)
            {
                integer = n;
                integer2 = this;
            }
            else
            {
                integer = this;
                integer2 = n;
            }
            return new BigInteger(sign * num, DoSubBigLil(integer.magnitude, integer2.magnitude), true);
        }

        private static int[] Subtract(int xStart, int[] x, int yStart, int[] y)
        {
            int length = x.Length;
            int num2 = y.Length;
            int num4 = 0;
            do
            {
                long num3 = (((x[--length] & 0xffffffffL) - (y[--num2] & 0xffffffffL))) + num4;
                x[length] = (int)num3;
                num4 = (int)(num3 >> 0x3f);
            }
            while (num2 > yStart);
            if (num4 != 0) while (--x[--length] == -1) { }
            return x;
        }

        public bool TestBit(int n)
        {
            if (n < 0) throw new ArithmeticException("Bit position must not be negative");
            if (sign < 0)  return !Not().TestBit(n);
            int num = n / 0x20;
            if (num >= magnitude.Length) return false;
            int num2 = magnitude[(magnitude.Length - 1) - num];
            return (((num2 >> (n % 0x20)) & 1) > 0);
        }

        public byte[] ToByteArray()
        {
            return ToByteArray(false);
        }

        private byte[] ToByteArray(bool unsigned)
        {
            if (sign == 0)
            {
                if (!unsigned) return new byte[1];
                return ZeroEncoding;
            }
            int nBits = (unsigned && (sign > 0)) ? BitLength : (BitLength + 1);
            byte[] buffer = new byte[GetByteLength(nBits)];
            int length = magnitude.Length;
            int num4 = buffer.Length;
            if (sign > 0)
            {
                while (length > 1)
                {
                    uint num5 = (uint)magnitude[--length];
                    buffer[--num4] = (byte)num5;
                    buffer[--num4] = (byte)(num5 >> 8);
                    buffer[--num4] = (byte)(num5 >> 0x10);
                    buffer[--num4] = (byte)(num5 >> 0x18);
                }
                uint num6 = (uint)magnitude[0];
                while (num6 > 0xff)
                {
                    buffer[--num4] = (byte)num6;
                    num6 = num6 >> 8;
                }
                buffer[--num4] = (byte)num6;
                return buffer;
            }
            bool flag = true;
            while (length > 1)
            {
                uint num7 = (uint)~magnitude[--length];
                if (flag)
                {
                    flag = ++num7 == 0;
                }
                buffer[--num4] = (byte)num7;
                buffer[--num4] = (byte)(num7 >> 8);
                buffer[--num4] = (byte)(num7 >> 0x10);
                buffer[--num4] = (byte)(num7 >> 0x18);
            }
            uint num8 = (uint)magnitude[0];
            if (flag) num8--;
            while (num8 > 0xff)
            {
                buffer[--num4] = (byte)~num8;
                num8 = num8 >> 8;
            }
            buffer[--num4] = (byte)~num8;
            if (num4 > 0) buffer[--num4] = 0xff;
            return buffer;
        }

        public byte[] ToByteArrayUnsigned()
        {
            return ToByteArray(true);
        }

        public override string ToString()
        {
            return ToString(10);
        }

        public string ToString(int radix)
        {
            int num4 = radix;
            if (((num4 != 2) && (num4 != 10)) && (num4 != 0x10)) throw new FormatException("Only bases 2, 10, 16 are allowed");
            if (magnitude == null) return "null";
            if (sign == 0) return "0";
            StringBuilder builder = new StringBuilder();
            if (radix == 0x10)
            {
                builder.Append(magnitude[0].ToString("x"));
                for (int i = 1; i < magnitude.Length; i++) builder.Append(magnitude[i].ToString("x8"));
            }
            else if (radix == 2)
            {
                builder.Append('1');
                for (int j = BitLength - 2; j >= 0; j--) builder.Append(TestBit(j) ? '1' : '0');
            }
            else
            {
                Stack stack = new Stack();
                BigInteger val = ValueOf(radix);
                for (BigInteger integer2 = Abs(); integer2.sign != 0; integer2 = integer2.Divide(val))
                {
                    BigInteger integer3 = integer2.Mod(val);
                    if (integer3.sign == 0) stack.Push("0");
                    else stack.Push(integer3.magnitude[0].ToString("d"));
                }
                while (stack.Count != 0) builder.Append((string)stack.Pop());
            }
            string str = builder.ToString();
            if (str[0] == '0')
            {
                int startIndex = 0;
                while (str[++startIndex] == '0') { }
                str = str.Substring(startIndex);
            }
            if (sign == -1) str = "-" + str;
            return str;
        }

        public static BigInteger ValueOf(long value)
        {
            long num = value;
            if (num <= 3L)
            {
                if (num < 0L) goto Label_0049;
                switch (((int)num))
                {
                    case 0:
                        return Zero;

                    case 1:
                        return One;

                    case 2:
                        return Two;

                    case 3:
                        return Three;
                }
            }
            if (num == 10L)
            {
                return Ten;
            }
        Label_0049:
            return CreateValueOf(value);
        }

        public BigInteger Xor(BigInteger value)
        {
            if (sign == 0) return value;
            if (value.sign == 0) return this;
            int[] numArray = (sign > 0) ? magnitude : Add(One).magnitude;
            int[] numArray2 = (value.sign > 0) ? value.magnitude : value.Add(One).magnitude;
            bool flag = ((sign < 0) && (value.sign >= 0)) || ((sign >= 0) && (value.sign < 0));
            int[] mag = new int[Math.Max(numArray.Length, numArray2.Length)];
            int num2 = mag.Length - numArray.Length;
            int num3 = mag.Length - numArray2.Length;
            for (int i = 0; i < mag.Length; i++)
            {
                int num5 = (i >= num2) ? numArray[i - num2] : 0;
                int num6 = (i >= num3) ? numArray2[i - num3] : 0;
                if (sign < 0) num5 = ~num5;
                if (value.sign < 0) num6 = ~num6;
                mag[i] = num5 ^ num6;
                if (flag) mag[i] = ~mag[i];
            }
            BigInteger integer = new BigInteger(1, mag, true);
            if (flag) integer = integer.Not();
            return integer;
        }

        private static void ZeroOut(int[] x)
        {
            Array.Clear(x, 0, x.Length);
        }

        public int BitCount
        {
            get
            {
                if (nBits == -1)
                {
                    if (sign < 0) nBits = Not().BitCount;
                    else
                    {
                        int num = 0;
                        for (int i = 0; i < magnitude.Length; i++)
                        {
                            num += bitCounts[(byte)magnitude[i]];
                            num += bitCounts[(byte)(magnitude[i] >> 8)];
                            num += bitCounts[(byte)(magnitude[i] >> 0x10)];
                            num += bitCounts[(byte)(magnitude[i] >> 0x18)];
                        }
                        nBits = num;
                    }
                }
                return nBits;
            }
        }

        public int BitLength
        {
            get
            {
                if (nBitLength == -1) nBitLength = (sign == 0) ? 0 : CalcBitLength(0, magnitude);
                return nBitLength;
            }
        }

        public int IntValue
        {
            get
            {
                if (sign == 0) return 0;
                if (sign <= 0) return -magnitude[magnitude.Length - 1];
                return magnitude[magnitude.Length - 1];
            }
        }

        public long LongValue
        {
            get
            {
                long num;
                if (sign == 0) return 0L;
                if (magnitude.Length > 1) num = (long)(magnitude[magnitude.Length - 2] << 0x20) | (magnitude[magnitude.Length - 1] & (0xffffffffL));
                else num = magnitude[magnitude.Length - 1] & (0xffffffffL);
                if (sign >= 0) return num;
                return -num;
            }
        }

        public int SignValue
        {
            get
            {
                return sign;
            }
        }
    }
}