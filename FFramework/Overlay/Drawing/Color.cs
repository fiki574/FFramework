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
using System.Runtime.InteropServices;

using SharpDX.Mathematics.Interop;

namespace FFramework.Overlay.Drawing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Color
    {
        public static Color Transparent => new Color(0.0f, 0.0f, 0.0f, 0.0f);
        public static Color Red => new Color(1.0f, 0.0f, 0.0f);
        public static Color Green => new Color(0.0f, 1.0f, 0.0f);
        public static Color Blue => new Color(0.0f, 0.0f, 1.0f);
        public float A;
        public float R;
        public float G;
        public float B;

        public Color(float r, float g, float b, float a = 1.0f)
        {
            R = NormalizeColor(r);
            G = NormalizeColor(g);
            B = NormalizeColor(b);

            A = NormalizeColor(a);
        }

        public Color(int r, int g, int b, int a = 255)
        {
            R = NormalizeColor(r);
            G = NormalizeColor(g);
            B = NormalizeColor(b);

            A = NormalizeColor(a);
        }

        public Color(byte r, byte g, byte b, byte a = 255)
        {
            R = r / 255.0f;
            G = g / 255.0f;
            B = b / 255.0f;

            A = a / 255.0f;
        }

        public Color(Color color, float alpha = 1.0f)
        {
            R = color.R;
            G = color.G;
            B = color.B;

            A = NormalizeColor(alpha);
        }

        public Color(Color color, int alpha = 255)
        {
            R = color.R;
            G = color.G;
            B = color.B;

            A = NormalizeColor(alpha);
        }

        public Color(Color color, byte alpha = 255)
        {
            R = color.R;
            G = color.G;
            B = color.B;

            A = alpha;
        }

        public override bool Equals(object obj)
        {
            if (obj is Color)
            {
                var clr = (Color)obj;

                return clr.R == R
                    && clr.G == G
                    && clr.B == B
                    && clr.A == A;
            }
            else
            {
                return false;
            }
        }

        public bool Equals(Color value)
        {
            return value.R == R
                && value.G == G
                && value.B == B
                && value.A == A;
        }

        public override int GetHashCode()
        {
            return OverrideHelper.HashCodes(
                R.GetHashCode(),
                G.GetHashCode(),
                B.GetHashCode(),
                A.GetHashCode());
        }

        public override string ToString()
        {
            return OverrideHelper.ToString(
                "R", R.ToString(),
                "G", G.ToString(),
                "B", B.ToString(),
                "A", A.ToString());
        }

        public int ToARGB()
        {
            return ((int)(R * 255.0f) << 16 | (int)(G * 255.0f) << 8 | (int)(B * 255.0f) | (int)(A * 255.0f) << 24) & -1;
        }

        public static Color FromARGB(int value)
        {
            return new Color(
                value >> 16 & 255,
                value >> 8 & 255,
                value & 255,
                value >> 24 & 255);
        }

        private static float NormalizeColor(float color)
        {
            if (color < 0.0f) color *= -1.0f;

            if (color <= 1.0f) return color;

            while (color > 255.0f) color /= 255.0f;

            return color / 255.0f;
        }

        private static float NormalizeColor(int color)
        {
            if (color < 0) color *= -1;

            while (color > 255) color /= 255;

            return color / 255.0f;
        }

        public static implicit operator Color(RawColor4 color)
        {
            return new Color(color.R, color.G, color.B, color.A);
        }

        public static implicit operator RawColor4(Color color)
        {
            return new RawColor4(color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// Determines whether two specified instances are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns><see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> represent the same value; otherwise, <see langword="false" />.</returns>
        public static bool operator ==(Color left, Color right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Color left, Color right)
        {
            return !Equals(left, right);
        }

        public static bool Equals(Color left, Color right)
        {
            return left.Equals(right);
        }
    }
}