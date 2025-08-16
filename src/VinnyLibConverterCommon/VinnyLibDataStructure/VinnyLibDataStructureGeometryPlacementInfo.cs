using System;
using System.Collections.Generic;
using System.Text;

namespace VinnyLibConverterCommon.VinnyLibDataStructure
{
    public class QuaternionInfo
    {
        public QuaternionInfo()
        {
            qx = 0;
            qy = 0;
            qz = 0;
            qw = 1;
        }

        public QuaternionInfo(float qx, float qy, float qz, float qw)
        {
            this.qx = qx;
            this.qy = qy;
            this.qz = qz;
            this.qw = qw;
        }

        public float qx { get; set; }
        public float qy { get; set; }
        public float qz { get; set; }
        public float qw { get; set; }
    }
    public sealed class VinnyLibDataStructureGeometryPlacementInfo
    {
        public VinnyLibDataStructureGeometryPlacementInfo(int id, int geometryId)
        {
            this.Id = id;
            this.IdGeometry = geometryId;
        }

        internal VinnyLibDataStructureGeometryPlacementInfo() { }

        public int Id { get; internal set; } = -1;
        public int IdGeometry { get; internal set; }
        public float[] Position { get; set; } = new float[3] { 0, 0, 0 };

        public float[] Scale { get; set; } = new float[3] { 1, 1, 1 };

        public float[] AngleX { get; set; } = new float[] { 1, 0, 0 };
        public float[] AngleY { get; set; } = new float[] { 0, 1, 0 };
        public float[] AngleZ { get; set; } = new float[] { 0, 0, 1 };

        public double[] CreateMatrix()
        {
            return new double[0];
        }

        public void SetRotationFromQuaternion(QuaternionInfo quaternion)
        {
            //TODO: реализовать преобразование кватерниона в мои AngleX, AngleY, AngleZ
        }

        /// <summary>
        /// Преобразует AngleX, AngleY, AngleZ в кватернион
        /// </summary>
        /// <returns></returns>
        public QuaternionInfo ToQuaternion()
        {
            //TODO
            return new QuaternionInfo();
        }
    }
}
