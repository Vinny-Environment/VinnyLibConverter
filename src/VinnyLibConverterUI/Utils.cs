using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VinnyLibConverterUI
{
    internal class Utils
    {
        public static void LoadVinnyLibConverterCommon()
        {
            string executingAssemblyFile = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;
            string executionDirectoryPath = System.IO.Path.GetDirectoryName(executingAssemblyFile);

            //That dll is located by-default at "\ui\net6.0-windows\VinnyLibConverterUI.dll"
            //VinnyLibConverterCommon.dll isplace in root =>
            string VinnyLibConverterCommonPath = Path.Combine(new DirectoryInfo(executionDirectoryPath).Parent.Parent.FullName, "VinnyLibConverterCommon.dll");
            Assembly.LoadFrom(VinnyLibConverterCommonPath);
        }
    }
}
