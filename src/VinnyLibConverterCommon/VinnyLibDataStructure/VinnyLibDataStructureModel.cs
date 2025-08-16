using System;
using System.Collections.Generic;
using System.Text;

namespace VinnyLibConverterCommon.VinnyLibDataStructure
{
    /// <summary>
    ///  Describe the structure of any data (hierarchy, geometry, attributes, materials and etc)
    ///  @details Any read-operations are store info in it; any write-operations using as input-data it class
    /// </summary>
    public class VinnyLibDataStructureModel
    {
        public VinnyLibDataStructureModel()
        {
            Header = new VinnyLibDataStructureHeader();
            ObjectsManager = new VinnyLibDataStructureObjectsManager();
            ParametersManager = new VinnyLibDataStructureParametersManager();
            GeometrtyManager = new VinnyLibDataStructureGeometryManager();
        }
        public VinnyLibDataStructureHeader Header { get; private set; }

        public VinnyLibDataStructureObjectsManager ObjectsManager { get; private set; }
        public VinnyLibDataStructureParametersManager ParametersManager { get; private set; }
        public VinnyLibDataStructureMaterialsManager MaterialsManager { get; private set; }
        public VinnyLibDataStructureGeometryManager GeometrtyManager { get; private set; }

    }
}
