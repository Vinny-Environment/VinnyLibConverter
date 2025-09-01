using System;
using System.Collections.Generic;
using System.Text;

namespace VinnyLibConverterCommon.Transformation
{
    [Serializable]
    public class TransformationGeodetic : ICoordinatesTransformation
    {
        public override CoordinatesTransformationVariant GetTransformationType()
        {
            return CoordinatesTransformationVariant.Geodetic;
        }
        private TransformationGeodetic() { }
        public TransformationGeodetic(string wktStartCs, string WktTargetCs)
        {
            WktStartCS = wktStartCs;
            WktTargetCS = WktTargetCs;
        }
        

        public override float[] TransformPoint3d(float[] xyz)
        {
            //TODO: подтянуть GDAL и реализовать пересчет (не хочется сюда тащить огромный GDAL OSR со своими абстракциями)
            //TODO (2): создать SWIG-обёртку над маленькой C библиотекой на основе PROJ (указание пути к proj.db, пересчет точки, пересчет точек)

            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "Geodetic";
            //TODO: return a CS's names after PROJ's initialize (create PROJ SWIG...)
        }

        public string WktStartCS { get; set; }
        public string WktTargetCS { get; set; }
    }
}
