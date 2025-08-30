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
            ReprojectOnlyPosition = true;
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
        //TODO: (напр., для файла мэппинга IFC)
        private string[] AuxliaryPaths { get; set; }
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

        /// <summary>
        /// Флаг, проверять ли дублирование цветов материалов
        /// </summary>
        public bool CheckMaterialsDubles { get; set; }

        /// <summary>
        /// Флаг, проверять ли уникальность параметров (внутренним имена и категории)
        /// </summary>
        public bool CheckParameterDefsDubles { get; set; }

        /// <summary>
        /// Если true, то при наличии в TransformationInfo параметров для Аффинного преобразования или геодезического будут пересчитаны только координаты точки вставки mesh'а, а координаты самой базовой геометрии останутся неизменными. Если false то все mesh'ы будут пересчитаны в соответствующие им VinnyLibDataStructureGeometryPlacementInfo и все VinnyLibDataStructureGeometryPlacementInfo "обнулены"
        /// </summary>
        public bool ReprojectOnlyPosition { get; set; }

        /// <summary>
        /// Набор последовательных преобразований координат
        /// </summary>
        public List<ICoordinatesTransformation> TransformationInfo { get; set; }
        public static ImportExportParameters mActiveConfig { get; set; } = new ImportExportParameters();
    }
}
