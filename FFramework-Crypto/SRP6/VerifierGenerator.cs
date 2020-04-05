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

namespace FFramework_Crypto.SRP6
{
	public class Srp6VerifierGenerator
	{
		protected BigInteger N;

		protected BigInteger g;

		protected IDigest digest;

		public virtual void Init(BigInteger N, BigInteger g, IDigest digest)
		{
			this.N = N;
			this.g = g;
			this.digest = digest;
		}

		public virtual BigInteger GenerateVerifier(byte[] salt, byte[] identity, byte[] password)
		{
			BigInteger exponent = Utilities.CalculateX(digest, N, salt, identity, password);
			return g.ModPow(exponent, N);
		}
	}
}