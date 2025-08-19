using System;
using System.Collections.Generic;
using System.Text;

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
            TransformationMatrixInfo = TransformationMatrix.CreateEmptyTransformationMatrix();

            Position = new float[] { 0, 0, 0 };
            Scale = new float[] { 1, 1, 1 };
            VectorOX = 0;
            VectorOY = 0;
            VectorOZ = 0;
        }

        public int Id { get; internal set; }
        public int IdGeometry { get; internal set; }

        public float[] Position {
            get
            {
                return TransformationMatrixInfo.GetPosition();
            }
            set
            {
                TransformationMatrixInfo.SetPosition(value[0], value[1], value[2]);
            }
        } 
        public float[] Scale
        {
            get
            {
                return Scale;
            }
            set
            {
                TransformationMatrixInfo.SetScale(value[0], value[1], value[2]);
            }
        } 
        //public float[] VectorOX { get; set; } = new float[] { 1, 0, 0 };
        //public float[] VectorOY { get; set; } = new float[] { 0, 1, 0 };
        //public float[] VectorOZ { get; set; } = new float[] { 0, 0, 1 };

        public float VectorOX
        {
            get
            {
                return TransformationMatrixInfo.GetRotation_OX();
            }
            set
            {
                TransformationMatrixInfo.SetRotation_OX(value);
            }
        }

        public float VectorOY
        {
            get
            {
                return TransformationMatrixInfo.GetRotation_OY();
            }
            set
            {
                TransformationMatrixInfo.SetRotation_OY(value);
            }
        }

        public float VectorOZ
        {
            get
            {
                return TransformationMatrixInfo.GetRotation_OZ();
            }
            set
            {
                TransformationMatrixInfo.SetRotation_OZ(value);
            }
        }

        public void SetRotationFromQuaternion(QuaternionInfo quaternion)
        {
            TransformationMatrixInfo.SetRotationFromQuaternion(quaternion);
        }

        public QuaternionInfo RotationToQuaternion()
        {
            return TransformationMatrixInfo.ToQuaternion();
        }

        public TransformationMatrix TransformationMatrixInfo { get; private set; }
    }
}
