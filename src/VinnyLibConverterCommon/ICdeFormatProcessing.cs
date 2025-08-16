using System;
using System.Collections.Generic;
using System.Text;
using VinnyLibConverterCommon.VinnyLibDataStructure;

namespace VinnyLibConverterCommon
{
    public enum CdeVariant
    {
        SMDX,
        MLT,
        IMC,
        DotBIM,
        NWC,
        FBX,
        GLTF
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
        VinnyLibDataStructureModel Import(VinnyLibDataStructureIOParameters openParameters);

        /// <summary>
        /// Сохраняет данные модели (data) в данный CDE-формат
        /// </summary>
        /// <param name="data"></param>
        /// <param name="outputParameters"></param>
        void Export(VinnyLibDataStructureModel data, VinnyLibDataStructureIOParameters outputParameters);
    }

    /// <summary>
    /// Шаблон класса для подготовки структуры данных из некой среды (САПР) и сохранения данных в целевой CDE-формат
    /// </summary>
    public interface ICadExportProcessing
    {
        VinnyLibDataStructureModel CreateData();
        void ExportTo(CdeVariant outputType, VinnyLibDataStructureModel data, VinnyLibDataStructureIOParameters outputParameters);
    }

    /// <summary>
    /// Шаблон класса для импорта в данную среду (САПР) данных из целевого CDE-формата
    /// </summary>
    public interface ICadImportProcessing
    {
        VinnyLibDataStructureModel ImportFrom(CdeVariant outputType, VinnyLibDataStructureIOParameters openParameters);
    }
}
