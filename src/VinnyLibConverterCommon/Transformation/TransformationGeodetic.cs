using System;
using System.Collections.Generic;
using System.Text;

namespace VinnyLibConverterCommon.Transformation
{
    public class TransformationGeodetic : ICoordinatesTransformation
    {
        private TransformationGeodetic() { }
        public TransformationGeodetic(string wktStartCs, string WktTargetCs)
        {
            pWktStartCS = wktStartCs;
            pWktTargetCS = WktTargetCs;
        }
        

        public float[] TransformPoint3d(float[] xyz)
        {
            //TODO: подтянуть GDAL и реализовать пересчет

            throw new NotImplementedException();
        }

        private string pWktStartCS;
        private string pWktTargetCS;
    }
}
