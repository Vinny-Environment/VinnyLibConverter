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

        public override double[] ComputeBounds()
        {
            var x = this.Points.Values.Select(c => c[0]);
            var y = this.Points.Values.Select(c => c[1]);
            var z = this.Points.Values.Select(c => c[2]);

            return new double[] {x.Min(), y.Min(), z.Min(), x.Max(), y.Max(), z.Max()};
        }

        public static VinnyLibDataStructureGeometryMesh asType (VinnyLibDataStructureGeometry geometry)
        {
            if (geometry.GetGeometryType() == VinnyLibDataStructureGeometryType.Mesh) return (VinnyLibDataStructureGeometryMesh)geometry;
            return null;
        }

        private void InitFields()
        {
            Points = new Dictionary<int, double[]>();
            Faces = new Dictionary<int, int[]>();
            Faces2Materials = new Dictionary<int, int>();
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

        public int AddVertex(double x, double y, double z)
        {
            return AddVertex(new double[3] { x, y, z });
        }
        public int AddVertex(double[] xyz)
        {
            if (xyz.Length != 3)
            {
                //TODO: make error
                throw new ArgumentException("Число координат не равно трем!");
            }

            if (!ImportExportParameters.mActiveConfig.CheckGeometryDubles) 
            {
                Points.Add(Points.Count, xyz);
                return Points.Count - 1;
            }
            else
            {
                double xNew = Convert.ToDouble(Math.Round(xyz[0], ImportExportParameters.mActiveConfig.VertexAccuracy));
                double yNew = Convert.ToDouble(Math.Round(xyz[1], ImportExportParameters.mActiveConfig.VertexAccuracy));
                double zNew = Convert.ToDouble(Math.Round(xyz[2], ImportExportParameters.mActiveConfig.VertexAccuracy));

                for (int vertexCounter = 0; vertexCounter < Points.Count; vertexCounter++)
                {
                    double[] pointCoords = Points[vertexCounter];
                    if (pointCoords[0] == xNew && pointCoords[1] == yNew && pointCoords[2] == zNew) return vertexCounter;
                }
                Points.Add(Points.Count, new double[3] { xNew, yNew, zNew });
                return Points.Count -  1;
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
            Faces.Add(Faces.Count, v123);
            return Faces.Count - 1;
        }

        public void AddFace(double[] point1, double[] point2, double[] point3)
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

        public double[] GetPointCoords(int pointIndex)
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


        [XmlIgnore]
        public Dictionary<int, double[]> Points { get; set; }

        [XmlArray("Points")]
        public List<PointInfo> PointsForXML { get; set; }

        [XmlIgnore]
        public Dictionary<int, int[]> Faces { get; set; }

        [XmlArray("Faces")]
        public List<FaceInfo> FacesForXML { get; set; }

        [XmlIgnore]
        public Dictionary<int, int> Faces2Materials { get; set; }

        [XmlArray("Faces2Materials")]
        public List<Face2MaterialInfo> Faces2MaterialsForXML { get; set; }

    }

    public class PointInfo
    {
        public int Id { get; set; }

        public double[] XYZ { get; set; }
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
