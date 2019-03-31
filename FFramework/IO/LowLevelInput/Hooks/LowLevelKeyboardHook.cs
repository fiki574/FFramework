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
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using FFramework.LowLevelInput.PInvoke;
using FFramework.LowLevelInput.PInvoke.Types;
using FFramework.LowLevelInput.WindowsHooks;

namespace FFramework.LowLevelInput.Hooks
{
    public class LowLevelKeyboardHook : IDisposable
    {
        public delegate void KeyboardEventHandler(VirtualKeyCode key, KeyState state);

        private readonly object _lockObject;
        private WindowsHook _hook;

        public LowLevelKeyboardHook()
        {
            _lockObject = new object();
        }

        public LowLevelKeyboardHook(bool clearInjectedFlag)
        {
            _lockObject = new object();
            ClearInjectedFlag = clearInjectedFlag;
        }

        public bool ClearInjectedFlag { get; set; }

        /// <summary>
        ///     Occurs when [on keyboard event].
        /// </summary>
        public event KeyboardEventHandler OnKeyboardEvent;

        ~LowLevelKeyboardHook()
        {
            Dispose(false);
        }

        private void Global_OnProcessExit()
        {
            Dispose();
        }

        private void Global_OnUnhandledException()
        {
            Dispose();
        }

        private void Hook_OnHookCalled(IntPtr wParam, IntPtr lParam)
        {
            if (lParam == IntPtr.Zero) return;

            if (ClearInjectedFlag)
                HelperMethods.KbdClearInjectedFlag(lParam);

            if (OnKeyboardEvent == null) return;

            var msg = (WindowsMessage) (uint) wParam.ToInt32();

            var key = (VirtualKeyCode) Marshal.ReadInt32(lParam);

            switch (msg)
            {
                case WindowsMessage.Keydown:
                case WindowsMessage.Syskeydown:
                    InvokeEventListeners(KeyState.Down, key);
                    break;

                case WindowsMessage.Keyup:
                case WindowsMessage.Syskeyup:
                    InvokeEventListeners(KeyState.Up, key);
                    break;
            }
        }

        private void InvokeEventListeners(KeyState state, VirtualKeyCode key)
        {
            Task.Factory.StartNew(() => OnKeyboardEvent?.Invoke(key, state));
        }

        public bool InstallHook()
        {
            lock (_lockObject)
            {
                if (_hook != null) return false;

                _hook = new WindowsHook(WindowsHookType.LowLevelKeyboard);
            }

            _hook.OnHookCalled += Hook_OnHookCalled;

            if (!_hook.InstallHook())
                WinApi.ThrowWin32Exception("Unknown error while installing hook.");

            Global.OnProcessExit += Global_OnProcessExit;
            Global.OnUnhandledException += Global_OnUnhandledException;

            return true;
        }

        public bool UninstallHook()
        {
            lock (_lockObject)
            {
                if (_hook == null) return false;

                Global.OnProcessExit -= Global_OnProcessExit;
                Global.OnUnhandledException -= Global_OnUnhandledException;

                _hook.OnHookCalled -= Hook_OnHookCalled;

                _hook.UninstallHook();

                _hook.Dispose();

                _hook = null;

                return true;
            }
        }

        #region IDisposable Support

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

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