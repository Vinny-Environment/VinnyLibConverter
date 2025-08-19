using System;
using System.Collections.Generic;
using System.Text;

namespace VinnyLibConverterCommon.Transformation
{
    public interface ICoordinatesTransformation
    {
        public float[] TransformPoint3d(float[] xyz);
    }
}
