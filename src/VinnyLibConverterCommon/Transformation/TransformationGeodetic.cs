using System;
using System.Collections.Generic;
using System.Text;

namespace VinnyLibConverterCommon.Transformation
{
    public class TransformationGeodetic : ICoordinatesTransformation
    {
        public CoordinatesTransformationVariant GetTransformationType()
        {
            return CoordinatesTransformationVariant.Geodetic;
        }
        private TransformationGeodetic() { }
        public TransformationGeodetic(string wktStartCs, string WktTargetCs)
        {
            pWktStartCS = wktStartCs;
            pWktTargetCS = WktTargetCs;
        }
        

        public float[] TransformPoint3d(float[] xyz)
        {
            //TODO: подтянуть GDAL и реализовать пересчет (не хочется сюда тащить огромный GDAL OSR со своими абстракциями)
            //TODO (2): создать SWIG-обёртку над маленькой C библиотекой на основе PROJ (указание пути к proj.db, пересчет точки, пересчет точек)

            throw new NotImplementedException();
        }

        private string pWktStartCS;
        private string pWktTargetCS;
    }
}
