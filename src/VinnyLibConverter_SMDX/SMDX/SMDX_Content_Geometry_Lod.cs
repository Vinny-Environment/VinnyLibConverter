using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinnyLibConverter_SMDX.SMDX
{
    public class SMDX_Content_Geometry_Lod
    {
        /// <summary>
        ///  Расстояния до точки обзора в окне просмотра от объекта
        /// </summary>
        public float? distance { get; set; }

        /// <summary>
        /// ???
        /// </summary>
        public string? type { get; set; }

        /// <summary>
        /// Наименование файла j3d в папке /geometry с геометрией объекта
        /// </summary>
        public string href { get; set; }
    }
}
