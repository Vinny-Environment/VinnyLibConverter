using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// Пересчитывает все mGeometries и mGeometriesPlacementInfo для заданных transformations
        /// </summary>
        /// <param name="transformations"></param>
        public void SetCoordinatesTransformation(List<ICoordinatesTransformation> transformations)
        {
            if (!transformations.Any()) return;
            //Если среди transformations только matrix4x4, то можно только обновить mGeometriesPlacementInfo
            //В противном случае придется пересчитывать каждую VinnyLibDataStructureGeometry, формируя для него новый VinnyLibDataStructureGeometryPlacementInfo и обновляя об этом информацию у VinnyLibDataStructureObject
            bool isOnlyMatrix4x4 = !transformations.Where(t => t.GetTransformationType() == CoordinatesTransformationVariant.Affine || t.GetTransformationType() == CoordinatesTransformationVariant.Geodetic).Any();

            if (isOnlyMatrix4x4)
            {
                foreach (int geometryPlInfoId in GeometrtyManager.mGeometriesPlacementInfo.Keys)
                {
                    VinnyLibDataStructureGeometryPlacementInfo onePlInfo = GeometrtyManager.mGeometriesPlacementInfo[geometryPlInfoId];
                    foreach (TransformationMatrix4x4 transformation in transformations)
                    {
                        onePlInfo.TransformationMatrixInfo.Matrix = MatrixImpl.Multiply(onePlInfo.TransformationMatrixInfo.Matrix, transformation.Matrix);
                    }
                }
            }
            else if (ImportExportParameters.mActiveConfig.ReprojectOnlyPosition == true)
            {
                foreach (int geometryPlInfoId in GeometrtyManager.mGeometriesPlacementInfo.Keys)
                {
                    VinnyLibDataStructureGeometryPlacementInfo onePlInfo = GeometrtyManager.mGeometriesPlacementInfo[geometryPlInfoId];
                    foreach (ICoordinatesTransformation transformation in transformations)
                    {
                        onePlInfo.Position = transformation.TransformPoint3d(onePlInfo.Position);
                    }
                    GeometrtyManager.SetGeometryPlacementInfo(geometryPlInfoId, onePlInfo);
                }
            }
            else
            {
                var oldGeometryKeys = GeometrtyManager.mGeometries.Keys;
                var oldGeometriesPlacementInfoKeys = GeometrtyManager.mGeometriesPlacementInfo.Keys;

                //После окончания редактирования старые данные надо будет удалить

                //1. Идёт по всем GeometriesPlacementInfo, пересчитываем геометрию и формируем новые записи
                int geomCounter = GeometrtyManager.mGeometries.Count;

                Dictionary<int, int> geomPlacementInfo_old2new = new Dictionary<int, int>();

                foreach (int geometryPlInfoId in GeometrtyManager.mGeometriesPlacementInfo.Keys)
                {
                    VinnyLibDataStructureGeometryPlacementInfo onePlInfo = GeometrtyManager.mGeometriesPlacementInfo[geometryPlInfoId];
                    VinnyLibDataStructureGeometry transformedGeometry = GeometrtyManager.TransformGeometry(transformations, onePlInfo);
                    int transformedGeometryId = GeometrtyManager.CreateGeometry(transformedGeometry);

                    int transformedGeometryPlacementInfoId = GeometrtyManager.CreateGeometryPlacementInfo(transformedGeometryId);
                    geomPlacementInfo_old2new.Add(geometryPlInfoId, transformedGeometryPlacementInfoId);
                }

                //2. Удаляем старые записи
                foreach (int oldKey_geom in oldGeometryKeys)
                {
                    GeometrtyManager.mGeometries.Remove(oldKey_geom);
                }

                foreach (int oldKey_geomPI in oldGeometriesPlacementInfoKeys)
                {
                    GeometrtyManager.mGeometriesPlacementInfo.Remove(oldKey_geomPI);
                }

                //3. Меняем у VinnyLibDataStructureObject идентификаторы VinnyLibDataStructureGeometryPlacementInfo
                foreach (int objectInfoKey in ObjectsManager.Objects.Keys)
                {
                    VinnyLibDataStructureObject objectInfo = ObjectsManager.Objects[objectInfoKey];
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

        public static VinnyLibDataStructureModel LoadFromFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                using (var stream = System.IO.File.OpenRead(path))
                {
                    var serializer = new XmlSerializer(typeof(VinnyLibDataStructureModel));
                    return serializer.Deserialize(stream) as VinnyLibDataStructureModel; ;
                }
            }
            return null;
        }

        public void Save(string path)
        {
            using (var writer = new System.IO.StreamWriter(path))
            {
                var serializer = new XmlSerializer(typeof(VinnyLibDataStructureModel));
                serializer.Serialize(writer, this);
                writer.Flush();
            }
        }
    }
}
