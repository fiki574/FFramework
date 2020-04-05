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
    public static class Math2
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DistanceInCircle(Vector2 point, Vector2 circleCenter)
        {
            return Math.Sqrt((circleCenter.X - point.X) * (circleCenter.X - point.X) +
                             (circleCenter.Y - point.Y) * (circleCenter.Y - point.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceOnScreen(float X, float Y, Vector2 screen)
        {
            return (float) Math.Sqrt(Math.Pow(Y - screen.Y / 2, 2) + Math.Pow(X - screen.X / 2, 2));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceOnScreen(Vector2 point, Vector2 screen)
        {
            return (float) Math.Sqrt(Math.Pow(point.Y - screen.Y / 2, 2) + Math.Pow(point.X - screen.X / 2, 2));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPointInCircle(Vector2 point, Vector2 circleCenter, int radius)
        {
            return Math.Sqrt((circleCenter.X - point.X) * (circleCenter.X - point.X) +
                             (circleCenter.Y - point.Y) * (circleCenter.Y - point.Y)) < radius;
        }
    }
}