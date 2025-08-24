using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace VinnyLibConverter_SMDX.SMDX.Materials
{
    /// <summary>
    /// Класс для описания схемы данных jmtl (json). Пока только для типа материала = "Blinn–Phong"
    /// </summary>
    public class SMDX_Material_BlinnPhong : SMDX_MaterialBase
    {
        public static SMDX_Material_BlinnPhong CreateFromRGB (int[] rgb)
        {
            SMDX_Material_BlinnPhong material = new SMDX_Material_BlinnPhong();
            material.type = SMDX_MaterialBase.type_BlinnPhong;
            material.ambient = new float[] { rgb[0] / 255.0f, rgb[1] / 255.0f, rgb[2] / 255.0f };
            material.diffuse = material.ambient;
            material.specular = new float[] { 1, 1, 1 };
            material.level = 1.0f;
            material.shininess = 0.2f;
            material.blur = 0;
            material.transparency = 0;
            material.illumination = 0;
            material.shading = "Wire";
            material.flags = 0;
            material.wire = 1;

            return material;
        }
        /// <summary>
        /// фоновое освещение
        /// </summary>
        public float[]? ambient { get; set; }

        /// <summary>
        /// рассеянный свет
        /// </summary>
        public float[]? diffuse { get; set; }

        /// <summary>
        /// бликовая составляющая
        /// </summary>
        public float[]? specular { get; set; }

        /// <summary>
        /// уровень яркости
        /// </summary>
        public float? level { get; set; }

        /// <summary>
        /// резкость зеркальных бликов
        /// </summary>
        public float? shininess { get; set; }

        /// <summary>
        /// степень размытия
        /// </summary>
        public float? blur { get; set; }

        /// <summary>
        ///  степень прозрачности
        /// </summary>
        public float? transparency { get; set; }

        /// <summary>
        ///  сила свечения
        /// </summary>
        public float? illumination { get; set; }

        /// <summary>
        /// тип затенения
        /// </summary>
        public string? shading { get; set; }

        public int? flags { get; set; }

        public int? wire { get; set; }

        public int[] GetColorRGB()
        {
            if (ambient != null && ambient.Length == 3) return new int[] { Convert.ToInt32(ambient[0] * 255), Convert.ToInt32(ambient[1] * 255), Convert.ToInt32(ambient[2] * 255) };
            return new int[] { 0, 0, 0 };
        }

        public void Save(string path)
        {
            File.WriteAllText(path, System.Text.Json.JsonSerializer.Serialize(this));
        }

        public static SMDX_Material_BlinnPhong LoadFrom(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException();

            SMDX_Material_BlinnPhong smdx_data;
            string file_data = File.ReadAllText(path);
            smdx_data = System.Text.Json.JsonSerializer.Deserialize<SMDX_Material_BlinnPhong>(file_data);
            return smdx_data;
        }
    }
}
