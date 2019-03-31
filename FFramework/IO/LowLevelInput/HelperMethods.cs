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
using System.Runtime.CompilerServices;

using FFramework.LowLevelInput.Hooks;
using FFramework.LowLevelInput.PInvoke.Types;

namespace FFramework.LowLevelInput
{
    internal static class HelperMethods
    {
        public static bool TryGetMouseData(IntPtr wParam, IntPtr lParam, out VirtualKeyCode key, out KeyState state)
        {
            var msg = (WindowsMessage)(uint)wParam.ToInt32();

            int mouseData = 0;

            switch (msg)
            {
                case WindowsMessage.Lbuttondblclk:
                case WindowsMessage.Nclbuttondblclk:
                case WindowsMessage.Lbuttondown:
                case WindowsMessage.Nclbuttondown:
                    key = VirtualKeyCode.Lbutton;
                    state = KeyState.Down;
                    return true;
                case WindowsMessage.Lbuttonup:
                case WindowsMessage.Nclbuttonup:
                    key = VirtualKeyCode.Lbutton;
                    state = KeyState.Up;
                    return true;
                case WindowsMessage.Mbuttondown:
                case WindowsMessage.Ncmbuttondown:
                case WindowsMessage.Mbuttondblclk:
                case WindowsMessage.Ncmbuttondblclk:
                    key = VirtualKeyCode.Mbutton;
                    state = KeyState.Down;
                    return true;
                case WindowsMessage.Mbuttonup:
                case WindowsMessage.Ncmbuttonup:
                    key = VirtualKeyCode.Mbutton;
                    state = KeyState.Up;
                    return true;
                case WindowsMessage.Rbuttondblclk:
                case WindowsMessage.Ncrbuttondblclk:
                case WindowsMessage.Rbuttondown:
                case WindowsMessage.Ncrbuttondown:
                    key = VirtualKeyCode.Rbutton;
                    state = KeyState.Down;
                    return true;
                case WindowsMessage.Rbuttonup:
                case WindowsMessage.Ncrbuttonup:
                    key = VirtualKeyCode.Rbutton;
                    state = KeyState.Up;
                    return true;
                case WindowsMessage.Xbuttondblclk:
                case WindowsMessage.Ncxbuttondblclk:
                case WindowsMessage.Xbuttondown:
                case WindowsMessage.Ncxbuttondown:
                    mouseData = Marshal.ReadInt32(lParam, 8);

                    if (HIWORD(mouseData) == 0x1)
                    {
                        key = VirtualKeyCode.Xbutton1;
                        state = KeyState.Down;
                    }
                    else
                    {
                        key = VirtualKeyCode.Xbutton2;
                        state = KeyState.Down;
                    }
                    return true;
                case WindowsMessage.Xbuttonup:
                case WindowsMessage.Ncxbuttonup:
                    mouseData = Marshal.ReadInt32(lParam, 8);

                    if (HIWORD(mouseData) == 0x1)
                    {
                        key = VirtualKeyCode.Xbutton1;
                        state = KeyState.Up;
                    }
                    else
                    {
                        key = VirtualKeyCode.Xbutton2;
                        state = KeyState.Up;
                    }
                    return true;
                case WindowsMessage.Mousewheel:
                case WindowsMessage.Mousehwheel:
                    key = VirtualKeyCode.Scroll;
                    state = KeyState.Pressed;
                    return true;
                case WindowsMessage.Mousemove:
                case WindowsMessage.Ncmousemove:
                    key = VirtualKeyCode.Invalid;
                    state = KeyState.Pressed;
                    return true;
                default:
                    key = VirtualKeyCode.Invalid;
                    state = KeyState.None;
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void KbdClearInjectedFlag(IntPtr lParam)
        {
            int flags = Marshal.ReadInt32(lParam + 8);

            flags = SetBit(flags, 1, false);
            flags = SetBit(flags, 4, false);

            Marshal.WriteInt32(lParam + 8, flags);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SetBit(int num, int index, bool value)
        {
            if(value)
            {
                return num | (1 << index);
            }
            else
            {
                return num & ~(1 << index);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort HIWORD(int number)
        {
            return (ushort)(((uint)number >> 16) & 0xFFFF);
        }
    }
}