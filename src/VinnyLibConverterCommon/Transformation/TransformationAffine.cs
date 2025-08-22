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
    public class TransformationAffine : ICoordinatesTransformation
    {
        public CoordinatesTransformationVariant GetTransformationType()
        {
            return CoordinatesTransformationVariant.Affine;
        }
        private TransformationAffine() { }

        /// <summary>
        /// Инициализация параметров аффиннового преобразования
        /// Xt = X * ScaleX + Y * RotationY + TranslationX
        /// Yt = X * ScaleY + Y * RotationX + TranslationY
        /// </summary>
        public TransformationAffine(float scaleX, float scaleY, float rotationX, float rotationY, float translationX, float translationY)
        {
            this.ScaleX = scaleX;
            this.ScaleY = scaleY;
            this.RotationX = rotationX;
            this.RotationY = rotationY;
            this.TranslationX = translationX;
            this.TranslationY = translationY;
        }

        public float[] TransformPoint3d(float[] xyz)
        {
            float x_new = xyz[0] * ScaleX + xyz[1] * RotationY + TranslationX;
            float y_new = xyz[0] * ScaleY + xyz[1] * RotationX + TranslationY;
            return new float[] { x_new, y_new, xyz[2] };
        }

        private MatrixImpl pMatrix;

        public float ScaleX { get; private set; }
        public float RotationX { get; private set; }
        public float RotationY { get; private set; }
        public float ScaleY { get; private set; }
        public float TranslationX { get; private set; }
        public float TranslationY { get; private set; }
    }
}
