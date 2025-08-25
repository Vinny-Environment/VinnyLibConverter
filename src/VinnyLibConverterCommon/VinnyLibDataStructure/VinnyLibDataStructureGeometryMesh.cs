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

        public override float[] ComputeBounds()
        {
            var x = this.Points.Values.Select(c => c[0]);
            var y = this.Points.Values.Select(c => c[1]);
            var z = this.Points.Values.Select(c => c[2]);

            return new float[] {x.Min(), y.Min(), z.Min(), x.Max(), y.Max(), z.Max()};
        }

        public static VinnyLibDataStructureGeometryMesh asType (VinnyLibDataStructureGeometry geometry)
        {
            if (geometry.GetGeometryType() == VinnyLibDataStructureGeometryType.Mesh) return (VinnyLibDataStructureGeometryMesh)geometry;
            return null;
        }

        internal VinnyLibDataStructureGeometryMesh(int id)
        {
            this.Id = id;

            Points = new Dictionary<int, float[]>();
            Faces = new Dictionary<int, int[]>();
            Faces2Materials = new Dictionary<int, int>();
        }
        private VinnyLibDataStructureGeometryMesh() { }

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
                Points.Add(Points.Count, xyz);
                return Points.Count;
            }
            else
            {
                float xNew = Convert.ToSingle(Math.Round(xyz[0], ImportExportParameters.mActiveConfig.VertexAccuracy));
                float yNew = Convert.ToSingle(Math.Round(xyz[1], ImportExportParameters.mActiveConfig.VertexAccuracy));
                float zNew = Convert.ToSingle(Math.Round(xyz[2], ImportExportParameters.mActiveConfig.VertexAccuracy));

                for (int vertexCounter = 0; vertexCounter < Points.Count; vertexCounter++)
                {
                    float[] pointCoords = Points[vertexCounter];
                    if (pointCoords[0] == xNew && pointCoords[1] == yNew && pointCoords[2] == zNew) return vertexCounter;
                }
                Points.Add(Points.Count, new float[3] { xNew, yNew, zNew });
                return Points.Count;
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
                for (int faceCounter = 0; faceCounter < Faces.Count; faceCounter++)
                {
                    int[] faceVertices = Faces[faceCounter];
                    if (faceVertices.SequenceEqual(v123)) return faceCounter;
                }
            }
            Faces.Add(Faces.Count(), v123);
            return Faces.Count;
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
         


        //Округление координат для сравнения точек
        private int mAccuracy = 5;

        public Dictionary<int, float[]> Points { get; internal set; }
        public Dictionary<int, int[]> Faces { get; internal set; }

        public Dictionary<int, int> Faces2Materials { get; internal set; }
    }
}
