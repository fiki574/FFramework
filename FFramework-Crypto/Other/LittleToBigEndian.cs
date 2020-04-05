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

namespace FFramework_Crypto.Other
{
    public static class LittleToBigEndian
    {
        public static long LTBE(long input)
        {
            return (long)(((ulong)LTBE((uint)((ulong)input & 0xFFFFFFFF)) << 32) | (ulong)LTBE((uint)(((ulong)input & 0xFFFFFFFF00000000) >> 32)));
        }

        public static ulong LTBE(ulong input)
        {
            return (ulong)(((ulong)LTBE((uint)(input & 0xFFFFFFFF)) << 32) | (ulong)LTBE((uint)((input & 0xFFFFFFFF00000000) >> 32)));
        }

        public static int LTBE(int input)
        {
            return ((input & 0xff) << 24) + ((input & 0xff00) << 8) + ((input & 0xff0000) >> 8) + ((input >> 24) & 0xff);
        }

        public static uint LTBE(uint input)
        {
            return ((input & 0xff) << 24) + ((input & 0xff00) << 8) + ((input & 0xff0000) >> 8) + ((input >> 24) & 0xff);
        }

        public static ushort LTBE(ushort input)
        {
            return (ushort)(((input >> 8) & 0xff) + ((input << 8) & 0xff00));
        }

        public static short LTBE(short input)
        {
            return (short)(((input >> 8) & 0xff) + ((input << 8) & 0xff00));
        }
    }
}