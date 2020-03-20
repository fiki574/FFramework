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
using System.IO;

namespace FFramework.Zlib
{
	public static class ZlibMgr
	{
		public static byte[] GetResult(byte[] Input, bool compress)
		{
			if (!compress)
			{
				return Decompress(Input);
			}
			return Compress(Input);
		}

		public static byte[] Compress(byte[] Input)
		{
			MemoryStream memoryStream = new MemoryStream();
			ZOutputStream zStream = new ZOutputStream(memoryStream, -1);
			Process(zStream, Input);
			return memoryStream.ToArray();
		}

		public static byte[] Decompress(byte[] Input)
		{
			MemoryStream memoryStream = new MemoryStream();
			ZOutputStream zStream = new ZOutputStream(memoryStream);
			Process(zStream, Input);
			return memoryStream.ToArray();
		}

		private static void Process(ZOutputStream ZStream, byte[] Input)
		{
			try
			{
				ZStream.Write(Input, 0, Input.Length);
				ZStream.Flush();
				ZStream.Close();
			}
			catch (Exception ex)
			{
                Console.WriteLine(ex.ToString());
			}
		}
	}
}