using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Xml.Serialization;
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
            MeshGeometriesPlacementInfo = new Dictionary<int, VinnyLibDataStructureGeometryPlacementInfo>();
            MeshGeometries = new Dictionary<int, VinnyLibDataStructureGeometryMesh>();
            mGeometryCounter = 0;
            mGeometryPlacementInfoCounter = 0;
        }

        public int CreateGeometry(VinnyLibDataStructureGeometryType geomType)
        {
            //VinnyLibDataStructureGeometry createdGeometry = new VinnyLibDataStructureGeometryUnknown();
            switch (geomType)
            {
                case VinnyLibDataStructureGeometryType.Mesh:
                    var createdGeometry = new VinnyLibDataStructureGeometryMesh(mGeometryCounter);
                    MeshGeometries.Add(mGeometryCounter, createdGeometry);
                    mGeometryCounter++;
                    return mGeometryCounter - 1;
            }
            return -1;
        }

        
        public int CreateGeometry(VinnyLibDataStructureGeometry otherGeometry)
        {
            otherGeometry.Id = mGeometryCounter;
            if (otherGeometry.GetGeometryType() == VinnyLibDataStructureGeometryType.Mesh) 
            {
                MeshGeometries.Add(mGeometryCounter, (VinnyLibDataStructureGeometryMesh)otherGeometry);
                mGeometryCounter++;
                return otherGeometry.Id;
            }
            return -1;
        }
        

        /// <summary>
        /// Вспомогательный метод, используется для обновления информации о геометрии
        /// </summary>
        /// <param name="id"></param>
        /// <param name="geometry"></param>
        public void SetMeshGeometry(int id, VinnyLibDataStructureGeometryMesh geometry)
        {
            this.MeshGeometries[id] = geometry;
        }

        public VinnyLibDataStructureGeometryMesh GetMeshGeometryById(int id)
        {
            VinnyLibDataStructureGeometryMesh outputGeometry = new VinnyLibDataStructureGeometryMesh();
            MeshGeometries.TryGetValue(id, out outputGeometry);
            return outputGeometry;
        }

        public int CreateGeometryPlacementInfo(int geometryId)
        {
            VinnyLibDataStructureGeometryPlacementInfo placementInfo = new VinnyLibDataStructureGeometryPlacementInfo(mGeometryPlacementInfoCounter, geometryId);
            //placementInfo.Id = mGeometryPlacementInfoCounter;
            this.MeshGeometriesPlacementInfo[mGeometryPlacementInfoCounter] = placementInfo;
            mGeometryPlacementInfoCounter++;
            return mGeometryPlacementInfoCounter-1;
        }

        public void SetMeshGeometryPlacementInfo(int id, VinnyLibDataStructureGeometryPlacementInfo placementInfo)
        {
            placementInfo.InitMatrix();
            this.MeshGeometriesPlacementInfo[id] = placementInfo;
        }

        public VinnyLibDataStructureGeometryPlacementInfo GetGeometryPlacementInfoById(int id)
        {
            VinnyLibDataStructureGeometryPlacementInfo outputPlacementInfo = new VinnyLibDataStructureGeometryPlacementInfo();
            if (MeshGeometriesPlacementInfo.TryGetValue(id, out outputPlacementInfo)) return outputPlacementInfo;
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

            VinnyLibDataStructureGeometry targetGeometry = MeshGeometries[geometryPlacementInfo.IdGeometry];

            foreach (ICoordinatesTransformation transformation in transformations)
            {
                //Для каждого transformation необходимо заново инициализировать MeshGeometriesForXML и MeshGeometriesPlacementInfoForXML (?)
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

        public float[] ComputeBoundsForMeshGeometries(VinnyLibDataStructureGeometryPlacementInfo[] geoms)
        {
            float[] x = new float[geoms.Length * 2];
            float[] y = new float[geoms.Length * 2];
            float[] z = new float[geoms.Length * 2];

            int counter = 0;
            foreach (VinnyLibDataStructureGeometryPlacementInfo geom in geoms)
            {
                var bounds = this.GetMeshGeometryById(geom.IdGeometry).ComputeBounds();
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

        [XmlIgnore]
        public Dictionary<int, VinnyLibDataStructureGeometryMesh> MeshGeometries { get; set; }

        [XmlArray("MeshGeometries")]
        public List<VinnyLibDataStructureGeometryMesh> MeshGeometriesForXML { get; set; }


        [XmlIgnore]
        public Dictionary<int, VinnyLibDataStructureGeometryPlacementInfo> MeshGeometriesPlacementInfo { get; set; }

        [XmlArray("MeshGeometriesPlacementInfo")]
        public List<VinnyLibDataStructureGeometryPlacementInfo> MeshGeometriesPlacementInfoForXML { get; set; }

        internal int mGeometryCounter;
        internal int mGeometryPlacementInfoCounter;
    }
}
