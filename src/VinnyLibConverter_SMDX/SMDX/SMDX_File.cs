using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VinnyLibConverterUtils;
using VinnyLibConverter_SMDX.SMDX.Materials;
using System.Xml.Linq;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;

namespace VinnyLibConverter_SMDX.SMDX
{
    public enum SmdxFileStatusVariant
    {
        Read,
        CreateAndWrite
    }
    /// <summary>
    /// Класс, описывающий структуру файла SMDX (ZIP + каталоги)
    /// </summary>
    public class SMDX_File
    {
        public const string smdx_catalog_documents = "documents";
        public const string smdx_catalog_effects = "effects";
        public const string smdx_catalog_geometry = "geometry";
        public const string smdx_catalog_materials = "materials";
        public const string smdx_catalog_textures = "textures";

        public SMDX_Content Content { get; private set; }

        #region Методы для создания новых данных
        public void CreateDocument(string path)
        {
            string docPath = Path.Combine(SmdxDataPath, smdx_catalog_documents, Path.GetFileNameWithoutExtension(path));
            if (!File.Exists(docPath)) 
            {
                File.Copy(path, docPath);
                Content.documents.Add(new SMDX_Content_Document() { href = Path.GetFileNameWithoutExtension(path) });
            }
        }

        public void CreateGeometry(SMDX_Content_Geometry geometryInfo, SMDX_Geometry_j3d geometryFile)
        {
            this.Content.geometry.Add(geometryInfo);
            string j3dPath = Path.Combine(SmdxDataPath, smdx_catalog_geometry, geometryInfo.href);
            geometryFile.Save(j3dPath);
        }

        public void CreateMaterial(SMDX_Material_BlinnPhong materialInfo, string materialName)
        {
            string jmtlPath = Path.Combine(SmdxDataPath, smdx_catalog_materials, materialName);
            materialInfo.Save(jmtlPath);
        }

        #endregion

        /// <summary>
        /// Абсолютный путь к папке, содержащей распакованную версию SMDX для чтения (или перед созданием)
        /// </summary>
        public string SmdxDataPath { get; private set; }

        /// <summary>
        /// Возвращает преобразованное описание геометрии
        /// </summary>
        /// <param name="j3d_name">Наименование файла геометрии с расширением</param>
        /// <returns></returns>
        public SMDX_Geometry_j3d GetGeometryJ3d(string j3dName)
        {
            string j3d_path = Path.Combine(SmdxDataPath, smdx_catalog_geometry, j3dName);
            if (File.Exists(j3d_path))
            {
                SMDX_Geometry_j3d g_def_raw = SMDX_Geometry_j3d.LoadFrom(j3d_path);
                //SMDX_GeometryHelper g_def = new SMDX_GeometryHelper(g_def_raw);
                return g_def_raw;
            }
            else
            {
                VinnyLibConverterLogger.InitLogger().WriteLog("Geometry-определение не было найдено по пути " + j3d_path);
                return null;
            }
        }

        //TODO: написать реализацию иных типов материалов
        public SMDX_Material_BlinnPhong GetMaterial(string jmtlName)
        {
            string jmtl_path = Path.Combine(SmdxDataPath, smdx_catalog_materials, jmtlName);
            if (File.Exists(jmtl_path))
            {
                return SMDX_Material_BlinnPhong.LoadFrom(jmtl_path);
            }
            else
            {
                VinnyLibConverterLogger.InitLogger().WriteLog("JMTL-определение не было найдено по пути " + jmtl_path);
                return null;
            }
        }

        public SMDX_File(string smdxFilePath)
        {
            string startPath = Path.GetTempPath();
#if DEBUG
            startPath = @"E:\DataTest\VinnyLibConverterSamples\smdx";
#endif
            SmdxDataPath = Path.Combine(startPath, Path.GetFileNameWithoutExtension(smdxFilePath));

            if (!Directory.Exists(SmdxDataPath))
            {
                ZipFile.ExtractToDirectory(smdxFilePath, SmdxDataPath);
            }

            Content = SMDX_Content.LoadSchema(Path.Combine(SmdxDataPath, "content.json"));
        }
        public SMDX_File()
        {
            string startPath = Path.GetTempPath();
#if DEBUG
            startPath = @"E:\DataTest\VinnyLibConverterSamples\smdx";
#endif
            SmdxDataPath = Path.Combine(startPath, Guid.NewGuid().ToString("N"));

            if (Directory.Exists(SmdxDataPath)) Directory.Delete(SmdxDataPath, true);
            Directory.CreateDirectory(SmdxDataPath);

            Directory.CreateDirectory(Path.Combine(SmdxDataPath, smdx_catalog_documents));
            Directory.CreateDirectory(Path.Combine(SmdxDataPath, smdx_catalog_effects));
            Directory.CreateDirectory(Path.Combine(SmdxDataPath, smdx_catalog_geometry));
            Directory.CreateDirectory(Path.Combine(SmdxDataPath, smdx_catalog_materials));
            Directory.CreateDirectory(Path.Combine(SmdxDataPath, smdx_catalog_textures));

            Content = new SMDX_Content();
        }

        public void Save(string path)
        {
            //Сохраняем все ресурсы
            Content.Save(Path.Combine(SmdxDataPath, SMDX_Content.ContentFileName));

            //save docs
            foreach (SMDX_Content_Document? doc in Content.documents)
            {
                
            }


            ZipFile.CreateFromDirectory(SmdxDataPath, path);
        }
    }
}
