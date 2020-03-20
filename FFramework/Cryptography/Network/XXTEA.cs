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

namespace FFramework.Cryptography.Other
{
    public class XXTEA
    {
        public static void Encrypt(uint[] v, uint[] k, int rounds)
        {
            const uint DELTA = 0x9E3779B9;
            uint sum, y, z, e;
            int n, p;

            sum = 0;
            n = v.Length;
            z = v[n - 1];

            while (rounds-- > 0)
            {
                sum += DELTA;
                e = (sum >> 2) & 3;

                for (p = 0; p < (n - 1); p++)
                {
                    y = v[p + 1];
                    z = v[p] += (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z);
                }

                y = v[0];
                z = v[n - 1] += (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z);
            }
        }
    }
}