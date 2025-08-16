using System;
using System.Collections.Generic;
using System.Text;

namespace VinnyLibConverterCommon.VinnyLibDataStructure
{
    /// <summary>
    /// Описание материала объекта
    /// </summary>
    public class VinnyLibDataStructureMaterial
    {
        public int Id { get; private set; }
        public string Name { get; internal set; } = "";
        public int ColorR { get; set; } = 0;
        public int ColorG { get; set; } = 0;
        public int ColorB { get; set; } = 0;
        public int ColorAlpha { get; set; } = 255;

        internal VinnyLibDataStructureMaterial() { }
        internal VinnyLibDataStructureMaterial(int id)
        {
            this.Id = id;
        }

        public override bool Equals(object obj)
        {
            return
                ColorR == ((VinnyLibDataStructureMaterial)obj).ColorR &&
                ColorG == ((VinnyLibDataStructureMaterial)obj).ColorG &&
                ColorB == ((VinnyLibDataStructureMaterial)obj).ColorB &&
                ColorAlpha == ((VinnyLibDataStructureMaterial)obj).ColorAlpha;
        }

    }
}
