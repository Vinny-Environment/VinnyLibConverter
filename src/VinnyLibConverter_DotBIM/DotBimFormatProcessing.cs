using System;
using System.IO;
using System.Reflection;

using VinnyLibConverterCommon;
using VinnyLibConverterCommon.VinnyLibDataStructure;

using System.Linq;
using System.Collections.Generic;
using VinnyLibConverterCommon.Transformation;
using VinnyLibConverterCommon.Interfaces;

namespace VinnyLibConverter_DotBIM
{
    public class DotBimFormatProcessing :  ICdeFormatProcessing
    {
        public CdeVariant GetCdeType()
        {
            return CdeVariant.DotBIM;
        }

        public bool IsReadable()
        {
            return true;
        }

        public bool IsWriteable()
        {
            return true;
        }

        public VinnyLibDataStructureModel Import(ImportExportParameters openParameters)
        {
            VinnyLibDataStructureModel dotbimFileDef = new VinnyLibDataStructureModel();
            ImportExportParameters.mActiveConfig = openParameters;
            //ImportExportParameters.mActiveConfig.CheckGeometryDubles = false;//потому что читаем формат

            dotbim.File dotbimFile = dotbim.File.Read(openParameters.Path);
            //header
            foreach (var metadata in dotbimFile.Info)
            {
                dotbimFileDef.Header.Parameters.Add(dotbimFileDef.ParametersManager.CreateParameterValueWithDefs(metadata.Key, metadata.Value));
            }
            //geometry
            Dictionary<int, int> dotbimGeometry2VinnyGeometry = new Dictionary<int, int>();
            foreach (var mesh in dotbimFile.Meshes)
            {
                int geomId = dotbimFileDef.GeometrtyManager.CreateGeometry(VinnyLibDataStructureGeometryType.Mesh);
                dotbimGeometry2VinnyGeometry[mesh.MeshId] = geomId;
                VinnyLibDataStructureGeometry meshGeomRaw = dotbimFileDef.GeometrtyManager.GetMeshGeometryById(geomId);
                VinnyLibDataStructureGeometryMesh meshGeom = VinnyLibDataStructureGeometryMesh.asType(meshGeomRaw);
                if (meshGeom != null)
                {
                    for (int dotbimMeshVertexCounter = 0; dotbimMeshVertexCounter < mesh.Coordinates.Count - 2; dotbimMeshVertexCounter += 3)
                    {
                        float x, y, z;
                        x = (float)mesh.Coordinates[dotbimMeshVertexCounter];
                        y = (float)mesh.Coordinates[dotbimMeshVertexCounter + 1];
                        z = (float)mesh.Coordinates[dotbimMeshVertexCounter + 2];

                        meshGeom.AddVertex(x, y, z);
                    }
                    for(int dotbimMeshFaceIndicesCounter = 0; dotbimMeshFaceIndicesCounter < mesh.Indices.Count - 2; dotbimMeshFaceIndicesCounter+=3)
                    {
                        int VertexId1 = mesh.Indices[dotbimMeshFaceIndicesCounter];
                        int VertexId2 = mesh.Indices[dotbimMeshFaceIndicesCounter + 1];
                        int VertexId3 = mesh.Indices[dotbimMeshFaceIndicesCounter + 2];

                        meshGeom.AddFace(VertexId1, VertexId2, VertexId3);

                        /*
                        float[] Vertex1 = GetVertexCoords(VertexId1);
                        float[] Vertex2 = GetVertexCoords(VertexId2);
                        float[] Vertex3 = GetVertexCoords(VertexId3);

                        meshGeom.AddFace(Vertex1, Vertex2, Vertex3);
                        */
                    }

                    dotbimFileDef.GeometrtyManager.SetMeshGeometry(geomId, meshGeom);
                }
                float[] GetVertexCoords(int VertexId)
                {
                    return new float[] { (float)mesh.Coordinates[VertexId * 3], (float)mesh.Coordinates[VertexId * 3 + 1], (float)mesh.Coordinates[VertexId * 3 + 2] };
                }
            }

            //elements
            int defaulParamsCateroryId = dotbimFileDef.ParametersManager.CreateCategory("ParametersForXML");
            int paramDefTypeId = dotbimFileDef.ParametersManager.CreateParameterDefinition("Type", VinnyLibDataStructureParameterDefinitionType.ParamString);

            foreach (var elem in dotbimFile.Elements)
            {
                VinnyLibDataStructureGeometry meshGeomRaw = dotbimFileDef.GeometrtyManager.GetMeshGeometryById(dotbimGeometry2VinnyGeometry[elem.MeshId]);
                VinnyLibDataStructureGeometryMesh meshGeom = VinnyLibDataStructureGeometryMesh.asType(meshGeomRaw);

                int objectId = dotbimFileDef.ObjectsManager.CreateObject();
                VinnyLibDataStructureObject objectDef = dotbimFileDef.ObjectsManager.GetObjectById(objectId);

                //задаем информацию о цветах граней для геометрии
                if (elem.FaceColors != null)
                {
                    for (int colorCounter = 0; colorCounter < elem.FaceColors.Count - 2; colorCounter += 3)
                    {
                        int[] RGBcolor = new int[]{
                    elem.FaceColors[colorCounter], elem.FaceColors[colorCounter + 1], elem.FaceColors[colorCounter + 2]};
                        int materialId = dotbimFileDef.MaterialsManager.CreateMaterial(RGBcolor);
                        meshGeom.AssignMaterialToFace(colorCounter / 3, materialId);
                    }
                }

                //задаем цвет
                meshGeom.MaterialId = dotbimFileDef.MaterialsManager.CreateMaterial(new int[] { elem.Color.R, elem.Color.G, elem.Color.B, elem.Color.A });

                dotbimFileDef.GeometrtyManager.SetMeshGeometry(dotbimGeometry2VinnyGeometry[elem.MeshId], meshGeom);

                

                objectDef.UniqueId = elem.Guid;

                //задаем свойства
                VinnyLibDataStructureParameterValue paramTypeValue = new VinnyLibDataStructureParameterValue() { ParamDefId = paramDefTypeId, ParamCategoryId = defaulParamsCateroryId };
                paramTypeValue.SetValue(elem.Type);
                objectDef.AddParameterValue(paramTypeValue);

                foreach (var paramInfo in elem.Info)
                {
                    int paramDefId = dotbimFileDef.ParametersManager.CreateParameterDefinition(paramInfo.Key, VinnyLibDataStructureParameterDefinitionType.ParamString);
                    VinnyLibDataStructureParameterValue paramValue = new VinnyLibDataStructureParameterValue() { ParamDefId = paramDefId, ParamCategoryId = defaulParamsCateroryId };
                    paramValue.SetValue(paramInfo.Value);

                    objectDef.AddParameterValue(paramValue);
                }

                //задаем информацию о положении геометрии
                int geomPlacementInfoId = dotbimFileDef.GeometrtyManager.CreateGeometryPlacementInfo(dotbimGeometry2VinnyGeometry[elem.MeshId]);
                VinnyLibDataStructureGeometryPlacementInfo placementInfo = dotbimFileDef.GeometrtyManager.GetGeometryPlacementInfoById(geomPlacementInfoId);
                placementInfo.Position = new float[] { (float)elem.Vector.X, (float)elem.Vector.Y, (float)elem.Vector.Z };
                QuaternionInfo q = new QuaternionInfo()
                {
                    X = (float)elem.Rotation.Qx,
                    Y = (float)elem.Rotation.Qy,
                    Z = (float)elem.Rotation.Qz,
                    W = (float)elem.Rotation.Qw
                };
                float[] XYZ_Euler = q.GetEulerAnglesRadians();
                placementInfo.VectorOX_Rad = XYZ_Euler[0];
                placementInfo.VectorOY_Rad = XYZ_Euler[1];
                placementInfo.VectorOZ_Rad = XYZ_Euler[2];

                dotbimFileDef.GeometrtyManager.SetMeshGeometryPlacementInfo(geomPlacementInfoId, placementInfo);

                objectDef.GeometryPlacementInfoIds.Add(geomPlacementInfoId);

                dotbimFileDef.ObjectsManager.SetObject(objectId, objectDef);

            }
            return dotbimFileDef;
        }

