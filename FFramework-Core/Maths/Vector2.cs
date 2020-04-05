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
using System.Runtime.InteropServices;

namespace FFramework_Core.Maths
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2
    {
        public static readonly Vector2 Empty;
        public static readonly Vector2 Invalid = new Vector2(float.NaN, float.NaN);
        public static readonly int Size = 8;
        public float X;
        public float Y;

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                }

                return index < 0 ? X : Y;
            }
            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    default:
                        if (index < 0) X = value;
                        else Y = value;
                        break;
                }
            }
        }

        public Vector2(float[] array)
        {
            if (array == null || array.Length < 2)
            {
                X = float.NaN;
                Y = float.NaN;
                return;
            }

            X = array[0];
            Y = array[1];
        }

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector2(double x, double y)
        {
            X = (float) x;
            Y = (float) y;
        }

        public Vector2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Vector2(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 8)
            {
                X = 0.0f;
                Y = 0.0f;
            }

            X = BitConverter.ToSingle(bytes, 0);
            Y = BitConverter.ToSingle(bytes, 4);
        }

        public Vector2(int dword)
        {
            X = dword & 65535;
            Y = (dword >> 16) & 65535;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector2)
                return (Vector2) obj == this;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "{X = " + X + ", Y = " + Y + "}";
        }

        public void Add(Vector2 vec)
        {
            X += vec.X;
            Y += vec.Y;
        }

        public void Add(float vec)
        {
            X += vec;
            Y += vec;
        }

        public void Add(int vec)
        {
            X += vec;
            Y += vec;
        }

        public void Subtract(Vector2 vec)
        {
            X -= vec.X;
            Y -= vec.Y;
        }

        public void Subtract(float vec)
        {
            X -= vec;
            Y -= vec;
        }

        public void Subtract(int vec)
        {
            X -= vec;
            Y -= vec;
        }

        public void Multiply(Vector2 vec)
        {
            X *= vec.X;
            Y *= vec.Y;
        }

        public void Multiply(float vec)
        {
            X *= vec;
            Y *= vec;
        }

        public void Multiply(int vec)
        {
            X *= vec;
            Y *= vec;
        }

        public void Divide(Vector2 vec)
        {
            if (vec.X == 0.0f) vec.X = float.Epsilon;
            if (vec.Y == 0.0f) vec.Y = float.Epsilon;

            X /= vec.X;
            Y /= vec.Y;
        }

        public void Divide(float vec)
        {
            if (vec == 0.0f) vec = float.Epsilon;

            X /= vec;
            Y /= vec;
        }

        public void Divide(int vec)
        {
            float tmp = vec;
            if (tmp == 0.0f) tmp = float.Epsilon;

            X /= tmp;
            Y /= tmp;
        }

        public Vector2 Clone()
        {
            return new Vector2(X, Y);
        }

        public void Clear()
        {
            X = 0.0f;
            Y = 0.0f;
        }

        public float DistanceTo(Vector2 vec)
        {
            return (this - vec).Length();
        }

        public float DotProduct(Vector2 vec)
        {
            return X * vec.X + Y * vec.Y;
        }

        public Vector2 CrossProduct(Vector2 vec)
        {
            return new Vector2(X * vec.Y - Y * vec.X, Y * vec.Y - X * vec.X);
        }

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[8];
            Buffer.BlockCopy(BitConverter.GetBytes(X), 0, bytes, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Y), 0, bytes, 4, 4);
            return bytes;
        }

        public float Length()
        {
            return (float) Math.Sqrt(X * X + Y * Y);
        }

        public bool IsEmpty()
        {
            return X == 0.0f && Y == 0.0f;
        }

        public bool RealIsEmpty()
        {
            return X < float.Epsilon && X > -float.Epsilon && Y < float.Epsilon && Y > -float.Epsilon;
        }

        public bool IsNaN()
        {
            return float.IsNaN(X) || float.IsNaN(Y);
        }

        public bool IsInfinity()
        {
            return float.IsInfinity(X) || float.IsInfinity(Y);
        }

        public bool IsValid()
        {
            if (IsNaN()) return false;

            return !IsInfinity();
        }

        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X + right.X, left.Y + right.Y);
        }

        public static Vector2 operator +(Vector2 left, float right)
        {
            return new Vector2(left.X + right, left.Y + right);
        }

        public static Vector2 operator +(Vector2 left, int right)
        {
            return new Vector2(left.X + right, left.Y + right);
        }

        public static Vector2 operator -(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X - right.X, left.Y - right.Y);
        }

        public static Vector2 operator -(Vector2 left, float right)
        {
            return new Vector2(left.X - right, left.Y - right);
        }

        public static Vector2 operator -(Vector2 left, int right)
        {
            return new Vector2(left.X - right, left.Y - right);
        }

        public static Vector2 operator *(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X * right.X, left.Y * right.Y);
        }

        public static Vector2 operator *(Vector2 left, float right)
        {
            return new Vector2(left.X * right, left.Y * right);
        }

        public static Vector2 operator *(Vector2 left, int right)
        {
            return new Vector2(left.X * right, left.Y * right);
        }

        public static Vector2 operator /(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X / right.X, left.Y / right.Y);
        }

        public static Vector2 operator /(Vector2 left, float right)
        {
            return new Vector2(left.X / right, left.Y / right);
        }

        public static Vector2 operator /(Vector2 left, int right)
        {
            return new Vector2(left.X / right, left.Y / right);
        }

        public static bool operator ==(Vector2 left, Vector2 right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator ==(Vector2 left, float right)
        {
            return left.X == right && left.Y == right;
        }

        public static bool operator ==(Vector2 left, int right)
        {
            return left.X == right && left.Y == right;
        }

        public static bool operator !=(Vector2 left, Vector2 right)
        {
            return left.X != right.X || left.Y != right.Y;
        }

        public static bool operator !=(Vector2 left, float right)
        {
            return left.X != right || left.Y != right;
        }

        public static bool operator !=(Vector2 left, int right)
        {
            return left.X != right || left.Y != right;
        }

        public static bool operator <(Vector2 left, Vector2 right)
        {
            return left.X < right.X && left.Y < right.Y;
        }

        public static bool operator <(Vector2 left, float right)
        {
            return left.X < right && left.Y < right;
        }

        public static bool operator <(Vector2 left, int right)
        {
            return left.X < right && left.Y < right;
        }

        public static bool operator >(Vector2 left, Vector2 right)
        {
            return left.X > right.X && left.Y > right.Y;
        }

        public static bool operator >(Vector2 left, float right)
        {
            return left.X > right && left.Y > right;
        }

        public static bool operator >(Vector2 left, int right)
        {
            return left.X > right && left.Y > right;
        }

        public static bool operator <=(Vector2 left, Vector2 right)
        {
            return left < right || left == right;
        }

        public static bool operator <=(Vector2 left, float right)
        {
            return left < right || left == right;
        }

        public static bool operator <=(Vector2 left, int right)
        {
            return left < right || left == right;
        }

        public static bool operator >=(Vector2 left, Vector2 right)
        {
            return left > right || left == right;
        }

        public static bool operator >=(Vector2 left, float right)
        {
            return left > right || left == right;
        }

        public static bool operator >=(Vector2 left, int right)
        {
            return left > right || left == right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(Vector2 left, Vector2 right)
        {
            return left.DistanceTo(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DotProduct(Vector2 left, Vector2 right)
        {
            return left.DotProduct(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 CrossProduct(Vector2 left, Vector2 right)
        {
            return left.CrossProduct(right);
        }
    }
}