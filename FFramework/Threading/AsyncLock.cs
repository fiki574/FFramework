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
using System.Threading;
using System.Threading.Tasks;

namespace FFramework.Threading
{
    public class AsyncLock
    {
        private readonly AsyncSemaphore Semaphore;
        private readonly Task<Releaser> TReleaser;

        public AsyncLock()
        {
            Semaphore = new AsyncSemaphore(1);
            TReleaser = Task.FromResult(new Releaser(this));
        }

        public Task<Releaser> LockAsync()
        {
            var wait = Semaphore.WaitAsync();
            return wait.IsCompleted ? TReleaser : wait.ContinueWith((_, state) => new Releaser((AsyncLock)state), this, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        public struct Releaser : IDisposable
        {
            private readonly AsyncLock ToRelease;

            internal Releaser(AsyncLock toRelease)
            {
                ToRelease = toRelease;
            }

            public void Dispose()
            {
                if (ToRelease != null)
                    ToRelease.Semaphore.Release();
            }
        }
    }
}