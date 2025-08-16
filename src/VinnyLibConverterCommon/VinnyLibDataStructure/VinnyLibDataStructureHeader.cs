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
    public sealed class VinnyLibDataStructureHeader
    {
        public VinnyLibDataStructureHeader()
        {
            Data = new List<Tuple<string, string>>();
        }

        public void AddHeaderData(string name, string value)
        {
            Data.Add(Tuple.Create<string, string>(name, value));
        }

        public Tuple<string, string> GetHeaderData(int index)
        {
            if (index >= Data.Count) return null;
            return Data[index];
        }

        public List<Tuple<string, string>> Data { get; set; }

    }
}
