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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FFramework_Core.Threading
{
    public class AsyncSemaphore
    {
        private static readonly Task Completed = Task.FromResult(true);
        private readonly Queue<TaskCompletionSource<bool>> Waiters = new Queue<TaskCompletionSource<bool>>();
        private int CurrentCount;

        public AsyncSemaphore(int initialCount)
        {
            if (initialCount < 0)
                throw new ArgumentOutOfRangeException("initialCount");

            CurrentCount = initialCount;
        }

        public Task WaitAsync()
        {
            lock (Waiters)
            {
                if (CurrentCount > 0)
                {
                    --CurrentCount;
                    return Completed;
                }

                var waiter = new TaskCompletionSource<bool>();
                Waiters.Enqueue(waiter);
                return waiter.Task;
            }
        }

        public void Release()
        {
            TaskCompletionSource<bool> toRelease = null;
            lock (Waiters)
            {
                if (Waiters.Count > 0)
                    toRelease = Waiters.Dequeue();
                else
                    ++CurrentCount;
            }
            if (toRelease != null)
                toRelease.SetResult(true);
        }
    }
}