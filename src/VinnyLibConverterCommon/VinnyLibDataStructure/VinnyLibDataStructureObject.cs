using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VinnyLibConverterCommon.VinnyLibDataStructure
{
    /// <summary>
    /// Описание объекта модели
    /// </summary>
    public class VinnyLibDataStructureObject
    {
        internal VinnyLibDataStructureObject()
        {
            Parameters = new List<VinnyLibDataStructureParameterValue>();
            GeometryPlacementInfoIds = new List<int>();
        }
        internal VinnyLibDataStructureObject(int id)
        {
            this.Id = id;
            Parameters = new List<VinnyLibDataStructureParameterValue>();
            GeometryPlacementInfoIds = new List<int>();
        }
        public int Id { get; private set; }
        public int ParentId { get; set; } = -1;

        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string UniqueId { get; set; }
        public string ModifiedDate { get; set; }

        public int MaterialId { get; set; }

        public void AddGeometryPlacementInfo(VinnyLibDataStructureGeometryPlacementInfo geometryPlacementInfo)
        {
            GeometryPlacementInfoIds.Add(geometryPlacementInfo.Id);
        }

        public void RemoveGeometryPlacementInfo(int geometryId)
        {
            GeometryPlacementInfoIds = GeometryPlacementInfoIds.Where(p => p == geometryId).ToList();
        }

        public void AddParameterValue(VinnyLibDataStructureParameterValue paramValue)
        {
            Parameters.Add(paramValue);
        }


        public List<int> GeometryPlacementInfoIds { get; internal set; }
        public List<VinnyLibDataStructureParameterValue> Parameters { get; internal set; }
    }
}
