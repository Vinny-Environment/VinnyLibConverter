using System;
using System.Collections.Generic;
using System.Text;
using VinnyLibConverterCommon.VinnyLibDataStructure;

namespace VinnyLibConverterCommon.Interfaces
{
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




}
