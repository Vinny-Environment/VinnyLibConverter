using System;
using System.Collections.Generic;
using System.Text;

namespace VinnyLibConverter_SMDX.SMDX.Materials
{

    public class SMDX_MaterialBase
    {
        public const string type_BlinnPhong = "Blinn-Phong";
        public const string type_Effect = "Effect";
        public string type { get; set; }
    }
}
