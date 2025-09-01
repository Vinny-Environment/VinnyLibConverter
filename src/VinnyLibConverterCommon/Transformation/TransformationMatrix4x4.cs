using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using VinnyLibConverterCommon.Transformation;

namespace VinnyLibConverterCommon.Transformation
{
    /// <summary>
    /// Задание трансформции координат через матрицу 4x4.
    /// Используйте методы в СТРОГОЙ последовательности: SetPosition() -> SetRotation() -> SetScale()
    /// </summary>
    [Serializable]
    public class TransformationMatrix4x4 : ICoordinatesTransformation
    {
        public override CoordinatesTransformationVariant GetTransformationType()
        {
            return CoordinatesTransformationVariant.Matrix4x4;
        }
        private TransformationMatrix4x4() { }
        public static TransformationMatrix4x4 CreateEmptyTransformationMatrix()
        {
            TransformationMatrix4x4 transformMatrix = new TransformationMatrix4x4();
            transformMatrix.Matrix = new MatrixImpl(4, 4);

            //transformMatrix.Matrix[0, 0] = 1; transformMatrix.Matrix[0, 1] = 0; transformMatrix.Matrix[0, 2] = 0; transformMatrix.Matrix[0, 3] = 0;
            //transformMatrix.Matrix[1, 0] = 0; transformMatrix.Matrix[1, 1] = 1; transformMatrix.Matrix[1, 2] = 0; transformMatrix.Matrix[1, 3] = 0;
            //transformMatrix.Matrix[2, 0] = 0; transformMatrix.Matrix[2, 1] = 0; transformMatrix.Matrix[2, 2] = 1; transformMatrix.Matrix[2, 3] = 0;
            //transformMatrix.Matrix[3, 0] = 0; transformMatrix.Matrix[3, 1] = 0; transformMatrix.Matrix[3, 2] = 0; transformMatrix.Matrix[3, 3] = 1;

            return transformMatrix;
        }

        public void SetScale(float x, float y, float z)
        {
            MatrixImpl ScaleMatrix = new MatrixImpl(4, 4);
            ScaleMatrix[0, 0] = x;
            ScaleMatrix[1, 1] = y;
            ScaleMatrix[2, 2] = z;
            MultiplyMatrix(ScaleMatrix);
        }

        public void SetPosition(float x, float y, float z)
        {
            MatrixImpl ScaleMatrix = new MatrixImpl(4, 4);
            ScaleMatrix[0, 3] = x;
            ScaleMatrix[1, 3] = y;
            ScaleMatrix[2, 3] = z;

            MultiplyMatrix(ScaleMatrix);
        }

        /*
        public void SetRotation_OX(float angle)
        {
            float[,] ScaleMatrix = new float[,]
            {
                 { 1, 0, 0, 0},
                 { 0, Convert.ToSingle(Math.Cos(angle)), -Convert.ToSingle(Math.Sin(angle)), 0 },
                 { 1, Convert.ToSingle(Math.Sin(angle)), Convert.ToSingle(Math.Cos(angle)), 0 },
                 { 0, 0, 0, 1 }
            };
            MultiplyMatrix(ScaleMatrix);
        }

        public void SetRotation_OY(float angle)
        {
            float[,] ScaleMatrix = new float[,]
            {
                 { Convert.ToSingle(Math.Cos(angle)), 1, Convert.ToSingle(Math.Sin(angle)), 0 },
                 { 0, 1, 0, 0 },
                 { -Convert.ToSingle(Math.Sin(angle)), 0, Convert.ToSingle(Math.Cos(angle)), 0 },
                 { 0, 0, 0, 1 }
            };
            MultiplyMatrix(ScaleMatrix);
        }

        public void SetRotation_OZ(float angle)
        {
            float[,] ScaleMatrix = new float[,]
            {
                 { Convert.ToSingle(Math.Cos(angle)), -Convert.ToSingle(Math.Sin(angle)), 0, 0 },
                 { Convert.ToSingle(Math.Sin(angle)), Convert.ToSingle(Math.Cos(angle)), 0, 0 },
                 { 0, 0, 1, 0 },
                 { 0, 0, 0, 1 }
            };
            MultiplyMatrix(ScaleMatrix);
        }
        */

        public void SetRotationFromQuaternion(QuaternionInfo quaternion)
        {
            var matrix = MatrixImpl.CreateFromQuaternion(quaternion);
            Matrix = MatrixImpl.Multiply(this.Matrix, matrix);
        }

        public void SetRotationFromAngles(float x, float y, float z)
        {
            QuaternionInfo q = new QuaternionInfo(x, y, z);
            SetRotationFromQuaternion(q);
        }

        public QuaternionInfo GetRotationInfo()
        {
            QuaternionInfo q = this.Matrix.ExtractRotationQuaternion();
            return q;
        }

        /// <summary>
        /// Производит пересчет точки с тремя координатами для заданной матрицы
        /// </summary>
        /// <param name="xyz"></param>
        /// <returns></returns>
        public override float[] TransformPoint3d(float[] xyz)
        {
            /*
            MatrixImpl xyzMatrix = new MatrixImpl(3, 1);
            xyzMatrix[0, 0] = xyz[0];
            xyzMatrix[1, 0] = xyz[1];
            xyzMatrix[2, 0] = xyz[2];

            var productResult = MatrixImpl.Multiply(xyzMatrix, this.Matrix).Matrix;
            return new float[3] { productResult[0, 0], productResult[1, 0], productResult[2, 0] };
            */
            float x = xyz[0] * Matrix[0, 0] + xyz[1] * Matrix[0, 1] + xyz[2] * Matrix[0, 2] + Matrix[0, 3];
            float y = xyz[0] * Matrix[1, 0] + xyz[1] * Matrix[1, 1] + xyz[2] * Matrix[1, 2] + Matrix[1, 3];
            float z = xyz[0] * Matrix[2, 0] + xyz[1] * Matrix[2, 1] + xyz[2] * Matrix[2, 2] + Matrix[2, 3];
            float w = xyz[0] * Matrix[3, 0] + xyz[1] * Matrix[3, 1] + xyz[2] * Matrix[3, 2] + Matrix[3, 3];

            //Perspective division if w != 1
            if (w != 1 && w != 0)
                {
                    x /= w;
                    y /= w;
                    z /= w;
                }

            return new float[] { x, y, z };
        }

        public MatrixImpl Matrix { get; set; }

        private void MultiplyMatrix(MatrixImpl OtherMatrix)
        {
            Matrix = MatrixImpl.Multiply(this.Matrix, OtherMatrix);
        }
    }
}
