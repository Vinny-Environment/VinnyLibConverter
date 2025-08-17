using System;
using VinnyLibConverterCommon;
using VinnyLibConverterCommon.VinnyLibDataStructure;

namespace VinnyLibConverterKernel
{
    public class VinnyLibConverter
    {
        private VinnyLibConverter() { }
        public VinnyLibConverter(string libPath)
        {
            //TODO: Load assemblies
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
    }
}
