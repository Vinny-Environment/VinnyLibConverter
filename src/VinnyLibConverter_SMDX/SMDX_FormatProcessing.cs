using System;
using System.IO;
using System.Reflection;

using VinnyLibConverterCommon;
using VinnyLibConverterCommon.VinnyLibDataStructure;

using System.Linq;
using System.Collections.Generic;
using VinnyLibConverterCommon.Transformation;
using VinnyLibConverter_SMDX.SMDX;
using System.Text.RegularExpressions;
using VinnyLibConverter_SMDX.SMDX.Materials;

namespace VinnyLibConverter_SMDX
{
    public class SMDX_FormatProcessing : ICdeFormatProcessing
    {
       
        public CdeVariant GetCdeType()
        {
            return CdeVariant.SMDX;
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
            VinnyLibDataStructureModel vinnyFileDef = new VinnyLibDataStructureModel();
            ImportExportParameters.mActiveConfig = openParameters;

            //Получение данных об SMDX
            SMDX_File smdxFileInfo = new SMDX_File(openParameters.Path);

            //обработка геометрии -- мэшей. Информацию о границах bounds и детализацию по LOD'ам пропускаем -- это специфичная информация. Тем не менее видимо для формирования SMDX bounds считать придется (проверить, будет ли грузиться без них?)

            //т.к. в j3d-файле геометрии может быть несколько определений mesh'а, нумерация id-шников геометрий будет идти своя в VinnyLibDataStructureGeometryManager и для последующего связыавания smdx-элементов с геометрией потребуется этот словарь
            //VinnyLibDataStructureGeometryPlacementInfo будут раскопированы для всеш мэшей, т.к. в блоке smdx-insertions нет разделения на под-геометрии

            Dictionary<int, List<int>> smdxGeometry2VinnyGeometry = new Dictionary<int, List<int>>();

            int smdxGeomertyCounter = 0;
            foreach (SMDX_Content_Geometry smdxGeometryInfo in smdxFileInfo.Content.geometry)
            {
                SMDX_Geometry_j3d smdxGeometryJ3dFile = smdxFileInfo.GetGeometryJ3d(smdxGeometryInfo.href);
                if (smdxGeometryJ3dFile == null) 
                {
                    smdxGeomertyCounter++;
                    continue;
                }
                smdxGeometry2VinnyGeometry[smdxGeomertyCounter] = new List<int>();

                foreach (var smdxGeometryPartInfoRaw in smdxGeometryJ3dFile.Meshes)
                {
                    SMDX_Geometry_j3d_Part smdxGeometryPartInfoDef = smdxGeometryPartInfoRaw.Value;
                    //наименование части пропускаем -- это какая-то неважная (точно?) информация
                    int vinnyGeometryInfoDefId = vinnyFileDef.GeometrtyManager.CreateGeometry(VinnyLibDataStructureGeometryType.Mesh);
                    VinnyLibDataStructureGeometryMesh vinnyGeometryInfoMeshDef = VinnyLibDataStructureGeometryMesh.asType(vinnyFileDef.GeometrtyManager.GetGeometryById(vinnyGeometryInfoDefId));

                    //сохраняем точки
                    for (int smdxVertexCounter = 0; smdxVertexCounter < smdxGeometryPartInfoDef.positions.Count - 2; smdxVertexCounter += 3)
                    {
                        float x, y, z;
                        x = smdxGeometryPartInfoDef.positions[smdxVertexCounter];
                        y = smdxGeometryPartInfoDef.positions[smdxVertexCounter + 1];
                        z = smdxGeometryPartInfoDef.positions[smdxVertexCounter + 2];

                        vinnyGeometryInfoMeshDef.AddVertex(x, y, z);
                    }
                    //сохраняем грани
                    for (int smdxFaceCounter = 0; smdxFaceCounter < smdxGeometryPartInfoDef.triangles.Count - 2; smdxFaceCounter += 3)
                    {
                        int v1, v2, v3;
                        v1 = smdxGeometryPartInfoDef.triangles[smdxFaceCounter];
                        v2 = smdxGeometryPartInfoDef.triangles[smdxFaceCounter + 1];
                        v3 = smdxGeometryPartInfoDef.triangles[smdxFaceCounter + 2];

                        //Вот тут может возникнуть косяк, если mActiveConfig.CheckGeometryDubles = true и каке-то вершины "ужались", но при ЧТЕНИИ формата этого быть не должно
                        vinnyGeometryInfoMeshDef.AddFace(v1, v2, v3);
                    }

                    //информация о цвете
                    foreach (var groupColorsInfo in smdxGeometryPartInfoDef.groups)
                    {
                        SMDX_Material_BlinnPhong material = smdxFileInfo.GetMaterial(groupColorsInfo.Key);
                        if (material != null && material.ambient != null && material.ambient.Length == 3)
                        {
                            int vinnyMaterialId = vinnyFileDef.MaterialsManager.CreateMaterial(material.GetColorRGB());
                            for (int faceIndex = groupColorsInfo.Value[0]; faceIndex < groupColorsInfo.Value[1]; faceIndex++)
                            {
                                vinnyGeometryInfoMeshDef.AssignMaterialToFace(faceIndex, vinnyMaterialId);
                            }
                        }
                    }

                    vinnyFileDef.GeometrtyManager.SetGeometry(vinnyGeometryInfoDefId, vinnyGeometryInfoMeshDef);
                    smdxGeometry2VinnyGeometry[smdxGeomertyCounter].Add(vinnyGeometryInfoDefId);
                }
                smdxGeomertyCounter++;
            }

            
            //обработка insertions -- это фактически VinnyLibDataStructureGeometryPlacementInfo и ТОЛЬКО они, несмотря на отсылку к group
            Dictionary<int, List<int>> smdxGroupObjects = new Dictionary<int, List<int>>();
            foreach (SMDX_Content_Insertion smdxInsertionInfo in smdxFileInfo.Content.insertions)
            {
                if (!smdxGroupObjects.ContainsKey(smdxInsertionInfo.group)) smdxGroupObjects.Add(smdxInsertionInfo.group, new List<int>());

                //заполняем VinnyLibDataStructureGeometryPlacementInfo
                List<int> geoms = smdxGeometry2VinnyGeometry[smdxInsertionInfo.geometry];
                foreach (int geomsId in geoms)
                {
                    int VinnyLibDataStructureGeometryPlacementInfoId = vinnyFileDef.GeometrtyManager.CreateGeometryPlacementInfo(geomsId);
                    VinnyLibDataStructureGeometryPlacementInfo geomPI = vinnyFileDef.GeometrtyManager.GetGeometryPlacementInfoById(VinnyLibDataStructureGeometryPlacementInfoId);

                    float[] posTmp = smdxFileInfo.Content.wcs;
                    if (smdxInsertionInfo.position != null)
                    {
                        if (smdxInsertionInfo.position.GetType() == typeof(Array))
                        {
                            float[] posRaw = ((Array)smdxInsertionInfo.position).Cast<float>().ToArray();
                            posTmp[0] += posRaw[0];
                            posTmp[1] += posRaw[1];
                            if (posRaw.Length == 2) posTmp[2] += posRaw[2];
                        }
                        else if (smdxInsertionInfo.position.GetType() == typeof(Dictionary<string, object>))
                        {
                            //пропускаем. Не знаю как это читать
                        }

                    }

                    geomPI.Position = posTmp;
                    if (smdxInsertionInfo.scale != null)
                    {
                        if (smdxInsertionInfo.scale.GetType() == typeof(Array))
                        {
                            geomPI.Scale = ((Array)smdxInsertionInfo.scale).Cast<float>().ToArray();
                        }
                        else if (smdxInsertionInfo.scale.GetType() == typeof(float))
                        {
                            geomPI.Scale = new float[3] { (float)smdxInsertionInfo.scale, (float)smdxInsertionInfo.scale, (float)smdxInsertionInfo.scale };
                        }
                    }

                    geomPI.VectorOZ_Rad = smdxInsertionInfo.angle;

                    if (smdxInsertionInfo.normal != null)
                    {
                        if (smdxInsertionInfo.normal.GetType() == typeof(Array))
                        {
                            float[] normalRaw = ((Array)smdxInsertionInfo.normal).Cast<float>().ToArray();
                            if (normalRaw.Length == 3)
                            {
                                QuaternionInfo normal = QuaternionInfo.NormalToQuaternion(new Vector3(normalRaw[0], normalRaw[1], normalRaw[2]));
                                var angles_XYZ = normal.GetEulerAnglesRadians();
                                geomPI.VectorOX_Rad = angles_XYZ[0];
                                geomPI.VectorOY_Rad = angles_XYZ[1];
                                geomPI.VectorOZ_Rad += angles_XYZ[2]; //проверить, коррректно ли?
                            }
                        }
                    }

                    

                    geomPI.InitMatrix();

                    smdxGroupObjects[smdxInsertionInfo.group].Add(geomPI.Id);

                    vinnyFileDef.GeometrtyManager.SetGeometryPlacementInfo(geomPI.Id, geomPI);
                }
            }

            //обработка groups -- это фактически объекты, геометрия которых лежит в блоке insertions, т.е. могут быть объекты без геометрии (структурные компоненты)
            //при этом один и тот же insertion из insertions может находиться одновременно в нескольких группах
            int smdxGroupCounter = 0;
            foreach (SMDX_Content_Group smdxGroupInfo in smdxFileInfo.Content.groups)
            {
                VinnyLibDataStructureObject vinnyObject = vinnyFileDef.ObjectsManager.GetObjectById(vinnyFileDef.ObjectsManager.CreateObject());
                //vinnyObject.IdCDE = smdxGroupCounter;
                //задаем информацию о геометрии из раннее обработанного блока insertions
                if (smdxGroupObjects.ContainsKey(smdxGroupCounter))
                {
                    foreach (int geomPIid in  smdxGroupObjects[smdxGroupCounter])
                    {
                        vinnyObject.GeometryPlacementInfoIds.Add(geomPIid);
                    }
                }

                vinnyObject.Name = smdxGroupInfo.name;
                vinnyObject.ParentId = smdxGroupInfo.parent ?? -1;
                foreach (SMDX_Content_Type_Property? smdxProperty in smdxGroupInfo.properties ?? new List<SMDX_Content_Type_Property>())
                {
                    if (smdxProperty == null) continue;

                    VinnyLibDataStructureParameterDefinitionType propType = VinnyLibDataStructureParameterDefinitionType.ParamString;
                    string propTypeStr = "";
                    if (smdxProperty.info != null && smdxProperty.info.type != null) propTypeStr = smdxProperty.info.type.ToString();

                    switch(propTypeStr)
                    {
                        case "bool":
                            propType = VinnyLibDataStructureParameterDefinitionType.ParamBool;
                            break;
                        case "int":
                            propType = VinnyLibDataStructureParameterDefinitionType.ParamInteger;
                            break;
                        case "float":
                            propType = VinnyLibDataStructureParameterDefinitionType.ParamReal;
                            break;
                    }

                    
                    int propId = vinnyFileDef.ParametersManager.CreateParameterDefinition(smdxProperty.tag, propType);
                    VinnyLibDataStructureParameterValue paramValue = new VinnyLibDataStructureParameterValue();
                    paramValue.ParamDefId = propId;
                    if (propType == VinnyLibDataStructureParameterDefinitionType.ParamBool)
                    {
                        paramValue.SetValue(smdxProperty.value, propType);
                    }
                    vinnyObject.Parameters.Add(paramValue);
                }
                vinnyFileDef.ObjectsManager.SetObject(vinnyObject.Id, vinnyObject);

                smdxGroupCounter++;
            }


            return vinnyFileDef;

        }

