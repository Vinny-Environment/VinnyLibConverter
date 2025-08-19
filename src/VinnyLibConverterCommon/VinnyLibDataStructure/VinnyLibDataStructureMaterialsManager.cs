using System;
using System.Collections.Generic;
using System.Text;

namespace VinnyLibConverterCommon.VinnyLibDataStructure
{
    /// <summary>
    /// Вспомогательный класс для создания и хранения материалов
    /// </summary>
    public class VinnyLibDataStructureMaterialsManager
    {
        public VinnyLibDataStructureMaterialsManager()
        {
            pMaterialCounter = 0;
            mMaterials = new Dictionary<int, VinnyLibDataStructureMaterial>();
        }

        public int CreateMaterial(int[] RGBA, string name = "")
        {
            VinnyLibDataStructureMaterial materialNew = new VinnyLibDataStructureMaterial(pMaterialCounter);
            materialNew.Name = name;
            materialNew.ColorR = RGBA[0];
            materialNew.ColorG = RGBA[1];
            materialNew.ColorB = RGBA[2];
            if (RGBA.Length == 4) materialNew.ColorAlpha = RGBA[3];

            if (IEParameters.mActiveConfig.CheckMaterialsDubles)
            {
                foreach (var materialInfo in mMaterials)
                {
                    if (materialInfo.Value.Equals(materialNew)) return materialInfo.Key;
                }
            }

            mMaterials.Add(pMaterialCounter, materialNew);
            pMaterialCounter++;
            return pMaterialCounter - 1;
        }

        public VinnyLibDataStructureMaterial GetMaterialById(int id)
        {
            VinnyLibDataStructureMaterial outputMaterial = new VinnyLibDataStructureMaterial();
            mMaterials.TryGetValue(id, out outputMaterial);
            //if (mMaterials.TryGetValue(id, out outputMaterial)) //return outputMaterial;
            return outputMaterial;
        }

        public Dictionary<int, VinnyLibDataStructureMaterial> mMaterials { get; private set; }
        private int pMaterialCounter = 0;
    }
}
