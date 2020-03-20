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
using System.Threading;

namespace FFramework.Generator
{
	public class ThreadedSeedGenerator
	{
		private class SeedGenerator
		{
			private volatile int counter;

			private volatile bool stop;

			private void Run(object ignored)
			{
				while (!stop)
				{
					counter++;
				}
			}

			public byte[] GenerateSeed(int numBytes, bool fast)
			{
				counter = 0;
				stop = false;
				byte[] array = new byte[numBytes];
				int num = 0;
				int num2 = fast ? numBytes : (numBytes * 8);
				ThreadPool.QueueUserWorkItem(Run);
				for (int i = 0; i < num2; i++)
				{
					while (counter == num)
					{
						try
						{
							Thread.Sleep(1);
						}
						catch (Exception)
						{
						}
					}
					num = counter;
					if (fast)
					{
						array[i] = (byte)num;
						continue;
					}
					int num3 = i / 8;
					array[num3] = (byte)((array[num3] << 1) | (num & 1));
				}
				stop = true;
				return array;
			}
		}

		public byte[] GenerateSeed(int numBytes, bool fast)
		{
			return new SeedGenerator().GenerateSeed(numBytes, fast);
		}
	}
}