using System;
using VinnyLibConverterKernel;
using VinnyLibConverterCommon;
using System.IO;
using System.Reflection;

using VinnyLibConverterCommon.Transformation;
using VinnyLibConverterCommon.VinnyLibDataStructure;

namespace VinnyLibConverterCLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //TODO: реализовать параметры командной строки с комментариями для ввода значений (и инициализация ImportExportParameters из консоли)
#if DEBUG
            string executingAssemblyFile = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;
            string execution_directory_path = Path.GetDirectoryName(executingAssemblyFile);

            VinnyTests tests = new VinnyTests(execution_directory_path, @"E:\DataTest\VinnyLibConverterSamples");
            //tests.cde_dotbim_1();
            //tests.cde_smdx_1();
            //tests.cde_smdx_2();
            tests.cde_dotbim_2();
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
            string path2 = Path.Combine(pSamplesDirPath, "dotbim", "Pyramid_Export.bim");

            var fotbimData = mConverter.ImportModel(CdeVariant.DotBIM, ImportExportParameters.CreateForLocalCDE(path1));
            mConverter.ExportModel(CdeVariant.DotBIM, fotbimData, ImportExportParameters.CreateForLocalCDE(path2));
        }

        public void cde_dotbim_2()
        {
            string path1 = Path.Combine(pSamplesDirPath, "dotbim", "BeamBridgeExample.bim");
            string path2 = Path.Combine(pSamplesDirPath, "dotbim", "BeamBridgeExample_Export.nwc");

            var data = mConverter.ImportModel(CdeVariant.DotBIM, ImportExportParameters.CreateForLocalCDE(path1));

            var writeParams = ImportExportParameters.CreateForLocalCDE(path2);
            TransformationMatrix4x4 matrix = TransformationMatrix4x4.CreateEmptyTransformationMatrix();
            matrix.SetPosition(500, 200, 0);
            writeParams.TransformationInfo.Add(matrix);
            mConverter.ExportModel(CdeVariant.NWC, data, writeParams);
        }
        #endregion

        #region SMDX
        public void cde_smdx_1()
        {
            string path1 = Path.Combine(pSamplesDirPath, "smdx", "MAF1.smdx");
            string path2 = Path.Combine(pSamplesDirPath, "smdx", "MAF1_Export.smdx");

            var data = mConverter.ImportModel(CdeVariant.SMDX, ImportExportParameters.CreateForLocalCDE(path1));
            mConverter.ExportModel(CdeVariant.SMDX, data, ImportExportParameters.CreateForLocalCDE(path2));
        }

        public void cde_smdx_2()
        {
            string path1 = Path.Combine(pSamplesDirPath, "smdx", "5ZD.smdx");
            string path2 = Path.Combine(pSamplesDirPath, "smdx", "5ZD_Export.smdx");

            var data = mConverter.ImportModel(CdeVariant.SMDX, ImportExportParameters.CreateForLocalCDE(path1));
            mConverter.ExportModel(CdeVariant.SMDX, data, ImportExportParameters.CreateForLocalCDE(path2));
        }

        public void cde_smdx_3()
        {
            string path1 = Path.Combine(pSamplesDirPath, "dotbim", "House.bim");
            string path2 = Path.Combine(pSamplesDirPath, "smdx", "House_Export.smdx");

            var data = mConverter.ImportModel(CdeVariant.DotBIM, ImportExportParameters.CreateForLocalCDE(path1));
            mConverter.ExportModel(CdeVariant.SMDX, data, ImportExportParameters.CreateForLocalCDE(path2));
        }

       
        #endregion

        private VinnyLibConverter mConverter;
        private string pExecDir;
        private string pSamplesDirPath;
    }
#endif
}
