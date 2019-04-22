/*
    MIT License

    Copyright (c) 2019 Michel
    Copyright (c) 2018 Lucas Trzesniewski

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using FFramework.IO.Memory;
using InlineIL;
using static InlineIL.IL.Emit;

namespace FFramework.IO.ProcessMemoryWrapper
{
    public static unsafe class ProcessWrapper
    {
        public const ProcessAccessFlags ProcessReadAccess = ProcessAccessFlags.VirtualMemoryRead;
        public const ProcessAccessFlags ProcessWriteAccess = ProcessAccessFlags.VirtualMemoryOperation | ProcessAccessFlags.VirtualMemoryWrite;
        public const ProcessAccessFlags ProcessReadWriteAccess = ProcessReadAccess | ProcessWriteAccess;
        public const ProcessAccessFlags ProcessInformationAccess = ProcessAccessFlags.QueryInformation | ProcessAccessFlags.QueryLimitedInformation;
        public const ProcessAccessFlags ProcessAllocateAccess = ProcessAccessFlags.VirtualMemoryOperation;
        public const ProcessAccessFlags ProcessExecuteAccess = ProcessReadWriteAccess | ProcessInformationAccess | ProcessAccessFlags.CreateThread;

        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procname);

        [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr GetModuleHandle(string modulename);

        private static readonly void* _ntOpenProcess;
        private static readonly void* _ntClose;

        private static readonly void* _ntReadVirtualMemory;
        private static readonly void* _ntWriteVirtualMemory;

        static ProcessWrapper()
        {
            var ntdll = GetModuleHandle("ntdll.dll");

            _ntOpenProcess = GetProcAddress(ntdll, "NtOpenProcess").ToPointer();
            _ntClose = GetProcAddress(ntdll, "NtClose").ToPointer();

            _ntReadVirtualMemory = GetProcAddress(ntdll, "NtReadVirtualMemory").ToPointer();
            _ntWriteVirtualMemory = GetProcAddress(ntdll, "NtWriteVirtualMemory").ToPointer();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CloseProcess(IntPtr handle)
        {
            Ldarg(nameof(handle));

            Ldsfld(new FieldRef(typeof(ProcessWrapper), nameof(_ntClose)));
            Calli(new StandAloneMethodSig(CallingConvention.StdCall, typeof(uint), typeof(IntPtr)));

            Ldc_I4_0();
            Ceq();

            return IL.Return<bool>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr OpenProcess(ProcessAccessFlags desiredAccess, int processId)
        {
            IL.DeclareLocals(true,
                new LocalVar("handle", typeof(IntPtr)),
                new LocalVar("objectAttributes", typeof(ObjectAttributes)),
                new LocalVar("clientID", typeof(ClientID)));
            
            Ldloca("handle");
            Initobj(typeof(IntPtr));
            Ldloca("objectAttributes");
            Initobj(typeof(ObjectAttributes));
            Ldloca("clientID");
            Initobj(typeof(ClientID));
            
            Ldloca("clientID");
            Ldarg(nameof(processId));
            Conv_U();
            Stfld(new FieldRef(typeof(ClientID), "UniqueProcess"));
            
            Ldloca("objectAttributes");
            Sizeof(typeof(ObjectAttributes));
            Stfld(new FieldRef(typeof(ObjectAttributes), "Length"));
            
            Ldloca("handle");
            Ldarg(nameof(desiredAccess));
            Ldloca("objectAttributes");
            Ldloca("clientID");
            
            Ldsfld(new FieldRef(typeof(ProcessWrapper), nameof(_ntOpenProcess)));
            Calli(new StandAloneMethodSig(CallingConvention.StdCall, typeof(uint), typeof(IntPtr), typeof(uint), typeof(IntPtr), typeof(IntPtr)));
            
            Pop();
            
            Ldloc("handle");
            return IL.Return<IntPtr>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadProcessMemory(IntPtr handle, IntPtr baseAddress, IntPtr buffer, IntPtr size)
        {
            Ldarg(nameof(handle));
            Ldarg(nameof(baseAddress));
            Ldarg(nameof(buffer));
            Ldarg(nameof(size));
            Ldc_I4_0();
            
            Ldsfld(new FieldRef(typeof(ProcessWrapper), nameof(_ntReadVirtualMemory)));
            Calli(new StandAloneMethodSig(CallingConvention.StdCall, typeof(int), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr)));

            Ldc_I4_0();
            Ceq();

            return IL.Return<bool>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadProcessMemory(IntPtr handle, IntPtr baseAddress, IntPtr buffer, IntPtr size, ref IntPtr numberOfBytesRead)
        {
            Ldarg(nameof(handle));
            Ldarg(nameof(baseAddress));
            Ldarg(nameof(buffer));
            Ldarg(nameof(size));
            Ldarg(nameof(numberOfBytesRead));
            
            Ldsfld(new FieldRef(typeof(ProcessWrapper), nameof(_ntReadVirtualMemory)));
            Calli(new StandAloneMethodSig(CallingConvention.StdCall, typeof(int), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr)));
            
            Ldc_I4_0();
            Ceq();

            return IL.Return<bool>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ReadProcessMemory(IntPtr handle, IntPtr baseAddress, int size)
        {
            IL.DeclareLocals(true, new LocalVar("buffer", typeof(byte[])));
            
            Ldarg(nameof(size));
            Newarr(typeof(byte));
            Stloc("buffer");
            
            Ldarg(nameof(handle));
            Ldarg(nameof(baseAddress));
            
            Ldloc("buffer");
            Ldc_I4_0();
            Ldelema(typeof(byte));

            Ldarg(nameof(size));
            Conv_I();

            Ldc_I4_0();
            
            Ldsfld(new FieldRef(typeof(ProcessWrapper), nameof(_ntReadVirtualMemory)));
            Calli(new StandAloneMethodSig(CallingConvention.StdCall, typeof(int), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr)));
            
            Pop();

            Ldloc("buffer");
            return IL.Return<byte[]>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ReadProcessMemory(IntPtr handle, IntPtr baseAddress, int size, ref IntPtr numberOfBytesRead)
        {
            IL.DeclareLocals(true, new LocalVar("buffer", typeof(byte[])));
            
            Ldarg(nameof(size));
            Newarr(typeof(byte));
            Stloc("buffer");
            
            Ldarg(nameof(handle));
            Ldarg(nameof(baseAddress));
            
            Ldloc("buffer");
            Ldc_I4_0();
            Ldelema(typeof(byte));

            Ldarg(nameof(size));
            Conv_I();

            Ldarg(nameof(numberOfBytesRead));
            
            Ldsfld(new FieldRef(typeof(ProcessWrapper), nameof(_ntReadVirtualMemory)));
            Calli(new StandAloneMethodSig(CallingConvention.StdCall, typeof(int), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr)));
            
            Pop();

            Ldloc("buffer");
            return IL.Return<byte[]>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ReadProcessMemory<T>(IntPtr handle, IntPtr baseAddress)
        {
            IL.DeclareLocals(true,
                new LocalVar("buffer", typeof(T)));

            Ldarg(nameof(handle));
            Ldarg(nameof(baseAddress));
            Ldloca("buffer");

            Sizeof(typeof(T));
            Conv_I();

            Ldc_I4_0();

            Ldsfld(new FieldRef(typeof(ProcessWrapper), nameof(_ntReadVirtualMemory)));
            Calli(new StandAloneMethodSig(CallingConvention.StdCall, typeof(int), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr)));

            Pop();

            Ldloc("buffer");
            return IL.Return<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ReadProcessMemory<T>(IntPtr handle, IntPtr baseAddress, ref IntPtr numberOfBytesRead)
        {
            IL.DeclareLocals(true,
                new LocalVar("buffer", typeof(T)));

            Ldarg(nameof(handle));
            Ldarg(nameof(baseAddress));
            Ldloca("buffer");

            Sizeof(typeof(T));
            Conv_I();

            Ldarg(nameof(numberOfBytesRead));

            Ldsfld(new FieldRef(typeof(ProcessWrapper), nameof(_ntReadVirtualMemory)));
            Calli(new StandAloneMethodSig(CallingConvention.StdCall, typeof(int), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr)));

            Pop();

            Ldloc("buffer");

            return IL.Return<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ReadProcessMemory<T>(IntPtr handle, IntPtr baseAddress, int size)
        {
            IL.DeclareLocals(true, new LocalVar("buffer", typeof(T[])));
            
            Ldarg(nameof(size));
            Newarr(typeof(T));
            Stloc("buffer");
            
            Ldarg(nameof(handle));
            Ldarg(nameof(baseAddress));
            
            Ldloc("buffer");
            Ldc_I4_0();
            Ldelema(typeof(T));

            Ldarg(nameof(size));
            Sizeof(typeof(T));
            Mul();
            Conv_I();

            Ldc_I4_0();
            
            Ldsfld(new FieldRef(typeof(ProcessWrapper), nameof(_ntReadVirtualMemory)));
            Calli(new StandAloneMethodSig(CallingConvention.StdCall, typeof(int), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr)));
            
            Pop();

            Ldloc("buffer");
            return IL.Return<T[]>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ReadProcessMemory<T>(IntPtr handle, IntPtr baseAddress, int size, ref IntPtr numberOfBytesRead)
        {
            IL.DeclareLocals(true, new LocalVar("buffer", typeof(T[])));
            
            Ldarg(nameof(size));
            Newarr(typeof(T));
            Stloc("buffer");
            
            Ldarg(nameof(handle));
            Ldarg(nameof(baseAddress));
            
            Ldloc("buffer");
            Ldc_I4_0();
            Ldelema(typeof(T));

            Ldarg(nameof(size));
            Sizeof(typeof(T));
            Mul();
            Conv_I();

            Ldarg(nameof(numberOfBytesRead));
            
            Ldsfld(new FieldRef(typeof(ProcessWrapper), nameof(_ntReadVirtualMemory)));
            Calli(new StandAloneMethodSig(CallingConvention.StdCall, typeof(int), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr)));
            
            Pop();

            Ldloc("buffer");
            return IL.Return<T[]>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool WriteProcessMemory(IntPtr handle, IntPtr baseAddress, IntPtr buffer, IntPtr size)
        {
            Ldarg(nameof(handle));
            Ldarg(nameof(baseAddress));
            Ldarg(nameof(buffer));
            Ldarg(nameof(size));
            Ldc_I4_0();

            Ldsfld(new FieldRef(typeof(ProcessWrapper), nameof(_ntWriteVirtualMemory)));
            Calli(new StandAloneMethodSig(CallingConvention.StdCall, typeof(int), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr)));

            Ldc_I4_0();
            Ceq();

            return IL.Return<bool>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool WriteProcessMemory(IntPtr handle, IntPtr baseAddress, IntPtr buffer, IntPtr size, ref IntPtr numberOfBytesWritten)
        {
            Ldarg(nameof(handle));
            Ldarg(nameof(baseAddress));
            Ldarg(nameof(buffer));
            Ldarg(nameof(size));
            Ldarg(nameof(numberOfBytesWritten));

            Ldsfld(new FieldRef(typeof(ProcessWrapper), nameof(_ntWriteVirtualMemory)));
            Calli(new StandAloneMethodSig(CallingConvention.StdCall, typeof(int), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr)));

            Ldc_I4_0();
            Ceq();

            return IL.Return<bool>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool WriteProcessMemory(IntPtr handle, IntPtr baseAddress, byte[] buffer)
        {
            Ldarg(nameof(handle));
            Ldarg(nameof(baseAddress));

            Ldarg(nameof(buffer));
            Ldc_I4_0();
            Ldelema(typeof(byte));

            Ldarg(nameof(buffer));
            Ldlen();
            Conv_I();

            Ldc_I4_0();

            Ldsfld(new FieldRef(typeof(ProcessWrapper), nameof(_ntWriteVirtualMemory)));
            Calli(new StandAloneMethodSig(CallingConvention.StdCall, typeof(int), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr)));

            Ldc_I4_0();
            Ceq();

            return IL.Return<bool>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool WriteProcessMemory(IntPtr handle, IntPtr baseAddress, byte[] buffer, ref IntPtr numberOfBytesWritten)
        {
            Ldarg(nameof(handle));
            Ldarg(nameof(baseAddress));

            Ldarg(nameof(buffer));
            Ldc_I4_0();
            Ldelema(typeof(byte));

            Ldarg(nameof(buffer));
            Ldlen();
            Conv_I();

            Ldarg(nameof(numberOfBytesWritten));

            Ldsfld(new FieldRef(typeof(ProcessWrapper), nameof(_ntWriteVirtualMemory)));
            Calli(new StandAloneMethodSig(CallingConvention.StdCall, typeof(int), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr)));

            Ldc_I4_0();
            Ceq();

            return IL.Return<bool>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool WriteProcessMemory<T>(IntPtr handle, IntPtr baseAddress, T buffer)
        {
            Ldarg(nameof(handle));
            Ldarg(nameof(baseAddress));
            Ldarga(nameof(buffer));

            Sizeof(typeof(T));
            Conv_I();

            Ldc_I4_0();

            Ldsfld(new FieldRef(typeof(ProcessWrapper), nameof(_ntWriteVirtualMemory)));
            Calli(new StandAloneMethodSig(CallingConvention.StdCall, typeof(int), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr)));

            Ldc_I4_0();
            Ceq();

            return IL.Return<bool>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool WriteProcessMemory<T>(IntPtr handle, IntPtr baseAddress, T buffer, ref IntPtr numberOfBytesWritten)
        {
            Ldarg(nameof(handle));
            Ldarg(nameof(baseAddress));
            Ldarga(nameof(buffer));

            Sizeof(typeof(T));
            Conv_I();

            Ldarg(nameof(numberOfBytesWritten));

            Ldsfld(new FieldRef(typeof(ProcessWrapper), nameof(_ntWriteVirtualMemory)));
            Calli(new StandAloneMethodSig(CallingConvention.StdCall, typeof(int), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr)));

            Ldc_I4_0();
            Ceq();

            return IL.Return<bool>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool WriteProcessMemory<T>(IntPtr handle, IntPtr baseAddress, T[] buffer)
        {
            Ldarg(nameof(handle));
            Ldarg(nameof(baseAddress));

            Ldarg(nameof(buffer));
            Ldc_I4_0();
            Ldelema(typeof(T));

            Ldarg(nameof(buffer));
            Ldlen();
            Sizeof(typeof(T));
            Mul();
            Conv_I();

            Ldc_I4_0();

            Ldsfld(new FieldRef(typeof(ProcessWrapper), nameof(_ntWriteVirtualMemory)));
            Calli(new StandAloneMethodSig(CallingConvention.StdCall, typeof(int), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr)));

            Ldc_I4_0();
            Ceq();

            return IL.Return<bool>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool WriteProcessMemory<T>(IntPtr handle, IntPtr baseAddress, T[] buffer, ref IntPtr numberOfBytesWritten)
        {
            Ldarg(nameof(handle));
            Ldarg(nameof(baseAddress));

            Ldarg(nameof(buffer));
            Ldc_I4_0();
            Ldelema(typeof(T));

            Ldarg(nameof(buffer));
            Ldlen();
            Sizeof(typeof(T));
            Mul();
            Conv_I();

            Ldarg(nameof(numberOfBytesWritten));

            Ldsfld(new FieldRef(typeof(ProcessWrapper), nameof(_ntWriteVirtualMemory)));
            Calli(new StandAloneMethodSig(CallingConvention.StdCall, typeof(int), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr)));

            Ldc_I4_0();
            Ceq();

            return IL.Return<bool>();
        }
    }
}