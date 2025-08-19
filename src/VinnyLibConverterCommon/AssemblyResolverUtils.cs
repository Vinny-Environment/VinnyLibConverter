using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace VinnyLibConverterCommon
{
    /// <summary>
    /// Вспомогательный класс для загрузки и выгрузки библиотек из домена
    /// </summary>
    public class AssemblyResolverUtils
    {
        public static AssemblyResolverUtils CreateInstance(string VinnyLibConverterPath)
        {
            if (mInstance == null) mInstance = new AssemblyResolverUtils() { mVinnyLibConverterPath = VinnyLibConverterPath };
            return mInstance;
        }

        public void LoadAuxiliaryAssembles(CdeVariant cdeLibrary)
        {
            foreach (string assPath in Directory.GetFiles(Path.Combine(mVinnyLibConverterPath, CdeFrmtNames.GetCdeFrmtName(cdeLibrary)), "*.dll", SearchOption.TopDirectoryOnly))
            {
                System.Reflection.Assembly.LoadFrom(assPath);
            }
        }

        private static AssemblyResolverUtils mInstance;
        private string mVinnyLibConverterPath { get; set; }
    }
}
