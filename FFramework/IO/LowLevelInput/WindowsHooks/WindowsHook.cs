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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

using FFramework.LowLevelInput.PInvoke;
using FFramework.LowLevelInput.PInvoke.Libraries;
using FFramework.LowLevelInput.PInvoke.Types;

namespace FFramework.LowLevelInput.WindowsHooks
{
    public class WindowsHook : IDisposable
    {
        public delegate void HookCalledEventHandler(IntPtr wParam, IntPtr lParam);

        private static readonly IntPtr MainModuleHandle = Process.GetCurrentProcess().MainModule.BaseAddress;
        private readonly object _lockObject;

        private IntPtr _hookHandler;
        private User32.HookProc _hookProc;
        private Thread _hookThread;
        private uint _hookThreadId;

        // ReSharper disable once UnusedMember.Local
        private WindowsHook()
        {
            _lockObject = new object();
        }

        public WindowsHook(WindowsHookType windowsHookType)
        {
            _lockObject = new object();
            WindowsHookType = windowsHookType;
        }

        public WindowsHookType WindowsHookType { get; }

        ~WindowsHook()
        {
            Dispose(false);
        }

        public event HookCalledEventHandler OnHookCalled;

        private IntPtr HookProcedure(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode == 0)
            {
                if (WindowsHookFilter.InternalFilterEventsHelper(wParam, lParam))
                {
                    return (IntPtr)(-1);
                }
                else
                {
                    OnHookCalled?.Invoke(wParam, lParam);
                }
            }

            return User32.CallNextHookEx(_hookHandler, nCode, wParam, lParam);
        }

        public bool InstallHook()
        {
            lock (_lockObject)
            {
                if (_hookHandler != IntPtr.Zero) return false;
                if (_hookThreadId != 0) return false;

                _hookThread = new Thread(InitializeHookThread)
                {
                    IsBackground = true
                };

                _hookThread.Start();
            }

            while (_hookThreadId == 0) Thread.Sleep(10);

            if (_hookHandler == IntPtr.Zero)
                WinApi.ThrowWin32Exception("Failed to \"SetWindowsHookEx\"");

            return true;
        }

        public bool UninstallHook()
        {
            lock (_lockObject)
            {
                if (_hookHandler == IntPtr.Zero) return false;
                if (_hookThreadId == 0) return false;

                if (User32.PostThreadMessage(_hookThreadId, (uint) WindowsMessage.Quit, IntPtr.Zero, IntPtr.Zero) != 0)
                    try
                    {
                        _hookThread.Join();
                    }
                    catch
                    {
                        // ignored
                    }

                _hookHandler = IntPtr.Zero;
                _hookThreadId = 0;
                _hookThread = null;

                return true;
            }
        }

        private void InitializeHookThread()
        {
            lock (_lockObject)
            {
                _hookProc = HookProcedure;

                var methodPtr = Marshal.GetFunctionPointerForDelegate(_hookProc);

                _hookHandler = User32.SetWindowsHookEx((int) WindowsHookType, methodPtr, MainModuleHandle, 0);

                _hookThreadId = Kernel32.GetCurrentThreadId();
            }

            if (_hookHandler == IntPtr.Zero) return;

            var msg = new Message();

            while (User32.GetMessage(ref msg, IntPtr.Zero, 0, 0) != 0)
                if (msg.Msg == (uint) WindowsMessage.Quit) break;

            User32.UnhookWindowsHookEx(_hookHandler);
        }

        #region IDisposable Support

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing)
                OnHookCalled = null;

            UninstallHook();

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}