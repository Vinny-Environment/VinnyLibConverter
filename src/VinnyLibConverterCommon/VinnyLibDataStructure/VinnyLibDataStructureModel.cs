using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using VinnyLibConverterCommon.Transformation;

namespace VinnyLibConverterCommon.VinnyLibDataStructure
{
    /// <summary>
    ///  Describe the structure of any data (hierarchy, geometry, attributes, materials and etc)
    ///  @details Any read-operations are store info in it; any write-operations using as input-data it class
    /// </summary>
    public class VinnyLibDataStructureModel
    {
        public VinnyLibDataStructureModel()
        {
            Header = new VinnyLibDataStructureHeader();
            ObjectsManager = new VinnyLibDataStructureObjectsManager();
            ParametersManager = new VinnyLibDataStructureParametersManager();
            GeometrtyManager = new VinnyLibDataStructureGeometryManager();
            MaterialsManager = new VinnyLibDataStructureMaterialsManager();
        }
        public VinnyLibDataStructureHeader Header { get; set; }

        public VinnyLibDataStructureObjectsManager ObjectsManager { get; set; }
        public VinnyLibDataStructureParametersManager ParametersManager { get; set; }
        public VinnyLibDataStructureMaterialsManager MaterialsManager { get; set; }
        public VinnyLibDataStructureGeometryManager GeometrtyManager { get; set; }

        #region Различные действия
        
        /// <summary>
        /// Пересчитывает все MeshGeometries и MeshGeometriesPlacementInfo для заданных transformations
        /// </summary>
        /// <param name="transformations"></param>
        public void SetCoordinatesTransformation(List<ICoordinatesTransformation> transformations)
        {
            if (!transformations.Any()) return;
            //Если среди transformations только matrix4x4, то можно только обновить MeshGeometriesPlacementInfo
            //В противном случае придется пересчитывать каждую VinnyLibDataStructureGeometry, формируя для него новый VinnyLibDataStructureGeometryPlacementInfo и обновляя об этом информацию у VinnyLibDataStructureObject
            bool isOnlyMatrix4x4 = !transformations.Where(t => t.GetTransformationType() == CoordinatesTransformationVariant.Affine || t.GetTransformationType() == CoordinatesTransformationVariant.Geodetic).Any();

            if (isOnlyMatrix4x4)
            {
                foreach (int geometryPlInfoId in GeometrtyManager.mMeshGeometriesPlacementInfo.Keys)
                {
                    VinnyLibDataStructureGeometryPlacementInfo onePlInfo = GeometrtyManager.mMeshGeometriesPlacementInfo[geometryPlInfoId];
                    foreach (TransformationMatrix4x4 transformation in transformations)
                    {
                        onePlInfo.TransformationMatrixInfo.Matrix = MatrixImpl.Multiply(onePlInfo.TransformationMatrixInfo.Matrix, transformation.Matrix);
                    }
                }
            }
            else if (ImportExportParameters.mActiveConfig.ReprojectOnlyPosition == true)
            {
                foreach (int geometryPlInfoId in GeometrtyManager.mMeshGeometriesPlacementInfo.Keys)
                {
                    VinnyLibDataStructureGeometryPlacementInfo onePlInfo = GeometrtyManager.mMeshGeometriesPlacementInfo[geometryPlInfoId];
                    foreach (ICoordinatesTransformation transformation in transformations)
                    {
                        onePlInfo.Position = transformation.TransformPoint3d(onePlInfo.Position);
                    }
                    GeometrtyManager.SetMeshGeometryPlacementInfo(geometryPlInfoId, onePlInfo);
                }
            }
            else
            {
                var oldGeometryKeys = GeometrtyManager.mMeshGeometries.Keys;
                var oldMeshGeometriesPlacementInfoKeys = GeometrtyManager.mMeshGeometriesPlacementInfo.Keys;

                //После окончания редактирования старые данные надо будет удалить

                //1. Идёт по всем MeshGeometriesPlacementInfo, пересчитываем геометрию и формируем новые записи
                int geomCounter = GeometrtyManager.mMeshGeometries.Count;

                Dictionary<int, int> geomPlacementInfo_old2new = new Dictionary<int, int>();

                foreach (int geometryPlInfoId in GeometrtyManager.mMeshGeometriesPlacementInfo.Keys)
                {
                    VinnyLibDataStructureGeometryPlacementInfo onePlInfo = GeometrtyManager.mMeshGeometriesPlacementInfo[geometryPlInfoId];
                    VinnyLibDataStructureGeometry transformedGeometry = GeometrtyManager.TransformGeometry(transformations, onePlInfo);
                    int transformedGeometryId = GeometrtyManager.CreateGeometry(transformedGeometry);

                    int transformedGeometryPlacementInfoId = GeometrtyManager.CreateGeometryPlacementInfo(transformedGeometryId);
                    geomPlacementInfo_old2new.Add(geometryPlInfoId, transformedGeometryPlacementInfoId);
                }

                //2. Удаляем старые записи
                foreach (int oldKey_geom in oldGeometryKeys)
                {
                    GeometrtyManager.mMeshGeometries.Remove(oldKey_geom);
                }

                foreach (int oldKey_geomPI in oldMeshGeometriesPlacementInfoKeys)
                {
                    GeometrtyManager.mMeshGeometriesPlacementInfo.Remove(oldKey_geomPI);
                }

                //3. Меняем у VinnyLibDataStructureObject идентификаторы VinnyLibDataStructureGeometryPlacementInfo
                foreach (int objectInfoKey in ObjectsManager.mObjects.Keys)
                {
                    VinnyLibDataStructureObject objectInfo = ObjectsManager.mObjects[objectInfoKey];
                    List<int> idsNew = new List<int>();
                    foreach (int idOld in objectInfo.GeometryPlacementInfoIds)
                    {
                        idsNew.Add(geomPlacementInfo_old2new[idOld]);
                    }

                    objectInfo.GeometryPlacementInfoIds = idsNew;
                    ObjectsManager.SetObject(objectInfoKey, objectInfo);
                }
            }
        }


