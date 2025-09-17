using System;
using VinnyLibConverterKernel;
using VinnyLibConverterCommon;
using System.IO;
using System.Reflection;

using VinnyLibConverterCommon.Transformation;
using VinnyLibConverterCommon.VinnyLibDataStructure;
using VinnyLibConverterCommon.Interfaces;

namespace VinnyLibConverterCLI
{
    internal class Program
    {
        private static void CLI(string[] args)
        {
            if (args.Length > 0)
            {
                string argFirst = args[0];
                if (argFirst == "-showForImport" || argFirst == "-showForExport")
                {
                    bool isImport = argFirst == "showForImport";
                    string uiLib = Path.Combine(pExecutionDirectoryPath, "ui", "net6.0-windows", "VinnyLibConverterUI.dll");
                    Assembly.LoadFrom(uiLib);

                    ShowSettingsWindow(isImport);
                    return;
                }
                else if (argFirst == "-convert")
                {
                    if (args.Length == 3)
                    {
                        string inputParamsPath = args[1];
                        string outputParamsPath = args[2];

                        if (!File.Exists(inputParamsPath))
                        {
                            Console.WriteLine("Файл с конфигурацией для импорта не существует " + inputParamsPath);
                            return;
                        }
                        if (!File.Exists(outputParamsPath))
                        {
                            Console.WriteLine("Файл с конфигурацией для экспорта не существует " + outputParamsPath);
                            return;
                        }

                        ImportExportParameters inputParams = ImportExportParameters.LoadFromFile(inputParamsPath);
                        ImportExportParameters outputParams = ImportExportParameters.LoadFromFile(outputParamsPath);


                        VinnyLibConverter Converter = VinnyLibConverter.CreateInstance(pExecutionDirectoryPath);
                        Converter.Convert(inputParams, outputParams);
                        return;

                    }
                }
            }
        }
        [STAThread]
        static void Main(string[] args)
        {
            string executingAssemblyFile = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;
            pExecutionDirectoryPath = Path.GetDirectoryName(executingAssemblyFile);

            Assembly.LoadFrom(Path.Combine(pExecutionDirectoryPath, "VinnyLibConverterKernel.dll"));
#if DEBUG
            //args = new string[] {"-showForImport"};
#endif
#if RELEASE
            CLI(args);
#endif



#if DEBUG

            VinnyTests tests = new VinnyTests(@"E:\DataTest\VinnyLibConverterSamples");
            //tests.ifc2nwc_1();
            //tests.dotbim2ifc_1();
            tests.cde_smdx_1();
            //tests.cde_smdx_2();
            //tests.cde_dotbim_2();
            //tests.cde_smdx_3();



#endif
            Console.WriteLine("\nEnd!");
            Console.Read();

        }

        private static void ShowSettingsWindow(bool isImport)
        {
            VinnyLibConverterUI.VLC_UI_MainWindow settingsWindow = new VinnyLibConverterUI.VLC_UI_MainWindow(isImport);
            if (settingsWindow.ShowDialog() == true)
            {
                return;
            }
        }

        internal static string pExecutionDirectoryPath;

    }
#if DEBUG
    class VinnyTests
    {
        public VinnyTests(string samplesPath)
        {
            this.pExecDir = Program.pExecutionDirectoryPath;
            this.pSamplesDirPath = samplesPath;
            mConverter = VinnyLibConverter.CreateInstance2();
        }

        #region DOTBIM
        public void cde_dotbim_1()
        {
            string path1 = Path.Combine(pSamplesDirPath, "dotbim", "Pyramid.bim");
            string path2 = Path.Combine(pSamplesDirPath, "dotbim", "Pyramid_Export.vlcxmlzip");

            var dotbimData = mConverter.ImportModel(new ImportExportParameters() { Path = path1, ModelType = CdeVariant.DotBIM});
            mConverter.ExportModel(dotbimData, new ImportExportParameters() { Path = path2, ModelType = CdeVariant.VinnyLibConverterCacheCompressed });
        }

