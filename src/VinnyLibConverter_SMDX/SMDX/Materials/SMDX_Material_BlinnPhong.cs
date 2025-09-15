using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
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
            material.ambient = new double[] { 0,0,0 };
            material.diffuse = new double[] { rgb[0] / 255.0f, rgb[1] / 255.0f, rgb[2] / 255.0f };
            material.specular = new double[] { 1, 1, 1 };
            material.level = 0.4f;
            material.shininess = 0.2f;
            material.blur = 0;
            material.transparency = 0;
            material.illumination = 0;
            material.shading = "Wire";
            material.flags = 1;
            material.wire = 1;

            return material;
        }
        /// <summary>
        /// фоновое освещение
        /// </summary>
        public double[]? ambient { get; set; }

        /// <summary>
        /// рассеянный свет
        /// </summary>
        public double[]? diffuse { get; set; }

        /// <summary>
        /// бликовая составляющая
        /// </summary>
        public double[]? specular { get; set; }

        /// <summary>
        /// уровень яркости
        /// </summary>
        public double? level { get; set; }

        /// <summary>
        /// резкость зеркальных бликов
        /// </summary>
        public double? shininess { get; set; }

        /// <summary>
        /// степень размытия
        /// </summary>
        public double? blur { get; set; }

        /// <summary>
        ///  степень прозрачности
        /// </summary>
        public double? transparency { get; set; }

        /// <summary>
        ///  сила свечения
        /// </summary>
        public double? illumination { get; set; }

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
            //из-за ОСОБЕЕННОСТЕЙ чтения робуром файла материала придется сохранять в текст, а не json
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "VinnyLibConverter_SMDX.Resources.JMTL_BlinnPhongTemplate.txt";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                result = result.
                    Replace("ColorR", diffuse[0].ToString()).
                    Replace("ColorG", diffuse[1].ToString()).
                    Replace("ColorB", diffuse[2].ToString());
                File.WriteAllText(path, result);
            }
            //File.WriteAllText(path, System.Text.Json.JsonSerializer.Serialize(this, InternalUtils.GetWriteOpts()));
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
