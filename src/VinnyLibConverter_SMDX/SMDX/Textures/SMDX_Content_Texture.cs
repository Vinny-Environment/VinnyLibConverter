using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinnyLibConverter_SMDX.SMDX.Textures
{
    /// <summary>
    /// Классс для описания текстуры объекта
    /// </summary>
    public class SMDX_Content_Texture
    {
        /// <summary>
        /// наименование текстуры
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Ширина текстуры
        /// </summary>
        public float? width { get; set; }

        /// <summary>
        /// Высота текстуры
        /// </summary>
        public float? height { get; set; }

        /// <summary>
        /// перечисление, указывающее на используемые каналы. 0 –альфа канал, 1 – сплошной цвет + альфа канал, 2 – оттенки серого, 3 – сплошной цвет
        /// </summary>
        public int? format { get; set; }

        /// <summary>
        /// перечисление, указывающее на тип текстуры. 0 – текстура, хранящая цвета, 1 – карта нормалей
        /// </summary>
        public int? filter { get; set; }

        /// <summary>
        /// литерал принимающий значение true или false, указывающий необходимость генерации mipmap-уровней
        /// </summary>
        public bool? mipmaps { get; set; }

        /// <summary>
        /// Описание областей, используемых текстурой
        /// </summary>
        public SMDX_Content_Texture_Source source { get; set; }
    }
}
