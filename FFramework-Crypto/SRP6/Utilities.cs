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
using FFramework_Crypto.SHA;
using FFramework_Crypto.Generator;

namespace FFramework_Crypto.SRP6
{
	public class Utilities
	{
		public static BigInteger CalculateK(IDigest digest, BigInteger N, BigInteger g)
		{
			return HashPaddedPair(digest, N, N, g);
		}

		public static BigInteger CalculateU(IDigest digest,BigInteger N, BigInteger A, BigInteger B)
		{
			return HashPaddedPair(digest, N, A, B);
		}

		public static BigInteger CalculateX(IDigest digest, BigInteger N, byte[] salt, byte[] identity, byte[] password)
		{
			byte[] array = new byte[digest.GetDigestSize()];
			digest.BlockUpdate(identity, 0, identity.Length);
			digest.Update(58);
			digest.BlockUpdate(password, 0, password.Length);
			digest.DoFinal(array, 0);
			digest.BlockUpdate(salt, 0, salt.Length);
			digest.BlockUpdate(array, 0, array.Length);
			digest.DoFinal(array, 0);
			return new BigInteger(1, array).Mod(N);
		}

		public static BigInteger GeneratePrivateValue(IDigest digest, BigInteger N, BigInteger g, SecureRandom random)
		{
			int num = Math.Min(256, N.BitLength / 2);
			BigInteger min = BigInteger.One.ShiftLeft(num - 1);
			BigInteger max = N.Subtract(BigInteger.One);
			return BigIntegers.CreateRandomInRange(min, max, random);
		}

		public static BigInteger ValidatePublicValue(BigInteger N, BigInteger val)
		{
			val = val.Mod(N);
			if (val.Equals(BigInteger.Zero))
			{
				throw new Exception("Invalid public value: 0");
			}
			return val;
		}

		private static BigInteger HashPaddedPair(IDigest digest, BigInteger N, BigInteger n1, BigInteger n2)
		{
			int length = (N.BitLength + 7) / 8;
			byte[] padded = GetPadded(n1, length);
			byte[] padded2 = GetPadded(n2, length);
			digest.BlockUpdate(padded, 0, padded.Length);
			digest.BlockUpdate(padded2, 0, padded2.Length);
			byte[] array = new byte[digest.GetDigestSize()];
			digest.DoFinal(array, 0);
			return new BigInteger(1, array).Mod(N);
		}

		private static byte[] GetPadded(BigInteger n, int length)
		{
			byte[] array = BigIntegers.AsUnsignedByteArray(n);
			if (array.Length < length)
			{
				byte[] array2 = new byte[length];
				Array.Copy(array, 0, array2, length - array.Length, array.Length);
				array = array2;
			}
			return array;
		}
	}
}