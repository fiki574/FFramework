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

using FFramework_Crypto.SHA;
using FFramework_Crypto.Generator;

namespace FFramework_Crypto.SRP6
{
	public class Client
	{
		protected BigInteger N;

		protected BigInteger g;

		protected BigInteger privA;

		protected BigInteger pubA;

		protected BigInteger B;

		protected BigInteger x;

		protected BigInteger u;

		protected BigInteger S;

		protected IDigest digest;

		protected SecureRandom random;

		public virtual void Init(BigInteger N, BigInteger g, IDigest digest, SecureRandom random)
		{
			this.N = N;
			this.g = g;
			this.digest = digest;
			this.random = random;
		}

		public virtual BigInteger GenerateClientCredentials(byte[] salt, byte[] identity, byte[] password)
		{
			x = Utilities.CalculateX(digest, N, salt, identity, password);
			privA = SelectPrivateValue();
			pubA = g.ModPow(privA, N);
			return pubA;
		}

		public virtual BigInteger CalculateSecret(BigInteger serverB)
		{
			B = Utilities.ValidatePublicValue(N, serverB);
			u = Utilities.CalculateU(digest, N, pubA, B);
			S = CalculateS();
			return S;
		}

		protected virtual BigInteger SelectPrivateValue()
		{
			return Utilities.GeneratePrivateValue(digest, N, g, random);
		}

		private BigInteger CalculateS()
		{
			BigInteger val = Utilities.CalculateK(digest, N, g);
			BigInteger exponent = u.Multiply(x).Add(privA);
			BigInteger n = g.ModPow(x, N).Multiply(val).Mod(N);
			return B.Subtract(n).Mod(N).ModPow(exponent, N);
		}
	}
}