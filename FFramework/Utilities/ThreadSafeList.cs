/*
    C# Framework with a lot of useful functions and classes
    Copyright (C) 2016 Bruno Fištrek

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
using System.Collections.Generic;

namespace FFramework.Utilities
{
    public class ThreadSafeList<T>
    {
        private List<T> list = new List<T>();

        public ThreadSafeList()
        {
        }

        public ThreadSafeList(List<T> list)
        {
            this.list = list;
        }

        public int Length
        {
            get
            {
                return list.Count;
            }
        }

        public List<R> Select<R>(Func<T, R> selector)
        {
            List<R> nList = new List<R>();
            lock (list)
            {
                foreach (T item in list) nList.Add(selector(item));
            }
            return nList;
        }

        public List<T> Where(Func<T, bool> selector)
        {
            List<T> nList = new List<T>();
            lock (list)
            {
                foreach (T item in list) if (selector(item)) nList.Add(item);
            }
            return nList;
        }

        public T Single(Func<T, bool> condition)
        {
            int count = Count(condition);
            if (count > 1) throw new Exception("Multiple entries found matching condition.");
            else if (count == 0) throw new Exception("No entry found matching condition.");
            else
                lock (list)
                {
                    foreach (T item in list) if (condition(item)) return item;
                }
            throw new Exception("No entry found matching condition.");
        }

        public T FirstOrDefault(Func<T, bool> condition)
        {
            lock (list)
            {
                foreach (T item in list) if (condition(item)) return item;
            }
            return default(T);
        }

        public T SingleOrDefault(Func<T, bool> condition)
        {
            int count = Count(condition);
            if (count > 1) throw new Exception("Multiple entries found matching condition.");
            else if (count == 0) return default(T);
            else
                lock (list)
                {
                    foreach (T item in list) if (condition(item)) return item;
                } 
            return default(T);
        }

        public T[] ToArray()
        {
            lock (list)
            {
                return list.ToArray();
            }
        }

        public void ForEach(Action<T> feachAction)
        {
            lock (list)
            {
                foreach (T item in list) feachAction(item);
            }
        }

        public int Count(Func<T, bool> selector)
        {
            int i = 0;
            lock (list)
            {
                foreach (T item in list) if (selector(item)) i++;
            }
            return i;
        }

        public bool Contains(Func<T, bool> selector)
        {
            lock (list)
            {
                foreach (T item in list) if (selector(item)) return true;
            }
            return false;
        }

        public bool Contains(T item)
        {
            lock (list)
            {
                foreach (T i in list) if (i.Equals(item)) return true;
                return false;
            }
        }

        public void Add(T item)
        {
            lock (list)
            {
                list.Add(item);
            }
        }

        public void Remove(T item)
        {
            lock (list)
            {
                list.Remove(item);
            }
        }

        public void Remove(Func<T, bool> selector)
        {
            lock (list)
            {
                for (int i = list.Count - 1; i >= 0; i--) if (selector(list[i])) list.RemoveAt(i);
            }
        }

        public void Set(List<T> list)
        {
            lock (this.list)
            {
                this.list = list;
            }
        }
    }
}