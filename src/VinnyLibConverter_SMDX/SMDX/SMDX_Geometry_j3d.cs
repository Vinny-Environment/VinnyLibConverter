using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinnyLibConverter_SMDX.SMDX
{
    /// <summary>
    /// Вспомогательный класс, описывает отдельный mesh в файле j3d
    /// </summary>
    public class SMDX_Geometry_j3d_Part
    {
        public SMDX_Geometry_j3d_Part()
        {
            positions = new List<float>();
            triangles = new List<int>();
            //textures = new List<float>();
            groups = new Dictionary<string, int[]>();
            //smoothing = new List<int>();
            //triangles_flags = new List<int>();
        }
        /// <summary>
        /// Массив с относительными отметками вершин (x, y, z)
        /// </summary>
        public List<float> positions { get; set; }

        /// <summary>
        /// Массив с информацией по граням (индекс точки = предыдущему значению + данное)
        /// </summary>
        public List<int> triangles { get; set; }

        /// <summary>
        ///  Массив текстурных координат XY описывающих область текстуры, используемую треугольником.Количество элементов массива соответствует количеству элементов массива positions;
        /// </summary>
        public List<float>? textures { get; set; }

        /// <summary>
        /// Информация, какой материал характерен для какого диапазона граней triangles. Ключ: имя файла в .\materials, value: диапазон граней
        /// </summary>
        public Dictionary<string, int[]> groups { get; set; }

        public List<int>? smoothing { get; set; }

        public List<int>? triangles_flags { get; set; }
    }

    /// <summary>
    /// Класс для описания схемы данных j3d (json)
    /// </summary>
    public class SMDX_Geometry_j3d
    {
        /// <summary>
        /// Абсолютный файловый путь к j3d-модели
        /// </summary>
        //internal string data_path { get; private set; }

        public Dictionary<string, SMDX_Geometry_j3d_Part> Meshes { get; set; }

        public static SMDX_Geometry_j3d LoadFrom(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException();

            string file_data = File.ReadAllText(path);
            var result_Meshes = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, SMDX_Geometry_j3d_Part>>(file_data);
            SMDX_Geometry_j3d smdx_geometry = new SMDX_Geometry_j3d();
            smdx_geometry.Meshes = result_Meshes;
            //smdx_geometry.data_path = path;

            return smdx_geometry;
        }

        public void Save (string path)
        {
            File.WriteAllText(path, System.Text.Json.JsonSerializer.Serialize(this));
            
        }
    }
}
