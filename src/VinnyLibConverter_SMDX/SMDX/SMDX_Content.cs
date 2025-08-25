using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace VinnyLibConverter_SMDX.SMDX
{
    /// <summary>
    /// Класс для описания схемы данных content.json (основной файл данных)
    /// </summary>
    public class SMDX_Content
    {
        public const string ContentFileName = "content.json";
        public SMDX_Content()
        {
            wcs = new float[] { 0, 0, 0 };
            insertions = new List<SMDX_Content_Insertion>();
            geometry = new List<SMDX_Content_Geometry>();
            types = new List<SMDX_Content_Type>();
            groups = new List<SMDX_Content_Group>();
            documents = new List<SMDX_Content_Document>();

        }
        /// <summary>
        ///  Массив координат X, Y, Z базовой точки модели относительно которой будут располагаться все элементы цифровой модели.
        /// </summary>
        public float[] wcs { get; set; }

        /// <summary>
        /// Информация об элементах в пространстве модели
        /// </summary>
        public List<SMDX_Content_Insertion> insertions { get; set; }

        /// <summary>
        /// Информация о геометрии элементов
        /// </summary>
        public List<SMDX_Content_Geometry> geometry { get; set; }

        /// <summary>
        /// Информация о типах элементов
        /// </summary>
        public List<SMDX_Content_Type> types { get; set; }


        /// <summary>
        /// Информация о группировании элементов
        /// </summary>
        public List<SMDX_Content_Group> groups { get; set; }

        /// <summary>
        /// Информация о вложениях в проект (документы, файлы и т.д.)
        /// </summary>
        public List<SMDX_Content_Document> documents { get; set; }


        /// <summary>
        /// Инициализирует SMDX файл (схему content.json)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static SMDX_Content LoadSchema(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException();
            string json_raw = File.ReadAllText(path);
            SMDX_Content data = System.Text.Json.JsonSerializer.Deserialize<SMDX_Content>(json_raw);
            return data;
        }

        public void Save(string path)
        {
            File.WriteAllText(path, System.Text.Json.JsonSerializer.Serialize(this, InternalUtils.GetWriteOpts()));
        }
    }
}