        #endregion

        internal void SaveDataToDictAnalogues()
        {
            //geometry manager and meshes
            GeometrtyManager.MeshGeometries = new List<VinnyLibDataStructureGeometryMesh>();
            foreach (var g in GeometrtyManager.mMeshGeometries)
            {
                g.Value.Points = new List<PointInfo>();
                foreach (var p in g.Value.mPoints)
                {
                    g.Value.Points.Add(new PointInfo() { Id = p.Key, XYZ = p.Value});
                }

                g.Value.Faces = new List<FaceInfo>();
                foreach (var f in g.Value.mFaces)
                {
                    g.Value.Faces.Add(new FaceInfo() { Id = f.Key, Indices = f.Value });
                }

                g.Value.Faces2Materials = new List<Face2MaterialInfo>();
                foreach (var f in g.Value.mFaces2Materials)
                {
                    g.Value.Faces2Materials.Add(new Face2MaterialInfo() { FaceId = f.Key, MaterialId = f.Value });
                }

                GeometrtyManager.MeshGeometries.Add(g.Value);
            }

            GeometrtyManager.MeshGeometriesPlacementInfo = new List<VinnyLibDataStructureGeometryPlacementInfo>();
            foreach (var g in GeometrtyManager.mMeshGeometriesPlacementInfo)
            {
                GeometrtyManager.MeshGeometriesPlacementInfo.Add(g.Value);
            }

            //materials
            MaterialsManager.Materials = new List<VinnyLibDataStructureMaterial>();
            foreach (var m in MaterialsManager.mMaterials)
            {
                MaterialsManager.Materials.Add(m.Value);
            }

            //objects
            ObjectsManager.Objects = new List<VinnyLibDataStructureObject>();
            foreach (var o in ObjectsManager.mObjects)
            {
                ObjectsManager.Objects.Add(o.Value);
            }

            //parameters
            ParametersManager.Parameters = new List<VinnyLibDataStructureParameterDefinition>();
            foreach (var p in ParametersManager.mParameters)
            {
                ParametersManager.Parameters.Add(p.Value);
            }

            ParametersManager.Categories = new List<CategoryInfo>();
            foreach (var c in ParametersManager.mCategories)
            {
                ParametersManager.Categories.Add(new CategoryInfo() { Id = c.Key, Name = c.Value });
            }
        }

