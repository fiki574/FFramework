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
