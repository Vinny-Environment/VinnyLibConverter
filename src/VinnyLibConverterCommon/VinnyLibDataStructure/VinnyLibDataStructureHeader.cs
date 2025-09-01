using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using System.Xml.Linq;

namespace VinnyLibConverterCommon.VinnyLibDataStructure
{
    /// <summary>
    /// Описывает метаданные файла/модели в формате ключ:значение. Ключи могут повторяться
    /// </summary>
    public sealed class VinnyLibDataStructureHeader : VinnyLibDataStructureObjectWithParametersBase
    {
        public VinnyLibDataStructureHeader()
        {
            Parameters = new List<VinnyLibDataStructureParameterValue>();
        }
    }
}
