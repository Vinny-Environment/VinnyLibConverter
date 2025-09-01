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
            CreateMaterial(new int[] { 0, 0, 0 }, "Default");
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
            //if (mMaterials.TryGetValue(id, out outputMaterial)) //return outputMaterial;
            return outputMaterial;
        }

        [XmlIgnore]
        public Dictionary<int, VinnyLibDataStructureMaterial> mMaterials { get; private set; }

        // Helper property for XML serialization
        [XmlArray("mMaterials")]
        [XmlArrayItem("mMaterials")]
        public List<KeyValuePair_Material> MaterialsList
        {
            get => mMaterials.Select(kv => new KeyValuePair_Material { Key = kv.Key, Value = kv.Value }).ToList();
            set => mMaterials = value.ToDictionary(item => item.Key, item => item.Value);
        }

        private int pMaterialCounter = 0;
    }

    // Helper class for mMaterials serrialization
    public class KeyValuePair_Material
    {
        public int Key { get; set; }

        public VinnyLibDataStructureMaterial Value { get; set; }
    }
}
