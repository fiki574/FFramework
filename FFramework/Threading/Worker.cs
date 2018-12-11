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
using System.Threading;

namespace FFramework.Threading
{
    public class Worker<T>
    {
        public Worker(ConcurrentQueue<WorkTask<T>> tasks, int waitInterval)
        {
            TaskQueue = tasks;
            WaitInterval = waitInterval;
            Thread = new Thread(() =>
            {
                while (!Stopped)
                {
                    if (!TaskQueue.IsEmpty)
                    {
                        try
                        {
                            HandleWork();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error on Worker: " + ex.Message);
                        }
                    }
                    else
                        Thread.Sleep(waitInterval);
                }
                OnWorkerStopped?.Invoke(this);
            });
        }

        public delegate void OnWorkerCompletedDelegate(object obj);
        public delegate void OnWorkerExceptionDelegate(object obj, Exception ex);
        public delegate void OnWorkerStoppedDelegate(Worker<T> worker);
        public event OnWorkerCompletedDelegate OnWorkCompleted;
        public event OnWorkerStoppedDelegate OnWorkerStopped;
        public event OnWorkerExceptionDelegate OnWorkException;
        public bool Stopped { get; set; }
        public Thread Thread { get; private set; }
        public int WaitInterval { get; set; }
        private ConcurrentQueue<WorkTask<T>> TaskQueue { get; set; }

        public void HandleWork()
        {
            WorkTask<T> task;
            if (TaskQueue.TryDequeue(out task))
            {
                try
                {
                    task.Work(task.Data);
                }
                catch (Exception ex)
                {
                    OnWorkException?.Invoke(task.Data, ex);
                }
                OnWorkCompleted?.Invoke(task.Data);
            }
        }

        public void Start()
        {
            Thread.Start();
        }

        public void Stop()
        {
            Stopped = true;
        }
    }
}