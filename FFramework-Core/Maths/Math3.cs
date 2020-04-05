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
    public static class Math3
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleDifference(Vector3 pointOfView, Vector3 angle_1, Vector3 angle_2)
        {
            float deltaX = (float) Math.Sin(MathF.DegreesToRadians(angle_1.X - angle_2.X));
            float deltaY = (float) Math.Sin(MathF.DegreesToRadians(angle_1.Y - angle_2.Y));

            return (deltaX * deltaX + deltaY * deltaY) * pointOfView.Length();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleDifference(float distance, Vector3 viewAngle, Vector3 targetAngle)
        {
            double pitch = Math.Sin(MathF.DegreesToRadians(viewAngle.X - targetAngle.X)) * distance;
            double yaw = Math.Sin(MathF.DegreesToRadians(viewAngle.Y - targetAngle.Y)) * distance;

            return (float) Math.Sqrt(pitch * pitch + yaw * yaw);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 AngleLinearSmooth(Vector3 source, Vector3 dest, float value)
        {
            dest -= source;
            if (!dest.AngleClampAndNormalize()) return Vector3.Invalid;
            dest /= value;
            if (!dest.AngleClampAndNormalize()) return Vector3.Invalid;
            dest += source;
            if (!dest.AngleClampAndNormalize()) return Vector3.Invalid;
            return dest;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 AngleVector(Vector3 vec)
        {
            float x = vec.X * MathF.Deg2Rad;
            float y = vec.Y * MathF.Deg2Rad;

            float sinYaw = (float) Math.Sin(y);
            float cosYaw = (float) Math.Cos(y);

            float sinPitch = (float) Math.Sin(x);
            float cosPitch = (float) Math.Cos(x);

            return new Vector3(cosPitch * cosYaw, cosPitch * sinYaw, -sinPitch);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 CalcAngle(Vector3 source, Vector3 dest)
        {
            var ret = new Vector3();
            var delta = source - dest;

            float hyp = (float) Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);

            ret.X = MathF.RadiansToDegrees((float) Math.Atan(delta.Z / hyp));
            ret.Y = MathF.RadiansToDegrees((float) Math.Atan(delta.Y / delta.X));

            if (delta.X >= 0.0f)
                ret.Y += 180.0f;

            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetFov(Vector3 viewAngle, Vector3 targetAngle, float distance)
        {
            double deltaX = Math.Sin(MathF.DegreesToRadians(Math.Abs(viewAngle.X - targetAngle.X)));
            double deltaY = Math.Sin(MathF.DegreesToRadians(Math.Abs(viewAngle.Y - targetAngle.Y)));

            return (float) Math.Sin(deltaX * deltaX + deltaY * deltaY) * distance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetFov(Vector3 angle, Vector3 src, Vector3 dst)
        {
            var tmp = dst - src;

            var ang = VectorAngles(tmp);

            var vTempAngles = ang - angle;

            while (vTempAngles.X > 180.0f) vTempAngles.X -= 360.0f;

            while (vTempAngles.X < -180.0f) vTempAngles.X += 360.0f;

            while (vTempAngles.Y > 180.0f) vTempAngles.Y -= 360.0f;

            while (vTempAngles.Y < -180.0f) vTempAngles.Y += 360.0f;

            while (vTempAngles.Z > 180.0f) vTempAngles.Z -= 360.0f;

            while (vTempAngles.Z < -180.0f) vTempAngles.Z += 360.0f;

            return vTempAngles.Length();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 VectorMid(Vector3 min, Vector3 max)
        {
            return (max - min) / 2.0f + min;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 VectorAngles(Vector3 forward)
        {
            float yaw;
            float pitch;

            if (forward.X == 0 && forward.Y == 0)
            {
                yaw = 0;
                pitch = forward.Z > 0.0f ? 270.0f : 90.0f;
            }
            else
            {
                yaw = MathF.RadiansToDegrees((float) Math.Atan2(forward.Y, forward.X));

                if (yaw < 0) yaw += 360.0f;

                pitch = MathF.RadiansToDegrees((float) Math.Atan2(-forward.Z,
                    Math.Sqrt(forward.X * forward.X + forward.Y * forward.Y)));

                if (pitch < 0) pitch += 360;
            }

            return new Vector3(pitch, yaw, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 VectorTransform(Vector3 vec, Matrix3x4 matrix)
        {
            float x = vec.DotProduct(new Vector3(matrix.M11, matrix.M12, matrix.M13)) + matrix.M14;
            float y = vec.DotProduct(new Vector3(matrix.M21, matrix.M22, matrix.M23)) + matrix.M24;
            float z = vec.DotProduct(new Vector3(matrix.M31, matrix.M32, matrix.M33)) + matrix.M34;

            return new Vector3(x, y, z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 VectorRotatePosition(Vector3 targetOrigin, Vector3 positionToRotate, float rotationYaw)
        {
            var deltaTarget = positionToRotate - targetOrigin;

            float radius = (float) Math.Sqrt(deltaTarget.X * deltaTarget.X + deltaTarget.Y * deltaTarget.Y);

            float rotationYawRad = rotationYaw * MathF.Deg2Rad;

            deltaTarget.X = (float) Math.Sin(rotationYawRad) * radius;
            deltaTarget.Y = (float) Math.Cos(rotationYawRad) * radius;

            return new Vector3(deltaTarget.X + targetOrigin.X, deltaTarget.Y + targetOrigin.Y, positionToRotate.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 VectorYawRotate(Vector3 targetOrigin, Vector3 positionToRotate, Vector3 targetAngle,
            float rotationYaw)
        {
            float deltaX = positionToRotate.X - targetOrigin.X;
            float deltaY = positionToRotate.Y - targetOrigin.Y;

            float rotationAngle = rotationYaw - targetAngle.Y;

            float radianAngle = rotationAngle * MathF.Deg2Rad;

            float cosX = (float) (deltaX * Math.Cos(radianAngle));
            float cosY = (float) (deltaY * Math.Cos(radianAngle));

            float sinX = (float) (deltaX * Math.Sin(radianAngle));
            float sinY = (float) (deltaY * Math.Sin(radianAngle));

            float rotationX = cosX - sinY;
            float rotationY = cosY + sinX;

            positionToRotate.X += rotationX;
            positionToRotate.Y += rotationY;

            return positionToRotate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 WorldToScreen(Matrix4x4 viewMatrix, Vector3 vec, float screenX, float screenY)
        {
            var returnVector = new Vector2(0, 0);
            float w = viewMatrix.M41 * vec.X + viewMatrix.M42 * vec.Y + viewMatrix.M43 * vec.Z + viewMatrix.M44;

            if (!(w >= 0.01f)) return returnVector;

            float inverseX = 1f / w;

            returnVector.X =
                screenX / 2f +
                (0.5f * (
                     (viewMatrix.M11 * vec.X + viewMatrix.M12 * vec.Y + viewMatrix.M13 * vec.Z + viewMatrix.M14)
                     * inverseX)
                 * screenX + 0.5f);

            returnVector.Y =
                screenY / 2f -
                (0.5f * (
                     (viewMatrix.M21 * vec.X + viewMatrix.M22 * vec.Y + viewMatrix.M23 * vec.Z + viewMatrix.M24)
                     * inverseX)
                 * screenY + 0.5f);

            return returnVector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 WorldToScreen(Matrix4x4 viewMatrix, Vector3 vec, Vector2 screen)
        {
            var returnVector = new Vector2(0, 0);
            float w = viewMatrix.M41 * vec.X + viewMatrix.M42 * vec.Y + viewMatrix.M43 * vec.Z + viewMatrix.M44;

            if (!(w >= 0.01f)) return returnVector;

            float inverseX = 1f / w;

            returnVector.X =
                screen.X / 2f +
                (0.5f * (
                     (viewMatrix.M11 * vec.X + viewMatrix.M12 * vec.Y + viewMatrix.M13 * vec.Z + viewMatrix.M14)
                     * inverseX)
                 * screen.X + 0.5f);
            returnVector.Y =
                screen.Y / 2f -
                (0.5f * (
                     (viewMatrix.M21 * vec.X + viewMatrix.M22 * vec.Y + viewMatrix.M23 * vec.Z + viewMatrix.M24)
                     * inverseX)
                 * screen.Y + 0.5f);

            return returnVector;
        }
    }
}