using System;
using System.Collections.Generic;
using System.Text;

namespace VinnyLibConverterCommon.VinnyLibDataStructure
{
    public enum VinnyLibDataStructureParameterDefinitionType : int
    {
        ParamString,
        ParamInteger,
        ParamReal,
        ParamDate,
        ParamBool

    };
    /// <summary>
    /// Описание отдельного параметра
    /// </summary>
    public class VinnyLibDataStructureParameterDefinition
    {
        public VinnyLibDataStructureParameterDefinition(int id, string name, VinnyLibDataStructureParameterDefinitionType paramType = VinnyLibDataStructureParameterDefinitionType.ParamString)
        {
            this.Id = id;
            this.Name = name;
            this.Caption = name;
        }
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Caption { get; set; } = "";
        public bool IsReadOnly { get; set; } = false;
        public VinnyLibDataStructureParameterDefinitionType ParamType { get; private set; }

        public override bool Equals(object obj)
        {
            return Name == ((VinnyLibDataStructureParameterDefinition)obj).Name && ParamType == ((VinnyLibDataStructureParameterDefinition)obj).ParamType;
        }
    }
}
