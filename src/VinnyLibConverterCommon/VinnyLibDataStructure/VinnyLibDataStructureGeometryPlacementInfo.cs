using System;
using System.Collections.Generic;
using System.Text;
using VinnyLibConverterCommon.Transformation;

namespace VinnyLibConverterCommon.VinnyLibDataStructure
{
    

    public sealed class VinnyLibDataStructureGeometryPlacementInfo
    {
        public VinnyLibDataStructureGeometryPlacementInfo(int id, int geometryId)
        {
            this.Id = id;
            this.IdGeometry = geometryId;
            SetDefaultValues();
        }
        internal VinnyLibDataStructureGeometryPlacementInfo() { SetDefaultValues(); }

        private void SetDefaultValues()
        {
            Id = -1;
            ResetGeometry();
        }

        public void ResetGeometry()
        {
            TransformationMatrixInfo = TransformationMatrix4x4.CreateEmptyTransformationMatrix();

            Position = new float[] { 0, 0, 0 };
            Scale = new float[] { 1, 1, 1 };
            VectorOX = 0;
            VectorOY = 0;
            VectorOZ = 0;
        }

        public int Id { get; internal set; }
        public int IdGeometry { get; internal set; }

        public float[] Position { get; set; }
        public float[] Scale { get; set; }
        //public float[] VectorOX { get; set; } = new float[] { 1, 0, 0 };
        //public float[] VectorOY { get; set; } = new float[] { 0, 1, 0 };
        //public float[] VectorOZ { get; set; } = new float[] { 0, 0, 1 };

        public float VectorOX { get; set; }

        public float VectorOY { get; set; }

        public float VectorOZ { get; set; }

        public TransformationMatrix4x4 TransformationMatrixInfo { get; private set; }
    }
}
