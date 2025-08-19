using System;
using System.Collections.Generic;
using System.Text;

namespace VinnyLibConverterCommon.Transformation
{
    public class QuaternionInfo
    {
        public QuaternionInfo()
        {
            X = 0;
            Y = 0;
            Z = 0;
            W = 1;
        }

        public QuaternionInfo(float qx, float qy, float qz, float qw)
        {
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

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }
    }
}
