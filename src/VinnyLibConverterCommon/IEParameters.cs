using System;
using System.Collections.Generic;
using System.Text;

namespace VinnyLibConverterCommon
{
    /// <summary>
    /// Вспомогательный класс для задания параметров чтения/записи сцены
    /// </summary>
    public sealed class IEParameters
    {
        public IEParameters()
        {
            TransformationMatrixInfo = TransformationMatrix.CreateEmptyTransformationMatrix();
            CheckGeometryDubles = true;
            CheckMaterialsDubles = true;
            CheckParameterDefsDubles = true;
        }

        /// <summary>
        /// Абсолютный файловый путь к данным (для чтения и записи)
        /// </summary>
        public string Path { get; set; }

        #region Для WEB CDE
        public string Token { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        #endregion

        public bool CheckGeometryDubles { get; set; }
        public bool CheckMaterialsDubles { get; set; }
        public bool CheckParameterDefsDubles { get; set; }

        public TransformationMatrix TransformationMatrixInfo { get; set; }
        public static IEParameters mActiveConfig { get; set; } = new IEParameters();
    }
}
