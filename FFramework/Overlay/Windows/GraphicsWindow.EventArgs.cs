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

using FFramework.Overlay.Drawing;

namespace FFramework.Overlay.Windows
{
    /// <summary>
    /// Provides data for the DrawGraphics event.
    /// </summary>
    public class DrawGraphicsEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the Graphics surface.
        /// </summary>
        public Graphics Graphics { get; private set; }

		private DrawGraphicsEventArgs()
        {

        }

        /// <summary>
        /// Initializes a new DrawGraphicsEventArgs with a Graphics surface.
        /// </summary>
        /// <param name="graphics"></param>
		public DrawGraphicsEventArgs(Graphics graphics)
        {
            Graphics = graphics ?? throw new ArgumentNullException(nameof(graphics));
        }
    }

    /// <summary>
    /// Provides data for the SetupGraphics event.
    /// </summary>
    public class SetupGraphicsEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the Graphics surface.
        /// </summary>
        public Graphics Graphics { get; private set; }

        private SetupGraphicsEventArgs()
        {

        }

        /// <summary>
        /// Initializes a new SetupGraphicsEventArgs with a Graphics surface.
        /// </summary>
        /// <param name="graphics"></param>
        public SetupGraphicsEventArgs(Graphics graphics)
        {
            Graphics = graphics ?? throw new ArgumentNullException(nameof(graphics));
        }
    }

    /// <summary>
    /// Provides data for the DestroyGraphics event.
    /// </summary>
    public class DestroyGraphicsEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the Graphics surface.
        /// </summary>
        public Graphics Graphics { get; private set; }

        private DestroyGraphicsEventArgs()
        {

        }

        /// <summary>
        /// Initializes a new DestroyGraphicsEventArgs with a Graphics surface.
        /// </summary>
        /// <param name="graphics"></param>
        public DestroyGraphicsEventArgs(Graphics graphics)
        {
            Graphics = graphics ?? throw new ArgumentNullException(nameof(graphics));
        }
    }
}
