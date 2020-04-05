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

namespace FFramework_Crypto.Generator
{
	public class DigestRandomGenerator : IRandomGenerator
	{
		private const long CYCLE_COUNT = 10L;

		private long stateCounter;

		private long seedCounter;

		private IDigest digest;

		private byte[] state;

		private readonly byte[] seed;

		public DigestRandomGenerator(IDigest digest)
		{
			this.digest = digest;
			seed = new byte[digest.GetDigestSize()];
			seedCounter = 1L;
			state = new byte[digest.GetDigestSize()];
			stateCounter = 1L;
		}

		public void AddSeedMaterial(byte[] inSeed)
		{
			lock (this)
			{
				DigestUpdate(inSeed);
				DigestUpdate(seed);
				DigestDoFinal(seed);
			}
		}

		public void AddSeedMaterial(long rSeed)
		{
			lock (this)
			{
				DigestAddCounter(rSeed);
				DigestUpdate(seed);
				DigestDoFinal(seed);
			}
		}

		public void NextBytes(byte[] bytes)
		{
			NextBytes(bytes, 0, bytes.Length);
		}

		public void NextBytes(byte[] bytes, int start, int len)
		{
			lock (this)
			{
				int num = 0;
				GenerateState();
				int num2 = start + len;
				for (int i = start; i < num2; i++)
				{
					if (num == state.Length)
					{
						GenerateState();
						num = 0;
					}
					bytes[i] = state[num++];
				}
			}
		}

		private void CycleSeed()
		{
			DigestUpdate(seed);
			DigestAddCounter(seedCounter++);
			DigestDoFinal(seed);
		}

		private void GenerateState()
		{
			DigestAddCounter(stateCounter++);
			DigestUpdate(state);
			DigestUpdate(seed);
			DigestDoFinal(state);
			if (stateCounter % 10 == 0)
			{
				CycleSeed();
			}
		}

		private void DigestAddCounter(long seedVal)
		{
			ulong num = (ulong)seedVal;
			for (int i = 0; i != 8; i++)
			{
				digest.Update((byte)num);
				num >>= 8;
			}
		}

		private void DigestUpdate(byte[] inSeed)
		{
			digest.BlockUpdate(inSeed, 0, inSeed.Length);
		}

		private void DigestDoFinal(byte[] result)
		{
			digest.DoFinal(result, 0);
		}
	}
}