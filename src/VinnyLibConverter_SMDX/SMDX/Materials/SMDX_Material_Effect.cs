using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VinnyLibConverter_SMDX.SMDX.Materials
{
    internal class SMDX_Material_Effect : SMDX_MaterialBase
    {
        public Dictionary<string, object> Data {  get; set; }


        public void Save(string path)
        {
            File.WriteAllText(path, System.Text.Json.JsonSerializer.Serialize(this));
        }

        public static SMDX_Material_Effect LoadFrom(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException();

            SMDX_Material_Effect smdx_data = new SMDX_Material_Effect();
            string file_data = File.ReadAllText(path);
            var effectMaterialInfo = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(file_data);
            smdx_data.Data = effectMaterialInfo;
            return smdx_data;
        }
    }
}
