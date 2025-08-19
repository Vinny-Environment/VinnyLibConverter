using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using VinnyLibConverterCommon;
using VinnyLibConverterCommon.VinnyLibDataStructure;

namespace VinnyLibConverterKernel
{
    public class VinnyLibConverter
    {
        private VinnyLibConverter() { }
        private VinnyLibConverter(string libDirectoryPath)
        {
            //TODO: Load assemblies
            string VinnyLibConverterCommonPath = Path.Combine(libDirectoryPath, "VinnyLibConverterCommon.dll");
            var ass = Assembly.LoadFrom(VinnyLibConverterCommonPath);

            foreach (string DepsDirPath in Directory.GetDirectories(Path.Combine(libDirectoryPath, "dependencies"), "*.*", SearchOption.TopDirectoryOnly))
            {
                foreach (string DepsAssPath in Directory.GetFiles(DepsDirPath, "*.dll", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        Assembly.LoadFrom(DepsAssPath);
                    }
                    catch (Exception ex) { Debug.Print(ex.Message); }
                }
            }
        }

        public static VinnyLibConverter CreateInstance(string libPath, bool force = false)
        {
            if (mInstance == null) mInstance = new VinnyLibConverter(libPath);
            else if (force) mInstance = new VinnyLibConverter(libPath);
            return mInstance;
        }

        public VinnyLibDataStructureModel ImportModel(CdeVariant ModelType, IEParameters openParameters)
        {
            switch(ModelType)
            {
                case CdeVariant.DotBIM: return new VinnyLibConverter_DotBIM.DotBimFormatProcessing().Import(openParameters);
            }
            return null;
        }

        public void ExportModel(CdeVariant ModelType, VinnyLibDataStructureModel ModelData, IEParameters outputParameters)
        {
            if (ModelData == null) return;
            if (File.Exists(outputParameters.Path)) File.Delete(outputParameters.Path);
            switch (ModelType)
            {
                case CdeVariant.DotBIM:
                    new VinnyLibConverter_DotBIM.DotBimFormatProcessing().Export(ModelData, outputParameters);
                    break;
            }
        }

        public void Convert(CdeVariant inputType, IEParameters openParameters, CdeVariant outputType, IEParameters outputParameters)
        {
            VinnyLibDataStructureModel data = ImportModel(inputType, openParameters);
            ExportModel(outputType, data, outputParameters);
        }

        private static VinnyLibConverter mInstance;
    }
}
