using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace VinnyLibConverterCommon.VinnyLibDataStructure
{
    /// <summary>
    /// Описание значение параметра
    /// </summary>
    public class VinnyLibDataStructureParameterValue
    {
        public VinnyLibDataStructureParameterValue() { }

        public VinnyLibDataStructureParameterValue(int paramDefId, int paramCategoryId)
        {
            this.ParamDefId = paramDefId;
            this.ParamCategoryId = paramCategoryId;
        }

        public int ParamDefId { get; set; }
        public int ParamCategoryId { get; set; }

        public void SetValue(int value)
        {
            mValue = value;
        }
        public void SetValue(double value)
        {
            mValue = value;
        }
        public void SetValue(float value)
        {
            mValue = value;
        }
        public void SetValue(string value)
        {
            mValue = value;
        }
        public void SetValue(bool value)
        {
            mValue = value;
        }

        public void SetValue(DateTime value)
        {
            mValue = value;
        }

        public void SetValue(object value, VinnyLibDataStructureParameterDefinitionType propType)
        {
            if (value == null) return;
            string valueStr = value.ToString();

            switch(propType)
            {
                case VinnyLibDataStructureParameterDefinitionType.ParamBool:
                    {
                        bool result;
                        if (bool.TryParse(valueStr, out result)) this.mValue = result;
                        break;
                    }
                    
                case VinnyLibDataStructureParameterDefinitionType.ParamInteger:
                    {
                        int result;
                        if (int.TryParse(valueStr, out result)) this.mValue = result;
                        break;
                    }
                case VinnyLibDataStructureParameterDefinitionType.ParamReal:
                    {
                        double result;
                        if (double.TryParse(valueStr, out result)) this.mValue = result;
                        break;
                    }
                case VinnyLibDataStructureParameterDefinitionType.ParamDate:
                    {
                        DateTime result;
                        if (DateTime.TryParse(valueStr, out result)) this.mValue = result;
                        break;
                    }
                default:
                    {
                        this.mValue = valueStr;
                        break;
                    }
            }

        }

        public bool GetBooleanValue(out bool value)
        {
            value = false;
            if (isValueNull) return false;

            return bool.TryParse(mValue.ToString(), out value);
        }

        public bool GetIntegerValue(out int value)
        {
            value = -1;
            if (isValueNull) return false;

            return int.TryParse(mValue.ToString(), out value);
        }

        public bool GetDoubleValue(out double value)
        {
            value = double.NaN;
            if (isValueNull) return false;

            return double.TryParse(mValue.ToString(), out value);
        }

        public bool GetFloatValue(out double value)
        {
            value = double.NaN;
            if (isValueNull) return false;

            return double.TryParse(mValue.ToString(), out value);
        }

        public bool GetStringValue(out string value)
        {
            value = "";
            if (isValueNull) return false;
            value = mValue.ToString();
            return true;
        }

        public bool GetFloatValue(out bool value)
        {
            value = false;
            if (isValueNull) return false;

            return bool.TryParse(mValue.ToString(), out value);
        }

        public bool GetDatetimeValue(out DateTime value)
        {
            value = DateTime.MinValue;
            if (isValueNull) return false;

            return DateTime.TryParse(mValue.ToString(), out value);
        }

        public override string ToString()
        {
            string value = "";
            if (!isValueNull) value = mValue.ToString();
            return value;
        }

        private bool isValueNull => mValue == null;
        public object mValue { get; set; }
    }
}
