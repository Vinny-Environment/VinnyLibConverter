using System;
using System.Collections.Generic;
using System.Text;

using VinnyLibConverterCommon;
using VinnyLibConverterCommon.VinnyLibDataStructure;

using System.Linq;

namespace VinnyLibConverter_DotBIM
{
    public class DotBimFormatProcessing : ICdeFormatProcessing
    {
        public DotBimFormatProcessing()
        {
            this.LoadAuxiliaryAssemblies();
        }
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

        public VinnyLibDataStructureModel Import(IEParameters openParameters)
        {
            VinnyLibDataStructureModel dotbimFileDef = new VinnyLibDataStructureModel();
            IEParameters.mActiveConfig = openParameters;
            IEParameters.mActiveConfig.CheckGeometryDubles = false;//потому что читаем формат

            dotbim.File dotbimFile = dotbim.File.Read(openParameters.Path);
            //header
            foreach (var metadata in dotbimFile.Info)
            {
                dotbimFileDef.Header.AddHeaderData(metadata.Key, metadata.Value);
            }
            //geometry
            foreach (var mesh in dotbimFile.Meshes)
            {
                int geomId = dotbimFileDef.GeometrtyManager.CreateGeometry(VinnyLibDataStructureGeometryType.Mesh, mesh.MeshId);
                VinnyLibDataStructureGeometry meshGeomRaw = dotbimFileDef.GeometrtyManager.GetGeometryById(geomId);
                VinnyLibDataStructureGeometryMesh meshGeom = VinnyLibDataStructureGeometryMesh.asType(meshGeomRaw);
                if (meshGeom != null)
                {
                    for(int dotbimMeshFaceIndicesCounter = 0; dotbimMeshFaceIndicesCounter < mesh.Indices.Count - 2; dotbimMeshFaceIndicesCounter+=3)
                    {
                        int VertexId1 = mesh.Indices[dotbimMeshFaceIndicesCounter];
                        int VertexId2 = mesh.Indices[dotbimMeshFaceIndicesCounter + 1];
                        int VertexId3 = mesh.Indices[dotbimMeshFaceIndicesCounter + 2];

                        float[] Vertex1 = GetVertexCoords(VertexId1);
                        float[] Vertex2 = GetVertexCoords(VertexId2);
                        float[] Vertex3 = GetVertexCoords(VertexId3);

                        meshGeom.AddFace(Vertex1, Vertex2, Vertex3);
                    }

                    dotbimFileDef.GeometrtyManager.SetGeometry(geomId, meshGeom);
                }
                float[] GetVertexCoords(int VertexId)
                {
                    return new float[] { (float)mesh.Coordinates[VertexId * 3], (float)mesh.Coordinates[VertexId * 3 + 1], (float)mesh.Coordinates[VertexId * 3 + 2] };
                }
            }

            //elements
            int defaulParamsCateroryId = dotbimFileDef.ParametersManager.CreateCategory("Parameters");
            int paramDefTypeId = dotbimFileDef.ParametersManager.CreateParameterDefinition("Type", VinnyLibDataStructureParameterDefinitionType.ParamString);

            foreach (var elem in dotbimFile.Elements)
            {
                VinnyLibDataStructureGeometry meshGeomRaw = dotbimFileDef.GeometrtyManager.GetGeometryById(elem.MeshId);
                VinnyLibDataStructureGeometryMesh meshGeom = VinnyLibDataStructureGeometryMesh.asType(meshGeomRaw);

                int objectId = dotbimFileDef.ObjectsManager.CreateObject();
                VinnyLibDataStructureObject objectDef = dotbimFileDef.ObjectsManager.GetObjectById(objectId);

                //задаем информацию о цветах граней для геометрии
                for (int colorCounter = 0; colorCounter < elem.FaceColors.Count - 2; colorCounter += 3)
                {
                    int[] RGBcolor = new int[]{
                    elem.FaceColors[colorCounter], elem.FaceColors[colorCounter + 1], elem.FaceColors[colorCounter + 2]};
                    int materialId = dotbimFileDef.MaterialsManager.CreateMaterial(RGBcolor);
                    meshGeom.AssignMaterialToFace(colorCounter / 3, materialId);
                }

                dotbimFileDef.GeometrtyManager.SetGeometry(elem.MeshId, meshGeom);

                //задаем цвет
                objectDef.MaterialId = dotbimFileDef.MaterialsManager.CreateMaterial(new int[] { elem.Color.R, elem.Color.G, elem.Color.B, elem.Color.A });

                objectDef.UniqueId = elem.Guid;

                //задаем свойства
                VinnyLibDataStructureParameterValue paramTypeValue = new VinnyLibDataStructureParameterValue() { ParamDefId = paramDefTypeId, ParamCategoryId = defaulParamsCateroryId };
                paramTypeValue.SetValue(elem.Type);

                foreach (var paramInfo in elem.Info)
                {
                    int paramDefId = dotbimFileDef.ParametersManager.CreateParameterDefinition(paramInfo.Key, VinnyLibDataStructureParameterDefinitionType.ParamString);
                    VinnyLibDataStructureParameterValue paramValue = new VinnyLibDataStructureParameterValue() { ParamDefId = paramDefId, ParamCategoryId = defaulParamsCateroryId };
                    paramValue.SetValue(paramInfo.Value);

                    objectDef.AddParameterValue(paramValue);
                }

                //задаем информацию о положении геометрии
                int geomPlacementInfoId = dotbimFileDef.GeometrtyManager.CreateGeometryPlacementInfo(elem.MeshId);
                VinnyLibDataStructureGeometryPlacementInfo placementInfo = dotbimFileDef.GeometrtyManager.GetGeometryPlacementInfoById(geomPlacementInfoId);
                placementInfo.Position = new float[] { (float)elem.Vector.X, (float)elem.Vector.Y, (float)elem.Vector.Z };
                placementInfo.SetRotationFromQuaternion(new QuaternionInfo() {
                    X = (float)elem.Rotation.Qx,
                    Y = (float)elem.Rotation.Qy,
                    Z = (float)elem.Rotation.Qz,
                    W = (float)elem.Rotation.Qw
                });
                dotbimFileDef.GeometrtyManager.SetGeometryPlacementInfo(geomPlacementInfoId, placementInfo);

                dotbimFileDef.ObjectsManager.SetObject(objectId, objectDef);

            }
            return dotbimFileDef;
        }

