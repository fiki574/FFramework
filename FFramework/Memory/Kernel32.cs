﻿/*
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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace FFramework.Memory
{
    public static class Kernel32
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

        public static IntPtr OpenProcess(Process process, ProcessAccessFlags flags)
        {
            return OpenProcess(flags, false, process.Id);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);

        public static int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer)
        {
            IntPtr bytesWritten;
            return WriteProcessMemory(hProcess, lpBaseAddress, lpBuffer, lpBuffer.Length, out bytesWritten);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

        public static int ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer)
        {
            IntPtr bytesRead;
            return ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, lpBuffer.Length, out bytesRead);
        }

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        [DllImport("kernel32.dll")]
        private static extern int VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        public static uint VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, int size, uint newProtect)
        {
            uint oldProtect;
            VirtualProtectEx(hProcess, lpAddress, new IntPtr(size), newProtect, out oldProtect);
            return oldProtect;
        }

        public static int WriteString(IntPtr handle, IntPtr address, string str)
        {
            IntPtr written;
            byte[] data = Encoding.Default.GetBytes(str + "\0");
            return WriteProcessMemory(handle, address, data, data.Length, out written);
        }
    }
}