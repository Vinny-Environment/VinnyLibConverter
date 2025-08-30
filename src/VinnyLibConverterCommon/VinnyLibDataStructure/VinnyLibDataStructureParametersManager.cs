using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VinnyLibConverterCommon.VinnyLibDataStructure
{
    /// <summary>
    /// Вспомогательный класс для хранения и создания параметров
    /// </summary>
    public class VinnyLibDataStructureParametersManager
    {
        public const string CategoryDefaultName = "Default";
        public VinnyLibDataStructureParametersManager()
        {
            mParamDefCounter = 0;
            Parameters = new Dictionary<int, VinnyLibDataStructureParameterDefinition>();

            mCategoriesCounter = 0;
            Categories = new Dictionary<int, string>();
            CreateCategory(CategoryDefaultName);
        }

        public int CreateParameterDefinition(string paramName, VinnyLibDataStructureParameterDefinitionType paramType = VinnyLibDataStructureParameterDefinitionType.ParamString)
        {
            VinnyLibDataStructureParameterDefinition paramDef = new VinnyLibDataStructureParameterDefinition(mParamDefCounter, paramName, paramType);
            if (ImportExportParameters.mActiveConfig.CheckParameterDefsDubles)
            {
                foreach (var paramDefInfo in Parameters)
                {
                    if (paramDefInfo.Value.Equals(paramDef)) return paramDefInfo.Key;
                }
            }
            Parameters.Add(mParamDefCounter, paramDef);
            mParamDefCounter++;
            return mParamDefCounter - 1;
        }

        public void SetParameterDef(int id, VinnyLibDataStructureParameterDefinition paramDef)
        {
            Parameters[id] = paramDef;
        }

        public VinnyLibDataStructureParameterDefinition GetParamDefById(int id)
        {
            VinnyLibDataStructureParameterDefinition outputParamDef = new VinnyLibDataStructureParameterDefinition(0, "");
            if (Parameters.TryGetValue(id, out outputParamDef)) return outputParamDef;
            return null;
        }

        public int CreateCategory(string categoryName)
        {
            if (Categories.ContainsValue(categoryName)) return Categories.Where(a => a.Value == categoryName).First().Key;
            Categories.Add(mCategoriesCounter, categoryName);
            mCategoriesCounter++;
            return mCategoriesCounter - 1;
        }

        public string GetCategoryNameById(int idCategory)
        {
            string catName = "";
            if (Categories.TryGetValue(idCategory, out catName)) return catName;
            return null;
        }

        public Dictionary<string, List<VinnyLibDataStructureParameterValue>> SortParamsByCategories(List<VinnyLibDataStructureParameterValue> paramValuesInfo)
        {
            Dictionary<string, List<VinnyLibDataStructureParameterValue>> data = new Dictionary<string, List<VinnyLibDataStructureParameterValue>>();
            foreach (VinnyLibDataStructureParameterValue param in paramValuesInfo)
            {
                string categoryName = GetCategoryNameById(param.ParamCategoryId);
                if (data.ContainsKey(categoryName)) data[categoryName].Add(param);
                else data[categoryName] = new List<VinnyLibDataStructureParameterValue>() { param };
            }

            return data;

        }

        public Dictionary<int, VinnyLibDataStructureParameterDefinition> Parameters { get; internal set; }
        private int mParamDefCounter = 0;

        public Dictionary<int, string> Categories { get; internal set; }
        public int mCategoriesCounter = 0;

    }
}
