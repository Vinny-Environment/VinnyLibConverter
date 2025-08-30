using System;
using System.Collections.Generic;
using System.Text;

namespace VinnyLibConverterCommon
{
    public enum CdeVariant : int
    {
        VinnyLibConverterCache = 0,
        MLT = 1,//? Частично проприетарный формат, CMesh бинарный под копирайтом
        IMC = 2,
        DotBIM = 3,
        NWC = 4,
        FBX = 5,
        GLTF = 6,
        IFC = 7,
        DXF = 8,
        LandXML = 9,
        SMDX = 10
    }
}
