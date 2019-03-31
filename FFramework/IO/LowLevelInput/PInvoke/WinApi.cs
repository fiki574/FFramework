/*
    MIT License

    Copyright (c) 2018 Michel

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
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;

namespace FFramework.LowLevelInput.PInvoke
{
    [SuppressUnmanagedCodeSecurity]
    internal static class WinApi
    {
        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern IntPtr InternalGetProcAddress(IntPtr hmodule, string procName);

        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryW", SetLastError = false, CharSet = CharSet.Unicode)]
        private static extern IntPtr InternalLoadLibraryW(string lpFileName);

        [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = false, CharSet = CharSet.Unicode)]
        private static extern IntPtr InternalGetModuleHandleW(string modulename);

        public static void ThrowWin32Exception(string message)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(),
                message + Environment.NewLine + "HResult: " + Marshal.GetHRForLastWin32Error());
        }

        public static IntPtr GetProcAddress(string modulename, string procname)
        {
            var hModule = InternalGetModuleHandleW(modulename);

            if (hModule == IntPtr.Zero)
            {
                hModule = InternalLoadLibraryW(modulename);

                if (hModule == IntPtr.Zero) ThrowWin32Exception("Failed to load \"" + modulename + "\".");
            }

            var result = InternalGetProcAddress(hModule, procname);

            if (result == IntPtr.Zero)
                ThrowWin32Exception("Failed to find exported symbol \"" + procname + "\" in \"" + modulename + "\".");

            return result;
        }

        public static T GetMethod<T>(string modulename, string procname)
        {
            var procAddress = GetProcAddress(modulename, procname);

            return (T) (object) Marshal.GetDelegateForFunctionPointer(procAddress, ObfuscatorNeedsThis<T>());
        }

        private static Type ObfuscatorNeedsThis<T>()
        {
            return typeof(T);
        }
    }
}