        internal void InitDataFromDictAnalogues()
        {
            GeometrtyManager.mMeshGeometries = new Dictionary<int, VinnyLibDataStructureGeometryMesh>();
            foreach (var g in GeometrtyManager.MeshGeometries)
            {
                g.mPoints = new Dictionary<int, float[]>();
                foreach (var p in g.Points)
                {
                    g.mPoints.Add(p.Id, p.XYZ);
                }

                g.mFaces = new Dictionary<int, int[]>();
                foreach (var f in g.Faces)
                {
                    g.mFaces.Add(f.Id, f.Indices);
                }

                g.mFaces2Materials = new Dictionary<int, int>();
                foreach (var f in g.Faces2Materials)
                {
                    g.mFaces2Materials.Add(f.FaceId, f.MaterialId);
                }

                GeometrtyManager.mMeshGeometries.Add(g.Id, g);
            }
            GeometrtyManager.mGeometryCounter = GeometrtyManager.mMeshGeometries.Keys.Max() + 1;
            

            GeometrtyManager.mMeshGeometriesPlacementInfo = new Dictionary<int, VinnyLibDataStructureGeometryPlacementInfo>();
            foreach (var g in GeometrtyManager.MeshGeometriesPlacementInfo)
            {
                GeometrtyManager.mMeshGeometriesPlacementInfo.Add(g.Id, g);
            }
            GeometrtyManager.mGeometryPlacementInfoCounter = GeometrtyManager.mMeshGeometriesPlacementInfo.Keys.Max() + 1;

            //materials
            MaterialsManager.mMaterials = new Dictionary<int, VinnyLibDataStructureMaterial>();
            foreach (var m in MaterialsManager.Materials)
            {
                MaterialsManager.mMaterials.Add(m.Id, m);
            }

            //objects
            ObjectsManager.mObjects = new Dictionary<int, VinnyLibDataStructureObject>();
            foreach (var o in ObjectsManager.Objects)
            {
                ObjectsManager.mObjects.Add(o.Id, o);
            }

            //parameters
            ParametersManager.mParameters = new Dictionary<int, VinnyLibDataStructureParameterDefinition>();
            foreach (var p in ParametersManager.Parameters)
            {
                ParametersManager.mParameters.Add(p.Id, p);
            }

            ParametersManager.mCategories = new Dictionary<int, string>();
            foreach (var c in ParametersManager.Categories)
            {
                ParametersManager.mCategories.Add(c.Id, c.Name);
            }
        }


        public static VinnyLibDataStructureModel LoadFromFile(string path)
        {
            if (path.EndsWith("vlcxml")) return LoadFromPlainFile(path);
            else
            {
                string tmpDirPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(tmpDirPath);
                ZipFile.ExtractToDirectory(path, tmpDirPath);


                VinnyLibDataStructureModel vinnyModel = LoadFromPlainFile(Path.Combine(tmpDirPath, "vinny.vlcxml"));

                Directory.Delete(tmpDirPath, true);
                return vinnyModel;
            }
        }

        private static VinnyLibDataStructureModel LoadFromPlainFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                using (var stream = System.IO.File.OpenRead(path))
                {
                    var serializer = new XmlSerializer(typeof(VinnyLibDataStructureModel));
                    var result =  serializer.Deserialize(stream) as VinnyLibDataStructureModel;
                    result.InitDataFromDictAnalogues();
                    return result;
                }
            }
            return null;
        }



        public void Save(string path)
        {
            if (path.EndsWith("vlcxml")) SavePlainXml(path);
            else 
            {
                string tmpDirPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(tmpDirPath);
                string tmpPath = Path.Combine(tmpDirPath, "vinny.vlcxml");

                SavePlainXml(tmpPath);
                ZipFile.CreateFromDirectory(tmpDirPath, path);

                Directory.Delete(tmpDirPath, true);
            }
        }

        private void SavePlainXml(string path)
        {
            SaveDataToDictAnalogues();
            using (var writer = new System.IO.StreamWriter(path))
            {
                var serializer = new XmlSerializer(typeof(VinnyLibDataStructureModel));
                serializer.Serialize(writer, this);
                writer.Flush();
            }
        }
    }
}
