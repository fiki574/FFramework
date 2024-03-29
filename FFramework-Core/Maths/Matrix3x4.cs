﻿/*
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
    © 2019 GitHub, Inc.
*/

using System.Runtime.InteropServices;

namespace FFramework_Core.Maths
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix3x4
    {
        public float M11;
        public float M12;
        public float M13;
        public float M14;
        public float M21;
        public float M22;
        public float M23;
        public float M24;
        public float M31;
        public float M32;
        public float M33;
        public float M34;
        public Vector3 GetLeft()
        {
            return GetRight() * -1f;
        }

        public Vector3 GetRight()
        {
            return new Vector3(M11, M21, M31);
        }

        public Vector3 GetUp()
        {
            return new Vector3(M12, M22, M33);
        }

        public Vector3 GetDown()
        {
            return GetUp() * -1f;
        }

        public Vector3 GetForward()
        {
            return GetBackward() * -1f;
        }

        public Vector3 GetBackward()
        {
            return new Vector3(M13, M23, M33);
        }
    }
}