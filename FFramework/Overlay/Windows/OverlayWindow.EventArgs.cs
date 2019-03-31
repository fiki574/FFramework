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

namespace FFramework.Overlay.Windows
{
    /// <summary>
    /// Provides data for the VisibilityChanged event.
    /// </summary>
    public class OverlayVisibilityEventArgs : EventArgs
    {
        /// <summary>
        /// Gets a Boolean indicating the visibility of the window.
        /// </summary>
        public bool IsVisible { get; private set; }

        private OverlayVisibilityEventArgs()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new OverlayVisibilityEventArgs using the given visibility.
        /// </summary>
        /// <param name="isVisible"></param>
        public OverlayVisibilityEventArgs(bool isVisible)
        {
            IsVisible = isVisible;
        }
    }

    /// <summary>
    /// Provides data for the PositionChanged event.
    /// </summary>
    public class OverlayPositionEventArgs : EventArgs
    {
        /// <summary>
        /// The new x-coordinate of the window.
        /// </summary>
        public int X { get; private set; }
        /// <summary>
        /// The new y-coordinate of the window.
        /// </summary>
        public int Y { get; private set; }

        private OverlayPositionEventArgs()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new OverlayPositionEventArgs using the given coordinates.
        /// </summary>
        /// <param name="x">The new x-coordinate of the window.</param>
        /// <param name="y">The new y-coordinate of the window.</param>
        public OverlayPositionEventArgs(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    /// <summary>
    /// Provides data for the SizeChanged event.
    /// </summary>
    public class OverlaySizeEventArgs : EventArgs
    {
        /// <summary>
        /// The new width of the window.
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        /// The new height of the window.
        /// </summary>
        public int Height { get; private set; }

        private OverlaySizeEventArgs()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new OverlaySizeEventArgs using the given width and height.
        /// </summary>
        /// <param name="width">The new width of the window.</param>
        /// <param name="height">The new height of the window.</param>
        public OverlaySizeEventArgs(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}
