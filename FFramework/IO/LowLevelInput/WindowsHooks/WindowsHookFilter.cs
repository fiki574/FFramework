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

using FFramework.LowLevelInput.Hooks;
using FFramework.LowLevelInput.PInvoke.Types;

namespace FFramework.LowLevelInput.WindowsHooks
{ 
    public static class WindowsHookFilter
    {
        public delegate bool WindowsHookFilterEventHandler(VirtualKeyCode key, KeyState state);
        public static WindowsHookFilterEventHandler Filter;

        internal static bool InternalFilterEventsHelper(IntPtr wParam, IntPtr lParam)
        {
            if (wParam == IntPtr.Zero || lParam == IntPtr.Zero) return false;

            var events = Filter;

            if (events == null) return false;

            var msg = (WindowsMessage)(uint)wParam.ToInt32();

            switch (msg)
            {

                case WindowsMessage.Keydown:
                case WindowsMessage.Syskeydown:
                    return events.Invoke((VirtualKeyCode)Marshal.ReadInt32(lParam), KeyState.Down);

                case WindowsMessage.Keyup:
                case WindowsMessage.Syskeyup:
                    return events.Invoke((VirtualKeyCode)Marshal.ReadInt32(lParam), KeyState.Up);

                default:
                    if(HelperMethods.TryGetMouseData(wParam, lParam, out VirtualKeyCode key, out KeyState state))
                    {
                        return events.Invoke(key, state);
                    }
                    else
                    {
                        return false;
                    }
            }
        }
    }
}