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

        /// <summary>
        /// Пересчитывает координаты VinnyLibDataStructureGeometry для заданных трансформаций и информации о положении VinnyLibDataStructureGeometryPlacementInfo
        /// </summary>
        /// <param name="transformations"></param>
        /// <param name="geometryPlacementInfo"></param>
        /// <returns></returns>
        public VinnyLibDataStructureGeometry TransformGeometry(List<ICoordinatesTransformation> transformations, VinnyLibDataStructureGeometryPlacementInfo geometryPlacementInfo)
        {
            List<ICoordinatesTransformation> transformations2 = new List<ICoordinatesTransformation>()
            {
                geometryPlacementInfo.TransformationMatrixInfo
            }.Concat(transformations).ToList();

            VinnyLibDataStructureGeometry targetGeometry = mGeometries[geometryPlacementInfo.IdGeometry];

            foreach (ICoordinatesTransformation transformation in transformations)
            {
                //Для каждого transformation необходимо заново инициализировать mGeometries и mGeometriesPlacementInfo (?)
                if (targetGeometry.GetGeometryType() == VinnyLibDataStructureGeometryType.Mesh)
                {
                    VinnyLibDataStructureGeometryMesh targetGeometry_Mesh = VinnyLibDataStructureGeometryMesh.asType(targetGeometry);
                    foreach (int PointKey in targetGeometry_Mesh.Points.Keys)
                    {
                        float[] XYZ_Converted = transformation.TransformPoint3d(targetGeometry_Mesh.Points[PointKey]);
                        targetGeometry_Mesh.Points[PointKey] = XYZ_Converted;
                    }
                }
            }
            return targetGeometry;
        }



        public Dictionary<int, VinnyLibDataStructureGeometryPlacementInfo> mGeometriesPlacementInfo { get; private set; }
        public Dictionary<int, VinnyLibDataStructureGeometry> mGeometries { get; private set; }
        private int mGeometryCounter;
        private int mGeometryPlacementInfoCounter;
    }
}
