using System;
using System.Collections.Generic;
using System.Text;

namespace VinnyLibConverterCommon.Transformation
{
    /// <summary>
    /// Вспомогательное определение кватерниона. Используется для задания информации об угле поворота в матрице трансформации.
    /// </summary>
    public class QuaternionInfo
    {
        public QuaternionInfo()
        {
            X = 0;
            Y = 0;
            Z = 0;
            W = 1;
        }

        /// <summary>
        /// Инициализация кватерниона по родным значениям-составляющим
        /// </summary>
        /// <param name="qx"></param>
        /// <param name="qy"></param>
        /// <param name="qz"></param>
        /// <param name="qw"></param>
        public QuaternionInfo(float qx, float qy, float qz, float qw)
        {
            this.X = qx;
            this.Y = qy;
            this.Z = qz;
            this.W = qw;
        }

        public static readonly QuaternionInfo Identity = new QuaternionInfo(0, 0, 0, 1);

        /// <summary>
        /// Создание кватерниона из углов эйлера (углы поворота осей координат XYZ)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public QuaternionInfo (float x = 0f, float y = 0f, float z = 0f)
        {
            float xRad = x * 0.5f;
            float yRad = y * 0.5f;
            float zRad = z * 0.5f;

            // Calculate trigonometric values
            float sinX = (float)Math.Sin(xRad);
            float cosX = (float)Math.Cos(xRad);
            float sinY = (float)Math.Sin(yRad);
            float cosY = (float)Math.Cos(yRad);
            float sinZ = (float)Math.Sin(zRad);
            float cosZ = (float)Math.Cos(zRad);

            // Calculate quaternion components
            float qx = sinX * cosY * cosZ - cosX * sinY * sinZ;
            float qy = cosX * sinY * cosZ + sinX * cosY * sinZ;
            float qz = cosX * cosY * sinZ - sinX * sinY * cosZ;
            float qw = cosX * cosY * cosZ + sinX * sinY * sinZ;

            this.X = qx;
            this.Y = qy;
            this.Z = qz;
            this.W = qw;
        }

        public static QuaternionInfo NormalToQuaternion(Vector3 normal, Vector3 upReference)
        {
            normal = normal.Normalized;
            upReference = upReference.Normalized;

            // Handle zero vector
            if (normal.Magnitude < 0.0001f)
                return QuaternionInfo.Identity;

            // Handle case where normal is parallel to up reference
            if (Math.Abs(Vector3.Dot(normal, upReference)) > 0.99f)
            {
                upReference = (Math.Abs(Vector3.Dot(normal, Vector3.Forward)) < 0.99f)
                    ? Vector3.Forward : Vector3.Right;
            }

            return LookRotation(normal, upReference);
        }

        public static QuaternionInfo NormalToQuaternion(Vector3 normal)
        {
            return NormalToQuaternion(normal, Vector3.Up);
        }

        public static QuaternionInfo LookRotation(Vector3 forward, Vector3 up)
        {
            forward = forward.Normalized;
            Vector3 right = Vector3.Cross(up.Normalized, forward).Normalized;
            up = Vector3.Cross(forward, right);

            float num1 = right.X + up.Y + forward.Z;
            QuaternionInfo quaternion = new QuaternionInfo();

            if (num1 > 0.0f)
            {
                float num2 = (float)Math.Sqrt(num1 + 1.0f);
                quaternion.W = num2 * 0.5f;
                float num3 = 0.5f / num2;
                quaternion.X = (up.Z - forward.Y) * num3;
                quaternion.Y = (forward.X - right.Z) * num3;
                quaternion.Z = (right.Y - up.X) * num3;
                return quaternion;
            }

            if (right.X >= up.Y && right.X >= forward.Z)
            {
                float num2 = (float)Math.Sqrt(1.0f + right.X - up.Y - forward.Z);
                float num3 = 0.5f / num2;
                quaternion.X = 0.5f * num2;
                quaternion.Y = (right.Y + up.X) * num3;
                quaternion.Z = (right.Z + forward.X) * num3;
                quaternion.W = (up.Z - forward.Y) * num3;
                return quaternion;
            }

            if (up.Y > forward.Z)
            {
                float num2 = (float)Math.Sqrt(1.0f + up.Y - right.X - forward.Z);
                float num3 = 0.5f / num2;
                quaternion.X = (up.X + right.Y) * num3;
                quaternion.Y = 0.5f * num2;
                quaternion.Z = (forward.Y + up.Z) * num3;
                quaternion.W = (forward.X - right.Z) * num3;
                return quaternion;
            }

            float num4 = (float)Math.Sqrt(1.0f + forward.Z - right.X - up.Y);
            float num5 = 0.5f / num4;
            quaternion.X = (forward.X + right.Z) * num5;
            quaternion.Y = (forward.Y + up.Z) * num5;
            quaternion.Z = 0.5f * num4;
            quaternion.W = (right.Y - up.X) * num5;
            return quaternion;
        }

