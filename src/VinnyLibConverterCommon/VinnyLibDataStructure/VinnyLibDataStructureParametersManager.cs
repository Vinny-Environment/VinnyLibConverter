using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

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
            mParameters = new Dictionary<int, VinnyLibDataStructureParameterDefinition>();

            mCategoriesCounter = 0;
            mCategories = new Dictionary<int, string>();
            CreateCategory(CategoryDefaultName);
        }

        public int CreateParameterDefinition(string paramName, VinnyLibDataStructureParameterDefinitionType paramType = VinnyLibDataStructureParameterDefinitionType.ParamString)
        {
            VinnyLibDataStructureParameterDefinition paramDef = new VinnyLibDataStructureParameterDefinition(mParamDefCounter, paramName, paramType);
            if (ImportExportParameters.mActiveConfig.CheckParameterDefsDubles)
            {
                foreach (var paramDefInfo in mParameters)
                {
                    if (paramDefInfo.Value.Equals(paramDef)) return paramDefInfo.Key;
                }
            }
            mParameters.Add(mParamDefCounter, paramDef);
            mParamDefCounter++;
            return mParamDefCounter - 1;
        }

        public void SetParameterDef(int id, VinnyLibDataStructureParameterDefinition paramDef)
        {
            mParameters[id] = paramDef;
        }

        public VinnyLibDataStructureParameterDefinition GetParamDefById(int id)
        {
            VinnyLibDataStructureParameterDefinition outputParamDef = new VinnyLibDataStructureParameterDefinition(0, "");
            if (mParameters.TryGetValue(id, out outputParamDef)) return outputParamDef;
            return null;
        }

        public int CreateCategory(string categoryName)
        {
            if (mCategories.ContainsValue(categoryName)) return mCategories.Where(a => a.Value == categoryName).First().Key;
            mCategories.Add(mCategoriesCounter, categoryName);
            mCategoriesCounter++;
            return mCategoriesCounter - 1;
        }

        public string GetCategoryNameById(int idCategory)
        {
            string catName = "";
            if (mCategories.TryGetValue(idCategory, out catName)) return catName;
            return null;
        }

        /// <summary>
        /// Вспомогательный метод для создания значения параметра с попутной регистрацией определения параметра, категории при их отсутствии
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <param name="paramCategory"></param>
        /// <param name="paramType"></param>
        /// <returns></returns>
        public VinnyLibDataStructureParameterValue CreateParameterValueWithDefs(string paramName,  object paramValue, string paramCategory = CategoryDefaultName, VinnyLibDataStructureParameterDefinitionType paramType = VinnyLibDataStructureParameterDefinitionType.ParamString, string paramCaption = "")
        {
            int paramDefId = CreateParameterDefinition(paramName, paramType);
            if (paramCaption != "")
            {
                VinnyLibDataStructureParameterDefinition paramDef = GetParamDefById(paramDefId);
                paramDef.Caption = paramCaption;
                this.SetParameterDef(paramDefId, paramDef);
            }
            int categoryId = CreateCategory(paramCategory);

            VinnyLibDataStructureParameterValue paramValueDef = new VinnyLibDataStructureParameterValue(paramDefId, categoryId);
            paramValueDef.SetValue(paramValue, paramType);
            return paramValueDef;
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

        [XmlIgnore]
        public Dictionary<int, VinnyLibDataStructureParameterDefinition> mParameters { get; set; }

        public List<VinnyLibDataStructureParameterDefinition> Parameters { get; set; }

        private int mParamDefCounter = 0;

        [XmlIgnore]
        public Dictionary<int, string> mCategories { get; set; }
        public List<CategoryInfo> Categories { get; set; }

        public int mCategoriesCounter = 0;

    }

    public class CategoryInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }




    
}
