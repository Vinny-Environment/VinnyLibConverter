using System;
using System.Collections.Generic;
using System.Text;
using VinnyLibConverterCommon.VinnyLibDataStructure;

namespace VinnyLibConverterCommon.Interfaces
{
    /// <summary>
    /// Шаблон класса для подготовки структуры данных из некой среды (САПР) и сохранения данных в целевой CDE-формат
    /// </summary>
    public interface ICadExportProcessing
    {
        VinnyLibDataStructureModel CreateData();
        void ExportTo(CdeVariant outputType, VinnyLibDataStructureModel vinnyData, ImportExportParameters outputParameters);
    }
}
