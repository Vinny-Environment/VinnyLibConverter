using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinnyLibConverter_SMDX.SMDX.Materials;
using VinnyLibConverterUtils;

namespace VinnyLibConverter_SMDX.SMDX
{
    /// <summary>
    /// Кдасс для описания триангулированной геометрии с материалами из SMDX_Geometry
    /// </summary>
    public class SMDX_GeometryHelper
    {
        #region Внешние данные
        public List<float[]> Points { get; private set; }
        public List<int[]> Faces { get; private set; }
        public List<int> FacesColors { get; private set; } //Один размер с Faces

        /// <summary>
        /// Определение свойств
        /// </summary>
        public List<int[]> Colors { get; private set; }

        #endregion
        public SMDX_GeometryHelper(SMDX_Geometry_j3d geometry)
        {
            p_SMDX_Directory = new DirectoryInfo(Path.GetDirectoryName(geometry.data_path)).Parent.FullName;
            p_GeometryRawInfo = geometry.Meshes;
            Points = new List<float[]>();
            Faces = new List<int[]>();
            FacesColors = new List<int>();
            Colors = new List<int[]>();
            //Читаем даные геометрии
            foreach (var MeshInfo in p_GeometryRawInfo)
            {
                SMDX_Geometry_j3d_Part geometryInfo = MeshInfo.Value;
                //this.Points = new float[geometryInfo.positions.Length][];
                for (int point_counter = 0; point_counter < geometryInfo.positions.Length; point_counter += 3)
                {
                    float x, y, z;
                    x = geometryInfo.positions[point_counter];
                    y = geometryInfo.positions[point_counter + 1];
                    z = geometryInfo.positions[point_counter + 2];
                    Points.Add(new float[] { x, y, z });
                }
                //Читаем информацию по цветам
                foreach (var materialsInfo in geometryInfo.groups)
                {
                    int[] color_def = GetMaterialAndTransparency(materialsInfo.Key);
                    Colors.Add(color_def);
                }
                if (!Colors.Any())
                {
                    Colors.Add(new int[] { 0, 0, 0, 0 });
                    VinnyLibConverterLogger.InitLogger().WriteLog("Для геометрии нет простых текстур"); //TOODO: id такой геометрии
                }

                //Читаем грани
                //this.Faces = new int[geometryInfo.triangles.Length][];
                //this.FacesColors = new int[geometryInfo.triangles.Length];
                int previous_pt_index = -1;
                for (int triangle_counter = 0; triangle_counter < geometryInfo.triangles.Length; triangle_counter += 3)
                {
                    int triangle_num = triangle_counter / 3;
                    if (triangle_counter == 0) previous_pt_index = geometryInfo.triangles[0];
                    else previous_pt_index += geometryInfo.triangles[triangle_counter];
                    int pt1 = previous_pt_index;
                    previous_pt_index += geometryInfo.triangles[triangle_counter + 1];
                    int pt2 = previous_pt_index;
                    previous_pt_index += geometryInfo.triangles[triangle_counter + 2];
                    int pt3 = previous_pt_index;

                    Faces.Add(new int[] { pt1, pt2, pt3 });
                    FacesColors.Add(GetColor(MeshInfo.Key, triangle_num));
                }
            }

        }

        private float[] _scale
        {
            get
            {
                return new float[] { 1f, 1f, 1f };
            }
        }

        /// <summary>
        /// Преобразует геометрию с учетом точки нуля SMDX (wcs), точки вставки мэша (center), угла поворота в радианах (angle), масштаба (scale)
        /// </summary>
        /// <param name="wcs"></param>
        /// <param name="center"></param>
        /// <param name="angle"></param>
        public void Transform(float[] wcs, float[] center, float angle, float[] scale = null)
        {
            if (scale == null) scale = _scale;
            //TODO: Реализовать преобразование координат через матрицы
            //Найти вариант указать scale массивом предзаданным
        }

        #region Вспомогательное
        /// <summary>
        /// Возвращает R G B Transparence
        /// </summary>
        /// <param name="material_name"></param>
        /// <returns></returns>
        private int[] GetMaterialAndTransparency(string material_name)
        {
            string material_path = Path.Combine(p_SMDX_Directory, "materials", material_name);
            SMDX_Material_BlinnPhong material_def = SMDX_Material_BlinnPhong.LoadFrom(material_path);

            int[] color_ambient = new int[] { 0, 0, 0, 0 };
            if (material_def != null)
            {
                var color = material_def.ambient.Select(a => Convert.ToInt32(a * 255)).ToArray();
                color_ambient[0] = color[0];
                color_ambient[1] = color[1];
                color_ambient[2] = color[2];
                color_ambient[3] = Convert.ToInt32(material_def.transparency);
            }
            return color_ambient;
        }

        /// <summary>
        /// Возвращает индекс цвета геометрии Colors для данной грани
        /// </summary>
        /// <param name="face_counter"></param>
        /// <returns></returns>
        private int GetColor(string key, int face_counter)
        {
            int counter = 0;
            foreach (var materialsInfo in p_GeometryRawInfo[key].groups)
            {
                for (int face_block_info_counter = 0; face_block_info_counter < materialsInfo.Value.Length; face_block_info_counter += 2)
                {
                    int tr_start = materialsInfo.Value[face_block_info_counter];
                    int tr_end = materialsInfo.Value[face_block_info_counter + 1];
                    if (face_counter >= tr_start && face_counter <= tr_end) return counter;
                }
                counter++;
            }
            return counter;
        }


        private string p_SMDX_Directory = "";
        private Dictionary<string, SMDX_Geometry_j3d_Part> p_GeometryRawInfo;
        #endregion
    }
}
