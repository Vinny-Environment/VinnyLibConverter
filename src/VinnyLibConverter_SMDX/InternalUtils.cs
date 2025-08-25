using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Xml;

namespace VinnyLibConverter_SMDX
{

    internal class InternalUtils
    {
        public static JsonSerializerOptions GetWriteOpts()
        {
            return new System.Text.Json.JsonSerializerOptions() {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull };
        }
    }
}
