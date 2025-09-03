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
        static void Main(string[] args)
        {
            //TODO: реализовать параметры командной строки с комментариями для ввода значений (и инициализация ImportExportParameters из консоли)
#if DEBUG
            string executingAssemblyFile = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;
            string executionDirectoryPath = Path.GetDirectoryName(executingAssemblyFile);

            VinnyTests tests = new VinnyTests(executionDirectoryPath, @"E:\DataTest\VinnyLibConverterSamples");
            tests.cde_nwc_3();
            //tests.cde_smdx_1();
            //tests.cde_smdx_2();
            //tests.cde_dotbim_2();
            //tests.cde_smdx_3();

            VinnyLibDataStructureGeometryPlacementInfo gi = new VinnyLibDataStructureGeometryPlacementInfo(-1, -1);
            gi.Position = new float[3] { 100, 50, 0 };
            gi.InitMatrix();

            var p = gi.TransformationMatrixInfo.TransformPoint3d(new float[] { 0, 0, 0 });

#endif
            Console.WriteLine("\nEnd!");

        }

    }
#if DEBUG
    class VinnyTests
    {
        public VinnyTests(string ExecDir, string samplesPath)
        {
            this.pExecDir = ExecDir;
            this.pSamplesDirPath = samplesPath;
            mConverter = VinnyLibConverter.CreateInstance(ExecDir);
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
            string path1 = "E:\\Temp\\Vinny\\gatehouse.vlcxmlzip";
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

        private VinnyLibConverter mConverter;
        private string pExecDir;
        private string pSamplesDirPath;
    }
#endif
}
