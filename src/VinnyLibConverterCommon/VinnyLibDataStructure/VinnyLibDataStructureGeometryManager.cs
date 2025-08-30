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
            otherGeometry.Id = mGeometryCounter;
            mGeometries.Add(mGeometryCounter, otherGeometry);
            return otherGeometry.Id;
        }

        /// <summary>
        /// Вспомогательный метод, используется для обновления информации о геометрии
        /// </summary>
        /// <param name="id"></param>
        /// <param name="geometry"></param>
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
            //placementInfo.Id = mGeometryPlacementInfoCounter;
            this.mGeometriesPlacementInfo[mGeometryPlacementInfoCounter] = placementInfo;
            mGeometryPlacementInfoCounter++;
            return mGeometryPlacementInfoCounter-1;
        }

        public void SetGeometryPlacementInfo(int id, VinnyLibDataStructureGeometryPlacementInfo placementInfo)
        {
            placementInfo.InitMatrix();
            this.mGeometriesPlacementInfo[id] = placementInfo;
        }

        public VinnyLibDataStructureGeometryPlacementInfo GetGeometryPlacementInfoById(int id)
        {
            VinnyLibDataStructureGeometryPlacementInfo outputPlacementInfo = new VinnyLibDataStructureGeometryPlacementInfo();
            if (mGeometriesPlacementInfo.TryGetValue(id, out outputPlacementInfo)) return outputPlacementInfo;
            return null;
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

        public float[] ComputeBoundsForGeometries(VinnyLibDataStructureGeometryPlacementInfo[] geoms)
        {
            float[] x = new float[geoms.Length * 2];
            float[] y = new float[geoms.Length * 2];
            float[] z = new float[geoms.Length * 2];

            int counter = 0;
            foreach (VinnyLibDataStructureGeometryPlacementInfo geom in geoms)
            {
                var bounds = this.GetGeometryById(geom.IdGeometry).ComputeBounds();
                x[counter * 2] = bounds[0];
                x[counter * 2 + 1] = bounds[3];
                y[counter * 2] = bounds[1];
                y[counter * 2 + 1] = bounds[4];
                z[counter * 2] = bounds[2];
                z[counter * 2 + 1] = bounds[5];
                counter++;
            }
            return new float[] { x.Min(), y.Min(), z.Min(), x.Max(), y.Max(), z.Max() };
        }



        public Dictionary<int, VinnyLibDataStructureGeometryPlacementInfo> mGeometriesPlacementInfo { get; private set; }
        public Dictionary<int, VinnyLibDataStructureGeometry> mGeometries { get; private set; }
        private int mGeometryCounter;
        private int mGeometryPlacementInfoCounter;
    }
}
