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

using System.Globalization;
using System.Runtime.InteropServices;

namespace FFramework_Core.Maths
{ 
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix4x4
    {
        public static Matrix4x4 Identity =>
            new Matrix4x4(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);

        public bool IsIdentity => M11 == 1f && M22 == 1f && M33 == 1f && M44 == 1f && M12 == 0f && M13 == 0f &&
                                  M14 == 0f && M21 == 0f && M23 == 0f && M24 == 0f && M31 == 0f && M32 == 0f &&
                                  M34 == 0f && M41 == 0f && M42 == 0f && M43 == 0f;

        public Vector3 Translation
        {
            get => new Vector3(M41, M42, M43);
            set
            {
                M41 = value.X;
                M42 = value.Y;
                M43 = value.Z;
            }
        }

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
        public float M41;
        public float M42;
        public float M43;
        public float M44;

        public Matrix4x4(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24,
            float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44)
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M14 = m14;
            M21 = m21;
            M22 = m22;
            M23 = m23;
            M24 = m24;
            M31 = m31;
            M32 = m32;
            M33 = m33;
            M34 = m34;
            M41 = m41;
            M42 = m42;
            M43 = m43;
            M44 = m44;
        }

        public Vector2 WorldToScreen(Vector3 vec, Vector2 screenSize)
        {
            return WorldToScreen(vec, screenSize.X, screenSize.Y);
        }

        public Vector2 WorldToScreen(Vector3 vec, float sizeX, float sizeY)
        {
            var returnVector = new Vector2(0, 0);
            float w = M41 * vec.X + M42 * vec.Y + M43 * vec.Z + M44;

            if (!(w >= 0.01f)) return returnVector;

            float inverseX = 1f / w;

            returnVector.X =
                sizeX / 2f +
                (0.5f * (
                     (M11 * vec.X + M12 * vec.Y + M13 * vec.Z + M14)
                     * inverseX)
                 * sizeX + 0.5f);

            returnVector.Y =
                sizeY / 2f -
                (0.5f * (
                     (M21 * vec.X + M22 * vec.Y + M23 * vec.Z + M24)
                     * inverseX)
                 * sizeY + 0.5f);

            return returnVector;
        }

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

        public static Matrix4x4 Add(Matrix4x4 value1, Matrix4x4 value2)
        {
            var result = new Matrix4x4
            {
                M11 = value1.M11 + value2.M11,
                M12 = value1.M12 + value2.M12,
                M13 = value1.M13 + value2.M13,
                M14 = value1.M14 + value2.M14,
                M21 = value1.M21 + value2.M21,
                M22 = value1.M22 + value2.M22,
                M23 = value1.M23 + value2.M23,
                M24 = value1.M24 + value2.M24,
                M31 = value1.M31 + value2.M31,
                M32 = value1.M32 + value2.M32,
                M33 = value1.M33 + value2.M33,
                M34 = value1.M34 + value2.M34,
                M41 = value1.M41 + value2.M41,
                M42 = value1.M42 + value2.M42,
                M43 = value1.M43 + value2.M43,
                M44 = value1.M44 + value2.M44
            };
            return result;
        }

        public static Matrix4x4 Subtract(Matrix4x4 value1, Matrix4x4 value2)
        {
            var result = new Matrix4x4
            {
                M11 = value1.M11 - value2.M11,
                M12 = value1.M12 - value2.M12,
                M13 = value1.M13 - value2.M13,
                M14 = value1.M14 - value2.M14,
                M21 = value1.M21 - value2.M21,
                M22 = value1.M22 - value2.M22,
                M23 = value1.M23 - value2.M23,
                M24 = value1.M24 - value2.M24,
                M31 = value1.M31 - value2.M31,
                M32 = value1.M32 - value2.M32,
                M33 = value1.M33 - value2.M33,
                M34 = value1.M34 - value2.M34,
                M41 = value1.M41 - value2.M41,
                M42 = value1.M42 - value2.M42,
                M43 = value1.M43 - value2.M43,
                M44 = value1.M44 - value2.M44
            };
            return result;
        }

