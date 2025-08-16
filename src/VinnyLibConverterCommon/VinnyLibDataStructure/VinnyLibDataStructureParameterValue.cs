using System;
using System.Collections.Generic;
using System.Text;

namespace VinnyLibConverterCommon.VinnyLibDataStructure
{
    /// <summary>
    /// Описание значение параметра
    /// </summary>
    public class VinnyLibDataStructureParameterValue
    {
        public VinnyLibDataStructureParameterValue() { }

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

        public bool GetFloatValue(out float value)
        {
            value = float.NaN;
            if (isValueNull) return false;

            return float.TryParse(mValue.ToString(), out value);
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
        private object mValue;
    }
}
