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

using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FFramework.Utilities
{
    public class PseudoRandomFunction
    {
        private readonly HMACSHA256 hmac;
        private readonly byte[] seed;

        public PseudoRandomFunction(byte[] secret, string label, IEnumerable<byte> seed)
        {
            this.seed = Encoding.ASCII.GetBytes(label).Concat(seed).ToArray();
            hmac = new HMACSHA256(secret);
        }

        public byte[] GenerateBytes(int outputLength)
        {
            return PHash(outputLength);
        }

        private byte[] PHash(int outputLength)
        {
            List<byte> result = new List<byte>(outputLength);
            int i = 1;
            do
            {
                byte[] hash = hmac.ComputeHash(A(i).Concat(seed).ToArray());
                result.AddRange(hash);
                i++;
            }
            while (result.Count < outputLength);
            return result.Take(outputLength).ToArray();
        }

        private byte[] A(int i)
        {
            return i == 0 ? seed : hmac.ComputeHash(A(i - 1));
        }
    }
}