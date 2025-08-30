using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace VinnyLibConverterCommon.Transformation
{
    /// <summary>
    /// Вспомогательный класс для работы с матрицами
    /// </summary>
    public class MatrixImpl
    {
        public int Rows { get; }
        public int Columns { get; }

        private MatrixImpl() { }
        public MatrixImpl(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            Matrix = new float[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Matrix[i, j] = 0;
                    if (i == j) Matrix[i, j] = 1;

                }
            }
        }

        public MatrixImpl(float[,] matrix, int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            Matrix = matrix;
        }


        #region Процедуры для работы с матрицами
        public static MatrixImpl CreateIdentityMatrix(int size)
        {
            var result = new MatrixImpl(size, size);
            for (int i = 0; i < size; i++)
            {
                result[i, i] = 1;
            }
            return result;
        }

        public static MatrixImpl Add(MatrixImpl a, MatrixImpl b)
        {
            if (a.Rows != b.Rows || a.Columns != b.Columns)
                throw new ArgumentException("Matrices must have the same dimensions");

            var result = new MatrixImpl(a.Rows, a.Columns);
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < a.Columns; j++)
                {
                    result[i, j] = a[i, j] + b[i, j];
                }
            }
            return result;
        }

        public static MatrixImpl Multiply(MatrixImpl a, MatrixImpl b)
        {
            if (a.Columns != b.Rows)
                throw new ArgumentException("Number of columns in first matrix must equal number of rows in second matrix");

            var result = new MatrixImpl(a.Rows, b.Columns);
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

        public static MatrixImpl ScalarMultiply(MatrixImpl matrix, float scalar)
        {
            var result = new MatrixImpl(matrix.Rows, matrix.Columns);
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    result[i, j] = matrix[i, j] * scalar;
                }
            }
            return result;
        }

        public MatrixImpl Transpose()
        {
            var result = new MatrixImpl(Columns, Rows);
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

        #region Вспомогательные методы
        public static MatrixImpl CreateFromQuaternion(QuaternionInfo q)
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

            var matrix = new MatrixImpl(4, 4);

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

        public QuaternionInfo ExtractRotationQuaternion()
        {
            float trace = this[0, 0] + this[1, 1] + this[2, 2];
            QuaternionInfo q = new QuaternionInfo();

            if (trace > 0)
            {
                float s = 0.5f / Convert.ToSingle(Math.Sqrt(trace + 1.0));
                q.W = 0.25f / s;
                q.X = (this[2, 1] - this[1, 2]) * s;
                q.Y = (this[0, 2] - this[2, 0]) * s;
                q.Z = (this[1, 0] - this[0, 1]) * s;
            }
            else
            {
                if (this[0, 0] > this[1, 1] && this[0, 0] > this[2, 2])
                {
                    float s = 2.0f * Convert.ToSingle(Math.Sqrt(1.0 + this[0, 0] - this[1, 1] - this[2, 2]));
                    q.W = (this[2, 1] - this[1, 2]) / s;
                    q.X = 0.25f * s;
                    q.Y = (this[0, 1] + this[1, 0]) / s;
                    q.Z = (this[0, 2] + this[2, 0]) / s;
                }
                else if (this[1, 1] > this[2, 2])
                {
                    float s = 2.0f * Convert.ToSingle(Math.Sqrt(1.0 + this[1, 1] - this[0, 0] - this[2, 2]));
                    q.W = (this[0, 2] - this[2, 0]) / s;
                    q.X = (this[0, 1] + this[1, 0]) / s;
                    q.Y = 0.25f * s;
                    q.Z = (this[1, 2] + this[2, 1]) / s;
                }
                else
                {
                    float s = 2.0f * Convert.ToSingle(Math.Sqrt(1.0 + this[2, 2] - this[0, 0] - this[1, 1]));
                    q.W = (this[1, 0] - this[0, 1]) / s;
                    q.X = (this[0, 2] + this[2, 0]) / s;
                    q.Y = (this[1, 2] + this[2, 1]) / s;
                    q.Z = 0.25f * s;
                }
            }

            q.Normalize();
            return q;
        }
        #endregion


        /// <summary>
        /// 4x4 transformation matrix
        /// https://ru.wikipedia.org/wiki/%D0%9C%D0%B0%D1%82%D1%80%D0%B8%D1%86%D0%B0_%D0%BF%D0%B5%D1%80%D0%B5%D1%85%D0%BE%D0%B4%D0%B0
        /// </summary>
        public float[,] Matrix { get; private set; }

        public float this[int row, int col]
        {
            get => Matrix[row, col];
            set => Matrix[row, col] = value;
        }
    }
}