        public static QuaternionInfo AngleAxis(float angle, Vector3 axis)
        {
            axis = axis.Normalized;
            float halfAngle = angle * 0.5f * (float)(Math.PI / 180.0);
            float sin = (float)Math.Sin(halfAngle);

            return new QuaternionInfo(
                axis.X * sin,
                axis.Y * sin,
                axis.Z * sin,
                (float)Math.Cos(halfAngle)
            );
        }

        public static QuaternionInfo FromToRotation(Vector3 fromDirection, Vector3 toDirection)
        {
            fromDirection = fromDirection.Normalized;
            toDirection = toDirection.Normalized;

            float dot = Vector3.Dot(fromDirection, toDirection);

            // If vectors are parallel, return identity or 180 degree rotation
            if (dot > 0.99999f)
                return QuaternionInfo.Identity;

            if (dot < -0.99999f)
            {
                // Find a perpendicular axis for 180 degree rotation
                Vector3 axis = Vector3.Cross(fromDirection, Vector3.Right);
                if (axis.Magnitude < 0.0001f)
                    axis = Vector3.Cross(fromDirection, Vector3.Up);
                return AngleAxis(180f, axis.Normalized);
            }

            Vector3 axisRotation = Vector3.Cross(fromDirection, toDirection).Normalized;
            float angle = (float)Math.Acos(dot) * (180f / (float)Math.PI);

            return AngleAxis(angle, axisRotation);
        }

        public static Vector3 operator *(QuaternionInfo rotation, Vector3 point)
        {
            float num1 = rotation.X * 2f;
            float num2 = rotation.Y * 2f;
            float num3 = rotation.Z * 2f;
            float num4 = rotation.X * num1;
            float num5 = rotation.Y * num2;
            float num6 = rotation.Z * num3;
            float num7 = rotation.X * num2;
            float num8 = rotation.X * num3;
            float num9 = rotation.Y * num3;
            float num10 = rotation.W * num1;
            float num11 = rotation.W * num2;
            float num12 = rotation.W * num3;

            return new Vector3(
                (1f - (num5 + num6)) * point.X + (num7 - num12) * point.Y + (num8 + num11) * point.Z,
                (num7 + num12) * point.X + (1f - (num4 + num6)) * point.Y + (num9 - num10) * point.Z,
                (num8 - num11) * point.X + (num9 + num10) * point.Y + (1f - (num4 + num5)) * point.Z
            );
        }

        public void Normalize()
        {
            float magnitude = Convert.ToSingle(Math.Sqrt(X * X + Y * Y + Z * Z + W * W));
            if (magnitude > 0)
            {
                X /= magnitude;
                Y /= magnitude;
                Z /= magnitude;
                W /= magnitude;
            }
        }

        /// <summary>
        /// Возвращает углы Эйлера вдоль осей XYZ из данного определения кватерниона
        /// </summary>
        /// <returns></returns>
        public float[] GetEulerAnglesRadians()
        {
            Normalize();

            // Roll (x-axis rotation)
            float sinr_cosp = 2f * (this.W * this.X + this.Y * this.Z);
            float cosr_cosp = 1f - 2f * (this.X * this.X + this.Y * this.Y);
            float roll = (float)Math.Atan2(sinr_cosp, cosr_cosp);

            // Pitch (y-axis rotation)
            float sinp = 2f * (this.W * this.Y - this.Z * this.X);
            float pitch;
            if (Math.Abs(sinp) >= 1f)
            {
                if (sinp > 0) pitch = (float)Math.PI / 2f;
                else pitch = -(float)Math.PI / 2f;

                //pitch = (float)Math.CopySign(Math.PI / 2f, sinp); // Use 90 degrees if out of range
            }

            else pitch = (float)Math.Asin(sinp);

            // Yaw (z-axis rotation)
            float siny_cosp = 2f * (this.W * this.Z + this.X * this.Y);
            float cosy_cosp = 1f - 2f * (this.Y * this.Y + this.Z * this.Z);
            float yaw = (float)Math.Atan2(siny_cosp, cosy_cosp);

            return new float[] { roll, pitch, yaw };
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }
    }
}