        public void cde_dotbim_2()
        {
            string path1 = Path.Combine(pSamplesDirPath, "dotbim", "BeamBridgeExample.bim");
            string path2 = Path.Combine(pSamplesDirPath, "dotbim", "BeamBridgeExample_Export.nwc");

            var data = mConverter.ImportModel(new ImportExportParameters() { Path = path1, ModelType = CdeVariant.DotBIM});

            var writeParams = new ImportExportParameters() {Path = path2 };
            TransformationMatrix4x4 matrix = TransformationMatrix4x4.CreateEmptyTransformationMatrix();
            matrix.SetPosition(500, 200, 0);
            writeParams.TransformationInfo.Add(matrix);
            writeParams.ModelType = CdeVariant.NWC;

            mConverter.ExportModel(data, writeParams);
        }


        public void cde_nwc_3()
        {
            string path1 = "E:\\Temp\\Vinny\\gatehouse.vlcxml";
            string path2 = "E:\\Temp\\Vinny\\gatehouse.nwc";

            var data = mConverter.ImportModel(new ImportExportParameters() { Path = path1, ModelType = CdeVariant.VinnyLibConverterCacheCompressed});

            var writeParams = new ImportExportParameters() { Path = path2, ModelType= CdeVariant.NWC };

            mConverter.ExportModel(data, writeParams);
        }

        public void cde_nwc_4()
        {
            string path1 = Path.Combine(pSamplesDirPath, "dotbim", "Pyramid.bim");
            string path2 = "E:\\Temp\\Vinny\\Pyramid.vlcxml";

            var data = mConverter.ImportModel(new ImportExportParameters() { Path = path1, ModelType = CdeVariant.DotBIM });

            var writeParams = new ImportExportParameters() { Path = path2, ModelType = CdeVariant.VinnyLibConverterCache };

            mConverter.ExportModel(data, writeParams);

            string path3 = "E:\\Temp\\Vinny\\Pyramid.nwc";
            mConverter.Convert(writeParams, new ImportExportParameters() { Path = path3, ModelType = CdeVariant.NWC });
        }

        #endregion

        #region SMDX
        public void cde_smdx_1()
        {
            string path1 = Path.Combine(pSamplesDirPath, "smdx", "MAF1.smdx");
            string path2 = Path.Combine(pSamplesDirPath, "smdx", "MAF1_Export.smdx");

            var data = mConverter.ImportModel(new ImportExportParameters() { Path = path1, ModelType = CdeVariant.SMDX});
            mConverter.ExportModel(data, new ImportExportParameters() { Path = path2, ModelType = CdeVariant.SMDX });
        }

        public void cde_smdx_2()
        {
            string path1 = Path.Combine(pSamplesDirPath, "smdx", "5ZD.smdx");
            string path2 = Path.Combine(pSamplesDirPath, "smdx", "5ZD_Export.smdx");

            var data = mConverter.ImportModel(new ImportExportParameters() { Path = path1, ModelType = CdeVariant.SMDX });
            mConverter.ExportModel(data, new ImportExportParameters() { Path = path2, ModelType = CdeVariant.SMDX });
        }

        public void cde_smdx_3()
        {
            string path1 = Path.Combine(pSamplesDirPath, "dotbim", "House.bim");
            string path2 = Path.Combine(pSamplesDirPath, "smdx", "House_Export.smdx");

            var data = mConverter.ImportModel(new ImportExportParameters() { Path = path1, ModelType = CdeVariant.DotBIM });
            mConverter.ExportModel(data, new ImportExportParameters() { Path = path2, ModelType = CdeVariant.SMDX });
        }

       
        #endregion

        public void ifc2nwc_1()
        {
            string path1 = Path.Combine(pSamplesDirPath, "ifc", "Эстакада.ifc");
            string path2 = "E:\\Temp\\Vinny\\Эстакада.nwc";

            mConverter.Convert(
                new ImportExportParameters() { Path = path1, ModelType = CdeVariant.IFC },
                new ImportExportParameters() { Path = path2, ModelType = CdeVariant.NWC });
        }

        public void dotbim2ifc_1()
        {
            string path1 = Path.Combine(pSamplesDirPath, "dotbim", "Pyramid.bim");
            string path2 = "E:\\Temp\\Vinny\\Pyramid.ifc";

            mConverter.Convert(
                new ImportExportParameters() { Path = path1, ModelType = CdeVariant.DotBIM },
                new ImportExportParameters() { Path = path2, ModelType = CdeVariant.IFC });
        }

        private VinnyLibConverter mConverter;
        private string pExecDir;
        private string pSamplesDirPath;
    }
#endif
}
