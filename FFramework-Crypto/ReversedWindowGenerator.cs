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

namespace FFramework_Crypto.Generator
{
	public class ReversedWindowGenerator : IRandomGenerator
	{
		private readonly IRandomGenerator generator;

		private byte[] window;

		private int windowCount;

		public ReversedWindowGenerator(IRandomGenerator generator, int windowSize)
		{
            if (windowSize < 2)
			{
				throw new ArgumentException("Window size must be at least 2", "windowSize");
			}
			this.generator = generator ?? throw new ArgumentNullException("generator");
			window = new byte[windowSize];
		}

		public virtual void AddSeedMaterial(byte[] seed)
		{
			lock (this)
			{
				windowCount = 0;
				generator.AddSeedMaterial(seed);
			}
		}

		public virtual void AddSeedMaterial(long seed)
		{
			lock (this)
			{
				windowCount = 0;
				generator.AddSeedMaterial(seed);
			}
		}

		public virtual void NextBytes(byte[] bytes)
		{
			DoNextBytes(bytes, 0, bytes.Length);
		}

		public virtual void NextBytes(byte[] bytes, int start, int len)
		{
			DoNextBytes(bytes, start, len);
		}

		private void DoNextBytes(byte[] bytes, int start, int len)
		{
			lock (this)
			{
				int num = 0;
				while (num < len)
				{
					if (windowCount < 1)
					{
						generator.NextBytes(window, 0, window.Length);
						windowCount = window.Length;
					}
					bytes[start + num++] = window[--windowCount];
				}
			}
		}
	}
}