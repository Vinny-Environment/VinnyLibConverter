using System;
using System.Collections.Generic;
using System.Text;

namespace VinnyLibConverterCommon.VinnyLibDataStructure
{
    public class VinnyLibDataStructureObjectWithParametersBase
    {
        public VinnyLibDataStructureObjectWithParametersBase()
        {
            Parameters = new List<VinnyLibDataStructureParameterValue>();
        }
        public List<VinnyLibDataStructureParameterValue> Parameters { get; set; }
    }
}
