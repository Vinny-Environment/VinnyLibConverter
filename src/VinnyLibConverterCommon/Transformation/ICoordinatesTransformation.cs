using System;
using System.Collections.Generic;
using System.Text;

namespace VinnyLibConverterCommon.Transformation
{
    public enum CoordinatesTransformationVariant
    {
        Matrix4x4,
        Affine,
        Geodetic
    }


    public interface ICoordinatesTransformation
    {
        public float[] TransformPoint3d(float[] xyz);

        public CoordinatesTransformationVariant GetTransformationType();
    }
}
