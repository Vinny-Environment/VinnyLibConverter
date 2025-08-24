using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinnyLibConverter_SMDX.SMDX
{
    /// <summary>
    /// Класс для описания типа элемента информационной модели
    /// </summary>
    public class SMDX_Content_Type
    {
        /// <summary>
        /// Идентификатор типа
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Наименование типа
        /// </summary>
        public string name { get; set; }

        /// <summary>
        ///  Опциональный индекс массива types, указывающий на родительский тип. Текущий тип будет наследовать свойства своего родительского типа
        /// </summary>
        public int? parent { get; set; } //По умолчанию надо придумать логику

        /// <summary>
        /// Массив свойств
        /// </summary>
        public List<SMDX_Content_Type_Property>? properties { get; set; }
    }
}
