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
    public class ThreadSafeDictionary<T, R>
    {
        private Dictionary<T, R> dict = new Dictionary<T, R>();
        
        public ThreadSafeDictionary()
        {
        }

        public ThreadSafeDictionary(Dictionary<T, R> dict)
        {
            this.dict = dict;
        }

        public int Length
        {
            get
            {
                return dict.Count;
            }
        }

        public Dictionary<T, R> Select(Func<T, R> selector)
        {
            Dictionary<T, R> nDict = new Dictionary<T, R>();
            lock (dict)
            {
                foreach (KeyValuePair<T, R> item in dict) nDict.Add(item.Key, item.Value);
            }
            return nDict;
        }

        public Dictionary<T, R> Where(Func<T, bool> selector)
        {
            Dictionary<T, R> nDict = new Dictionary<T, R>();
            lock (dict)
            {
                foreach (KeyValuePair<T, R> item in dict) nDict.Add(item.Key, item.Value);
            }
            return nDict;
        }

        public void ForEach(Action<T> feachAction)
        {
            lock (dict)
            {
                foreach (KeyValuePair<T, R> item in dict) feachAction(item.Key);
            }
        }

        public bool Contains(T item)
        {
            lock (dict)
            {
                foreach (KeyValuePair<T, R> i in dict) if (i.Equals(item)) return true;
                return false;
            }
        }

        public void Add(T key, R value)
        {
            lock (dict)
            {
                dict.Add(key, value);
            }
        }

        public void Remove(T key)
        {
            lock (dict)
            {
                dict.Remove(key);
            }
        }

        public void Set(Dictionary<T, R> dict)
        {
            lock (this.dict)
            {
                this.dict = dict;
            }
        }
    }
}
