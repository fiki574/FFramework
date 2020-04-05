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
using FFramework_Crypto.Generator;

namespace FFramework_Crypto.SRP6
{
	public sealed class BigIntegers
	{
		private const int MaxIterations = 1000;

		private BigIntegers()
		{
		}

		public static byte[] AsUnsignedByteArray(BigInteger n)
		{
			return n.ToByteArrayUnsigned();
		}

		public static BigInteger CreateRandomInRange(BigInteger min, BigInteger max, SecureRandom random)
		{
			int num = min.CompareTo(max);
			if (num >= 0)
			{
				if (num > 0)
				{
					throw new ArgumentException("'min' may not be greater than 'max'");
				}
				return min;
			}
			if (min.BitLength > max.BitLength / 2)
			{
				return CreateRandomInRange(BigInteger.Zero, max.Subtract(min), random).Add(min);
			}
			for (int i = 0; i < 1000; i++)
			{
				BigInteger bigInteger = new BigInteger(max.BitLength, random);
				if (bigInteger.CompareTo(min) >= 0 && bigInteger.CompareTo(max) <= 0)
				{
					return bigInteger;
				}
			}
			return new BigInteger(max.Subtract(min).BitLength - 1, random).Add(min);
		}
	}
}