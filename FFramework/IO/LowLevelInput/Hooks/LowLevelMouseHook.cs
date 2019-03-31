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
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using FFramework.LowLevelInput.PInvoke;
using FFramework.LowLevelInput.PInvoke.Types;
using FFramework.LowLevelInput.WindowsHooks;

namespace FFramework.LowLevelInput.Hooks
{
    public class LowLevelMouseHook : IDisposable
    {
        public delegate void MouseEventHandler(VirtualKeyCode key, KeyState state, int x, int y);

        private readonly object _lockObject;
        private WindowsHook _hook;

        public LowLevelMouseHook()
        {
            _lockObject = new object();
            CaptureMouseMove = false;
            ClearInjectedFlag = false;
        }

        public LowLevelMouseHook(bool captureMouseMove)
        {
            _lockObject = new object();
            CaptureMouseMove = captureMouseMove;
            ClearInjectedFlag = false;
        }

        public LowLevelMouseHook(bool captureMouseMove, bool clearInjectedFlag)
        {
            _lockObject = new object();
            CaptureMouseMove = captureMouseMove;
            ClearInjectedFlag = clearInjectedFlag;
        }

        public bool CaptureMouseMove { get; set; }
        public bool ClearInjectedFlag { get; set; }
        public bool IsLeftMouseButtonPressed { get; private set; }
        public bool IsMiddleMouseButtonPressed { get; private set; }
        public bool IsRightMouseButtonPressed { get; private set; }
        public bool IsXButton1Pressed { get; private set; }
        public bool IsXButton2Pressed { get; private set; }
        public event MouseEventHandler OnMouseEvent;

        ~LowLevelMouseHook()
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

            IsMiddleMouseButtonPressed = false;

            var msg = (WindowsMessage) (uint) wParam.ToInt32();

            int x = Marshal.ReadInt32(lParam);
            int y = Marshal.ReadInt32(lParam, 4);

            int mouseData = Marshal.ReadInt32(lParam, 8);

            if (ClearInjectedFlag)
                Marshal.WriteInt32(lParam, 12, 0);

