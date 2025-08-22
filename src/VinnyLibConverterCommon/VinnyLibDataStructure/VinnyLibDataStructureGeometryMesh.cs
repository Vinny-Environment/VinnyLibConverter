using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace VinnyLibConverterCommon.VinnyLibDataStructure
{
    public sealed class VinnyLibDataStructureGeometryMesh : VinnyLibDataStructureGeometry
    {
        public override VinnyLibDataStructureGeometryType GetGeometryType() 
        {
            return VinnyLibDataStructureGeometryType.Mesh;
        }

        public static VinnyLibDataStructureGeometryMesh asType (VinnyLibDataStructureGeometry geometry)
        {
            if (geometry.GetGeometryType() == VinnyLibDataStructureGeometryType.Mesh) return (VinnyLibDataStructureGeometryMesh)geometry;
            return null;
        }

        internal VinnyLibDataStructureGeometryMesh(int id)
        {
            mId = id;

            Points = new Dictionary<int, float[]>();
            Faces = new Dictionary<int, int[]>();
            Faces2Materials = new Dictionary<int, int>();
        }
        private VinnyLibDataStructureGeometryMesh() { }

        public void AddFace(float[] point1, float[] point2, float[] point3)
        {
            int p1 = GetPointIndex(point1);
            int p2 = GetPointIndex(point2);
            int p3 = GetPointIndex(point3);
            int[] checkArrayTmp = new int[] { p1, p2, p3 };

            if (ImportExportParameters.mActiveConfig.CheckGeometryDubles)
            {
                for (int faceCounter = 0; faceCounter < Faces.Count; faceCounter++)
                {
                    int[] faceVertices = Faces[faceCounter];
                    if (faceVertices.SequenceEqual(checkArrayTmp)) return;
                }
            }
            Faces.Add(Faces.Count(), checkArrayTmp);
        }

        public int[] GetFaceVertices(int faceIndex)
        {
            if (faceIndex <= this.Faces.Count) return this.Faces[faceIndex];
            return null;
        }

        public float[] GetPointCoords(int pointIndex)
        {
            if (pointIndex <= this.Points.Count) return this.Points[pointIndex];
            return null;
        }

        public void AssignMaterialToFace(int FaceIndex, int MaterialIndex)
        {
            this.Faces2Materials[FaceIndex] = MaterialIndex;
        }

        public int GetMaterialIndexByFaceIndex(int FaceIndex)
        {
            int outputMaterialIndex = 0;
            this.Faces2Materials.TryGetValue(FaceIndex, out outputMaterialIndex);
            return outputMaterialIndex;
        }
         

        private int GetPointIndex(float[] xyz)
        {
            return GetPointIndex(xyz[0], xyz[1], xyz[2]);
        }
        private int GetPointIndex(float x, float y, float z)
        {
            float xNew = Convert.ToSingle(Math.Round(x, 5));
            float yNew = Convert.ToSingle(Math.Round(y, 5));
            float zNew = Convert.ToSingle(Math.Round(z, 5));

            if (ImportExportParameters.mActiveConfig.CheckGeometryDubles)
            {
                for (int vertexCounter = 0; vertexCounter < Points.Count; vertexCounter++)
                {
                    float[] pointCoords = Points[vertexCounter];
                    if (pointCoords[0] == xNew && pointCoords[1] == yNew && pointCoords[1] == zNew) return vertexCounter;
                }
               
            }
            Points.Add(Points.Count(), new float[3] { xNew, yNew, zNew });
            return Points.Count;            
        }


        //Округление координат для сравнения точек
        private int mAccuracy = 5;

        public Dictionary<int, float[]> Points { get; internal set; }
        public Dictionary<int, int[]> Faces { get; internal set; }

        public Dictionary<int, int> Faces2Materials { get; internal set; }
    }
}
