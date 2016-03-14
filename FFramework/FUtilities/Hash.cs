using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace FFramework.FUtilities
{
    public class Hash
    {
        ///<summary>Function that uses SHA256 crypto service to hash your text, password or whatever you want</summary>
        ///<param name="text">String you want to hash</param>
        ///<returns>Hashed string of the inputted text</returns>
        public static string SHA256Hash(string text)
        {
            SHA256 sha = new SHA256CryptoServiceProvider();
            sha.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            byte[] result = sha.Hash;
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++) strBuilder.Append(result[i].ToString("x2"));
            return strBuilder.ToString();
        }

        ///<summary>Creates a random byte hash</summary>
        public static byte[] RandomHash(int seed = 0, int size = 8)
        {
            var hash = new byte[size];
            if (seed > 0) new Random(seed).NextBytes(hash);
            else new Random(DateTime.Now.Millisecond).NextBytes(hash);
            return hash;
        }
    }
}
