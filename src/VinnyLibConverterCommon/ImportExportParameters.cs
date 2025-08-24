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
        internal ImportExportParameters()
        {
            CheckGeometryDubles = false;
            CheckMaterialsDubles = true;
            CheckParameterDefsDubles = true;
            TransformationInfo = new List<ICoordinatesTransformation>();
        }

        public static ImportExportParameters CreateForLocalCDE(string path)
        {
            ImportExportParameters p = new ImportExportParameters();
            p.Path = path;
            return p;
        }

        public static ImportExportParameters CreateForDatabaseCDE(string dbPath, string login, string password)
        {
            ImportExportParameters p = new ImportExportParameters();
            p.Path = dbPath;
            p.Login = login;
            p.Password = password;
            return p;
        }

        public static ImportExportParameters CreateForWebCDE(string token, string login, string password)
        {
            ImportExportParameters p = new ImportExportParameters();
            p.Token = token;
            p.Login = login;
            p.Password = password;
            return p;
        }



        #region Для локальных CDE (файлы, БД)
        /// <summary>
        /// Абсолютный файловый путь к данным (для чтения и записи)
        /// </summary>
        public string Path { get; set; }
        #endregion

        #region Для WEB CDE
        public string Token { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        #endregion

        /// <summary>
        /// Точность округления чисел кооординат. Если CheckGeometryDubles = true, будут искаться дубли с данным округлением
        /// </summary>
        public int VertexAccuracy { get; set; } = 5;
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
