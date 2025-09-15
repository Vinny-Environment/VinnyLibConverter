using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace VinnyLibConverterCommon.Transformation
{
    /// <summary>
    /// Wrapper around VinnyProjCoordinateSystemVariant
    /// </summary>
    public enum CsDefVariant
    {
        NameOrCode = 0,
        WKT = 1
    }

    [Serializable]
    public class TransformationGeodetic : ICoordinatesTransformation
    {
        public override CoordinatesTransformationVariant GetTransformationType()
        {
            return CoordinatesTransformationVariant.Geodetic;
        }
        public TransformationGeodetic()
        {
            mProj = new VinnyProj();

            string executingAssemblyFile = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;
            string ExecutionDirectoryPath = Path.GetDirectoryName(executingAssemblyFile);

            SetProjDbPath(Path.Combine(ExecutionDirectoryPath, "share"));
        }

        public void SetProjDbPath(string dbDirPath)
        {
            mProj.SetProjDbPath(dbDirPath);
        }

        public bool InitData(out string csSourceErrors, out string csDestErrors)
        {
            csSourceErrors = "";
            csDestErrors = "";

            bool csSourceStat = false;
            if ((SourceCsType == CsDefVariant.NameOrCode)) 
            {
                if (mProj.ValidateCs(SourceCs)) csSourceStat = true;
                else csSourceErrors = "Некорректное название или код СК";
            }

            else if (SourceCsType == CsDefVariant.WKT)
            {
                csSourceErrors = mProj.ValidateWktCode(SourceCs);
                if (csSourceErrors == "") csSourceStat = true;
            }

            bool csDestStat = false;
            if ((DestCsType == CsDefVariant.NameOrCode))
            {
                if (mProj.ValidateCs(DestCs)) csDestStat = true;
                else csSourceErrors = "Некорректное название или код СК";
            }

            else if (DestCsType == CsDefVariant.WKT)
            {
                csSourceErrors = mProj.ValidateWktCode(DestCs);
                if (csSourceErrors == "") csDestStat = true;
            }

            if (csSourceStat && csDestStat) return true;
            return false;
        }

        public override double[] TransformPoint3d(double[] xyz)
        {
            Point3dVector p = new Point3dVector();
            p.Add(new VinnyPoint3d(xyz[0], xyz[1], xyz[2]));
            Point3dVector pRecalc = mProj.TransformPoints(SourceCs, (VinnyProjCoordinateSystemVariant)SourceCsType, DestCs, (VinnyProjCoordinateSystemVariant)DestCsType, p);

            if (pRecalc != null) return new double[] { pRecalc[0].X, pRecalc[0].Y, pRecalc[0].Z };
            return null;
        }

        public override double[][] TransformPoints3d(double[][] xyz_array)
        {
            Point3dVector p = new Point3dVector(xyz_array.Length);
            for (int pCounter = 0; pCounter < xyz_array.Length; pCounter++)
            {
                double[] xyz = xyz_array[pCounter];
                p[pCounter] = new VinnyPoint3d(xyz[0], xyz[1], xyz[2]);
            }

            Point3dVector pRecalc = mProj.TransformPoints(SourceCs, (VinnyProjCoordinateSystemVariant)SourceCsType, DestCs, (VinnyProjCoordinateSystemVariant)DestCsType, p);
            if (pRecalc != null)
            {
                double[][] ret = new double[pRecalc.Count][];
                for (int pCounter = 0; pCounter < pRecalc.Count; pCounter++)
                {
                    var xyz = pRecalc[pCounter];
                    ret[pCounter] = new double[] { xyz.X, xyz.Y, xyz.Z };
                }
                return ret;
            }
            return xyz_array;
        }

        public override string ToString()
        {
            string cs1 = SourceCs;
            if (SourceCs.Length > 36) cs1 = cs1.Substring(0, 35);

            string cs2 = DestCs;
            if (DestCs.Length > 36) cs2 = cs2.Substring(0, 35);

            return $"Geodetic {cs1}\t{cs2}";
            //TODO: return a CS's names after PROJ's initialize (create PROJ SWIG...)
        }

        public string SourceCs { get; set; }
        public CsDefVariant SourceCsType { get; set; }
        public string DestCs { get; set; }
        public CsDefVariant DestCsType { get; set; }

        private VinnyProj mProj;
    }
}
