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
*/

using System;
using System.Collections.Generic;

namespace FFramework.Threading
{
    public class ThreadSafeDictionary<A, B>
    {
        private Dictionary<A, B> dict = new Dictionary<A, B>();

        public B this[A key]
        {
            get
            {
                lock (dict)
                    return dict[key];
            }
            set
            {
                lock (dict)
                    dict[key] = value;
            }
        }

        public B this[int index]
        {
            get
            {
                lock (dict)
                {
                    int current = 0;
                    foreach (KeyValuePair<A, B> pair in dict)
                    {
                        if (current == index)
                            return pair.Value;

                        current += 1;
                    }

                    return default(B);
                }
            }
            set
            {
                lock (dict)
                {
                    int current = 0;
                    foreach (KeyValuePair<A, B> pair in dict)
                    {
                        if (current == index)
                            dict[pair.Key] = value;

                        current += 1;
                    }
                }
            }
        }

        public int Length
        {
            get
            {
                lock (dict)
                    return dict.Count;
            }
        }

        public ThreadSafeDictionary()
        {
        }

        public ThreadSafeDictionary(Dictionary<A, B> dict)
        {
            this.dict = dict;
        }

        public void Add(A key, B value)
        {
            lock (dict)
                dict.Add(key, value);
        }

        public void Remove(A key)
        {
            lock (dict)
                dict.Remove(key);
        }

        public void Remove(B value)
        {
            lock (dict)
            {
                int count = 0, pos = 0;
                foreach (KeyValuePair<A, B> pair in dict)
                    if (pair.Value.Equals(value))
                        count += 1;

                A[] keys = new A[count];
                foreach (KeyValuePair<A, B> pair in dict)
                    if (pair.Value.Equals(value))
                        keys[pos++] = pair.Key;

                foreach (A key in keys)
                    dict.Remove(key);
            }
        }

        public Dictionary<A, B> WhereByKey(Func<A, bool> selector)
        {
            Dictionary<A, B> idict = new Dictionary<A, B>();
            lock (dict)
            {
                foreach (KeyValuePair<A, B> item in dict)
                    if (selector(item.Key))
                        idict.Add(item.Key, item.Value);
            }
            return idict;
        }

        public Dictionary<A, B> WhereByValue(Func<B, bool> selector)
        {
            Dictionary<A, B> idict = new Dictionary<A, B>();
            lock (dict)
            {
                foreach (KeyValuePair<A, B> item in dict)
                    if (selector(item.Value))
                        idict.Add(item.Key, item.Value);
            }
            return idict;
        }

        public KeyValuePair<A, B> FirstOrDefaultByKey(Func<A, bool> condition)
        {
            lock (dict)
            {
                foreach (KeyValuePair<A, B> item in dict)
                    if (condition(item.Key))
                        return item;
            }
            return default(KeyValuePair<A, B>);
        }

        public KeyValuePair<A, B> FirstOrDefaultByValue(Func<B, bool> condition)
        {
            lock (dict)
            {
                foreach (KeyValuePair<A, B> item in dict)
                    if (condition(item.Value))
                        return item;
            }
            return default(KeyValuePair<A, B>);
        }

        public A[] KeysToArray()
        {
            A[] a = null;
            lock (dict)
            {
                a = new A[dict.Count];
                dict.Keys.CopyTo(a, 0);
            }
            return a;
        }

        public B[] ValuesToArray()
        {
            B[] b = null;
            lock (dict)
            {
                b = new B[dict.Count];
                dict.Values.CopyTo(b, 0);
            }
            return b;
        }

        public void ForEachByKey(Action<A> feachAction)
        {
            lock (dict)
                foreach (A item in dict.Keys)
                    feachAction(item);
        }

        public void ForEachByValue(Action<B> feachAction)
        {
            lock (dict)
                foreach (B item in dict.Values)
                    feachAction(item);
        }

        public void ForEachByKeyValuePair(Action<KeyValuePair<A, B>> feachAction)
        {
            lock (dict)
                foreach (KeyValuePair<A, B> item in dict)
                    feachAction(item);
        }

        public int CountByKey(Func<A, bool> selector)
        {
            int i = 0;
            lock (dict)
            {
                foreach (A item in dict.Keys)
                    if (selector(item))
                        i++;
            }
            return i;
        }

        public int CountByValue(Func<B, bool> selector)
        {
            int i = 0;
            lock (dict)
            {
                foreach (B item in dict.Values)
                    if (selector(item))
                        i++;
            }
            return i;
        }

        public B GetValue(A key)
        {
            B value = default(B);
            lock (dict)
                dict.TryGetValue(key, out value);

            return value;
        }

        public bool ContainsKey(A key)
        {
            lock (dict)
                return dict.ContainsKey(key);
        }

        public bool ContainsValue(A key)
        {
            lock (dict)
                return dict.ContainsKey(key);
        }

        public void Clear()
        {
            lock (dict)
                dict.Clear();
        }

        public void Set(Dictionary<A, B> dict)
        {
            lock (this.dict)
                this.dict = dict;
        }

        public B ValueAt(int index)
        {
            lock (dict)
            {
                int current = 0;
                foreach (KeyValuePair<A, B> pair in dict)
                {
                    if (current == index)
                        return pair.Value;

                    current += 1;
                }
                return default(B);
            }
        }

        public A KeyAt(int index)
        {
            lock (dict)
            {
                int current = 0;
                foreach (KeyValuePair<A, B> pair in dict)
                {
                    if (current == index)
                        return pair.Key;

                    current += 1;
                }
                return default(A);
            }
        }

        public KeyValuePair<A, B> KeyValuePairAt(int index)
        {
            lock (dict)
            {
                int current = 0;
                foreach (KeyValuePair<A, B> pair in dict)
                {
                    if (current == index)
                        return pair;

                    current += 1;
                }
                return default(KeyValuePair<A, B>);
            }
        }
    }
}
