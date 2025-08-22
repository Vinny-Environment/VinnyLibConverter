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
        void Export(VinnyLibDataStructureModel data, ImportExportParameters outputParameters);

        /// <summary>
        /// Загрузить в AppDomain вспомогательные библиотеки, используемые данной
        /// </summary>
        void LoadAuxiliaryAssemblies();

        /// <summary>
        /// Выгрузить из AppDomain вспомогательные библиотеки, используемые данной
        /// </summary>
        void UnloadAuxiliaryAssemblies();
    }

    /// <summary>
    /// Шаблон класса для подготовки структуры данных из некой среды (САПР) и сохранения данных в целевой CDE-формат
    /// </summary>
    public interface ICadExportProcessing
    {
        VinnyLibDataStructureModel CreateData();
        void ExportTo(CdeVariant outputType, VinnyLibDataStructureModel data, ImportExportParameters outputParameters);
    }

    /// <summary>
    /// Шаблон класса для импорта в данную среду (САПР) данных из целевого CDE-формата
    /// </summary>
    public interface ICadImportProcessing
    {
        VinnyLibDataStructureModel ImportFrom(CdeVariant outputType, ImportExportParameters openParameters);
    }
}