            switch (msg)
            {
                case WindowsMessage.Lbuttondblclk:
                case WindowsMessage.Nclbuttondblclk:
                    IsLeftMouseButtonPressed = true;

                    InvokeEventListeners(KeyState.Down, VirtualKeyCode.Lbutton, x, y);

                    IsLeftMouseButtonPressed = false;

                    InvokeEventListeners(KeyState.Up, VirtualKeyCode.Lbutton, x, y);
                    break;

                case WindowsMessage.Lbuttondown:
                case WindowsMessage.Nclbuttondown:
                    IsLeftMouseButtonPressed = true;
                    InvokeEventListeners(KeyState.Down, VirtualKeyCode.Lbutton, x, y);
                    break;

                case WindowsMessage.Lbuttonup:
                case WindowsMessage.Nclbuttonup:
                    IsLeftMouseButtonPressed = false;
                    InvokeEventListeners(KeyState.Up, VirtualKeyCode.Lbutton, x, y);
                    break;

                case WindowsMessage.Mbuttondown:
                case WindowsMessage.Ncmbuttondown:
                    IsMiddleMouseButtonPressed = true;
                    InvokeEventListeners(KeyState.Down, VirtualKeyCode.Mbutton, x, y);
                    break;

                case WindowsMessage.Mbuttonup:
                case WindowsMessage.Ncmbuttonup:
                    IsMiddleMouseButtonPressed = false;
                    InvokeEventListeners(KeyState.Up, VirtualKeyCode.Mbutton, x, y);
                    break;

                case WindowsMessage.Mbuttondblclk:
                case WindowsMessage.Ncmbuttondblclk:
                    IsMiddleMouseButtonPressed = true;

                    InvokeEventListeners(KeyState.Down, VirtualKeyCode.Mbutton, x, y);

                    IsMiddleMouseButtonPressed = false;

                    InvokeEventListeners(KeyState.Up, VirtualKeyCode.Mbutton, x, y);
                    break;

                case WindowsMessage.Rbuttondblclk:
                case WindowsMessage.Ncrbuttondblclk:
                    IsRightMouseButtonPressed = true;

                    InvokeEventListeners(KeyState.Down, VirtualKeyCode.Rbutton, x, y);

                    IsRightMouseButtonPressed = false;

                    InvokeEventListeners(KeyState.Up, VirtualKeyCode.Rbutton, x, y);
                    break;

                case WindowsMessage.Rbuttondown:
                case WindowsMessage.Ncrbuttondown:
                    IsRightMouseButtonPressed = true;

                    InvokeEventListeners(KeyState.Down, VirtualKeyCode.Rbutton, x, y);
                    break;

                case WindowsMessage.Rbuttonup:
                case WindowsMessage.Ncrbuttonup:
                    IsRightMouseButtonPressed = false;

                    InvokeEventListeners(KeyState.Up, VirtualKeyCode.Rbutton, x, y);
                    break;

                case WindowsMessage.Xbuttondblclk:
                case WindowsMessage.Ncxbuttondblclk:
                    if (HelperMethods.HIWORD(mouseData) == 0x1)
                    {
                        IsXButton1Pressed = true;

                        InvokeEventListeners(KeyState.Down, VirtualKeyCode.Xbutton1, x, y);

                        IsXButton1Pressed = false;

                        InvokeEventListeners(KeyState.Up, VirtualKeyCode.Xbutton1, x, y);
                    }
                    else
                    {
                        IsXButton2Pressed = true;

                        InvokeEventListeners(KeyState.Down, VirtualKeyCode.Xbutton2, x, y);

                        IsXButton2Pressed = false;

                        InvokeEventListeners(KeyState.Up, VirtualKeyCode.Xbutton2, x, y);
                    }
                    break;

                case WindowsMessage.Xbuttondown:
                case WindowsMessage.Ncxbuttondown:
                    if (HelperMethods.HIWORD(mouseData) == 0x1)
                    {
                        IsXButton1Pressed = true;

                        InvokeEventListeners(KeyState.Down, VirtualKeyCode.Xbutton1, x, y);
                    }
                    else
                    {
                        IsXButton2Pressed = true;

                        InvokeEventListeners(KeyState.Down, VirtualKeyCode.Xbutton2, x, y);
                    }
                    break;

                case WindowsMessage.Xbuttonup:
                case WindowsMessage.Ncxbuttonup:
                    if (HelperMethods.HIWORD(mouseData) == 0x1)
                    {
                        IsXButton1Pressed = false;

                        InvokeEventListeners(KeyState.Up, VirtualKeyCode.Xbutton1, x, y);
                    }
                    else
                    {
                        IsXButton2Pressed = false;

                        InvokeEventListeners(KeyState.Up, VirtualKeyCode.Xbutton2, x, y);
                    }
                    break;

                case WindowsMessage.Mousewheel:
                case WindowsMessage.Mousehwheel:
                    InvokeEventListeners(KeyState.None, VirtualKeyCode.Scroll, HelperMethods.HIWORD(mouseData), HelperMethods.HIWORD(mouseData));

                    break;

                case WindowsMessage.Mousemove:
                case WindowsMessage.Ncmousemove:
                    if (CaptureMouseMove)
                        InvokeEventListeners(KeyState.None, VirtualKeyCode.Invalid, x, y);
                    break;
            }
        }

        private void InvokeEventListeners(KeyState state, VirtualKeyCode key, int x = 0, int y = 0)
        {
            Task.Factory.StartNew(() => OnMouseEvent?.Invoke(key, state, x, y));
        }

        public bool InstallHook()
        {
            lock (_lockObject)
            {
                if (_hook != null) return false;

                _hook = new WindowsHook(WindowsHookType.LowLevelMouse);
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
            if (disposing)
            {
            }

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