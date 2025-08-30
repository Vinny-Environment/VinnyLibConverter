using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VinnyLibConverterCommon;
using VinnyLibConverterCommon.VinnyLibDataStructure;
using VinnyLibConverterCommon.Transformation;

using VinnyLibConverterUtils;
using static VinnyLibConverterCommon.VinnyLibDataStructure.VinnyLibDataStructureObjectsManager;

namespace VinnyLibConverter_nwcreate
{
    public class nwcreate_FormatProcessing : ICdeFormatProcessing
    {
        public CdeVariant GetCdeType()
        {
            return CdeVariant.NWC;
        }

        public bool IsReadable()
        {
            return false;
        }

        public bool IsWriteable()
        {
            return true;
        }

        public VinnyLibDataStructureModel Import(ImportExportParameters openParameters)
        {
            throw new NotImplementedException();
        }

        public void Export(VinnyLibDataStructureModel vinnyData, ImportExportParameters outputParameters)
        {
            vinnyData.SetCoordinatesTransformation(outputParameters.TransformationInfo);

            LcNwcSceneWrapper nwcScene = new LcNwcSceneWrapper();
            //заголовок
            //LcNwcGroupWrapper nwcHeader = new LcNwcGroupWrapper();
            //nwcHeader.SetLayer(true);
            //nwcHeader.SetName("_metadata");
            LcNwcPropertyAttributeWrapper nwcProperty = new LcNwcPropertyAttributeWrapper();
            foreach (var vinnyHeaderData in vinnyData.Header.Data)
            {
                LcNwcDataWrapper nwcData = new LcNwcDataWrapper();
                nwcData.SetWideString(vinnyHeaderData.Item2);
                
                nwcProperty.AddProperty(vinnyHeaderData.Item1, vinnyHeaderData.Item1, nwcData);
            }

            //структура модели
            LcNwcGroupWrapper nwcStructure = new LcNwcGroupWrapper();
            nwcStructure.SetLayer(true);
            nwcStructure.SetName("ModelData");
            nwcStructure.AddAttribute(nwcProperty);
            VinnyLibDataStructureObjectsManager.StructureInfo[] vinnyModelStructureInfo = vinnyData.ObjectsManager.GetAllStructure();
            foreach (VinnyLibDataStructureObjectsManager.StructureInfo vinnyModelStructureGroupInfo in vinnyModelStructureInfo)
            {
                ProcessObject(vinnyModelStructureGroupInfo, ref nwcStructure);
            }
            nwcScene.AddNode(nwcStructure);

            void ProcessObject(StructureInfo obj, ref LcNwcGroupWrapper parentObject)
            {
                VinnyLibDataStructureObject vinnyObj = vinnyData.ObjectsManager.GetObjectById(obj.Id);
                if (vinnyObj == null) return;
                LcNwcGroupWrapper nwcObjectDef = new LcNwcGroupWrapper();
                nwcObjectDef.SetLayer(true);
                nwcObjectDef.SetName(vinnyObj.Name);
                if (vinnyObj.Name == "") nwcObjectDef.SetName("Object");


                //geometry
                LcNwcGroupWrapper nwcObjectGeometryCollectionDef = new LcNwcGroupWrapper();
                nwcObjectGeometryCollectionDef.SetLayer(false);
                nwcObjectGeometryCollectionDef.SetName("GeometryCollection");

                if (vinnyObj.GeometryPlacementInfoIds.Any())
                {
                    foreach (int vinnyGeometryPlacementInfoId in vinnyObj.GeometryPlacementInfoIds)
                    {
                        VinnyLibDataStructureGeometryPlacementInfo vinnyGeometryPlacementInfo = vinnyData.GeometrtyManager.GetGeometryPlacementInfoById(vinnyGeometryPlacementInfoId);
                        VinnyLibDataStructureGeometry vinnyGeometryInfo = vinnyData.GeometrtyManager.GetGeometryById(vinnyGeometryPlacementInfoId);
                        if (vinnyGeometryInfo == null) continue;

                        LcNwcGeometryWrapper nwcObjectGeometryDef = new LcNwcGeometryWrapper();
                        //assign material
                        VinnyLibDataStructureMaterial objectDefMaterial = vinnyData.MaterialsManager.GetMaterialById(vinnyGeometryInfo.MaterialId);
                        LcNwcMaterialWrapper nwcMaterial = new LcNwcMaterialWrapper();
                        nwcMaterial.SetDiffuseColor(objectDefMaterial.ColorR / 255.0, objectDefMaterial.ColorG / 255.0, objectDefMaterial.ColorB / 255.0);
                        nwcMaterial.SetAmbientColor(objectDefMaterial.ColorR / 255.0, objectDefMaterial.ColorG / 255.0, objectDefMaterial.ColorB / 255.0);
                        nwcMaterial.SetName(objectDefMaterial.Name);
                        nwcMaterial.SetTransparency(objectDefMaterial.ColorAlpha);
                        nwcObjectGeometryDef.AddAttribute(nwcMaterial);

                        //assign geometry
                        if (vinnyGeometryInfo.GetGeometryType() == VinnyLibDataStructureGeometryType.Mesh)
                        {
                            VinnyLibDataStructureGeometryMesh vinnyGeometryMeshInfo = VinnyLibDataStructureGeometryMesh.asType(vinnyGeometryInfo);
                            LcNwcGeometryStreamWrapper nwcGeomStreamDef = nwcObjectGeometryDef.OpenStream();
                            nwcGeomStreamDef.Begin(0);
                            //Сопоставление индексов точек в mesh с результатом nwcGeomStreamDef.IndexedVertex
                            Dictionary<int, int> vinnyPoint2nwcVertex = new Dictionary<int, int>();
                            foreach (var facesInfo in vinnyGeometryMeshInfo.Faces)
                            {
                                

                                var faceVertex1 = vinnyGeometryPlacementInfo.TransformationMatrixInfo.TransformPoint3d(
                                    vinnyGeometryMeshInfo.GetPointCoords(facesInfo.Value[0]));
                                var faceVertex2 = vinnyGeometryPlacementInfo.TransformationMatrixInfo.TransformPoint3d(
                                    vinnyGeometryMeshInfo.GetPointCoords(facesInfo.Value[1]));
                                var faceVertex3 = vinnyGeometryPlacementInfo.TransformationMatrixInfo.TransformPoint3d(
                                    vinnyGeometryMeshInfo.GetPointCoords(facesInfo.Value[2]));

                                //faceVertex1 = vinnyGeometryMeshInfo.GetPointCoords(facesInfo.Value[0]);
                                //faceVertex2 = vinnyGeometryMeshInfo.GetPointCoords(facesInfo.Value[1]);
                                //faceVertex3 = vinnyGeometryMeshInfo.GetPointCoords(facesInfo.Value[2]);

                                int p1 = VertexProcess(0);
                                int p2 = VertexProcess(1);
                                int p3 = VertexProcess(2);

                                nwcGeomStreamDef.TriangleIndex(p1);
                                nwcGeomStreamDef.TriangleIndex(p2);
                                nwcGeomStreamDef.TriangleIndex(p3);

                                //nwcGeomStreamDef.TriangleVertex(faceVertex1[0], faceVertex1[1], faceVertex1[2]);
                                //nwcGeomStreamDef.TriangleVertex(faceVertex2[0], faceVertex2[1], faceVertex2[2]);
                                //nwcGeomStreamDef.TriangleVertex(faceVertex3[0], faceVertex3[1], faceVertex3[2]);

                                int VertexProcess(int pos)
                                {
                                    int pointIndex = facesInfo.Value[pos];
                                    if (vinnyPoint2nwcVertex.ContainsKey(pointIndex)) return vinnyPoint2nwcVertex[pointIndex];

                                    var faceVertexInfo = vinnyGeometryPlacementInfo.TransformationMatrixInfo.TransformPoint3d(
                                    vinnyGeometryMeshInfo.GetPointCoords(pointIndex));
                                    int nwcVertexIndex = nwcGeomStreamDef.IndexedVertex(faceVertexInfo[0], faceVertexInfo[1], faceVertexInfo[2]);
                                    vinnyPoint2nwcVertex[pointIndex] = nwcVertexIndex;

                                    return nwcVertexIndex;
                                }

                            }
                            nwcGeomStreamDef.End();
                            nwcObjectGeometryDef.CloseStream(nwcGeomStreamDef);
                            nwcObjectGeometryCollectionDef.AddNode(nwcObjectGeometryDef);
                        }
                    }
                }
                nwcObjectDef.AddNode(nwcObjectGeometryCollectionDef);

                //properties
                foreach (var cat2props in vinnyData.ParametersManager.SortParamsByCategories(vinnyObj.Parameters))
                {
                    LcNwcPropertyAttributeWrapper nwcObjectCategoryProps = new LcNwcPropertyAttributeWrapper();
                    nwcObjectCategoryProps.SetName(cat2props.Key);
                    nwcObjectCategoryProps.SetClassName(cat2props.Key, "Category " + cat2props.Key);

                    foreach (VinnyLibDataStructureParameterValue prop in cat2props.Value)
                    {
                        VinnyLibDataStructureParameterDefinition propDef = vinnyData.ParametersManager.GetParamDefById(prop.ParamDefId);
                        if (propDef == null) continue;


                        LcNwcDataWrapper propValue = new LcNwcDataWrapper();
                        propValue.SetWideString("");


                        switch (propDef.ParamType)
                        {
                            case VinnyLibDataStructureParameterDefinitionType.ParamInteger:
                                {
                                    int val;
                                    if (prop.GetIntegerValue(out val)) propValue.SetInt32(val);
                                    break;
                                }
                            case VinnyLibDataStructureParameterDefinitionType.ParamBool:
                                {
                                    bool val;
                                    if (prop.GetBooleanValue(out val)) propValue.SetBoolean(val);
                                    break;
                                }
                            case VinnyLibDataStructureParameterDefinitionType.ParamReal:
                                {
                                    double val;
                                    if (prop.GetDoubleValue(out val)) propValue.SetFloat(val);
                                    break;
                                }
                            case VinnyLibDataStructureParameterDefinitionType.ParamDate:
                                {
                                    DateTime val;
                                    if (prop.GetDatetimeValue(out val)) propValue.SetTime(val.ToBinary()); //?
                                    break;
                                }
                            case VinnyLibDataStructureParameterDefinitionType.ParamString:
                                {
                                    string val;
                                    if (prop.GetStringValue(out val)) propValue.SetWideString(val);
                                    break;
                                }
                            default:

                                propValue.SetWideString(prop.ToString());                                
                                break;

                        }

                        nwcObjectCategoryProps.AddProperty(propDef.Caption, propDef.Name, propValue);
                    }
                    nwcObjectDef.AddAttribute(nwcObjectCategoryProps);
                }
                

                foreach (var subObject in obj.Childs)
                {
                    ProcessObject(subObject, ref nwcObjectDef);
                }
                parentObject.AddNode(nwcObjectDef);
            }

            nwcScene.WriteCache("", outputParameters.Path);
        }
    }
}
