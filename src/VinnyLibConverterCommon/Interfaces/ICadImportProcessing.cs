using System;
using System.Collections.Generic;
using System.Text;
using VinnyLibConverterCommon.VinnyLibDataStructure;

namespace VinnyLibConverterCommon.Interfaces
{
    /// <summary>
    /// Шаблон класса для импорта в данную среду (САПР) данных из целевого CDE-формата
    /// </summary>
    public interface ICadImportProcessing
    {
        VinnyLibDataStructureModel ImportFrom(CdeVariant outputType, ImportExportParameters openParameters);
    }
}
