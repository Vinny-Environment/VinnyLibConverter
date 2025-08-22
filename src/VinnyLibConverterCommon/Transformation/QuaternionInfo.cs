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
