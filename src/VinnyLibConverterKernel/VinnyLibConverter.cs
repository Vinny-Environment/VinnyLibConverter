using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using VinnyLibConverterCommon;
using VinnyLibConverterCommon.Interfaces;
using VinnyLibConverterCommon.VinnyLibDataStructure;

using VinnyLibConverterUtils;

namespace VinnyLibConverterKernel
{
    public class VinnyLibConverter
    {
        private VinnyLibConverter() { }
        private VinnyLibConverter(string VinnyLibDirectoryPath)
        {
            //TODO: Load assemblies
            pLibsLoadingStatus = new Dictionary<CdeVariant, bool>();
            pVinnyLibDirectory = VinnyLibDirectoryPath;
            string VinnyLibConverterCommonPath = Path.Combine(pVinnyLibDirectory, "VinnyLibConverterCommon.dll");
            var ass = Assembly.LoadFrom(VinnyLibConverterCommonPath);

            //Add to PATH env nwcreate-dir
            string nwcreateDir = Path.Combine(VinnyLibDirectoryPath, pDepsDirName, "nwcreate");
            string newEnwPathValue = Environment.GetEnvironmentVariable("PATH");
            if (newEnwPathValue.EndsWith(";")) newEnwPathValue += nwcreateDir + ";";
            else newEnwPathValue += ";" + nwcreateDir + ";";

            Environment.SetEnvironmentVariable("PATH", newEnwPathValue);

            foreach (string depsDir in Directory.GetDirectories(Path.Combine(VinnyLibDirectoryPath, pDepsDirName), "*.*", SearchOption.TopDirectoryOnly))
            {
                
                foreach (string DepsAssPath in Directory.GetFiles(depsDir, "*.dll", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        Assembly.LoadFrom(DepsAssPath);
                    }
                    catch (Exception ex) { VinnyLibConverterLogger.InitLogger().WriteLog(ex.Message); }
                }
            }

            //принудительная загрузка некоторых библиотек
            //string VinnyLibDepsNwcreate_nwcreateWrapperLib = Path.Combine(pVinnyLibDirectory, pDepsDirName, "nwcreate", "nwcreateWrapperLib.dll");
            //ass = Assembly.LoadFrom(VinnyLibDepsNwcreate_nwcreateWrapperLib);

        }

        /*
        /// <summary>
        /// Отложенная загрузка библиотек-конвертеров (при необходимости)
        /// </summary>
        /// <param name="libType"></param>
        private void LoadAssemblies(CdeVariant libType)
        {
            if (!pLibsLoadingStatus.ContainsKey(libType) || !pLibsLoadingStatus[libType])//&& !pLibsLoadingStatus[libType]
            {
                string assPath = Path.Combine(this.pVinnyLibDirectory, pDepsDirName, CdeFrmtNames.CdeFormatName[libType]);
                foreach (string DepsAssPath in Directory.GetFiles(assPath, "*.dll", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        Assembly.LoadFrom(DepsAssPath);
                    }
                    catch (Exception ex) { VinnyLibConverterLogger.InitLogger().WriteLog(ex.Message); }
                }
                pLibsLoadingStatus[libType] = true;
            }
        }
        */

        public static VinnyLibConverter CreateInstance(string libPath, bool force = false)
        {
            if (mInstance == null) mInstance = new VinnyLibConverter(libPath);
            else if (force) mInstance = new VinnyLibConverter(libPath);
            return mInstance;
        }

        public VinnyLibDataStructureModel ImportModel(ImportExportParameters openParameters)
        {
            switch (openParameters.ModelType)
            {
                case CdeVariant.VinnyLibConverterCache: return VinnyLibDataStructureModel.LoadFromFile(openParameters.Path);
                case CdeVariant.DotBIM: return new VinnyLibConverter_DotBIM.DotBimFormatProcessing().Import(openParameters);
                case CdeVariant.SMDX: return new VinnyLibConverter_SMDX.SMDX_FormatProcessing().Import(openParameters);
                case CdeVariant.NWC: return new VinnyLibConverter_nwcreate.nwcreate_FormatProcessing().Import(openParameters);

            }
            return null;
        }

        public void ExportModel(VinnyLibDataStructureModel ModelData, ImportExportParameters outputParameters)
        {
            if (ModelData == null) return;
            if (File.Exists(outputParameters.Path)) File.Delete(outputParameters.Path);
            switch (outputParameters.ModelType)
            {
                case CdeVariant.DotBIM:
                    new VinnyLibConverter_DotBIM.DotBimFormatProcessing().Export(ModelData, outputParameters);
                    break;
                case CdeVariant.SMDX:
                    new VinnyLibConverter_SMDX.SMDX_FormatProcessing().Export(ModelData, outputParameters);
                    break;
                case CdeVariant.NWC:
                    new VinnyLibConverter_nwcreate.nwcreate_FormatProcessing().Export(ModelData, outputParameters);
                    break;
                case CdeVariant.VinnyLibConverterCache:
                    {
                        ModelData.SetCoordinatesTransformation(outputParameters.TransformationInfo);
                        ModelData.Save(outputParameters.Path);
                    }
                    break;
            }
        }

        public void Convert(ImportExportParameters openParameters, ImportExportParameters outputParameters)
        {
            VinnyLibDataStructureModel data = ImportModel(openParameters);
            ExportModel(data, outputParameters);
        }

        private static VinnyLibConverter mInstance;
        private Dictionary<CdeVariant, bool> pLibsLoadingStatus;
        private string pVinnyLibDirectory;
        private const string pDepsDirName = "dependencies";
    }
}
