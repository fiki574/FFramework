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

namespace FFramework.Cryptography
{
    public static class StringCrypt
    {
        public static string Encrypt(string s, int key)
        {
            if (key <= 0 || key > 128)
                return "<invalid key>";

            char[] r = new char[s.Length];
            int i = 0, j = r.Length;
            for (i = 0; i < s.Length; i++)
                r[i] = s[i];

            for (i = 0; i < j; i++)
            {
                char c = r[i];
                if (i + 2 < j)
                {
                    r[i] = (char)(((r[i + 2] + 1) << 2) * key);
                    r[i + 2] = c;
                }
                else
                {
                    if (i + 1 < j)
                    {
                        r[i] = r[i + 1];
                        r[i + 1] = c;
                    }
                }
            }
            return new string(r);
        }

        public static string Decrypt(string s, int key)
        {
            if (key <= 0 || key > 128)
                return "<invalid key>";

            char[] r = new char[s.Length];
            int i = 0, j = r.Length;
            for (i = 0; i < s.Length; i++)
                r[i] = s[i];

            for (i = j - 1; i >= 0; i--)
            {
                char c = r[i];
                if (i - 2 >= 0)
                {
                    r[i] = (char)(((r[i - 2] - 1) >> 2) / key);
                    r[i - 2] = c;
                }
                else
                {
                    if (i - 1 >= 0)
                    {
                        r[i] = r[i - 1];
                        r[i - 1] = c;
                    }
                }
            }
            return new string(r);
        }
    }
}