        public void Export(VinnyLibDataStructureModel vinnyData, ImportExportParameters outputParameters)
        {
            vinnyData.SetCoordinatesTransformation(outputParameters.TransformationInfo);

            dotbim.File dotbimFile = new dotbim.File();
            dotbimFile.SchemaVersion = "1.2.0";

            //метаданные
            dotbimFile.Info = new Dictionary<string, string>();
            foreach (var metadata in vinnyData.Header.Parameters)
            {
                var paramDef = vinnyData.ParametersManager.GetParamDefById(metadata.ParamDefId);
                if (paramDef == null) continue;
                if (!dotbimFile.Info.ContainsKey(paramDef.Name)) dotbimFile.Info.Add(paramDef.Name, metadata.ToString());
            }

            //элементы + геометрия

            dotbimFile.Elements = new List<dotbim.Element>();
            dotbimFile.Meshes = new List<dotbim.Mesh>();
            foreach (var objectDefRaw in vinnyData.ObjectsManager.mObjects)
            {
                VinnyLibDataStructureObject objectDef = objectDefRaw.Value;

                dotbim.Element dotbimElement = new dotbim.Element();
                dotbimElement.Guid = objectDef.UniqueId;

                //метаданные объекта
                dotbimElement.Info = new Dictionary<string, string>();

                foreach(var paramValue in objectDef.Parameters)
                {
                    var paramDef = vinnyData.ParametersManager.GetParamDefById(paramValue.ParamDefId);
                    if (paramDef == null) continue;
                    if (!dotbimElement.Info.ContainsKey(paramDef.Name)) dotbimElement.Info.Add(paramDef.Name, paramValue.ToString());

                    if (paramDef.Name == "Type") dotbimElement.Type = paramValue.ToString();
                }

                //положение
                //задается по первому GeometryPlacementInfo, остальные выравниваются по данному (TODO)
                var placementIdFirst = objectDef.GeometryPlacementInfoIds.First();
                VinnyLibDataStructureGeometryPlacementInfo placementFirst = vinnyData.GeometrtyManager.GetGeometryPlacementInfoById(placementIdFirst);
                VinnyLibDataStructureGeometry geometryDef = vinnyData.GeometrtyManager.GetMeshGeometryById(placementFirst.IdGeometry);
                VinnyLibDataStructureGeometryMesh geometryMeshDef = VinnyLibDataStructureGeometryMesh.asType(geometryDef);

                //материал объекта
                var objectDefMaterial = vinnyData.MaterialsManager.GetMaterialById(geometryDef.MaterialId);
                dotbimElement.Color = new dotbim.Color() { R = objectDefMaterial.ColorR, G = objectDefMaterial.ColorG, B = objectDefMaterial.ColorB, A = objectDefMaterial.ColorAlpha };

                dotbimElement.Vector = new dotbim.Vector() {
                    X = placementFirst.Position[0], Y = placementFirst.Position[1], Z = placementFirst.Position[2] };

                var quaternionInfo = placementFirst.TransformationMatrixInfo.GetRotationInfo();
                dotbimElement.Rotation = new dotbim.Rotation() { Qx = quaternionInfo.X, Qy = quaternionInfo.Y, Qz = quaternionInfo.Z, Qw = quaternionInfo.W };
                dotbimElement.MeshId = placementFirst.IdGeometry;

                vinnyData.GeometrtyManager.SetMeshGeometryPlacementInfo(placementFirst.Id, placementFirst);

                dotbim.Mesh dotbimMesh = new dotbim.Mesh();
                dotbimMesh.MeshId = placementFirst.IdGeometry;
                dotbimMesh.Coordinates = new List<double>();
                dotbimMesh.Indices = new List<int>();

                foreach (var coordInfo in geometryMeshDef.Points)
                {
                    float[] coord = coordInfo.Value;

                    dotbimMesh.Coordinates.Add(coord[0]);
                    dotbimMesh.Coordinates.Add(coord[1]);
                    dotbimMesh.Coordinates.Add(coord[2]);
                }

                foreach (var faceInfo in geometryMeshDef.Faces)
                {
                    int[] face = faceInfo.Value;
                    dotbimMesh.Indices.Add(face[0]);
                    dotbimMesh.Indices.Add(face[1]);
                    dotbimMesh.Indices.Add(face[2]);
                }
                dotbimFile.Meshes.Add(dotbimMesh);

                dotbimElement.FaceColors = new List<int>();
                foreach(var materialFaceInfo in geometryMeshDef.Faces2Materials)
                {
                    var materialInfo = vinnyData.MaterialsManager.GetMaterialById(materialFaceInfo.Value);
                    dotbimElement.FaceColors.Add(materialInfo.ColorR);
                    dotbimElement.FaceColors.Add(materialInfo.ColorG);
                    dotbimElement.FaceColors.Add(materialInfo.ColorB);
                }
                dotbimFile.Elements.Add(dotbimElement);
            }
            dotbimFile.Save(outputParameters.Path);
        }

        ~DotBimFormatProcessing()
        {

        }
    }
}
