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
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FFramework.Threading
{
    public class WorkerPool<T>
    {
        public WorkerPool(int workerCount, int waitInterval)
        {
            WaitTaskInterval = waitInterval;
            AdjustWorkerCount(workerCount);
        }

        public int MaxThreads { get; private set; }
        public int WaitTaskInterval { get; set; } = 100;
        public ConcurrentQueue<WorkTask<T>> WorkQueue { get; private set; } = new ConcurrentQueue<WorkTask<T>>();
        private List<Worker<T>> Workers { get; set; } = new List<Worker<T>>();

        public void QueueWork(T obj, Action<T> work)
        {
            WorkQueue.Enqueue(new WorkTask<T>()
            {
                Data = obj,
                Work = work
            });
        }

        public void SetWorkerCount(int newCap)
        {
            AdjustWorkerCount(newCap);
        }

        public void SetWorkerInterval(int newInterval)
        {
            WaitTaskInterval = newInterval;
            lock (Workers)
            {
                foreach (Worker<T> w in Workers)
                    w.WaitInterval = newInterval;
            }
        }

        private void AdjustWorkerCount(int newCap)
        {
            lock (Workers)
            {
                if (newCap < Workers.Count)
                    for (int i = 0; i < (Workers.Count - newCap); i++)
                        Workers[i].Stop();
                else
                {
                    List<Worker<T>> newWorkers = new List<Worker<T>>();
                    for (int i = 0; i < (newCap - Workers.Count); i++)
                    {
                        Worker<T> w = new Worker<T>(WorkQueue, WaitTaskInterval);
                        w.OnWorkCompleted += HandleWorkerCompleted;
                        w.OnWorkerStopped += HandleWorkerStopped;
                        w.OnWorkException += HandleWorkerException;
                        newWorkers.Add(w);
                        Workers.Add(w);
                    }

                    foreach (Worker<T> w in newWorkers)
                        w.Start();
                }
            }
        }

        private void HandleWorkerCompleted(object obj)
        {
        }

        private void HandleWorkerException(object obj, Exception ex)
        {
        }

        private void HandleWorkerStopped(Worker<T> w)
        {
            lock (Workers)
            {
                Workers.Remove(w);
            }
        }
    }
}