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

namespace FFramework.Cryptography.Network
{
	public class ARC4
	{
		private readonly byte[] state;
		private byte x;
		private byte y;

		public ARC4(byte[] key)
		{
			state = new byte[256];
			x = y = 0;
			KeySetup(key);
		}

		public int Process(byte[] buffer, int start, int count)
		{
			return InternalTransformBlock(buffer, start, count, buffer, start);
		}

		private void KeySetup(byte[] key)
		{
			byte b = 0;
			byte b2 = 0;
			for (int i = 0; i < 256; i++)
				state[i] = (byte)i;

			x = 0;
			y = 0;
			for (int j = 0; j < 256; j++)
			{
				b2 = (byte)(key[b] + state[j] + b2);
				byte b3 = state[j];
				state[j] = state[b2];
				state[b2] = b3;
				b = (byte)((b + 1) % key.Length);
			}
		}

		private int InternalTransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			for (int i = 0; i < inputCount; i++)
			{
				x++;
				y = (byte)(state[x] + y);
				byte b = state[x];
				state[x] = state[y];
				state[y] = b;
				byte b2 = (byte)(state[x] + state[y]);
				outputBuffer[outputOffset + i] = (byte)(inputBuffer[inputOffset + i] ^ state[b2]);
			}
			return inputCount;
		}
	}
}