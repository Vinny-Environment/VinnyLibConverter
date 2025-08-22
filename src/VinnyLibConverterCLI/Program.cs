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
#if DEBUG
            string executingAssemblyFile = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;
            string execution_directory_path = Path.GetDirectoryName(executingAssemblyFile);

            VinnyTests tests = new VinnyTests(execution_directory_path);
            tests.cde_dotbim_1();
#endif
            Console.WriteLine("\nEnd!");

        }

    }
#if DEBUG
    class VinnyTests
    {
        public VinnyTests(string ExecDir)
        {
            this.pExecDir = ExecDir;
            mConverter = VinnyLibConverter.CreateInstance(ExecDir);
        }

        #region DOTBIM
        public void cde_dotbim_1()
        {
            string path1 = @"C:\Users\Georg\Documents\GitHub\dotbim\test\ExampleFiles\TestFilesFromC#\Pyramid.bim";
            string path2 = @"C:\Users\Georg\Documents\GitHub\dotbim\test\ExampleFiles\TestFilesFromC#\Pyramid_Export.bim";

            var fotbimData = mConverter.ImportModel(CdeVariant.DotBIM, ImportExportParameters.CreateForLocalCDE(path1));
            mConverter.ExportModel(CdeVariant.DotBIM, fotbimData, ImportExportParameters.CreateForLocalCDE(path2));
        }
        #endregion

        private VinnyLibConverter mConverter;
        private string pExecDir;
    }
#endif
}
