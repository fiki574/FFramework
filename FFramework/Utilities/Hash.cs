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
using System.Text;
using System.Security.Cryptography;

namespace FFramework.Utilities
{
    public class Hash
    {
        public static string SHA256Hash(string text)
        {
            SHA256 sha = new SHA256CryptoServiceProvider();
            sha.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            byte[] result = sha.Hash;
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++) strBuilder.Append(result[i].ToString("x2"));
            return strBuilder.ToString();
        }

        public static byte[] GenerateRandomHash(int seed = 0, int size = 8)
        {
            var hash = new byte[size];
            if (seed > 0) new Random(seed).NextBytes(hash);
            else new Random(DateTime.Now.Millisecond).NextBytes(hash);
            return hash;
        }
    }
}
