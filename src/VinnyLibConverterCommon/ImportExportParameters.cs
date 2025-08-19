using System;
using System.Collections.Generic;
using System.Text;
using VinnyLibConverterCommon.Transformation;

namespace VinnyLibConverterCommon
{
    /// <summary>
    /// Вспомогательный класс для задания параметров чтения/записи сцены
    /// </summary>
    public sealed class ImportExportParameters
    {
        public ImportExportParameters()
        {
            CheckGeometryDubles = true;
            CheckMaterialsDubles = true;
            CheckParameterDefsDubles = true;
            TransformationInfo = new List<ICoordinatesTransformation>
            {
                TransformationMatrix4x4.CreateEmptyTransformationMatrix()
            };
        }

        #region Для локальных CDE (файлы)
        /// <summary>
        /// Абсолютный файловый путь к данным (для чтения и записи)
        /// </summary>
        public string Path { get; set; }
        #endregion

        #region Для локальных CDE (БД)
        public string DbConnectionString { get; set; }
        #endregion

        #region Для WEB CDE
        public string Token { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        #endregion

        public bool CheckGeometryDubles { get; set; }
        public bool CheckMaterialsDubles { get; set; }
        public bool CheckParameterDefsDubles { get; set; }

        /// <summary>
        /// Набор последовательных преобразований координат
        /// </summary>
        public List<ICoordinatesTransformation> TransformationInfo { get; set; }
        public static ImportExportParameters mActiveConfig { get; set; } = new ImportExportParameters();
    }
}