        public static Matrix4x4 CreateTranslation(Vector3 position)
        {
            var result = new Matrix4x4
            {
                M11 = 1f,
                M12 = 0f,
                M13 = 0f,
                M14 = 0f,
                M21 = 0f,
                M22 = 1f,
                M23 = 0f,
                M24 = 0f,
                M31 = 0f,
                M32 = 0f,
                M33 = 1f,
                M34 = 0f,
                M41 = position.X,
                M42 = position.Y,
                M43 = position.Z,
                M44 = 1f
            };
            return result;
        }

        public static bool Invert(Matrix4x4 matrix, out Matrix4x4 result)
        {
            float m = matrix.M11;
            float m2 = matrix.M12;
            float m3 = matrix.M13;
            float m4 = matrix.M14;
            float m5 = matrix.M21;
            float m6 = matrix.M22;
            float m7 = matrix.M23;
            float m8 = matrix.M24;
            float m9 = matrix.M31;
            float m10 = matrix.M32;
            float m11 = matrix.M33;
            float m12 = matrix.M34;
            float m13 = matrix.M41;
            float m14 = matrix.M42;
            float m15 = matrix.M43;
            float m16 = matrix.M44;
            float num = m11 * m16 - m12 * m15;
            float num2 = m10 * m16 - m12 * m14;
            float num3 = m10 * m15 - m11 * m14;
            float num4 = m9 * m16 - m12 * m13;
            float num5 = m9 * m15 - m11 * m13;
            float num6 = m9 * m14 - m10 * m13;
            float num7 = m6 * num - m7 * num2 + m8 * num3;
            float num8 = -(m5 * num - m7 * num4 + m8 * num5);
            float num9 = m5 * num2 - m6 * num4 + m8 * num6;
            float num10 = -(m5 * num3 - m6 * num5 + m7 * num6);
            float num11 = m * num7 + m2 * num8 + m3 * num9 + m4 * num10;
            if (MathF.Abs(num11) < 1.401298E-45f)
            {
                result = new Matrix4x4(float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN,
                    float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN);
                return false;
            }
            float num12 = 1f / num11;
            result.M11 = num7 * num12;
            result.M21 = num8 * num12;
            result.M31 = num9 * num12;
            result.M41 = num10 * num12;
            result.M12 = -(m2 * num - m3 * num2 + m4 * num3) * num12;
            result.M22 = (m * num - m3 * num4 + m4 * num5) * num12;
            result.M32 = -(m * num2 - m2 * num4 + m4 * num6) * num12;
            result.M42 = (m * num3 - m2 * num5 + m3 * num6) * num12;
            float num13 = m7 * m16 - m8 * m15;
            float num14 = m6 * m16 - m8 * m14;
            float num15 = m6 * m15 - m7 * m14;
            float num16 = m5 * m16 - m8 * m13;
            float num17 = m5 * m15 - m7 * m13;
            float num18 = m5 * m14 - m6 * m13;
            result.M13 = (m2 * num13 - m3 * num14 + m4 * num15) * num12;
            result.M23 = -(m * num13 - m3 * num16 + m4 * num17) * num12;
            result.M33 = (m * num14 - m2 * num16 + m4 * num18) * num12;
            result.M43 = -(m * num15 - m2 * num17 + m3 * num18) * num12;
            float num19 = m7 * m12 - m8 * m11;
            float num20 = m6 * m12 - m8 * m10;
            float num21 = m6 * m11 - m7 * m10;
            float num22 = m5 * m12 - m8 * m9;
            float num23 = m5 * m11 - m7 * m9;
            float num24 = m5 * m10 - m6 * m9;
            result.M14 = -(m2 * num19 - m3 * num20 + m4 * num21) * num12;
            result.M24 = (m * num19 - m3 * num22 + m4 * num23) * num12;
            result.M34 = -(m * num20 - m2 * num22 + m4 * num24) * num12;
            result.M44 = (m * num21 - m2 * num23 + m3 * num24) * num12;
            return true;
        }

