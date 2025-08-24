using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinnyLibConverter_SMDX.SMDX
{
    /// <summary>
    /// Класс для описания нестандартного типа свойств (например, enum)
    /// </summary>
    public class SMDX_Content_Type_Property_Info
    {
        /// <summary>
        /// Тип свойства
        /// </summary>
        public object type { get; set; }

        /// <summary>
        /// Варианты значений свойства
        /// </summary>
        public Dictionary<string, object>? values { get; set; }
    }
}
