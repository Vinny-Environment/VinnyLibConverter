using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace VinnyLibConverterCommon.Transformation
{
    public enum CoordinatesTransformationVariant
    {
        _Unkwnown,
        Matrix4x4,
        Affine,
        Geodetic
    }


    [Serializable]
    [XmlInclude(typeof(TransformationMatrix4x4))]
    [XmlInclude(typeof(TransformationGeodetic))]
    [XmlInclude(typeof(TransformationAffine))]
    public abstract class ICoordinatesTransformation
    {
        public virtual float[] TransformPoint3d(float[] xyz)
        {
            return new float[] { };
        }

        public virtual CoordinatesTransformationVariant GetTransformationType()
        {
            return CoordinatesTransformationVariant._Unkwnown;
        }
    }
}
