using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public VinnyLibDataStructureHeader Header { get; private set; }

        public VinnyLibDataStructureObjectsManager ObjectsManager { get; private set; }
        public VinnyLibDataStructureParametersManager ParametersManager { get; private set; }
        public VinnyLibDataStructureMaterialsManager MaterialsManager { get; private set; }
        public VinnyLibDataStructureGeometryManager GeometrtyManager { get; private set; }

        #region Различные действия
        
        /// <summary>
        /// Пересчитывает все mGeometries и mGeometriesPlacementInfo для заданных transformations
        /// </summary>
        /// <param name="transformations"></param>
        public void SetCoordinatesTransformation(List<ICoordinatesTransformation> transformations)
        {
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
                    VinnyLibDataStructureGeometry transformedGeometry = TransformGeometry(transformations, onePlInfo);
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

        /// <summary>
        /// Пересчитывает координаты VinnyLibDataStructureGeometry для заданных трансформаций и информации о положении VinnyLibDataStructureGeometryPlacementInfo
        /// </summary>
        /// <param name="transformations"></param>
        /// <param name="geometryPlacementInfo"></param>
        /// <returns></returns>
        public VinnyLibDataStructureGeometry TransformGeometry(List<ICoordinatesTransformation> transformations, VinnyLibDataStructureGeometryPlacementInfo geometryPlacementInfo)
        {
            List<ICoordinatesTransformation> transformations2 = new List<ICoordinatesTransformation>()
            {
                geometryPlacementInfo.TransformationMatrixInfo
            }.Concat(transformations).ToList();

            VinnyLibDataStructureGeometry targetGeometry = GeometrtyManager.mGeometries[geometryPlacementInfo.IdGeometry];

            foreach (ICoordinatesTransformation transformation in transformations)
            {
                //Для каждого transformation необходимо заново инициализировать mGeometries и mGeometriesPlacementInfo (?)
                if (targetGeometry.GetGeometryType() == VinnyLibDataStructureGeometryType.Mesh)
                {
                    VinnyLibDataStructureGeometryMesh targetGeometry_Mesh = VinnyLibDataStructureGeometryMesh.asType(targetGeometry);
                    foreach (int PointKey in targetGeometry_Mesh.Points.Keys)
                    {
                        float[] XYZ_Converted = transformation.TransformPoint3d(targetGeometry_Mesh.Points[PointKey]);
                        targetGeometry_Mesh.Points[PointKey] = XYZ_Converted;
                    }
                }
            }
            return targetGeometry;
        }
        #endregion
    }
}
