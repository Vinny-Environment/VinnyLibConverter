using System;
using System.Collections.Generic;
using System.Text;
using VinnyLibConverterCommon.VinnyLibDataStructure;

namespace VinnyLibConverterCommon
{
    public enum CdeVariant
    {
        SMDX,
        MLT,//? Мёртвый проприетарный формат, уж лучше мб на кадлиб сразу
        IMC,
        DotBIM,
        NWC,
        FBX,
        GLTF,
        IFC,
        DXF,
        LandXML
    }

    /// <summary>
    /// Строковые сокращения из CdeVariant
    /// </summary>
    public class CdeFrmtNames
    {
        public const string CdeFrmt_SMDX = "smdx";
        public const string CdeFrmt_MLT = "mlt";
        public const string CdeFrmt_IMC = "imc";
        public const string CdeFrmt_DotBim = "dotbim";
        public const string CdeFrmt_NWC = "nwcreate";
        public const string CdeFrmt_FBX = "fbx";
        public const string CdeFrmt_GLTF = "gltf";
        public const string CdeFrmt_IFC = "ifc";
        public const string CdeFrmt_DXF = "dxf";
        public const string CdeFrmt_LandXML = "landxml";

        public static Dictionary<CdeVariant, string> CdeFormatName
        {
            get
            {
                return new Dictionary<CdeVariant, string>
                {
                    {CdeVariant.SMDX, CdeFrmt_SMDX},
                    {CdeVariant.MLT, CdeFrmt_MLT},
                    {CdeVariant.IMC, CdeFrmt_IMC},
                    {CdeVariant.DotBIM, CdeFrmt_DotBim},
                    {CdeVariant.NWC, CdeFrmt_NWC},
                    {CdeVariant.FBX, CdeFrmt_FBX},
                    {CdeVariant.GLTF, CdeFrmt_GLTF},
                    {CdeVariant.IFC, CdeFrmt_IFC},
                    {CdeVariant.DXF, CdeFrmt_DXF},
                    {CdeVariant.LandXML, CdeFrmt_LandXML},

                };
            }
        }


        /// <summary>
        /// Вспомогательный метод для получения строки-идентификатора CDE. Используется пока только в путях к библиотекам. 
        /// </summary>
        /// <param name="cdeType"></param>
        /// <returns></returns>
        public static string GetCdeFrmtName(CdeVariant cdeType)
        {
            string res = "";
            switch(cdeType)
            {
                case CdeVariant.SMDX:
                    res = CdeFrmt_SMDX;
                    break;
                case CdeVariant.MLT:
                    res = CdeFrmt_MLT;
                    break;
                case CdeVariant.IMC:
                    res = CdeFrmt_IMC;
                    break;
                case CdeVariant.DotBIM:
                    res = CdeFrmt_DotBim;
                    break;
                case CdeVariant.NWC:
                    res = CdeFrmt_NWC;
                    break;
                case CdeVariant.FBX:
                    res = CdeFrmt_FBX;
                    break;
                case CdeVariant.GLTF:
                    res = CdeFrmt_GLTF;
                    break;
            }
            return res;
        }
    }

    

    /// <summary>
    /// Шаблон класса, описывающий отдельный CDE-формат
    /// </summary>
    public interface ICdeFormatProcessing
    {
        /// <summary>
        /// Возвращает тип CDE-формата
        /// </summary>
        /// <returns></returns>
        CdeVariant GetCdeType();

        /// <summary>
        /// Возвращает признак, доступно ли чтение данного CDE-формата
        /// </summary>
        /// <returns></returns>
        bool IsReadable();

        /// <summary>
        /// Возвращает признак, доступна ли запись в данный CDE-формат
        /// </summary>
        /// <returns></returns>
        bool IsWriteable();

        /// <summary>
        /// Загружает данные из модели в данном CDE-формате
        /// </summary>
        /// <param name="openParameters"></param>
        /// <returns></returns>
        VinnyLibDataStructureModel Import(ImportExportParameters openParameters);

        /// <summary>
        /// Сохраняет данные модели (data) в данный CDE-формат
        /// </summary>
        /// <param name="data"></param>
        /// <param name="outputParameters"></param>
        void Export(VinnyLibDataStructureModel vinnyData, ImportExportParameters outputParameters);

        /// <summary>
        /// Загрузить в AppDomain вспомогательные библиотеки, используемые данной
        /// </summary>
        //void LoadAuxiliaryAssemblies();

        /// <summary>
        /// Выгрузить из AppDomain вспомогательные библиотеки, используемые данной
        /// </summary>
        //void UnloadAuxiliaryAssemblies();
    }

    /// <summary>
    /// Шаблон класса для подготовки структуры данных из некой среды (САПР) и сохранения данных в целевой CDE-формат
    /// </summary>
    public interface ICadExportProcessing
    {
        VinnyLibDataStructureModel CreateData();
        void ExportTo(CdeVariant outputType, VinnyLibDataStructureModel vinnyData, ImportExportParameters outputParameters);
    }

    /// <summary>
    /// Шаблон класса для импорта в данную среду (САПР) данных из целевого CDE-формата
    /// </summary>
    public interface ICadImportProcessing
    {
        VinnyLibDataStructureModel ImportFrom(CdeVariant outputType, ImportExportParameters openParameters);
    }
}
