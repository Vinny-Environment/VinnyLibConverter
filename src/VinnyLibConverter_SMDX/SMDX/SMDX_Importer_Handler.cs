using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinnyLibConverter_SMDX.SMDX
{
    /// <summary>
    /// Класс для обработки SMDX файлов
    /// </summary>
    internal class SMDX_Importer_Handler
    {
        private string[] p_Files;
        private SMDX_File[] p_RawData;

        /// <summary>
        /// Ключ: имя href (j3d-файла), значение -- СЫРАЯ геометрия для него (до применения Transform)
        /// </summary>
        private Dictionary<string, SMDX_GeometryHelper> geometryCashe;
        public SMDX_Importer_Handler(string[] files)
        {
            p_Files = files;
            p_RawData = new SMDX_File[p_Files.Length];
            geometryCashe = new Dictionary<string, SMDX_GeometryHelper>();
        }

        //public CADLib_Data_Structure[] GetData()
        //{

        //    CADLib_Data_Structure[] target_data = new CADLib_Data_Structure[p_RawData.Length];

        //    for (int file_counter = 0; file_counter < p_Files.Length; file_counter++)
        //    {
        //        string current_filePath = p_Files[file_counter];
        //        SMDX_Data data = new SMDX_Data(current_filePath);

        //        /*
        //         Перебираем блок insertions. 
        //         */

        //        CADLib_Data_Structure smdx_info = new CADLib_Data_Structure();
        //        foreach (var insertionFeature in data.Content.insertions)
        //        {
        //            CADLib_Entity_Structure entDef = new CADLib_Entity_Structure();
        //            //geometry
        //            if (insertionFeature.geometry >= 0 && insertionFeature.geometry < data.Content.geometry.Length)
        //            {
        //                SMDX_Content_Geometry entGeometryContent = data.Content.geometry[(int)insertionFeature.geometry];
        //                SMDX_GeometryHelper entGeometryDefHelper = null;
        //                if (!geometryCashe.ContainsKey(entGeometryContent.href))
        //                {
        //                    entGeometryDefHelper = data.GetGeometry(entGeometryContent.href);
        //                }
        //                else entGeometryDefHelper = geometryCashe[entGeometryContent.href];

        //                if (insertionFeature.angle == null) insertionFeature.angle = 0.0;
        //                if (insertionFeature.scale == null) insertionFeature.scale = new float[3] { 1.0, 1.0, 1.0 };

        //                if (entGeometryDefHelper != null)
        //                {
        //                    entGeometryDefHelper.Transform(data.Content.wcs, insertionFeature.position, (float)insertionFeature.angle, insertionFeature.scale);

        //                    entDef.Points = entGeometryDefHelper.Points.ToList();
        //                    entDef.Faces = entGeometryDefHelper.Faces.ToList();
        //                    entDef.Colors = entGeometryDefHelper.Colors;
        //                    entDef.Faces2Colors = entGeometryDefHelper.FacesColors; //TODO: Как там будет переделано, то ок
        //                    entDef.CalculateMesh();
        //                }
        //                else
        //                {
        //                    //Exception TODO
        //                }
        //            }
        //            smdx_info.Entities.Add(entDef);
        //        }

        //        target_data[file_counter] = smdx_info;

        //        TBS_CADLib_Utils.WriteLog($"Файл {current_filePath} был успешно обработан");

        //    }
        //    //Обработка сформированных файлов
        //    TBS_CADLib_Utils.WriteLog($"Обработка файлов завершена! Переходим к чтению сформированных данных...");

        //    return target_data;
        //}
    }
}
