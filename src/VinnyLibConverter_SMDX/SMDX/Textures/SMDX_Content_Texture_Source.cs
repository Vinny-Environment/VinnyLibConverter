using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinnyLibConverter_SMDX.SMDX.Textures
{
    /// <summary>
    /// Класс для описания областей, используемых текстурой
    /// </summary>
    public class SMDX_Content_Texture_Source
    {
        /// <summary>
        /// Ширина текстуры
        /// </summary>
        public double? width { get; set; }

        /// <summary>
        /// Высота текстуры
        /// </summary>
        public double? height { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double? x { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double? y { get; set; }

        /// <summary>
        /// Области использования
        /// </summary>
        public SMDX_Content_Texture_Source_Target[] target { get; set; }
    }
}
