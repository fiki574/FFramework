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

namespace FFramework_Core.Threading
{
    public class ThreadSafeObject<T>
    {
        private readonly object objectLock = new object();
        private T value;
        private bool canWrite;

        public ThreadSafeObject(T value)
        {
            this.value = value;
            canWrite = true;
        }

        public ThreadSafeObject(T value, bool canWrite)
        {
            this.value = value;
            this.canWrite = canWrite;
        }

        public T Value
        {
            get
            {
                lock (objectLock)
                {
                    return value;
                }
            }
            set
            {
                lock (objectLock)
                {
                    if (!canWrite)
                        throw new InvalidOperationException("This object is readonly");

                    this.value = value;
                }
            }
        }
    }
}