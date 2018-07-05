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
    
    Credits: https://github.com/usertoroot
*/

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using FFramework.Threading;

namespace FFramework.Memory
{
    public class BufferManager : Singleton<BufferManager>
    {
        List<byte[]> bufferBlocks;
        ConcurrentQueue<BufferBlock> freeBlocks;
        int bufferSize, bufferCount, blockSize;
        AutoResetEvent waiter = new AutoResetEvent(false);
        public int MaxWaitTime = 10;

        public int TotalAllocatedMemory
        {
            get
            {
                return bufferBlocks.Count * bufferSize;
            }
        }

        public int FreeMemory
        {
            get
            {
                return freeBlocks.Count * blockSize;
            }
        }

        public void Init(int bufferSize, int bufferCount, int blockSize)
        {
            if (bufferBlocks == null)
            {
                this.bufferSize = bufferSize;
                this.bufferCount = bufferCount;
                this.blockSize = blockSize;
                bufferBlocks = new List<byte[]>();
                freeBlocks = new ConcurrentQueue<BufferBlock>();
                for (int i = 0; i < bufferCount; i++) ExtendBuffer();
            }
            else
                return;
        }

        void ExtendBuffer()
        {
            lock (bufferBlocks)
            {
                byte[] buffer = new byte[bufferSize];
                bufferBlocks.Add(buffer);
                int blocks = bufferSize / blockSize;
                for (int j = 0; j < blocks; j++)
                {
                    BufferBlock block = new BufferBlock();
                    block.StartIndex = j * blockSize;
                    block.MaxLength = blockSize;
                    block.Buffer = buffer;
                    freeBlocks.Enqueue(block);
                    waiter.Set();
                }
            }
        }

        public BufferBlock RequestBufferBlock()
        {
            if (bufferBlocks == null) Init(0x800000, 4, 0x1000);
            BufferBlock block;
            while (!freeBlocks.TryDequeue(out block))
                if (!waiter.WaitOne(MaxWaitTime))
                    ExtendBuffer();
            block.inUse = true;
            return block;
        }

        internal void FreeBufferBlock(BufferBlock block)
        {
            freeBlocks.Enqueue(block);
            waiter.Set();
        }
    }
}
