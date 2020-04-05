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
	public class Server
	{
		protected BigInteger N;

		protected BigInteger g;

		protected BigInteger v;

		protected SecureRandom random;

		protected IDigest digest;

		protected BigInteger A;

		protected BigInteger privB;

		protected BigInteger pubB;

		protected BigInteger u;

		protected BigInteger S;

		public virtual void Init(BigInteger N, BigInteger g, BigInteger v, IDigest digest, SecureRandom random)
		{
			this.N = N;
			this.g = g;
			this.v = v;
			this.random = random;
			this.digest = digest;
		}

		public virtual BigInteger GenerateServerCredentials()
		{
			BigInteger bigInteger = Utilities.CalculateK(digest, N, g);
			privB = SelectPrivateValue();
			pubB = bigInteger.Multiply(v).Mod(N).Add(g.ModPow(privB, N))
				.Mod(N);
			return pubB;
		}

		public virtual BigInteger CalculateSecret(BigInteger clientA)
		{
			A = Utilities.ValidatePublicValue(N, clientA);
			u = Utilities.CalculateU(digest, N, A, pubB);
			S = CalculateS();
			return S;
		}

		protected virtual BigInteger SelectPrivateValue()
		{
			return Utilities.GeneratePrivateValue(digest, N, g, random);
		}

		private BigInteger CalculateS()
		{
			return v.ModPow(u, N).Multiply(A).Mod(N).ModPow(privB, N);
		}
	}
}