using System;
using System.Collections.Generic;
using System.Text;
using VinnyLibConverterCommon.Interfaces;

namespace VinnyLibConverterCommon
{
    public struct DataExchangeFormatInfo
    {
        public string Caption { get; set; }
        public CdeVariant Type { get; set; }
        public string[] Extensions { get; set; }
        public bool IsReadable { get; set; }
        public bool IsWritable { get; set; }
    }
    public class CommomUtils
    {
        // Строковые сокращения из CdeVariant
        public const string CdeFrmt_SMDX = "smdx";
        public const string CdeFrmt_MLT = "mlt";
        public const string CdeFrmt_IMC = "imc";
        public const string CdeFrmt_DotBim = "dotbim";
        public const string CdeFrmt_NWC = "nwcreate";
        public const string CdeFrmt_FBX = "fbx";
        public const string CdeFrmt_GLTF = "gltf";
        public const string CdeFrmt_IFC = "ifc";
        public const string CdeFrmt_DXF = "dxf";
        public const string CdeFrmt_LandXML = "landxml";

        public static Dictionary<CdeVariant, string> CdeFormatName
        {
            get
            {
                return new Dictionary<CdeVariant, string>
                {
                    {CdeVariant.SMDX, CdeFrmt_SMDX},
                    {CdeVariant.MLT, CdeFrmt_MLT},
                    {CdeVariant.IMC, CdeFrmt_IMC},
                    {CdeVariant.DotBIM, CdeFrmt_DotBim},
                    {CdeVariant.NWC, CdeFrmt_NWC},
                    {CdeVariant.FBX, CdeFrmt_FBX},
                    {CdeVariant.GLTF, CdeFrmt_GLTF},
                    {CdeVariant.IFC, CdeFrmt_IFC},
                    {CdeVariant.DXF, CdeFrmt_DXF},
                    {CdeVariant.LandXML, CdeFrmt_LandXML},

                };
            }
        }

        public static DataExchangeFormatInfo[] GetCurrentFormats()
        {
            return new DataExchangeFormatInfo[]
            {
                new DataExchangeFormatInfo(){
                    Caption = "VinnyLibConverter Cached XML Data",
                    Type = CdeVariant.DotBIM,
                    Extensions= new string[]{".vlcxml" },
                    IsReadable=true,
                    IsWritable=true
                },
                new DataExchangeFormatInfo(){
                    Caption = "DotBIM",
                    Type = CdeVariant.DotBIM,
                    Extensions= new string[]{".bim" },
                    IsReadable=true,
                    IsWritable=true
                },
                new DataExchangeFormatInfo(){
                    Caption = "Topomatic SMDX",
                    Type = CdeVariant.SMDX,
                    Extensions= new string[]{".smdx" },
                    IsReadable=true,
                    IsWritable=true
                },
                new DataExchangeFormatInfo(){
                    Caption = "Navisworks cashe (NWC)",
                    Type = CdeVariant.NWC,
                    Extensions= new string[]{".nwc" },
                    IsReadable=false,
                    IsWritable=true
                }
            };
        }

        public static float DegreeToRadians(float degree)
        {
            return degree / 180.0f * (float)Math.PI;
        }

        public static float RadiansToDegree(float radians)
        {
            return radians / (float)Math.PI * 180.0f;
        }


    }
}