        public static Matrix4x4 Lerp(Matrix4x4 matrix1, Matrix4x4 matrix2, float amount)
        {
            var result = new Matrix4x4
            {
                M11 = matrix1.M11 + (matrix2.M11 - matrix1.M11) * amount,
                M12 = matrix1.M12 + (matrix2.M12 - matrix1.M12) * amount,
                M13 = matrix1.M13 + (matrix2.M13 - matrix1.M13) * amount,
                M14 = matrix1.M14 + (matrix2.M14 - matrix1.M14) * amount,
                M21 = matrix1.M21 + (matrix2.M21 - matrix1.M21) * amount,
                M22 = matrix1.M22 + (matrix2.M22 - matrix1.M22) * amount,
                M23 = matrix1.M23 + (matrix2.M23 - matrix1.M23) * amount,
                M24 = matrix1.M24 + (matrix2.M24 - matrix1.M24) * amount,
                M31 = matrix1.M31 + (matrix2.M31 - matrix1.M31) * amount,
                M32 = matrix1.M32 + (matrix2.M32 - matrix1.M32) * amount,
                M33 = matrix1.M33 + (matrix2.M33 - matrix1.M33) * amount,
                M34 = matrix1.M34 + (matrix2.M34 - matrix1.M34) * amount,
                M41 = matrix1.M41 + (matrix2.M41 - matrix1.M41) * amount,
                M42 = matrix1.M42 + (matrix2.M42 - matrix1.M42) * amount,
                M43 = matrix1.M43 + (matrix2.M43 - matrix1.M43) * amount,
                M44 = matrix1.M44 + (matrix2.M44 - matrix1.M44) * amount
            };
            return result;
        }

        public static Matrix4x4 Multiply(Matrix4x4 value1, Matrix4x4 value2)
        {
            var result = new Matrix4x4
            {
                M11 = value1.M11 * value2.M11 + value1.M12 * value2.M21 + value1.M13 * value2.M31 +
                      value1.M14 * value2.M41,
                M12 = value1.M11 * value2.M12 + value1.M12 * value2.M22 + value1.M13 * value2.M32 +
                      value1.M14 * value2.M42,
                M13 = value1.M11 * value2.M13 + value1.M12 * value2.M23 + value1.M13 * value2.M33 +
                      value1.M14 * value2.M43,
                M14 = value1.M11 * value2.M14 + value1.M12 * value2.M24 + value1.M13 * value2.M34 +
                      value1.M14 * value2.M44,
                M21 = value1.M21 * value2.M11 + value1.M22 * value2.M21 + value1.M23 * value2.M31 +
                      value1.M24 * value2.M41,
                M22 = value1.M21 * value2.M12 + value1.M22 * value2.M22 + value1.M23 * value2.M32 +
                      value1.M24 * value2.M42,
                M23 = value1.M21 * value2.M13 + value1.M22 * value2.M23 + value1.M23 * value2.M33 +
                      value1.M24 * value2.M43,
                M24 = value1.M21 * value2.M14 + value1.M22 * value2.M24 + value1.M23 * value2.M34 +
                      value1.M24 * value2.M44,
                M31 = value1.M31 * value2.M11 + value1.M32 * value2.M21 + value1.M33 * value2.M31 +
                      value1.M34 * value2.M41,
                M32 = value1.M31 * value2.M12 + value1.M32 * value2.M22 + value1.M33 * value2.M32 +
                      value1.M34 * value2.M42,
                M33 = value1.M31 * value2.M13 + value1.M32 * value2.M23 + value1.M33 * value2.M33 +
                      value1.M34 * value2.M43,
                M34 = value1.M31 * value2.M14 + value1.M32 * value2.M24 + value1.M33 * value2.M34 +
                      value1.M34 * value2.M44,
                M41 = value1.M41 * value2.M11 + value1.M42 * value2.M21 + value1.M43 * value2.M31 +
                      value1.M44 * value2.M41,
                M42 = value1.M41 * value2.M12 + value1.M42 * value2.M22 + value1.M43 * value2.M32 +
                      value1.M44 * value2.M42,
                M43 = value1.M41 * value2.M13 + value1.M42 * value2.M23 + value1.M43 * value2.M33 +
                      value1.M44 * value2.M43,
                M44 = value1.M41 * value2.M14 + value1.M42 * value2.M24 + value1.M43 * value2.M34 +
                      value1.M44 * value2.M44
            };
            return result;
        }

