/*
    C# Framework with a lot of useful functions and classes
    Copyright (C) 2018/2019 Bruno Fištrek

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
    
    Credits: https://github.com/usertoroot/PakPacker
*/

using System;

namespace FFramework.IO.Memory
{
    public class VariableBuffer
    {
        public byte[] Data;
        private int m_sizeStep;

        public VariableBuffer(int initialSize = 2048, int sizeStep = 2048)
        {
            Data = new byte[initialSize];
            m_sizeStep = sizeStep;
        }

        public void EnsureLength(int length)
        {
            if (length > Data.Length)
            {
                float f = length / (float)m_sizeStep;
                int newSize = m_sizeStep * (int)Math.Ceiling(f);
                Data = new byte[newSize];
            }
        }
    }
}