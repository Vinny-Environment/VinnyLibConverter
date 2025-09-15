using System;
using System.Collections.Generic;
using System.Text;

namespace VinnyLibConverterCommon.Transformation
{
    /// <summary>
    /// Задание аффиннового преобразования координат
    /// Xt = X*a + Y*b + c
    /// Yt = X*a2 + Y*b2 + c2
    /// </summary>
    [Serializable]
    public class TransformationAffine : ICoordinatesTransformation
    {
        public override CoordinatesTransformationVariant GetTransformationType()
        {
            return CoordinatesTransformationVariant.Affine;
        }
        private TransformationAffine() { }

        /// <summary>
        /// Инициализация параметров аффиннового преобразования
        /// Xt = X * ScaleX + Y * RotationY + TranslationX
        /// Yt = X * ScaleY + Y * RotationX + TranslationY
        /// </summary>
        public TransformationAffine(double scaleX, double scaleY, double rotationX, double rotationY, double translationX, double translationY)
        {
            this.ScaleX = scaleX;
            this.ScaleY = scaleY;
            this.RotationX = rotationX;
            this.RotationY = rotationY;
            this.TranslationX = translationX;
            this.TranslationY = translationY;
        }

        public override double[] TransformPoint3d(double[] xyz)
        {
            double x_new = xyz[0] * ScaleX + xyz[1] * RotationY + TranslationX;
            double y_new = xyz[0] * ScaleY + xyz[1] * RotationX + TranslationY;
            return new double[] { x_new, y_new, xyz[2] };
        }

        public override double[][] TransformPoints3d(double[][] xyz_array)
        {
            double[][] ret = new double[xyz_array.Length][];
            for (int pCounter = 0; pCounter < xyz_array.Length; pCounter++)
            {
                ret[pCounter] = TransformPoint3d(xyz_array[pCounter]);
            }
            return ret;
        }

        public override string ToString()
        {
            return $"X'={ScaleX}*X + {RotationY}*Y + {TranslationX};Y'={ScaleY}*X + {RotationX}*Y + {TranslationY}";
        }


        public double ScaleX { get; set; } = 1;
        public double RotationX { get; set; } = 1;
        public double RotationY { get; set; } = 1;
        public double ScaleY { get; set; } = 1;
        public double TranslationX { get; set; } = 0;
        public double TranslationY { get; set; } = 0;
    }
}
