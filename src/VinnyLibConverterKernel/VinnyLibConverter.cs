using System;
using VinnyLibConverterCommon;
using VinnyLibConverterCommon.VinnyLibDataStructure;

using VinnyLibConverter_DotBIM;

namespace VinnyLibConverterKernel
{
    public class VinnyLibConverter
    {
        public VinnyLibDataStructureModel ImportModel(CdeVariant ModelType, VinnyLibDataStructureIOParameters openParameters)
        {
            switch(ModelType)
            {
                case CdeVariant.DotBIM: return new DotBimFormatProcessing().Import(openParameters);
            }
            return null;
        }

        public void ExportModel(CdeVariant ModelType, VinnyLibDataStructureModel ModelData, VinnyLibDataStructureIOParameters outputParameters)
        {
            if (ModelData == null) return;
            switch (ModelType)
            {
                case CdeVariant.DotBIM:
                    new DotBimFormatProcessing().Export(ModelData, outputParameters);
                    break;
            }
        }

        public void Convert(CdeVariant inputType, VinnyLibDataStructureIOParameters openParameters, CdeVariant outputType, VinnyLibDataStructureIOParameters outputParameters)
        {
            VinnyLibDataStructureModel data = ImportModel(inputType, openParameters);
            ExportModel(outputType, data, outputParameters);
        }


    }
}
