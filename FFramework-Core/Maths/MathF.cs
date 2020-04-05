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
    © 2019 GitHub, Inc.
*/

using System;
using System.Runtime.CompilerServices;

namespace FFramework_Core.Maths
{
    public static class MathF
    {
        private static readonly Random RandomNumberGenerator = new Random();
        public static readonly float Pi = 3.14159274f;
        public static readonly float Deg2Rad = (float) (Math.PI / 180f);
        public static readonly float Rad2Deg = (float) (180.0f / Math.PI);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Abs(float x)
        {
            return Math.Abs(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Acos(float x)
        {
            return (float) Math.Acos(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cos(float x)
        {
            return (float) Math.Cos(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float IeeeRemainder(float x, float y)
        {
            return (float)Math.IEEERemainder(x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Pow(float x, float y)
        {
            return (float) Math.Pow(x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sin(float x)
        {
            return (float) Math.Sin(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sqrt(float x)
        {
            return (float) Math.Sqrt(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Tan(float x)
        {
            return (float) Math.Tan(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;

            return value > max ? max : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DegreesToRadians(float deg)
        {
            return deg * Deg2Rad;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Normalize(float value, float min, float max)
        {
            float norm = max < 0.0f ? max * -1.0f : max;

            while (value < min)
                value += norm;
            while (value > max)
                value -= norm;

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Normalize(float value, float min, float max, float norm)
        {
            while (value < min)
                value += norm;
            while (value > max)
                value -= norm;

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RadiansToDegrees(float rad)
        {
            return rad * Rad2Deg;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RandomFloat(float min, float max)
        {
            return (float) RandomNumberGenerator.NextDouble() * (max - min) + min;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RandomInt(int min, int max)
        {
            return RandomNumberGenerator.Next(min, max);
        }
    }
}