using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FFramework.FPatch
{
    public static class Kernel32
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

        public static IntPtr OpenProcess(Process process, ProcessAccessFlags flags)
        {
            IntPtr handle = OpenProcess(flags, false, process.Id);
            if (handle == IntPtr.Zero) throw new Exception(String.Format("Failed to open process ({0}).", Kernel32.GetLastError()));
            return handle;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);

        public static int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer)
        {
            IntPtr bytesWritten;
            int result = WriteProcessMemory(hProcess, lpBaseAddress, lpBuffer, lpBuffer.Length, out bytesWritten);
            if (result == 0) throw new Exception(String.Format("Failed to write to process ({0}).", Kernel32.GetLastError()));
            return result;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

        public static int ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer)
        {
            IntPtr bytesRead;
            int result = ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, lpBuffer.Length, out bytesRead);
            if (result == 0) throw new Exception("Failed to read from process.");
            return result;
        }

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        [DllImport("kernel32.dll")]
        private static extern int VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        public static uint VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, int size, uint newProtect)
        {
            uint oldProtect;
            int result = VirtualProtectEx(hProcess, lpAddress, new IntPtr(size), newProtect, out oldProtect);
            if (result == 0) throw new Exception(String.Format("Failed to chance access ({0}).", Kernel32.GetLastError()));
            return oldProtect;
        }
    }
}