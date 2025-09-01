using System;
using System.Collections.Generic;
using System.Text;

namespace VinnyLibConverterCommon.VinnyLibDataStructure
{
    public class VinnyLibDataStructureGeometryUnknown : VinnyLibDataStructureGeometry
    {
        public override VinnyLibDataStructureGeometryType GetGeometryType()
        {
            return VinnyLibDataStructureGeometryType._Unknown;
        }

        public static VinnyLibDataStructureGeometryUnknown asType(VinnyLibDataStructureGeometry geometry)
        {
            if (geometry.GetGeometryType() == VinnyLibDataStructureGeometryType._Unknown) return (VinnyLibDataStructureGeometryUnknown)geometry;
            return null;
        }

        public VinnyLibDataStructureGeometryUnknown() { }

    }
}
