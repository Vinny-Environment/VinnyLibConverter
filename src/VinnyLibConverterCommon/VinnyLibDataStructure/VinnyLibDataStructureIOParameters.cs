using System;
using System.Collections.Generic;
using System.Text;

namespace VinnyLibConverterCommon.VinnyLibDataStructure
{
    /// <summary>
    /// Вспомогательный класс для задания параметров чтения/записи сцены
    /// </summary>
    public sealed class VinnyLibDataStructureIOParameters
    {
        public VinnyLibDataStructureIOParameters()
        {

        }

        /// <summary>
        /// Абсолютный файловый путь к данным
        /// </summary>
        public string Path { get; set; }

        public bool CheckGeometryDubles { get; set; } = true;
        public bool CheckMaterialsDubles { get; set; } = true;
        public bool CheckParameterDefsDubles { get; set; } = true;

        public float[] Scale { get; set; } = new float[3] { 1.0f, 1.0f, 1.0f };
        public float[] OffsetPoint { get; set; } = new float[3] { 0, 0, 0 };
        public float AngleGrad { get; set; } = 0;
        public static VinnyLibDataStructureIOParameters mActiveConfig { get; set; } = new VinnyLibDataStructureIOParameters();
    }
}
