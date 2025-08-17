using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace VinnyLibConverterCommon
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

    public class TransformationMatrix
    {
        //public const int mMatrixSize = 4;
        public int Rows { get; }
        public int Columns { get; }

        private TransformationMatrix() { }
        public TransformationMatrix(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            Matrix = new float[rows, columns];
        }

        public TransformationMatrix(float[,] matrix)
        {
            Matrix = matrix;
        }

        public static TransformationMatrix CreateEmptyTransformationMatrix()
        {
            var matrix = new TransformationMatrix(4, 4);
            matrix[0, 0] = 1; matrix[0, 1] = 0; matrix[0, 2] = 0; matrix[0, 3] = 0;
            matrix[1, 0] = 0; matrix[1, 1] = 1; matrix[1, 2] = 0; matrix[1, 3] = 0;
            matrix[2, 0] = 0; matrix[2, 1] = 0; matrix[2, 2] = 1; matrix[2, 3] = 0;
            matrix[3, 0] = 0; matrix[3, 1] = 0; matrix[3, 2] = 1; matrix[3, 3] = 0;
            return matrix;
        }

        public static TransformationMatrix CreateFromQuaternion(QuaternionInfo q)
        {
            // Normalize the quaternion first
            q.Normalize();

            // Calculate the rotation matrix elements
            float xx = q.X * q.X;
            float xy = q.X * q.Y;
            float xz = q.X * q.Z;
            float xw = q.X * q.W;

            float yy = q.Y * q.Y;
            float yz = q.Y * q.Z;
            float yw = q.Y * q.W;

            float zz = q.Z * q.Z;
            float zw = q.Z * q.W;

            var matrix = new TransformationMatrix(4, 4);

            // First row
            matrix[0, 0] = 1 - 2 * (yy + zz);
            matrix[0, 1] = 2 * (xy - zw);
            matrix[0, 2] = 2 * (xz + yw);
            matrix[0, 3] = 0;

            // Second row
            matrix[1, 0] = 2 * (xy + zw);
            matrix[1, 1] = 1 - 2 * (xx + zz);
            matrix[1, 2] = 2 * (yz - xw);
            matrix[1, 3] = 0;

            // Third row
            matrix[2, 0] = 2 * (xz - yw);
            matrix[2, 1] = 2 * (yz + xw);
            matrix[2, 2] = 1 - 2 * (xx + yy);
            matrix[2, 3] = 0;

            // Fourth row (translation/identity)
            matrix[3, 0] = 0;
            matrix[3, 1] = 0;
            matrix[3, 2] = 0;
            matrix[3, 3] = 1;

            return matrix;
        }

        public static QuaternionInfo ExtractRotationQuaternion(TransformationMatrix matrix)
        {
            float trace = matrix[0, 0] + matrix[1, 1] + matrix[2, 2];
            QuaternionInfo q = new QuaternionInfo();

            if (trace > 0)
            {
                float s = 0.5f / Convert.ToSingle(Math.Sqrt(trace + 1.0));
                q.W = 0.25f / s;
                q.X = (matrix[2, 1] - matrix[1, 2]) * s;
                q.Y = (matrix[0, 2] - matrix[2, 0]) * s;
                q.Z = (matrix[1, 0] - matrix[0, 1]) * s;
            }
            else
            {
                if (matrix[0, 0] > matrix[1, 1] && matrix[0, 0] > matrix[2, 2])
                {
                    float s = 2.0f * Convert.ToSingle(Math.Sqrt(1.0 + matrix[0, 0] - matrix[1, 1] - matrix[2, 2]));
                    q.W = (matrix[2, 1] - matrix[1, 2]) / s;
                    q.X = 0.25f * s;
                    q.Y = (matrix[0, 1] + matrix[1, 0]) / s;
                    q.Z = (matrix[0, 2] + matrix[2, 0]) / s;
                }
                else if (matrix[1, 1] > matrix[2, 2])
                {
                    float s = 2.0f * Convert.ToSingle(Math.Sqrt(1.0 + matrix[1, 1] - matrix[0, 0] - matrix[2, 2]));
                    q.W = (matrix[0, 2] - matrix[2, 0]) / s;
                    q.X = (matrix[0, 1] + matrix[1, 0]) / s;
                    q.Y = 0.25f * s;
                    q.Z = (matrix[1, 2] + matrix[2, 1]) / s;
                }
                else
                {
                    float s = 2.0f * Convert.ToSingle(Math.Sqrt(1.0 + matrix[2, 2] - matrix[0, 0] - matrix[1, 1]));
                    q.W = (matrix[1, 0] - matrix[0, 1]) / s;
                    q.X = (matrix[0, 2] + matrix[2, 0]) / s;
                    q.Y = (matrix[1, 2] + matrix[2, 1]) / s;
                    q.Z = 0.25f * s;
                }
            }

            q.Normalize();
            return q;
        }


        #region Процедуры для работы с матрицами
        public static TransformationMatrix CreateIdentityMatrix(int size)
        {
            var result = new TransformationMatrix(size, size);
            for (int i = 0; i < size; i++)
            {
                result[i, i] = 1;
            }
            return result;
        }

        public static TransformationMatrix Add(TransformationMatrix a, TransformationMatrix b)
        {
            if (a.Rows != b.Rows || a.Columns != b.Columns)
                throw new ArgumentException("Matrices must have the same dimensions");

            var result = new TransformationMatrix(a.Rows, a.Columns);
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < a.Columns; j++)
                {
                    result[i, j] = a[i, j] + b[i, j];
                }
            }
            return result;
        }

        public static TransformationMatrix Multiply(TransformationMatrix a, TransformationMatrix b)
        {
            if (a.Columns != b.Rows)
                throw new ArgumentException("Number of columns in first matrix must equal number of rows in second matrix");

            var result = new TransformationMatrix(a.Rows, b.Columns);
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < b.Columns; j++)
                {
                    float sum = 0;
                    for (int k = 0; k < a.Columns; k++)
                    {
                        sum += a[i, k] * b[k, j];
                    }
                    result[i, j] = sum;
                }
            }
            return result;
        }

        public static TransformationMatrix ScalarMultiply(TransformationMatrix matrix, float scalar)
        {
            var result = new TransformationMatrix(matrix.Rows, matrix.Columns);
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    result[i, j] = matrix[i, j] * scalar;
                }
            }
            return result;
        }

        public TransformationMatrix Transpose()
        {
            var result = new TransformationMatrix(Columns, Rows);
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    result[j, i] = this[i, j];
                }
            }
            return result;
        }

        public override string ToString()
        {
            string result = "";
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    result += $"{this[i, j]:0.##}\t";
                }
                result += Environment.NewLine;
            }
            return result;
        }
        #endregion

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
            var matrix = CreateFromQuaternion(quaternion);
            Matrix = TransformationMatrix.Multiply(this, matrix).Matrix;
        }

        /// <summary>
        /// Извлекает информацию из Matrix об угле поворота в виде кватерниона
        /// </summary>
        /// <returns></returns>
        public QuaternionInfo ToQuaternion()
        {
            return ExtractRotationQuaternion(this);
        }

        



        /// <summary>
        /// Производит пересчет точки с тремя координатами для заданной матрицы
        /// </summary>
        /// <param name="xyz"></param>
        /// <returns></returns>
        public float[] TransformPoint3d(float[] xyz)
        {
            //TransformationMatrix xyzMatrix = new TransformationMatrix(3, 1);
            //xyzMatrix[0, 0] = xyz[0];
            //xyzMatrix[1, 0] = xyz[1];
            //xyzMatrix[2, 0] = xyz[2];

            //var productResult = TransformationMatrix.Multiply(xyzMatrix, this).Matrix;
            //return new float[3] { productResult[0, 0], productResult[1, 0], productResult[2, 0] };

            float x = xyz[0] * Matrix[0, 0] + xyz[1] * Matrix[0, 1] + xyz[2] * Matrix[0, 2] + Matrix[0, 3];
            float y = xyz[0] * Matrix[1, 0] + xyz[1] * Matrix[1, 1] + xyz[2] * Matrix[1, 2] + Matrix[1, 3];
            float z = xyz[0] * Matrix[2, 0] + xyz[1] * Matrix[2, 1] + xyz[2] * Matrix[2, 2] + Matrix[2, 3];
            float w = xyz[0] * Matrix[3, 0] + xyz[1] * Matrix[3, 1] + xyz[2] * Matrix[3, 2] + Matrix[3, 3];

            // Perspective division if w != 1
            if (w != 1 && w != 0)
            {
                x /= w;
                y /= w;
                z /= w;
            }

            return new float[] { x, y, z };
        }

        /// <summary>
        /// 4x4 transformation matrix
        /// https://ru.wikipedia.org/wiki/%D0%9C%D0%B0%D1%82%D1%80%D0%B8%D1%86%D0%B0_%D0%BF%D0%B5%D1%80%D0%B5%D1%85%D0%BE%D0%B4%D0%B0
        /// </summary>
        public float[,] Matrix { get; private set; }

        private void MultiplyMatrix(float[,] OtherMatrix)
        {
            Matrix = TransformationMatrix.Multiply(this, new TransformationMatrix(OtherMatrix)).Matrix;
        }

        private float this[int row, int col]
        {
            get => Matrix[row, col];
            set => Matrix[row, col] = value;
        }
    }
}
