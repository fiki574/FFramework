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
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

namespace FFramework.IO.Memory
{
    public class KeyboardHooking
    {
        [StructLayout(LayoutKind.Sequential)]
        public class KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public KBDLLHOOKSTRUCTFlags flags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }

        [Flags]
        public enum KBDLLHOOKSTRUCTFlags : uint
        {
            LLKHF_NONE = 0x00,
            LLKHF_EXTENDED = 0x01,
            LLKHF_INJECTED = 0x10,
            LLKHF_ALTDOWN = 0x20,
            LLKHF_UP = 0x80
        }

        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);

        private IntPtr HookID = IntPtr.Zero, Window = IntPtr.Zero;
        private Process Proc = null;

        public KeyboardHooking(string process, string window, LowLevelKeyboardProc hook)
        {
            Proc = GetProcess(process);
            Window = FindWindow(null, window);
            if (Proc == null || Window == null)
            {
                while (Proc == null || Window == null)
                {
                    Proc = GetProcess(process);
                    Window = FindWindow(null, window);
                    Thread.Sleep(1000);
                }
            }
            HookID = SetHook(hook);
        }

        public void SendKey(Keys key, int delay = 0)
        {
            SendMessage(Window, 0x0100, (IntPtr)key, CreateLParam(key, KBDLLHOOKSTRUCTFlags.LLKHF_NONE));
            Thread.Sleep(delay);
            SendMessage(Window, 0x0101, (IntPtr)key, CreateLParam(key, KBDLLHOOKSTRUCTFlags.LLKHF_UP));
        }

        public void Unhook()
        {
            UnhookWindowsHookEx(HookID);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            return SetWindowsHookEx(13, proc, GetModuleHandle(Proc.MainModule.ModuleName), 0);
        }

        private Process GetProcess(string process_name)
        {
            Process[] pname = Process.GetProcessesByName(process_name);
            if (pname.Length == 0)
                return null;
            else
                return pname[0];
        }

        private IntPtr CreateLParam(Keys key, KBDLLHOOKSTRUCTFlags flag)
        {
            KBDLLHOOKSTRUCT lParam = new KBDLLHOOKSTRUCT()
            {
                vkCode = (uint)key,
                scanCode = MapVirtualKey((uint)key, 0),
                flags = flag,
                time = (uint)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                dwExtraInfo = UIntPtr.Zero
            };
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(lParam));
            Marshal.StructureToPtr(lParam, ptr, true);
            return ptr;
        }
    }
}