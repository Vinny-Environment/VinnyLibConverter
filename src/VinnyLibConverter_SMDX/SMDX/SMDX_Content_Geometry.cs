using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinnyLibConverter_SMDX.SMDX
{
    /// <summary>
    /// Класс для описания геометрии объектов
    /// </summary>
    public class SMDX_Content_Geometry
    {
        public SMDX_Content_Geometry()
        {
            //lods = new List<SMDX_Content_Geometry_Lod>();
        }
        /// <summary>
        /// Наименование файла j3d в папке /geometry с геометрией объекта
        /// </summary>
        public string href { get; set; }

        /// <summary>
        /// массив координат XYZ описывающих границы трёхмерного объекта
        /// </summary>
        public double[] bounds { get; set; }

        /// <summary>
        /// Детализация отображения графики в зависимости от расстояния до точки обзора в окне просмотра
        /// </summary>
        public List<SMDX_Content_Geometry_Lod>? lods { get; set; }
    }
}