        public static Matrix4x4 Multiply(Matrix4x4 value1, float value2)
        {
            var result = new Matrix4x4
            {
                M11 = value1.M11 * value2,
                M12 = value1.M12 * value2,
                M13 = value1.M13 * value2,
                M14 = value1.M14 * value2,
                M21 = value1.M21 * value2,
                M22 = value1.M22 * value2,
                M23 = value1.M23 * value2,
                M24 = value1.M24 * value2,
                M31 = value1.M31 * value2,
                M32 = value1.M32 * value2,
                M33 = value1.M33 * value2,
                M34 = value1.M34 * value2,
                M41 = value1.M41 * value2,
                M42 = value1.M42 * value2,
                M43 = value1.M43 * value2,
                M44 = value1.M44 * value2
            };
            return result;
        }

        public static Matrix4x4 Negate(Matrix4x4 value)
        {
            Matrix4x4 result;
            result.M11 = -value.M11;
            result.M12 = -value.M12;
            result.M13 = -value.M13;
            result.M14 = -value.M14;
            result.M21 = -value.M21;
            result.M22 = -value.M22;
            result.M23 = -value.M23;
            result.M24 = -value.M24;
            result.M31 = -value.M31;
            result.M32 = -value.M32;
            result.M33 = -value.M33;
            result.M34 = -value.M34;
            result.M41 = -value.M41;
            result.M42 = -value.M42;
            result.M43 = -value.M43;
            result.M44 = -value.M44;
            return result;
        }

        public static Matrix4x4 Transpose(Matrix4x4 matrix)
        {
            var result = new Matrix4x4
            {
                M11 = matrix.M11,
                M12 = matrix.M21,
                M13 = matrix.M31,
                M14 = matrix.M41,
                M21 = matrix.M12,
                M22 = matrix.M22,
                M23 = matrix.M32,
                M24 = matrix.M42,
                M31 = matrix.M13,
                M32 = matrix.M23,
                M33 = matrix.M33,
                M34 = matrix.M43,
                M41 = matrix.M14,
                M42 = matrix.M24,
                M43 = matrix.M34,
                M44 = matrix.M44
            };
            return result;
        }

        public static Matrix4x4 operator +(Matrix4x4 value1, Matrix4x4 value2)
        {
            var result = new Matrix4x4
            {
                M11 = value1.M11 + value2.M11,
                M12 = value1.M12 + value2.M12,
                M13 = value1.M13 + value2.M13,
                M14 = value1.M14 + value2.M14,
                M21 = value1.M21 + value2.M21,
                M22 = value1.M22 + value2.M22,
                M23 = value1.M23 + value2.M23,
                M24 = value1.M24 + value2.M24,
                M31 = value1.M31 + value2.M31,
                M32 = value1.M32 + value2.M32,
                M33 = value1.M33 + value2.M33,
                M34 = value1.M34 + value2.M34,
                M41 = value1.M41 + value2.M41,
                M42 = value1.M42 + value2.M42,
                M43 = value1.M43 + value2.M43,
                M44 = value1.M44 + value2.M44
            };
            return result;
        }

        public static bool operator ==(Matrix4x4 value1, Matrix4x4 value2)
        {
            return value1.M11 == value2.M11 && value1.M22 == value2.M22 && value1.M33 == value2.M33 &&
                   value1.M44 == value2.M44 && value1.M12 == value2.M12 && value1.M13 == value2.M13 &&
                   value1.M14 == value2.M14 && value1.M21 == value2.M21 && value1.M23 == value2.M23 &&
                   value1.M24 == value2.M24 && value1.M31 == value2.M31 && value1.M32 == value2.M32 &&
                   value1.M34 == value2.M34 && value1.M41 == value2.M41 && value1.M42 == value2.M42 &&
                   value1.M43 == value2.M43;
        }

