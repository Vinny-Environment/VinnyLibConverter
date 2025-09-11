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
            SetDefaultValues();
            this.Id = id;
            this.IdGeometry = geometryId;
            
        }
        public VinnyLibDataStructureGeometryPlacementInfo() { SetDefaultValues(); }

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
            VectorOX_Rad = 0;
            VectorOY_Rad = 0;
            VectorOZ_Rad = 0;
        }

        public int Id { get; set; }
        public int IdGeometry { get; set; }

        public float[] Position { get; set; }
        public float[] Scale { get; set; }
        public float VectorOX_Rad { get; set; }
        public float VectorOY_Rad { get; set; }
        public float VectorOZ_Rad { get; set; }

        public float[] TransformatiomMatrix { get; set; }

        /// <summary>
        /// Вспомогательный метод, вызывается для инициализации внутренней матрицы пересчета TransformationMatrixInfo для заданных значений Position, Scale, VectorOX_Rad, VectorOY_Rad, VectorOZ_Rad
        /// </summary>
        public void InitMatrix()
        {
            TransformationMatrixInfo = TransformationMatrix4x4.CreateEmptyTransformationMatrix();
            if (TransformatiomMatrix != null && TransformatiomMatrix.Length == 16)
            {
                TransformationMatrixInfo.SetFromOtherMatrix(TransformatiomMatrix);
            }
            else
            {
                TransformationMatrixInfo.SetPosition(Position[0], Position[1], Position[2]);
                TransformationMatrixInfo.SetRotationFromAngles(VectorOX_Rad, VectorOY_Rad, VectorOZ_Rad);
                TransformationMatrixInfo.SetScale(Scale[0], Scale[1], Scale[2]);
            }
        }

        public TransformationMatrix4x4 TransformationMatrixInfo { get; set; }
    }
}
