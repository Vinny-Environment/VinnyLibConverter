using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

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
            //CreateMaterial(new int[] { 0, 0, 0 }, "Default");
        }

        public int CreateMaterial(int[] RGBA, string name = "")
        {
            VinnyLibDataStructureMaterial materialNew = new VinnyLibDataStructureMaterial(pMaterialCounter);
            materialNew.Name = name;
            materialNew.ColorR = RGBA[0];
            materialNew.ColorG = RGBA[1];
            materialNew.ColorB = RGBA[2];
            if (RGBA.Length == 4) materialNew.ColorAlpha = RGBA[3];

            if (ImportExportParameters.mActiveConfig.CheckMaterialsDubles)
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
            //if (Materials.TryGetValue(id, out outputMaterial)) //return outputMaterial;
            return outputMaterial;
        }

        [XmlIgnore]
        public Dictionary<int, VinnyLibDataStructureMaterial> mMaterials { get; set; }

        public List<VinnyLibDataStructureMaterial> Materials { get; set; }
        private int pMaterialCounter = 0;
    }
}
