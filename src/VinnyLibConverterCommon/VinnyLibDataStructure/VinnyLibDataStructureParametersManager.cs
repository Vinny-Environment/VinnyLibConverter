using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        /// <summary>
        /// Вспомогательный метод для создания значения параметра с попутной регистрацией определения параметра, категории при их отсутствии
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <param name="paramCategory"></param>
        /// <param name="paramType"></param>
        /// <returns></returns>
        public VinnyLibDataStructureParameterValue CreateParameterValueWithDefs(string paramName, object paramValue, string paramCategory = CategoryDefaultName, VinnyLibDataStructureParameterDefinitionType paramType = VinnyLibDataStructureParameterDefinitionType.ParamString)
        {
            int paramDefId = CreateParameterDefinition(paramName, paramType);
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
        public Dictionary<int, VinnyLibDataStructureParameterDefinition> Parameters { get; set; }

        // Helper property for XML serialization
        [XmlArray("Parameters")]
        [XmlArrayItem("Parameters")]
        public List<KeyValuePair_Parameter> ParametersList
        {
            get => Parameters.Select(kv => new KeyValuePair_Parameter { Key = kv.Key, Value = kv.Value }).ToList();
            set => Parameters = value.ToDictionary(item => item.Key, item => item.Value);
        }


        private int mParamDefCounter = 0;

        [XmlIgnore]
        public Dictionary<int, string> Categories { get; set; }

        // Helper property for XML serialization
        [XmlArray("Categories")]
        [XmlArrayItem("Categories")]
        public List<KeyValuePair_Category> CategoriesList
        {
            get => Categories.Select(kv => new KeyValuePair_Category { Key = kv.Key, Value = kv.Value }).ToList();
            set => Categories = value.ToDictionary(item => item.Key, item => item.Value);
        }

        public int mCategoriesCounter = 0;

    }


    // Helper class for Objects serrialization
    public class KeyValuePair_Parameter
    {
        public int Key { get; set; }

        public VinnyLibDataStructureParameterDefinition Value { get; set; }
    }

    // Helper class for Categories serrialization
    public class KeyValuePair_Category
    {
        public int Key { get; set; }

        public string Value { get; set; }
    }
}
