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
using System.Security;

using FFramework.LowLevelInput.PInvoke.Types;

namespace FFramework.LowLevelInput.PInvoke.Libraries
{
    [SuppressUnmanagedCodeSecurity]
    internal static class User32
    {
        public delegate IntPtr CallNextHookExDelegate(IntPtr hHook, int nCode, IntPtr wParam, IntPtr lParam);

        public delegate int GetMessageDelegate(ref Message lpMessage, IntPtr hwnd, uint msgFilterMin,
            uint msgFilterMax);

        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        public delegate int PostThreadMessageDelegate(uint threadId, uint msg, IntPtr wParam, IntPtr lParam);

        public delegate IntPtr SetWindowsHookExDelegate(int type, IntPtr hookProcedure, IntPtr hModule, uint threadId);

        public delegate int UnhookWindowsHookExDelegate(IntPtr hHook);

        public static CallNextHookExDelegate CallNextHookEx =
            WinApi.GetMethod<CallNextHookExDelegate>("user32.dll", "CallNextHookEx");

        public static GetMessageDelegate GetMessage = WinApi.GetMethod<GetMessageDelegate>("user32.dll", "GetMessageW");

        public static PostThreadMessageDelegate PostThreadMessage =
            WinApi.GetMethod<PostThreadMessageDelegate>("user32.dll", "PostThreadMessageW");

        public static SetWindowsHookExDelegate SetWindowsHookEx =
            WinApi.GetMethod<SetWindowsHookExDelegate>("user32.dll", "SetWindowsHookExW");

        public static UnhookWindowsHookExDelegate UnhookWindowsHookEx =
            WinApi.GetMethod<UnhookWindowsHookExDelegate>("user32.dll", "UnhookWindowsHookEx");
    }
}