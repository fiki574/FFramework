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
using System.Threading;

namespace FFramework.Threading
{
    public class ProcessInformation
    {
        public ThreadExecutor Executor;
        public int ProcessCount;
        public ThreadExecutor.OnProcessStartDelegate OnStart;
        public ThreadExecutor.OnProcessExecutedDelegate OnExecuted;
        public ThreadExecutor.OnProcessEndDelegate OnEnd;
    }

    public class ThreadExecutor
    {
        public delegate void ProcessDelegate(ThreadExecutor Executor);
        public delegate void OnProcessStartDelegate();
        public delegate void OnProcessExecutedDelegate();
        public delegate void OnProcessEndDelegate();
        static public ThreadExecutor MainExecutor;
        static public int WaitTimeMS = 50; 
        static public List<ThreadExecutor> Executors = new List<ThreadExecutor>();

        static public void Start(int StartThreadCount, int WaitTimeMS)
        {
            MainExecutor = new ThreadExecutor(-1);
            ThreadExecutor.WaitTimeMS = WaitTimeMS;
            for (int i = 0; i < StartThreadCount; ++i) CreateThread();
        }

        static public ThreadExecutor CreateThread()
        {
            ThreadExecutor Executor = new ThreadExecutor(Executors.Count);
            Executors.Add(Executor);
            return Executor;
        }

        static public ThreadExecutor GetThread(bool Create = false)
        {
            ThreadExecutor Selected = null;
            lock (Executors)
            {
                if (Create == false)
                {
                    foreach (ThreadExecutor Executor in Executors)
                    {
                        if (Executor.Processing)
                            continue;
                        Selected = Executor;
                        break;
                    }
                }
                else
                    Selected = CreateThread();
            }
            return Selected;
        }

        public int ThreadID = 0;
        public bool IsRunning = true;
        public Thread CurrentThread;
        public ThreadStart ExecutingThread;
        public bool Processing = false;
        private ProcessDelegate ProcessEvent;

        public ThreadExecutor(int ThreadID)
        {
            this.ThreadID = ThreadID;
            ExecutingThread = new ThreadStart(Process);
            CurrentThread = new Thread(ExecutingThread);
            CurrentThread.Start();
        }

        public void Process()
        {
            while (IsRunning)
            {
                if (ProcessEvent != null)
                {
                    Processing = true;
                    long StartTime = GetTimeStampMS();
                    try
                    {
                        ProcessEvent.Invoke(this);
                    }
                    catch 
                    {
                        return;
                    }

                    long Elapsed = GetTimeStampMS() - StartTime;
                    Processing = false;
                    ProcessEvent = null;
                    if (Elapsed < WaitTimeMS && IsRunning)
                        Thread.Sleep((int)(WaitTimeMS - Elapsed));
                }
                else if (IsRunning)
                    Thread.Sleep(WaitTimeMS);
            }
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public int GetTimeStamp()
        {
            return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public long GetTimeStampMS()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
    }
}
