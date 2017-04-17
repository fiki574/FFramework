/*
    C# Framework with a lot of useful functions and classes
    Copyright (C) 2017 Bruno Fištrek
    
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

namespace FFramework.Utilities
{
    public class Object<T>
    {
        private T data;

        public Object()
        {
        }

        public Object(T data)
        {
            this.data = data;
        }

        public void SetValue(T data)
        {
            this.data = data;
        }

        public T GetValue()
        {
            return data;
        }

        public override string ToString()
        {
            return data.ToString();
        }

        public short ToInt16()
        {
            short result;
            try
            {
                result = Convert.ToInt16(data);
            }
            catch
            {
                result = -1;
            }
            return result;
        }

        public int ToInt32()
        {
            int result;
            try
            {
                result = Convert.ToInt32(data);
            }
            catch
            {
                result = -1;
            }
            return result;
        }

        public long ToInt64()
        {
            long result;
            try
            {
                result = Convert.ToInt64(data);
            }
            catch
            {
                result = -1;
            }
            return result;
        }

        public ushort ToUInt16()
        {
            ushort result;
            try
            {
                result = Convert.ToUInt16(data);
            }
            catch
            {
                result = 0;
            }
            return result;
        }

        public uint ToUInt32()
        {
            uint result;
            try
            {
                result = Convert.ToUInt32(data);
            }
            catch
            {
                result = 0;
            }
            return result;
        }

        public ulong ToUInt64()
        {
            ulong result;
            try
            {
                result = Convert.ToUInt64(data);
            }
            catch
            {
                result = 0;
            }
            return result;
        }

        public bool ToBoolean()
        {
            bool result;
            try
            {
                result = Convert.ToBoolean(data);
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public byte ToByte()
        {
            byte result;
            try
            {
                result = Convert.ToByte(data);
            }
            catch
            {
                result = 0;
            }
            return result;
        }

        public char ToChar()
        {
            char result;
            try
            {
                result = Convert.ToChar(data);
            }
            catch
            {
                result = '0';
            }
            return result;
        }

        public DateTime ToDateTime()
        {
            DateTime result;
            try
            {
                result = Convert.ToDateTime(data);
            }
            catch
            {
                result = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            }
            return result;
        }

        public decimal ToDecimal()
        {
            decimal result;
            try
            {
                result = Convert.ToDecimal(data);
            }
            catch
            {
                result = 0;
            }
            return result;
        }

        public double ToDouble()
        {
            double result;
            try
            {
                result = Convert.ToDouble(data);
            }
            catch
            {
                result = 0;
            }
            return result;
        }

        public sbyte ToSByte()
        {
            sbyte result;
            try
            {
                result = Convert.ToSByte(data);
            }
            catch
            {
                result = 0;
            }
            return result;
        }

        public float ToSingle()
        {
            float result;
            try
            {
                result = Convert.ToSingle(data);
            }
            catch
            {
                result = 0.0f;
            }
            return result;
        }
    }
}
