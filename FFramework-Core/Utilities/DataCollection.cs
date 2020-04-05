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

namespace FFramework_Core.Utilities
{
    public class DataCollection<T>
    {
        private T[] data;
        private int position;

        public DataCollection()
        {
            data = new T[0];
        }

        public DataCollection(T[] data)
        {
            this.data = data;
        }

        public T Read()
        {
            T tmp = data[position];
            position += 1;
            return tmp;
        }

        public T[] ReadRange(int count)
        {
            var tmp = new T[count];
            Array.Copy(data, position, tmp, 0, count);
            position += count;
            return tmp;
        }

        public void Write(T input)
        {
            var tmp = new T[data.Length + 1];
            Array.Copy(data, tmp, data.Length);
            tmp[data.Length] = input;
            data = tmp;
        }

        public void WriteRange(T[] input)
        {
            var tmp = new T[data.Length + input.Length];
            Array.Copy(data, tmp, data.Length);
            Array.Copy(input, 0, tmp, data.Length, input.Length);
            data = tmp;
        }

        public T[] ToArray()
        {
            return data;
        }

        public int Position
        {
            get
            {
                return position;
            }
            set
            {
                if (value > data.Length)
                    throw new IndexOutOfRangeException();

                position = value;
            }
        }

        public int Length
        {
            get
            {
                return data.Length;
            }
        }
    }
}