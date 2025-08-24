using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinnyLibConverter_SMDX.SMDX
{
    /// <summary>
    /// Класс для описаний свойств присущих данному типу
    /// </summary>
    public class SMDX_Content_Type_Property
    {
        /// <summary>
        /// Внутреннее имя свойства
        /// </summary>
        public string tag { get; set; }

        /// <summary>
        /// Опционально. Отображаемое имя свойста
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Опционально. Значение свойства, тип разный (для type = enum)
        /// </summary>
        public object? value { get; set; }

        /// <summary>
        /// Опционально. Единицы свойств
        /// </summary>
        public string? units { get; set; }

        /// <summary>
        /// Описание свойства (если оно не стандартных типов)
        /// </summary>
        public SMDX_Content_Type_Property_Info? info { get; set; }
    }
}
