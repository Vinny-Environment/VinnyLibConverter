using System;
using VinnyLibConverterKernel;
using VinnyLibConverterCommon;
using System.IO;
using System.Reflection;

namespace VinnyLibConverterCLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string executingAssemblyFile = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;
            string execution_directory_path = Path.GetDirectoryName(executingAssemblyFile);

            
            VinnyLibConverter converterSession = VinnyLibConverter.CreateInstance(execution_directory_path);

            var fotbimData = converterSession.ImportModel(CdeVariant.DotBIM, new ImportExportParameters() { Path = @"C:\Users\user\Documents\GitHub\dotbim\test\ExampleFiles\TestFilesFromC#\Pyramid.bim" });
            converterSession.ExportModel(CdeVariant.DotBIM, fotbimData, new ImportExportParameters() { Path = @"C:\Users\user\Documents\GitHub\dotbim\test\ExampleFiles\TestFilesFromC#\Pyramid_Export.bim" });
            

            Console.WriteLine("\nEnd!");
        }
    }
}