        public static bool operator !=(Matrix4x4 value1, Matrix4x4 value2)
        {
            return value1.M11 != value2.M11 || value1.M12 != value2.M12 || value1.M13 != value2.M13 ||
                   value1.M14 != value2.M14 || value1.M21 != value2.M21 || value1.M22 != value2.M22 ||
                   value1.M23 != value2.M23 || value1.M24 != value2.M24 || value1.M31 != value2.M31 ||
                   value1.M32 != value2.M32 || value1.M33 != value2.M33 || value1.M34 != value2.M34 ||
                   value1.M41 != value2.M41 || value1.M42 != value2.M42 || value1.M43 != value2.M43 ||
                   value1.M44 != value2.M44;
        }

        public static Matrix4x4 operator *(Matrix4x4 value1, Matrix4x4 value2)
        {
            Matrix4x4 result;
            result.M11 = value1.M11 * value2.M11 + value1.M12 * value2.M21 + value1.M13 * value2.M31 +
                         value1.M14 * value2.M41;
            result.M12 = value1.M11 * value2.M12 + value1.M12 * value2.M22 + value1.M13 * value2.M32 +
                         value1.M14 * value2.M42;
            result.M13 = value1.M11 * value2.M13 + value1.M12 * value2.M23 + value1.M13 * value2.M33 +
                         value1.M14 * value2.M43;
            result.M14 = value1.M11 * value2.M14 + value1.M12 * value2.M24 + value1.M13 * value2.M34 +
                         value1.M14 * value2.M44;
            result.M21 = value1.M21 * value2.M11 + value1.M22 * value2.M21 + value1.M23 * value2.M31 +
                         value1.M24 * value2.M41;
            result.M22 = value1.M21 * value2.M12 + value1.M22 * value2.M22 + value1.M23 * value2.M32 +
                         value1.M24 * value2.M42;
            result.M23 = value1.M21 * value2.M13 + value1.M22 * value2.M23 + value1.M23 * value2.M33 +
                         value1.M24 * value2.M43;
            result.M24 = value1.M21 * value2.M14 + value1.M22 * value2.M24 + value1.M23 * value2.M34 +
                         value1.M24 * value2.M44;
            result.M31 = value1.M31 * value2.M11 + value1.M32 * value2.M21 + value1.M33 * value2.M31 +
                         value1.M34 * value2.M41;
            result.M32 = value1.M31 * value2.M12 + value1.M32 * value2.M22 + value1.M33 * value2.M32 +
                         value1.M34 * value2.M42;
            result.M33 = value1.M31 * value2.M13 + value1.M32 * value2.M23 + value1.M33 * value2.M33 +
                         value1.M34 * value2.M43;
            result.M34 = value1.M31 * value2.M14 + value1.M32 * value2.M24 + value1.M33 * value2.M34 +
                         value1.M34 * value2.M44;
            result.M41 = value1.M41 * value2.M11 + value1.M42 * value2.M21 + value1.M43 * value2.M31 +
                         value1.M44 * value2.M41;
            result.M42 = value1.M41 * value2.M12 + value1.M42 * value2.M22 + value1.M43 * value2.M32 +
                         value1.M44 * value2.M42;
            result.M43 = value1.M41 * value2.M13 + value1.M42 * value2.M23 + value1.M43 * value2.M33 +
                         value1.M44 * value2.M43;
            result.M44 = value1.M41 * value2.M14 + value1.M42 * value2.M24 + value1.M43 * value2.M34 +
                         value1.M44 * value2.M44;
            return result;
        }

        public static Matrix4x4 operator *(Matrix4x4 value1, float value2)
        {
            Matrix4x4 result;
            result.M11 = value1.M11 * value2;
            result.M12 = value1.M12 * value2;
            result.M13 = value1.M13 * value2;
            result.M14 = value1.M14 * value2;
            result.M21 = value1.M21 * value2;
            result.M22 = value1.M22 * value2;
            result.M23 = value1.M23 * value2;
            result.M24 = value1.M24 * value2;
            result.M31 = value1.M31 * value2;
            result.M32 = value1.M32 * value2;
            result.M33 = value1.M33 * value2;
            result.M34 = value1.M34 * value2;
            result.M41 = value1.M41 * value2;
            result.M42 = value1.M42 * value2;
            result.M43 = value1.M43 * value2;
            result.M44 = value1.M44 * value2;
            return result;
        }

