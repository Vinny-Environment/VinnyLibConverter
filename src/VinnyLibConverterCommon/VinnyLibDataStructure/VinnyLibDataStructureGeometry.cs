using System;
using System.Collections.Generic;
using System.Text;

namespace VinnyLibConverterCommon.VinnyLibDataStructure
{
    public enum VinnyLibDataStructureGeometryType : int
    {
        _Unknown = 0,
        Mesh = 1,
        Cad = 2,
        Brep = 3
    };

    public abstract class VinnyLibDataStructureGeometry
    {
        public virtual VinnyLibDataStructureGeometryType GetGeometryType()
        {
            return VinnyLibDataStructureGeometryType._Unknown;
        }

        internal int mId;
    }
}
