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
        public QuaternionInfo(double qx, double qy, double qz, double qw)
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
        public QuaternionInfo (double x = 0f, double y = 0f, double z = 0f)
        {
            double xRad = x * 0.5f;
            double yRad = y * 0.5f;
            double zRad = z * 0.5f;

            // Calculate trigonometric values
            double sinX = (double)Math.Sin(xRad);
            double cosX = (double)Math.Cos(xRad);
            double sinY = (double)Math.Sin(yRad);
            double cosY = (double)Math.Cos(yRad);
            double sinZ = (double)Math.Sin(zRad);
            double cosZ = (double)Math.Cos(zRad);

            // Calculate quaternion components
            double qx = sinX * cosY * cosZ - cosX * sinY * sinZ;
            double qy = cosX * sinY * cosZ + sinX * cosY * sinZ;
            double qz = cosX * cosY * sinZ - sinX * sinY * cosZ;
            double qw = cosX * cosY * cosZ + sinX * sinY * sinZ;

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

            double num1 = right.X + up.Y + forward.Z;
            QuaternionInfo quaternion = new QuaternionInfo();

            if (num1 > 0.0f)
            {
                double num2 = (double)Math.Sqrt(num1 + 1.0f);
                quaternion.W = num2 * 0.5f;
                double num3 = 0.5f / num2;
                quaternion.X = (up.Z - forward.Y) * num3;
                quaternion.Y = (forward.X - right.Z) * num3;
                quaternion.Z = (right.Y - up.X) * num3;
                return quaternion;
            }

            if (right.X >= up.Y && right.X >= forward.Z)
            {
                double num2 = (double)Math.Sqrt(1.0f + right.X - up.Y - forward.Z);
                double num3 = 0.5f / num2;
                quaternion.X = 0.5f * num2;
                quaternion.Y = (right.Y + up.X) * num3;
                quaternion.Z = (right.Z + forward.X) * num3;
                quaternion.W = (up.Z - forward.Y) * num3;
                return quaternion;
            }

            if (up.Y > forward.Z)
            {
                double num2 = (double)Math.Sqrt(1.0f + up.Y - right.X - forward.Z);
                double num3 = 0.5f / num2;
                quaternion.X = (up.X + right.Y) * num3;
                quaternion.Y = 0.5f * num2;
                quaternion.Z = (forward.Y + up.Z) * num3;
                quaternion.W = (forward.X - right.Z) * num3;
                return quaternion;
            }

            double num4 = (double)Math.Sqrt(1.0f + forward.Z - right.X - up.Y);
            double num5 = 0.5f / num4;
            quaternion.X = (forward.X + right.Z) * num5;
            quaternion.Y = (forward.Y + up.Z) * num5;
            quaternion.Z = 0.5f * num4;
            quaternion.W = (right.Y - up.X) * num5;
            return quaternion;
        }

        public static QuaternionInfo AngleAxis(double angle, Vector3 axis)
        {
            axis = axis.Normalized;
            double halfAngle = angle * 0.5f * (double)(Math.PI / 180.0);
            double sin = (double)Math.Sin(halfAngle);

            return new QuaternionInfo(
                axis.X * sin,
                axis.Y * sin,
                axis.Z * sin,
                (double)Math.Cos(halfAngle)
            );
        }

        public static QuaternionInfo FromToRotation(Vector3 fromDirection, Vector3 toDirection)
        {
            fromDirection = fromDirection.Normalized;
            toDirection = toDirection.Normalized;

            double dot = Vector3.Dot(fromDirection, toDirection);

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
            double angle = (double)Math.Acos(dot) * (180f / (double)Math.PI);

            return AngleAxis(angle, axisRotation);
        }

        public static Vector3 operator *(QuaternionInfo rotation, Vector3 point)
        {
            double num1 = rotation.X * 2f;
            double num2 = rotation.Y * 2f;
            double num3 = rotation.Z * 2f;
            double num4 = rotation.X * num1;
            double num5 = rotation.Y * num2;
            double num6 = rotation.Z * num3;
            double num7 = rotation.X * num2;
            double num8 = rotation.X * num3;
            double num9 = rotation.Y * num3;
            double num10 = rotation.W * num1;
            double num11 = rotation.W * num2;
            double num12 = rotation.W * num3;

            return new Vector3(
                (1f - (num5 + num6)) * point.X + (num7 - num12) * point.Y + (num8 + num11) * point.Z,
                (num7 + num12) * point.X + (1f - (num4 + num6)) * point.Y + (num9 - num10) * point.Z,
                (num8 - num11) * point.X + (num9 + num10) * point.Y + (1f - (num4 + num5)) * point.Z
            );
        }

        public void Normalize()
        {
            double magnitude = Convert.ToDouble(Math.Sqrt(X * X + Y * Y + Z * Z + W * W));
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
        public double[] GetEulerAnglesRadians()
        {
            Normalize();

            // Roll (x-axis rotation)
            double sinr_cosp = 2f * (this.W * this.X + this.Y * this.Z);
            double cosr_cosp = 1f - 2f * (this.X * this.X + this.Y * this.Y);
            double roll = (double)Math.Atan2(sinr_cosp, cosr_cosp);

            // Pitch (y-axis rotation)
            double sinp = 2f * (this.W * this.Y - this.Z * this.X);
            double pitch;
            if (Math.Abs(sinp) >= 1f)
            {
                if (sinp > 0) pitch = (double)Math.PI / 2f;
                else pitch = -(double)Math.PI / 2f;

                //pitch = (double)Math.CopySign(Math.PI / 2f, sinp); // Use 90 degrees if out of range
            }

            else pitch = (double)Math.Asin(sinp);

            // Yaw (z-axis rotation)
            double siny_cosp = 2f * (this.W * this.Z + this.X * this.Y);
            double cosy_cosp = 1f - 2f * (this.Y * this.Y + this.Z * this.Z);
            double yaw = (double)Math.Atan2(siny_cosp, cosy_cosp);

            return new double[] { roll, pitch, yaw };
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double W { get; set; }
    }
}