        public static Matrix4x4 operator -(Matrix4x4 value1, Matrix4x4 value2)
        {
            Matrix4x4 result;
            result.M11 = value1.M11 - value2.M11;
            result.M12 = value1.M12 - value2.M12;
            result.M13 = value1.M13 - value2.M13;
            result.M14 = value1.M14 - value2.M14;
            result.M21 = value1.M21 - value2.M21;
            result.M22 = value1.M22 - value2.M22;
            result.M23 = value1.M23 - value2.M23;
            result.M24 = value1.M24 - value2.M24;
            result.M31 = value1.M31 - value2.M31;
            result.M32 = value1.M32 - value2.M32;
            result.M33 = value1.M33 - value2.M33;
            result.M34 = value1.M34 - value2.M34;
            result.M41 = value1.M41 - value2.M41;
            result.M42 = value1.M42 - value2.M42;
            result.M43 = value1.M43 - value2.M43;
            result.M44 = value1.M44 - value2.M44;
            return result;
        }

        public static Matrix4x4 operator -(Matrix4x4 value)
        {
            Matrix4x4 result;
            result.M11 = -value.M11;
            result.M12 = -value.M12;
            result.M13 = -value.M13;
            result.M14 = -value.M14;
            result.M21 = -value.M21;
            result.M22 = -value.M22;
            result.M23 = -value.M23;
            result.M24 = -value.M24;
            result.M31 = -value.M31;
            result.M32 = -value.M32;
            result.M33 = -value.M33;
            result.M34 = -value.M34;
            result.M41 = -value.M41;
            result.M42 = -value.M42;
            result.M43 = -value.M43;
            result.M44 = -value.M44;
            return result;
        }

        public override string ToString()
        {
            var currentCulture = CultureInfo.CurrentCulture;
            return string.Format(currentCulture,
                "{{ {{M11:{0} M12:{1} M13:{2} M14:{3}}} {{M21:{4} M22:{5} M23:{6} M24:{7}}} {{M31:{8} M32:{9} M33:{10} M34:{11}}} {{M41:{12} M42:{13} M43:{14} M44:{15}}} }}",
                M11.ToString(currentCulture), M12.ToString(currentCulture), M13.ToString(currentCulture),
                M14.ToString(currentCulture), M21.ToString(currentCulture), M22.ToString(currentCulture),
                M23.ToString(currentCulture), M24.ToString(currentCulture), M31.ToString(currentCulture),
                M32.ToString(currentCulture), M33.ToString(currentCulture), M34.ToString(currentCulture),
                M41.ToString(currentCulture), M42.ToString(currentCulture), M43.ToString(currentCulture),
                M44.ToString(currentCulture));
        }

        public override int GetHashCode()
        {
            return M11.GetHashCode() + M12.GetHashCode() + M13.GetHashCode() + M14.GetHashCode() + M21.GetHashCode() +
                   M22.GetHashCode() + M23.GetHashCode() + M24.GetHashCode() + M31.GetHashCode() + M32.GetHashCode() +
                   M33.GetHashCode() + M34.GetHashCode() + M41.GetHashCode() + M42.GetHashCode() + M43.GetHashCode() +
                   M44.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is Matrix4x4 && Equals((Matrix4x4) obj);
        }

        public bool Equals(Matrix4x4 other)
        {
            return M11 == other.M11 && M22 == other.M22 && M33 == other.M33 && M44 == other.M44 && M12 == other.M12 &&
                   M13 == other.M13 && M14 == other.M14 && M21 == other.M21 && M23 == other.M23 && M24 == other.M24 &&
                   M31 == other.M31 && M32 == other.M32 && M34 == other.M34 && M41 == other.M41 && M42 == other.M42 &&
                   M43 == other.M43;
        }
    }
}