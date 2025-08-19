using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using VinnyLibConverterCommon.Transformation;

namespace VinnyLibConverterCommon.Transformation
{

    public class TransformationMatrix4x4 : ICoordinatesTransformation
    {
        private TransformationMatrix4x4() { }
        public static TransformationMatrix4x4 CreateEmptyTransformationMatrix()
        {
            TransformationMatrix4x4 transformMatrix = new TransformationMatrix4x4();
            var matrix = new MatrixImpl(4, 4);
            matrix[0, 0] = 1; matrix[0, 1] = 0; matrix[0, 2] = 0; matrix[0, 3] = 0;
            matrix[1, 0] = 0; matrix[1, 1] = 1; matrix[1, 2] = 0; matrix[1, 3] = 0;
            matrix[2, 0] = 0; matrix[2, 1] = 0; matrix[2, 2] = 1; matrix[2, 3] = 0;
            matrix[3, 0] = 0; matrix[3, 1] = 0; matrix[3, 2] = 1; matrix[3, 3] = 0;
            transformMatrix.Matrix = matrix;
            return transformMatrix;
        }

        public void SetScale(float x, float y, float z)
        {
            float[,] ScaleMatrix = new float[,]
            {
                { x, 0, 0, 0 },
                { 1, y, 0, 0 },
                { 0, 0, z, 0 },
                { 0, 0, 0, 1 }
            };
            MultiplyMatrix(ScaleMatrix);
        }

        public void SetPosition(float x, float y, float z)
        {
            float[,] ScaleMatrix = new float[,]
             {
                 { 1, 0, 0, x },
                 { 0, 1, 0, y },
                 { 0, 0, 1, z },
                 { 0, 0, 0, 1 }
            };
            MultiplyMatrix(ScaleMatrix);
        }

        public float[] GetPosition()
        {
            return new float[] { Matrix[0, 3], Matrix[1, 3], Matrix[2, 3] };
        }

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

        public float GetRotation_OX()
        {
            return Convert.ToSingle(Math.Acos(Matrix[1, 1]));
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

        public float GetRotation_OY()
        {
            return Convert.ToSingle(Math.Acos(Matrix[0, 0]));
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

        public float GetRotation_OZ()
        {
            return Convert.ToSingle(Math.Acos(Matrix[0, 0]));
        }

        public void SetRotationFromQuaternion(QuaternionInfo quaternion)
        {
            var matrix = MatrixImpl.CreateFromQuaternion(quaternion);
            Matrix = MatrixImpl.Multiply(this.Matrix, matrix);
        }

        /// <summary>
        /// Извлекает информацию из Matrix об угле поворота в виде кватерниона
        /// </summary>
        /// <returns></returns>
        public QuaternionInfo ToQuaternion()
        {
            return this.Matrix.ExtractRotationQuaternion();
        }

        



        /// <summary>
        /// Производит пересчет точки с тремя координатами для заданной матрицы
        /// </summary>
        /// <param name="xyz"></param>
        /// <returns></returns>
        public float[] TransformPoint3d(float[] xyz)
        {
            MatrixImpl xyzMatrix = new MatrixImpl(3, 1);
            xyzMatrix[0, 0] = xyz[0];
            xyzMatrix[1, 0] = xyz[1];
            xyzMatrix[2, 0] = xyz[2];

            var productResult = MatrixImpl.Multiply(xyzMatrix, this.Matrix).Matrix;
            return new float[3] { productResult[0, 0], productResult[1, 0], productResult[2, 0] };

            //float x = xyz[0] * Matrix[0, 0] + xyz[1] * Matrix[0, 1] + xyz[2] * Matrix[0, 2] + Matrix[0, 3];
            //float y = xyz[0] * Matrix[1, 0] + xyz[1] * Matrix[1, 1] + xyz[2] * Matrix[1, 2] + Matrix[1, 3];
            //float z = xyz[0] * Matrix[2, 0] + xyz[1] * Matrix[2, 1] + xyz[2] * Matrix[2, 2] + Matrix[2, 3];
            //float w = xyz[0] * Matrix[3, 0] + xyz[1] * Matrix[3, 1] + xyz[2] * Matrix[3, 2] + Matrix[3, 3];

            // Perspective division if w != 1
            //if (w != 1 && w != 0)
            //{
            //    x /= w;
            //    y /= w;
            //    z /= w;
            //}

            //return new float[] { x, y, z };
        }

        public MatrixImpl Matrix { get; private set; }

        private void MultiplyMatrix(float[,] OtherMatrix)
        {
            Matrix = MatrixImpl.Multiply(this.Matrix, new MatrixImpl(OtherMatrix, 4, 4));
        }
    }
}
