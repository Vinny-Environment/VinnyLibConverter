using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinnyLibConverter_SMDX.SMDX.Textures
{
    /// <summary>
    /// Класс для описания отдельной области, используемой текстурой для одного объекта геометрии
    /// </summary>
    public class SMDX_Content_Texture_Source_Target
    {
        /// <summary>
        /// 4x4 ... матрица трансформации?
        /// </summary>
        public float[] world { get; set; }

        /// <summary>
        /// 4x4 ...тоже матрица ???
        /// </summary>
        public float[] view { get; set; }

        /// <summary>
        /// 4x4 ...тоже матрица ???. Для сечения?
        /// </summary>
        public float[] projection { get; set; }

        /// <summary>
        /// Наименование файла геометрии, к которому относится текстура
        /// </summary>
        public string geometry { get; set; }

        /// <summary>
        /// Тип действия текстуры?
        /// </summary>
        public string technique { get; set; }
    }
}
