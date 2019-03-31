﻿/*
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

namespace FFramework.Overlay.Drawing
{
    /// <summary>
    /// Represents a Scene / frame of a Graphics surface.
    /// </summary>
    public class Scene : IDisposable
    {
        /// <summary>
        /// The Graphics surface.
        /// </summary>
        public Graphics Device { get; private set; }

        private Scene()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new Scene using a Graphics surface
        /// </summary>
        /// <param name="device">A Graphics surface</param>
        public Scene(Graphics device)
        {
            Device = device ?? throw new ArgumentNullException(nameof(device));
            device.BeginScene();
        }

        /// <summary>
        /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~Scene()
        {
            Dispose(false);
        }

        /// <summary>
        /// Returns a value indicating whether this instance and a specified <see cref="T:System.Object" /> represent the same type and value.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true" /> if <paramref name="obj" /> is a Scene and equal to this instance; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            var scene = obj as Scene;

            if (scene == null)
            {
                return false;
            }
            else
            {
                return scene.Device == Device;
            }
        }

        /// <summary>
        /// Returns a value indicating whether two specified instances of Scene represent the same value.
        /// </summary>
        /// <param name="value">An object to compare to this instance.</param>
        /// <returns><see langword="true" /> if <paramref name="value" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
        public bool Equals(Scene value)
        {
            return value != null
                && value.Device == Device;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return Device.GetHashCode();
        }

        /// <summary>
        /// Converts this Scene to a human-readable string.
        /// </summary>
        /// <returns>A string representation of this Scene.</returns>
        public override string ToString()
        {
            return OverrideHelper.ToString(
                "Scene", GetHashCode().ToString(),
                "Device", Device.ToString());
        }

        #region IDisposable Support
        private bool disposedValue = false;

        /// <summary>
        /// Releases all resources used by this Scene.
        /// </summary>
        /// <param name="disposing">A Boolean value indicating whether this is called from the destructor.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Device.EndScene();

                disposedValue = true;
            }
        }

        /// <summary>
        /// Releases all resources used by this Scene.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        /// <summary>
        /// Converts a Scene to a Graphics surface.
        /// </summary>
        /// <param name="scene">The Scene object.</param>
        public static implicit operator Graphics(Scene scene)
        {
            if (scene.Device == null) throw new InvalidOperationException(nameof(scene.Device) + " is null");

            return scene.Device;
        }

        /// <summary>
        /// Returns a value indicating whether two specified instances of Scene represent the same value.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns> <see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <see langword="false" />.</returns>
        public static bool Equals(Scene left, Scene right)
        {
            return left != null
                && left.Equals(right);
        }
    }
}
