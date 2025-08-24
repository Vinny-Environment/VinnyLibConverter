using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinnyLibConverter_SMDX.SMDX
{
    /// <summary>
    /// Класс для описания группы элементов
    /// </summary>
    public class SMDX_Content_Group
    {
        /// <summary>
        /// Наименование группы
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// индекс массива groups, указывающий на родительскую группу, свойства которой надо наследовать
        /// </summary>
        public int? parent { get; set; }

        /// <summary>
        /// индекс массива types, указывающий на тип, используемый для данной группы
        /// </summary>
        public int? type { get; set; }

        public List<SMDX_Content_Type_Property>? properties { get; set; } = new List<SMDX_Content_Type_Property>();
    }

}
