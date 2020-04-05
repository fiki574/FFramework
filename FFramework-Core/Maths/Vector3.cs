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
    public struct Vector3
    {
        public static readonly Vector3 Empty;
        public static readonly Vector3 Zero;
        public static readonly Vector3 Invalid = new Vector3(float.NaN, float.NaN, float.NaN);
        public static readonly int Size = 12;

        public float X;
        public float Y;
        public float Z;

        public float this[int index]
        {
            get
            {
                if (index < 0 || index > 2) return float.NaN;

                switch (index)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                }

                return Z;
            }
            set
            {
                if (index < 0 || index > 2) return;

                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    default:
                        Z = value;
                        break;
                }
            }
        }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3(double x, double y, double z)
        {
            X = (float) x;
            Y = (float) y;
            Z = (float) z;
        }

        public Vector3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3(float[] array)
        {
            if (array == null || array.Length != 3)
            {
                X = float.NaN;
                Y = float.NaN;
                Z = float.NaN;
                return;
            }

            X = array[0];
            Y = array[1];
            Z = array[2];
        }

        public Vector3(byte[] array)
        {
            if (array == null || array.Length != 12)
            {
                X = float.NaN;
                Y = float.NaN;
                Z = float.NaN;
                return;
            }

            X = BitConverter.ToSingle(array, 0);
            Y = BitConverter.ToSingle(array, 4);
            Z = BitConverter.ToSingle(array, 8);
        }

        public override bool Equals(object obj)
        {
            return obj is Vector3 && Equals((Vector3) obj);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        public override string ToString()
        {
            return "{X = " + X + ", Y = " + Y + ", Z = " + Z + "}";
        }

        public void Add(Vector3 vec)
        {
            X += vec.X;
            Y += vec.Y;
            Z += vec.Z;
        }

        public void Add(float vec)
        {
            X += vec;
            Y += vec;
            Z += vec;
        }

        public void Add(int vec)
        {
            X += vec;
            Y += vec;
            Z += vec;
        }

        public void Subtract(Vector3 vec)
        {
            X -= vec.X;
            Y -= vec.Y;
            Z -= vec.Z;
        }

        public void Subtract(float vec)
        {
            X -= vec;
            Y -= vec;
            Z -= vec;
        }

        public void Subtract(int vec)
        {
            X -= vec;
            Y -= vec;
            Z -= vec;
        }

        public void Multiply(Vector3 vec)
        {
            X *= vec.X;
            Y *= vec.Y;
            Z *= vec.Z;
        }

        public void Multiply(float vec)
        {
            X *= vec;
            Y *= vec;
            Z *= vec;
        }

        public void Multiply(int vec)
        {
            X *= vec;
            Y *= vec;
            Z *= vec;
        }

        public void Divide(Vector3 vec)
        {
            if (vec.X == 0.0f) vec.X = float.Epsilon;
            if (vec.Y == 0.0f) vec.Y = float.Epsilon;
            if (vec.Z == 0.0f) vec.Z = float.Epsilon;

            X /= vec.X;
            Y /= vec.Y;
            Z /= vec.Z;
        }

        public void Divide(float vec)
        {
            if (vec == 0.0f) vec = float.Epsilon;

            X /= vec;
            Y /= vec;
            Z /= vec;
        }

        public void Divide(int vec)
        {
            float tmp = vec;
            if (tmp == 0.0f) tmp = float.Epsilon;

            X /= tmp;
            Y /= tmp;
            Z /= tmp;
        }

        public Vector3 Clone()
        {
            return new Vector3(X, Y, Z);
        }

        public void Clear()
        {
            X = 0.0f;
            Y = 0.0f;
            Z = 0.0f;
        }

        public float DistanceTo(Vector3 vec)
        {
            return (this - vec).Length();
        }

        public float Length()
        {
            return (float) Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public float LengthSquared()
        {
            return X * X + Y * Y + Z * Z;
        }

        public float DotProduct(Vector3 right)
        {
            return X * right.X + Y * right.Y + Z * right.Z;
        }

        public Vector3 CrossProduct(Vector3 right)
        {
            return new Vector3
            {
                X = Y * right.Z - Z * right.Y,
                Y = Z * right.X - X * right.Z,
                Z = X * right.Y - Y * right.X
            };
        }

        public Vector3 Lerp(Vector3 right, float amount)
        {
            return new Vector3(X + (right.X - X) * amount, Y + (right.Y - Y) * amount, Z + (right.Z - Z) * amount);
        }

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[12];
            Buffer.BlockCopy(BitConverter.GetBytes(X), 0, bytes, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Y), 0, bytes, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Z), 0, bytes, 8, 4);
            return bytes;
        }

        public bool IsEmpty()
        {
            return X == 0.0f && Y == 0.0f && Z == 0.0f;
        }

        public bool RealIsEmpty()
        {
            return X < float.Epsilon && X > -float.Epsilon && Y < float.Epsilon && Y > -float.Epsilon &&
                   Z < float.Epsilon && Z > -float.Epsilon;
        }

        public bool IsNaN()
        {
            return float.IsNaN(X) || float.IsNaN(Y) || float.IsNaN(Z);
        }

        public bool IsInfinity()
        {
            return float.IsInfinity(X) || float.IsInfinity(Y) || float.IsInfinity(Z);
        }

        public bool IsValid()
        {
            if (IsNaN()) return false;

            return !IsInfinity();
        }

        public bool AngleClamp()
        {
            if (!IsValid()) return false;

            X = MathF.Clamp(X, -89.0f, 89.0f);

            Z = 0.0f;

            return IsValid();
        }

        public bool AngleClamp(float min, float max)
        {
            if (!IsValid()) return false;

            X = MathF.Clamp(X, min, max);

            Z = 0.0f;

            return IsValid();
        }

        public bool AngleNormalize()
        {
            if (!IsValid()) return false;

            Y = MathF.Normalize(Y, -180.0f, 180.0f, 360.0f);

            Z = 0.0f;

            return IsValid();
        }

        public bool AngleNormalize(float min, float max, float norm)
        {
            if (!IsValid()) return false;

            Y = MathF.Normalize(Y, min, max, norm);

            Z = 0.0f;

            return IsValid();
        }

        public bool AngleClampAndNormalize()
        {
            if (!IsValid()) return false;

            X = MathF.Clamp(X, -89.0f, 89.0f);
            Y = MathF.Normalize(Y, -180.0f, 180.0f, 360.0f);
            Z = 0.0f;

            return IsValid();
        }

        public bool VectorNormalize()
        {
            if (!IsValid()) return false;

            float length = Length();

            if (length == 0.0f) return true;

            X /= length;
            Y /= length;
            Z /= length;

            return IsValid();
        }

        public static Vector3 operator +(Vector3 left, Vector3 right)
        {
            return new Vector3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        public static Vector3 operator +(Vector3 left, float right)
        {
            return new Vector3(left.X + right, left.Y + right, left.Z + right);
        }

        public static Vector3 operator +(Vector3 left, int right)
        {
            return new Vector3(left.X + right, left.Y + right, left.Z + right);
        }

        public static Vector3 operator -(Vector3 left, Vector3 right)
        {
            return new Vector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public static Vector3 operator -(Vector3 left, float right)
        {
            return new Vector3(left.X - right, left.Y - right, left.Z - right);
        }

        public static Vector3 operator -(Vector3 left, int right)
        {
            return new Vector3(left.X - right, left.Y - right, left.Z - right);
        }

        public static Vector3 operator *(Vector3 left, Vector3 right)
        {
            return new Vector3(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
        }

        public static Vector3 operator *(Vector3 left, float right)
        {
            return new Vector3(left.X * right, left.Y * right, left.Z * right);
        }

        public static Vector3 operator *(Vector3 left, int right)
        {
            return new Vector3(left.X * right, left.Y * right, left.Z * right);
        }

        public static Vector3 operator /(Vector3 left, Vector3 right)
        {
            return new Vector3(left.X / right.X, left.Y / right.Y, left.Z / right.Z);
        }

        public static Vector3 operator /(Vector3 left, float right)
        {
            return new Vector3(left.X / right, left.Y / right, left.Z / right);
        }

        public static Vector3 operator /(Vector3 left, int right)
        {
            return new Vector3(left.X / right, left.Y / right, left.Z / right);
        }

        public static bool operator ==(Vector3 left, Vector3 right)
        {
            return left.X == right.X
                   && left.Y == right.Y
                   && left.Z == right.Z;
        }

        public static bool operator ==(Vector3 left, float right)
        {
            return left.X == right
                   && left.Y == right
                   && left.Z == right;
        }

        public static bool operator ==(Vector3 left, int right)
        {
            return left.X == right
                   && left.Y == right
                   && left.Z == right;
        }

        public static bool operator !=(Vector3 left, Vector3 right)
        {
            return left.X != right.X
                   || left.Y != right.Y
                   || left.Z != right.Z;
        }

        public static bool operator !=(Vector3 left, float right)
        {
            return left.X != right
                   || left.Y != right
                   || left.Z != right;
        }

        public static bool operator !=(Vector3 left, int right)
        {
            return left.X != right
                   || left.Y != right
                   || left.Z != right;
        }

        public static bool operator <(Vector3 left, Vector3 right)
        {
            return left.X < right.X
                   && left.Y < right.Y
                   && left.Z < right.Z;
        }

        public static bool operator <(Vector3 left, float right)
        {
            return left.X < right
                   && left.Y < right
                   && left.Z < right;
        }

        public static bool operator <(Vector3 left, int right)
        {
            return left.X < right
                   && left.Y < right
                   && left.Z < right;
        }

        public static bool operator >(Vector3 left, Vector3 right)
        {
            return left.X > right.X
                   && left.Y > right.Y
                   && left.Z > right.Z;
        }

        public static bool operator >(Vector3 left, float right)
        {
            return left.X > right
                   && left.Y > right
                   && left.Z > right;
        }

        public static bool operator >(Vector3 left, int right)
        {
            return left.X > right
                   && left.Y > right
                   && left.Z > right;
        }

        public static bool operator <=(Vector3 left, Vector3 right)
        {
            return left == right || left < right;
        }

        public static bool operator <=(Vector3 left, float right)
        {
            return left == right || left < right;
        }

        public static bool operator <=(Vector3 left, int right)
        {
            return left == right || left < right;
        }

        public static bool operator >=(Vector3 left, Vector3 right)
        {
            return left == right || left > right;
        }

        public static bool operator >=(Vector3 left, float right)
        {
            return left == right || left > right;
        }

        public static bool operator >=(Vector3 left, int right)
        {
            return left == right || left > right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(Vector3 left, Vector3 right)
        {
            return left.DistanceTo(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DotProduct(Vector3 left, Vector3 right)
        {
            return left.DotProduct(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 CrossProduct(Vector3 left, Vector3 right)
        {
            return left.CrossProduct(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Lerp(Vector3 left, Vector3 right, float amount)
        {
            return left.Lerp(right, amount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 AngleClamp(Vector3 vec)
        {
            var tmp = vec.Clone();
            tmp.AngleClamp();
            return tmp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 AngleClamp(Vector3 vec, float min, float max)
        {
            var tmp = vec.Clone();
            tmp.AngleClamp(min, max);
            return tmp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 AngleNormalize(Vector3 vec)
        {
            var tmp = vec.Clone();
            tmp.AngleNormalize();
            return tmp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 AngleNormalize(Vector3 vec, float min, float max, float norm)
        {
            var tmp = vec.Clone();
            tmp.AngleNormalize(min, max, norm);
            return tmp;
        }
    }
}