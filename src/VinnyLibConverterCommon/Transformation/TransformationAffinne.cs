using System;
using System.Collections.Generic;
using System.Text;

namespace VinnyLibConverterCommon.Transformation
{
    public class TransformationAffinne : ICoordinatesTransformation
    {
        private TransformationAffinne() { }
        public TransformationAffinne(float A, float B, float C, float A2, float B2, float C2)
        {
            this.pA = A;
            this.pA2 = A2;
            this.pB = B;
            this.pB2 = B2;
            this.pC = C;
            this.pC2 = C2;

            //TODO: проверить, что это действительно те параметры (названия)
            this.Omega = A;
            this.Tx = A2;
            this.Phi = B;
            this.Ty = B2;
            this.Kappa = C;
            this.Tz = C2;

            

            // Rotation matrices
            double cosΩ = Math.Cos(Omega);
            double sinΩ = Math.Sin(Omega);
            double cosΦ = Math.Cos(Phi);
            double sinΦ = Math.Sin(Phi);
            double cosΚ = Math.Cos(Kappa);
            double sinΚ = Math.Sin(Kappa);

            pMatrix = new MatrixImpl(3, 3);

            // Combined rotation matrix (R = Rz * Ry * Rx)
            pMatrix[0, 0] = Convert.ToSingle(cosΦ * cosΚ);
            pMatrix[0, 1] = Convert.ToSingle(cosΩ * sinΚ + sinΩ * sinΦ * cosΚ);
            pMatrix[0, 2] = Convert.ToSingle(sinΩ * sinΚ - cosΩ * sinΦ * cosΚ);

            pMatrix[1, 0] = Convert.ToSingle(-cosΦ * sinΚ);
            pMatrix[1, 1] = Convert.ToSingle(cosΩ * cosΚ - sinΩ * sinΦ * sinΚ);
            pMatrix[1, 2] = Convert.ToSingle(sinΩ * cosΚ + cosΩ * sinΦ * sinΚ);

            pMatrix[2, 0] = Convert.ToSingle(sinΦ);
            pMatrix[2, 1] = Convert.ToSingle(-sinΩ * cosΦ);
            pMatrix[2, 2] = Convert.ToSingle(cosΩ * cosΦ);
        }

        public float[] TransformPoint3d(float[] xyz)
        {
            float x = pMatrix[0, 0] * xyz[0] + pMatrix[0, 1] * xyz[1] + pMatrix[0, 2] * xyz[2] + Tx;
            float y = pMatrix[1, 0] * xyz[0] + pMatrix[1, 1] * xyz[1] + pMatrix[1, 2] * xyz[2] + Ty;
            float z = pMatrix[2, 0] * xyz[0] + pMatrix[2, 1] * xyz[1] + pMatrix[2, 2] * xyz[2] + Tz;


            return new float[] { x, y, z };
        }

        private MatrixImpl pMatrix;

        //Rotation parameters(in radians)
        private float Omega { get; set; }   // Rotation around X-axis
        private float Phi { get; set; }     // Rotation around Y-axis
        private float Kappa { get; set; }  // Rotation around Z-axis

        // Translation parameters
        private float Tx { get; set; }      // Translation in X
        private float Ty { get; set; }      // Translation in Y
        private float Tz { get; set; }      // Translation in Z

        private float pA;
        private float pA2;
        private float pB;
        private float pB2;
        private float pC;
        private float pC2;
    }
}