        public void Export(VinnyLibDataStructureModel data, IEParameters outputParameters)
        {
            dotbim.File dotbimFile = new dotbim.File();
            dotbimFile.SchemaVersion = "1.2.0";

            //метаданные
            dotbimFile.Info = new Dictionary<string, string>();
            foreach (var metadata in data.Header.Data)
            {
                if (!dotbimFile.Info.ContainsKey(metadata.Item1)) dotbimFile.Info.Add(metadata.Item1, metadata.Item2);
            }

            //элементы + геометрия
            dotbimFile.Elements = new List<dotbim.Element>();
            dotbimFile.Meshes = new List<dotbim.Mesh>();
            foreach (var objectDefRaw in data.ObjectsManager.Objects)
            {
                VinnyLibDataStructureObject objectDef = objectDefRaw.Value;

                dotbim.Element dotbimElement = new dotbim.Element();
                dotbimElement.Guid = objectDef.UniqueId;

                //материал объекта
                var objectDefMaterial = data.MaterialsManager.GetMaterialById(objectDef.MaterialId);
                dotbimElement.Color = new dotbim.Color() { R = objectDefMaterial.ColorR, G = objectDefMaterial.ColorG, B = objectDefMaterial.ColorB, A = objectDefMaterial.ColorAlpha };

                //метаданные объекта
                dotbimElement.Info = new Dictionary<string, string>();

                foreach(var paramValue in objectDef.Parameters)
                {
                    var paramDef = data.ParametersManager.GetParamDefById(paramValue.ParamDefId);
                    if (paramDef == null) continue;
                    if (!dotbimElement.Info.ContainsKey(paramDef.Name)) dotbimElement.Info.Add(paramDef.Name, paramValue.ToString());

                    if (paramDef.Name == "Type") dotbimElement.Type = paramValue.ToString();
                }

                //положение
                //задается по первому GeometryPlacementInfo, остальные выравниваются по данному
                var placementFirst = objectDef.GeometryPlacementInfos.First();

                dotbimElement.Vector = new dotbim.Vector() {
                    X = placementFirst.Position[0], Y = placementFirst.Position[1], Z = placementFirst.Position[2] };
                var quaternionInfo = placementFirst.TransformationMatrixInfo.ToQuaternion();
                dotbimElement.Rotation = new dotbim.Rotation() { Qx = quaternionInfo.X, Qy = quaternionInfo.Y, Qz = quaternionInfo.Z, Qw = quaternionInfo.W };


                dotbimElement.MeshId = placementFirst.IdGeometry;
                VinnyLibDataStructureGeometry geometryDef = data.GeometrtyManager.GetGeometryById(placementFirst.IdGeometry);
                VinnyLibDataStructureGeometryMesh geometryMeshDef = VinnyLibDataStructureGeometryMesh.asType(geometryDef);

                dotbim.Mesh dotbimMesh = new dotbim.Mesh();
                dotbimMesh.MeshId = placementFirst.IdGeometry;
                dotbimMesh.Coordinates = new List<double>();
                dotbimMesh.Indices = new List<int>();
                

                foreach (float[] coord in geometryMeshDef.Points)
                {
                    dotbimMesh.Coordinates.Add(coord[0]);
                    dotbimMesh.Coordinates.Add(coord[1]);
                    dotbimMesh.Coordinates.Add(coord[2]);
                }

                foreach (int[] face in geometryMeshDef.Faces)
                {
                    dotbimMesh.Indices.Add(face[0]);
                    dotbimMesh.Indices.Add(face[1]);
                    dotbimMesh.Indices.Add(face[2]);
                }
                dotbimFile.Meshes.Add(dotbimMesh);

                dotbimElement.FaceColors = new List<int>();
                foreach(var materialFaceInfo in geometryMeshDef.Faces2Materials)
                {
                    var materialInfo = data.MaterialsManager.GetMaterialById(materialFaceInfo.Value);
                    dotbimElement.FaceColors.Add(materialInfo.ColorR);
                    dotbimElement.FaceColors.Add(materialInfo.ColorG);
                    dotbimElement.FaceColors.Add(materialInfo.ColorB);
                }

                dotbimFile.Elements.Add(dotbimElement);

            }

            dotbimFile.Save(outputParameters.Path);

        }

        public void LoadAuxiliaryAssemblies()
        {
            //TODO:
        }

        public void UnloadAuxiliaryAssemblies()
        {
            //TODO:
        }

        ~DotBimFormatProcessing()
        {
            this.UnloadAuxiliaryAssemblies();
        }
    }
}
