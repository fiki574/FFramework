using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFramework.FUtilities
{
    ///<summary>Used in exact same way as normal lists, though this kind of lists are thread-safe, plus have some more functionalities than the default ones</summary>
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

        //TODO: Update() function
    }
}