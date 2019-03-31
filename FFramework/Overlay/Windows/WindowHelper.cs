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
using System.Collections.Generic;

using FFramework.Overlay.PInvoke;

namespace FFramework.Overlay.Windows
{
    /// <summary>
    /// Provides methods to interact with windows.
    /// </summary>
    public static class WindowHelper
    {
        private const int MinRandomStringLen = 8;
        private const int MaxRandomStringLen = 16;

        private static readonly Random _random = new Random();
        private static readonly object _blacklistLock = new object();

        private static List<string> _windowClassesBlacklist = new List<string>();

        /// <summary>
        /// Generates a random window title.
        /// </summary>
        /// <returns>The string this method creates.</returns>
        public static string GenerateRandomTitle()
        {
            return GenerateRandomAsciiString(MinRandomStringLen, MaxRandomStringLen);
        }
        /// <summary>
        /// Generates a random window class name.
        /// </summary>
        /// <returns>The string this method creates.</returns>
        public static string GenerateRandomClass()
        {
            lock (_blacklistLock)
            {
                while (true)
                {
                    string name = GenerateRandomAsciiString(MinRandomStringLen, MaxRandomStringLen);

                    if (_windowClassesBlacklist.Contains(name))
                    {
                        continue;
                    }
                    else
                    {
                        _windowClassesBlacklist.Add(name);

                        return name;
                    }
                }
            }
        }

        /// <summary>
        /// Adds the topmost flag to a window.
        /// </summary>
        /// <param name="hwnd">A IntPtr representing the handle of a window.</param>
        public static void MakeTopmost(IntPtr hwnd) => User32.SetWindowPos(hwnd, User32.HwndInsertTopMost, 0, 0, 0, 0, SwpFlags.ShowWindow | SwpFlags.NoActivate | SwpFlags.NoMove | SwpFlags.NoSize);
        /// <summary>
        /// Removes the topmost flag from a window.
        /// </summary>
        /// <param name="hwnd">A IntPtr representing the handle of a window.</param>
        public static void RemoveTopmost(IntPtr hwnd) => User32.SetWindowPos(hwnd, User32.HwndInsertNoTopmost, 0, 0, 0, 0, SwpFlags.NoActivate | SwpFlags.NoMove | SwpFlags.NoSize);

        /// <summary>
        /// Returns the boundaries of a window.
        /// </summary>
        /// <param name="hwnd">A IntPtr representing the handle of a window.</param>
        /// <param name="bounds">A WindowBounds structure representing the boundaries of a window.</param>
        /// <returns></returns>
        public static bool GetWindowBounds(IntPtr hwnd, out WindowBounds bounds)
        {
            if (User32.GetWindowRect(hwnd, out var rect))
            {
                bounds = new WindowBounds()
                {
                    Left = rect.Left,
                    Top = rect.Top,
                    Right = rect.Right,
                    Bottom = rect.Bottom
                };

                return true;
            }
            else
            {
                bounds = default(WindowBounds);

                return false;
            }
        }
        /// <summary>
        /// Returns the boundaries of a windows client area.
        /// </summary>
        /// <param name="hwnd">A IntPtr representing the handle of a window.</param>
        /// <param name="bounds">A WindowBounds structure representing the boundaries of a window.</param>
        /// <returns></returns>
        public static bool GetWindowClientBounds(IntPtr hwnd, out WindowBounds bounds)
        {
            if (GetWindowClientInternal(hwnd, out var rect))
            {
                bounds = new WindowBounds()
                {
                    Left = rect.Left,
                    Top = rect.Top,
                    Right = rect.Right,
                    Bottom = rect.Bottom
                };

                return true;
            }
            else
            {
                bounds = default(WindowBounds);

                return false;
            }
        }
        
        /// <summary>
        /// Extends a windows frame into the client area of the window.
        /// </summary>
        /// <param name="hwnd">A IntPtr representing the handle of a window.</param>
        public static void ExtendFrameIntoClientArea(IntPtr hwnd)
        {
            var margin = new NativeMargin
            {
                cxLeftWidth = -1,
                cxRightWidth = -1,
                cyBottomHeight = -1,
                cyTopHeight = -1
            };

            DwmApi.DwmExtendFrameIntoClientArea(hwnd, ref margin);
        }

        private static bool GetWindowClientInternal(IntPtr hwnd, out NativeRect rect)
        {
            // calculates the window bounds based on the difference of the client and the window rect

            rect = new NativeRect();

            if (!User32.GetWindowRect(hwnd, out rect)) return false;
            if (!User32.GetClientRect(hwnd, out var clientRect)) return true;

            int clientWidth = clientRect.Right - clientRect.Left;
            int clientHeight = clientRect.Bottom - clientRect.Top;

            int windowWidth = rect.Right - rect.Left;
            int windowHeight = rect.Bottom - rect.Top;

            if (clientWidth == windowWidth && clientHeight == windowHeight) return true;

            if (clientWidth != windowWidth)
            {
                int difX = clientWidth > windowWidth ? clientWidth - windowWidth : windowWidth - clientWidth;
                difX /= 2;

                rect.Right -= difX;
                rect.Left += difX;

                if (clientHeight != windowHeight)
                {
                    int difY = clientHeight > windowHeight ? clientHeight - windowHeight : windowHeight - clientHeight;

                    rect.Top += difY - difX;
                    rect.Bottom -= difX;
                }
            }
            else if (clientHeight != windowHeight)
            {
                int difY = clientHeight > windowHeight ? clientHeight - windowHeight : windowHeight - clientHeight;
                difY /= 2;

                rect.Bottom -= difY;
                rect.Top += difY;
            }

            return true;
        }

        private static string GenerateRandomAsciiString(int minLength, int maxLength)
        {
            int length = _random.Next(minLength, maxLength);

            char[] chars = new char[length];

            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = (char)_random.Next(97, 123); // ascii range for small letters
            }

            return new string(chars);
        }
    }
}
