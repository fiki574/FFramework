/*
    MIT License

    Copyright (c) 2016-2019 michel-pi
    Copyright (c) 2010-2014 SharpDX - Alexandre Mutel

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
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace FFramework.Overlay.PInvoke
{
    internal static class DynamicImport
    {
        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procname);

        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryW", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr GetModuleHandle(string modulename);

        public static IntPtr ImportLibrary(string libraryName)
        {
            if (libraryName == string.Empty) throw new ArgumentOutOfRangeException(nameof(libraryName));

            IntPtr hModule = GetModuleHandle(libraryName);

            if (hModule == IntPtr.Zero)
            {
                hModule = LoadLibrary(libraryName);
            }

            if (hModule == IntPtr.Zero)
            {
                throw new DynamicImportException("DynamicImport failed to import library \"" + libraryName + "\"!");
            }
            else
            {
                return hModule;
            }
        }

        public static IntPtr ImportMethod(IntPtr moduleHandle, string methodName)
        {
            if (moduleHandle == IntPtr.Zero) throw new ArgumentOutOfRangeException(nameof(moduleHandle));
            if (string.IsNullOrEmpty(methodName)) throw new ArgumentOutOfRangeException(nameof(methodName));

            IntPtr procAddress = GetProcAddress(moduleHandle, methodName);

            if (procAddress == IntPtr.Zero)
            {
                throw new DynamicImportException("DynamicImport failed to find method \"" + methodName + "\" in module \"0x" + moduleHandle.ToString("X") + "\"!");
            }
            else
            {
                return procAddress;
            }
        }

        public static IntPtr ImportMethod(string libraryName, string methodName)
        {
            return ImportMethod(ImportLibrary(libraryName), methodName);
        }

        public static T Import<T>(IntPtr moduleHandle, string methodName)
        {
            var address = ImportMethod(moduleHandle, methodName);

            return (T)(object)Marshal.GetDelegateForFunctionPointer(address, typeof(T));
        }

        public static T Import<T>(string libraryName, string methodName)
        {
            var address = ImportMethod(libraryName, methodName);

            return (T)(object)Marshal.GetDelegateForFunctionPointer(address, typeof(T));
        }
    }

    internal class DynamicImportException : Win32Exception
    {
        public DynamicImportException()
        {
        }

        public DynamicImportException(int error) : base(error)
        {
        }

        public DynamicImportException(string message) : base(message + Environment.NewLine + "ErrorCode: " + Marshal.GetLastWin32Error())
        {
        }

        public DynamicImportException(int error, string message) : base(error, message)
        {
        }

        public DynamicImportException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DynamicImportException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}