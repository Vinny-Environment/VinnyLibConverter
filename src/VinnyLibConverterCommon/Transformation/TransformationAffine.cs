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

        public override string ToString()
        {
            return $"X'={ScaleX}*X + {RotationY}*Y + {TranslationX};Y'={ScaleY}*X + {RotationX}*Y + {TranslationY}";
        }


        public float ScaleX { get; private set; } = 1;
        public float RotationX { get; private set; } = 1;
        public float RotationY { get; private set; } = 1;
        public float ScaleY { get; private set; } = 1;
        public float TranslationX { get; private set; } = 0;
        public float TranslationY { get; private set; } = 0;
    }
}
