using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;

namespace VinnyLibConverterCommon.VinnyLibDataStructure
{
    [Serializable]
    public sealed class VinnyLibDataStructureGeometryMesh : VinnyLibDataStructureGeometry
    {
        public override VinnyLibDataStructureGeometryType GetGeometryType() 
        {
            return VinnyLibDataStructureGeometryType.Mesh;
        }

        public override float[] ComputeBounds()
        {
            var x = this.mPoints.Values.Select(c => c[0]);
            var y = this.mPoints.Values.Select(c => c[1]);
            var z = this.mPoints.Values.Select(c => c[2]);

            return new float[] {x.Min(), y.Min(), z.Min(), x.Max(), y.Max(), z.Max()};
        }

        public static VinnyLibDataStructureGeometryMesh asType (VinnyLibDataStructureGeometry geometry)
        {
            if (geometry.GetGeometryType() == VinnyLibDataStructureGeometryType.Mesh) return (VinnyLibDataStructureGeometryMesh)geometry;
            return null;
        }

        private void InitFields()
        {
            mPoints = new Dictionary<int, float[]>();
            mFaces = new Dictionary<int, int[]>();
            mFaces2Materials = new Dictionary<int, int>();
        }

        internal VinnyLibDataStructureGeometryMesh(int id)
        {
            this.Id = id;
            InitFields();
        }
        public VinnyLibDataStructureGeometryMesh()
        {
            InitFields();
        }

        public int AddVertex(float x, float y, float z)
        {
            return AddVertex(new float[3] { x, y, z });
        }
        public int AddVertex(float[] xyz)
        {
            if (xyz.Length != 3)
            {
                //TODO: make error
                throw new ArgumentException("Число координат не равно трем!");
            }

            if (!ImportExportParameters.mActiveConfig.CheckGeometryDubles) 
            {
                mPoints.Add(mPoints.Count, xyz);
                return mPoints.Count - 1;
            }
            else
            {
                float xNew = Convert.ToSingle(Math.Round(xyz[0], ImportExportParameters.mActiveConfig.VertexAccuracy));
                float yNew = Convert.ToSingle(Math.Round(xyz[1], ImportExportParameters.mActiveConfig.VertexAccuracy));
                float zNew = Convert.ToSingle(Math.Round(xyz[2], ImportExportParameters.mActiveConfig.VertexAccuracy));

                for (int vertexCounter = 0; vertexCounter < mPoints.Count; vertexCounter++)
                {
                    float[] pointCoords = mPoints[vertexCounter];
                    if (pointCoords[0] == xNew && pointCoords[1] == yNew && pointCoords[2] == zNew) return vertexCounter;
                }
                mPoints.Add(mPoints.Count, new float[3] { xNew, yNew, zNew });
                return mPoints.Count -  1;
            }
        }

        public int AddFace(int vertex1, int vertex2, int vertex3)
        {
            return AddFace(new int[3] { vertex1, vertex2, vertex3 });
        }

        /// <summary>
        /// Добавление индексов точек в явном виде. Тут может возникнуть косяк, если mActiveConfig.CheckGeometryDubles = true и какие-то вершины "ужались", но при ЧТЕНИИ формата этого быть не должно
        /// </summary>
        /// <param name="v123"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public int AddFace(int[] v123)
        {
            if (v123.Length != 3)
            {
                //TODO: make error
                throw new ArgumentException("Число индексов не равно трем!");
            }

            if (ImportExportParameters.mActiveConfig.CheckGeometryDubles)
            {
                for (int faceCounter = 0; faceCounter < mFaces.Count; faceCounter++)
                {
                    int[] faceVertices = mFaces[faceCounter];
                    if (faceVertices.SequenceEqual(v123)) return faceCounter;
                }
            }
            mFaces.Add(mFaces.Count, v123);
            return mFaces.Count - 1;
        }

        public void AddFace(float[] point1, float[] point2, float[] point3)
        {
            int p1 = AddVertex(point1);
            int p2 = AddVertex(point2);
            int p3 = AddVertex(point3);

            AddFace(new int[] {p1, p2, p3 });
        }

        public int[] GetFaceVertices(int faceIndex)
        {
            if (faceIndex <= this.mFaces.Count) return this.mFaces[faceIndex];
            return null;
        }

        public float[] GetPointCoords(int pointIndex)
        {
            if (pointIndex <= this.mPoints.Count) return this.mPoints[pointIndex];
            return null;
        }

        public void AssignMaterialToFace(int FaceIndex, int MaterialIndex)
        {
            this.mFaces2Materials[FaceIndex] = MaterialIndex;
        }

        public int GetMaterialIndexByFaceIndex(int FaceIndex)
        {
            int outputMaterialIndex = 0;
            this.mFaces2Materials.TryGetValue(FaceIndex, out outputMaterialIndex);
            return outputMaterialIndex;
        }
         


        //Округление координат для сравнения точек
        private int mAccuracy = 5;


        [XmlIgnore]
        public Dictionary<int, float[]> mPoints { get; set; }

        public List<PointInfo> Points { get; set; }

        [XmlIgnore]
        public Dictionary<int, int[]> mFaces { get; set; }

        public List<FaceInfo> Faces { get; set; }

        [XmlIgnore]
        public Dictionary<int, int> mFaces2Materials { get; set; }

        public List<Face2MaterialInfo> Faces2Materials { get; set; }

    }

    public class PointInfo
    {
        public int Id { get; set; }

        public float[] XYZ { get; set; }
    }

    public class FaceInfo
    {
        public int Id { get; set; }

        public int[] Indices { get; set; }
    }

    public class Face2MaterialInfo
    {
        public int FaceId { get; set; }

        public int MaterialId { get; set; }
    }

}