        public void Export(VinnyLibDataStructureModel vinnyData, ImportExportParameters outputParameters)
        {
            vinnyData.SetCoordinatesTransformation(outputParameters.TransformationInfo);

            SMDX_File smdxProject = new SMDX_File();

            //заносим материалы (только сохраняем как файлы)
            //Сопоставление id' материала и имени файла jmtl ресурсов SMDX
            Dictionary<int, string> materialId2smdxJmtlNames = new Dictionary<int, string>();
            foreach (int materialIndex in vinnyData.MaterialsManager.mMaterials.Keys)
            {
                VinnyLibDataStructureMaterial materialDef = vinnyData.MaterialsManager.mMaterials[materialIndex];
                string materialName = $"material_{materialIndex}.jmtl";
                SMDX_Material_BlinnPhong smdxMaterial = SMDX_Material_BlinnPhong.CreateFromRGB(materialDef.GetRGB());
                smdxProject.CreateMaterial(smdxMaterial, materialName);
                materialId2smdxJmtlNames.Add(materialIndex, materialName);
            }

            //заносим геометрию (мэши)
            Dictionary<int, int> vinnyGeometryId2smdxIds = new Dictionary<int, int>();
            int smdxGeometryCounter = 0;
            foreach (var vinnyGeometryInfo in vinnyData.GeometrtyManager.mGeometries)
            {
                VinnyLibDataStructureGeometry vinnyGeometryDef = vinnyGeometryInfo.Value;
                if (vinnyGeometryDef.GetGeometryType() == VinnyLibDataStructureGeometryType.Mesh)
                {
                    VinnyLibDataStructureGeometryMesh vinnyGeometryMeshDef = VinnyLibDataStructureGeometryMesh.asType(vinnyGeometryDef);

                    SMDX_Geometry_j3d smdxMeshGeometry = new SMDX_Geometry_j3d();
                    SMDX_Geometry_j3d_Part smdxMeshGeometryPart = new SMDX_Geometry_j3d_Part();

                    foreach (var vinnyMeshPointInfo in vinnyGeometryMeshDef.Points)
                    {
                        float[] vinnyMeshPointCoordsInfo = vinnyMeshPointInfo.Value;
                        smdxMeshGeometryPart.positions.Add(vinnyMeshPointCoordsInfo[0]);
                        smdxMeshGeometryPart.positions.Add(vinnyMeshPointCoordsInfo[1]);
                        smdxMeshGeometryPart.positions.Add(vinnyMeshPointCoordsInfo[2]);
                    }

                    //задаем информацию о цвете. Из-за "особенности" j3d-файла, грани надо отсортировать по отдельным цветам -- то есть сперва перебираем vinnyGeometryMeshDef.Faces2Materials, затем перетасовываем vinnyGeometryMeshDef.Faces и сохраняем их в j3d
                    //Если количество Faces2Materials и Faces не согласованное, то цветом будет значение vinnyGeometryMeshDef.MaterialId

                    //Ключ: VinnyLibDataStructureMaterial.Id; Значение: список номеров граней с данным материалом
                    Dictionary<int, List<int>> materialsOfFacesCollection = new Dictionary<int, List<int>>();
                    if (vinnyGeometryMeshDef.Faces2Materials.Count == vinnyGeometryMeshDef.Faces.Count)
                    {
                        foreach (var faceMaterial in vinnyGeometryMeshDef.Faces2Materials)
                        {
                            if (!materialsOfFacesCollection.ContainsKey(faceMaterial.Key)) materialsOfFacesCollection.Add(faceMaterial.Key, new List<int>());
                            materialsOfFacesCollection[faceMaterial.Key].Add(faceMaterial.Value);
                        }
                    }
                    else materialsOfFacesCollection.Add(vinnyGeometryMeshDef.MaterialId, vinnyGeometryMeshDef.Faces.Keys.Cast<int>().ToList());

                    //заполняем groups у j3d-файла
                    int prevoiusFacesCount = 0;
                    foreach (var materialOfFacesCollection in materialsOfFacesCollection)
                    {
                        smdxMeshGeometryPart.groups.Add(materialId2smdxJmtlNames[materialOfFacesCollection.Key], new int[] { prevoiusFacesCount, materialOfFacesCollection.Value.Count});
                        //добавим в triangles j3d-файла одноцветные грани из materialOfFacesCollection.Value
                        foreach (int faceIndex in materialOfFacesCollection.Value)
                        {
                            int[] faceDef = vinnyGeometryMeshDef.Faces[faceIndex];
                            smdxMeshGeometryPart.triangles.Add(faceDef[0]);
                            smdxMeshGeometryPart.triangles.Add(faceDef[1]);
                            smdxMeshGeometryPart.triangles.Add(faceDef[2]);
                        }
                    }
                    smdxMeshGeometry.Meshes.Add("geometry", smdxMeshGeometryPart);

                    smdxProject.CreateGeometry(new SMDX_Content_Geometry() { href = $"geometry_{vinnyGeometryInfo.Key}.j3d", bounds = vinnyGeometryMeshDef.ComputeBounds() }, smdxMeshGeometry);

                    vinnyGeometryId2smdxIds.Add(vinnyGeometryMeshDef.Id, smdxGeometryCounter);
                }
                else
                {
                    //Другие типы не поддерживаются вроде бы в SMDX
                    continue;
                }
            }

            //заполняем groups
            foreach (var vinnyObjectInfo in vinnyData.ObjectsManager.Objects)
            {
                VinnyLibDataStructureObject vinnyObjectDef = vinnyObjectInfo.Value;

            }

            //заполняем insertions
            foreach (var vinnyGeometryPI_Info in vinnyData.GeometrtyManager.mGeometriesPlacementInfo)
            {
                VinnyLibDataStructureGeometryPlacementInfo vinnyGeometryPI = vinnyGeometryPI_Info.Value;

                SMDX_Content_Insertion smdxInsertionDef = new SMDX_Content_Insertion();
                smdxInsertionDef.geometry = vinnyGeometryId2smdxIds[vinnyGeometryPI.IdGeometry];
            }


            smdxProject.Save(outputParameters.Path);
        }
    }
}
