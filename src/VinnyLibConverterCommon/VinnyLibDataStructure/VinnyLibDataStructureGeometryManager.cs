using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using VinnyLibConverterCommon.Transformation;

namespace VinnyLibConverterCommon.VinnyLibDataStructure
{
    /// <summary>
    /// Вспомогательный класс для создания и хранения объектной геометрии
    /// </summary>
    public sealed class VinnyLibDataStructureGeometryManager
    {
        public VinnyLibDataStructureGeometryManager()
        {
            mGeometriesPlacementInfo = new Dictionary<int, VinnyLibDataStructureGeometryPlacementInfo>();
            mGeometries = new Dictionary<int, VinnyLibDataStructureGeometry>();
            mGeometryCounter = 0;
            mGeometryPlacementInfoCounter = 0;
        }

        public int CreateGeometry(VinnyLibDataStructureGeometryType geomType, int geometryId = -1)
        {
            int geometryIdUsed = geometryId;
            if (geometryId == -1) geometryIdUsed = mGeometryCounter;

            VinnyLibDataStructureGeometry createdGeometry = new VinnyLibDataStructureGeometryUnknown();
            switch (geomType)
            {
                case VinnyLibDataStructureGeometryType.Mesh:
                    createdGeometry = new VinnyLibDataStructureGeometryMesh(geometryIdUsed);
                    break;
            }

            if (createdGeometry.GetGeometryType() != VinnyLibDataStructureGeometryType._Unknown)
            {
                mGeometries.Add(geometryIdUsed, createdGeometry);
                if (geometryId == -1) mGeometryCounter++;
                return geometryIdUsed;
            }
            return -1;
        }

        public int CreateGeometry(VinnyLibDataStructureGeometry otherGeometry)
        {
            otherGeometry.mId = mGeometryCounter;
            mGeometries.Add(mGeometryCounter, otherGeometry);
            return otherGeometry.mId;
        }

        public void SetGeometry(int id, VinnyLibDataStructureGeometry geometry)
        {
            this.mGeometries[id] = geometry;
        }

        public VinnyLibDataStructureGeometry GetGeometryById(int id)
        {
            VinnyLibDataStructureGeometry outputGeometry = new VinnyLibDataStructureGeometryUnknown();
            mGeometries.TryGetValue(id, out outputGeometry);
            return outputGeometry;
        }

        public int CreateGeometryPlacementInfo(int geometryId)
        {
            VinnyLibDataStructureGeometryPlacementInfo placementInfo = new VinnyLibDataStructureGeometryPlacementInfo(mGeometryPlacementInfoCounter, geometryId);
            placementInfo.Id = mGeometryPlacementInfoCounter;
            this.mGeometriesPlacementInfo[mGeometryPlacementInfoCounter] = placementInfo;
            mGeometryPlacementInfoCounter++;
            return mGeometryPlacementInfoCounter-1;
        }

        public void SetGeometryPlacementInfo(int id, VinnyLibDataStructureGeometryPlacementInfo placementInfo)
        {
            this.mGeometriesPlacementInfo[id] = placementInfo;
        }

        public VinnyLibDataStructureGeometryPlacementInfo GetGeometryPlacementInfoById(int id)
        {
            VinnyLibDataStructureGeometryPlacementInfo outputPlacementInfo = new VinnyLibDataStructureGeometryPlacementInfo();
            mGeometriesPlacementInfo.TryGetValue(id, out outputPlacementInfo);
            if (outputPlacementInfo.Id == -1) return null;
            return outputPlacementInfo;
        }

        

        public Dictionary<int, VinnyLibDataStructureGeometryPlacementInfo> mGeometriesPlacementInfo { get; private set; }
        public Dictionary<int, VinnyLibDataStructureGeometry> mGeometries { get; private set; }
        private int mGeometryCounter;
        private int mGeometryPlacementInfoCounter;
    }
}
