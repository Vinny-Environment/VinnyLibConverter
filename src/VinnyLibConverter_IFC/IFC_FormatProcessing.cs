using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

using VinnyLibConverterCommon;
using VinnyLibConverterCommon.VinnyLibDataStructure;
using VinnyLibConverterCommon.GeometryUtils;
using VinnyLibConverterCommon.Transformation;
using VinnyLibConverterUtils;
using VinnyLibConverterCommon.Interfaces;
using System.Runtime.InteropServices.ComTypes;


using GeometryGym.Ifc;
using GeometryGym.STEP;
using System.Diagnostics;
using static VinnyLibConverterCommon.VinnyLibDataStructure.VinnyLibDataStructureObjectsManager;


namespace VinnyLibConverter_IFC
{
    public class IFC_FormatProcessing : ICdeFormatProcessing
    {
        public CdeVariant GetCdeType()
        {
            return CdeVariant.IFC;
        }

        public bool IsReadable()
        {
            return true;
        }

        public bool IsWriteable()
        {
            return true;
        }

        private void ProcessIfcProperties(IfcElement ifcElement, VinnyLibDataStructureObjectWithParametersBase vinnyObjectDef)
        {
            if (ifcElement == null) return;
            foreach (IfcRelDefinesByProperties rdp in ifcElement.IsDefinedBy)
            {
                foreach (IfcPropertySet ifcPset in rdp.RelatingPropertyDefinition.OfType<IfcPropertySet>())
                {
                    foreach (System.Collections.Generic.KeyValuePair<string, IfcProperty> ifcPropInfo in ifcPset.HasProperties)
                    {
                        IfcProperty ifcPropDef = ifcPropInfo.Value;
                        if (ifcPropDef == null) continue;

                        IfcPropertySingleValue ifcPropDefAsSingleValue;
                        if ((ifcPropDefAsSingleValue = ifcPropDef as IfcPropertySingleValue) != null)
                        {
                            var vinnyPropValue = this.mVinnyModelDef.ParametersManager.CreateParameterValueWithDefs(ifcPropDefAsSingleValue.Name, ifcPropDefAsSingleValue.NominalValue.ValueString, ifcPset.Name);
                            vinnyObjectDef.Parameters.Add(vinnyPropValue);
                        }    
                    }
                }
            }
        }

        private void SetMeshDataFrom(VinnyLibDataStructureGeometryMesh vinnyMeshDef, SET<IfcFace> ifcFaces)
        {
            foreach (IfcFace face in ifcFaces)
            {
                IfcFaceBound faceBound = face.Bounds.First();
                IfcLoop faceLoop = faceBound.Bound;
                IfcPolyLoop faceLoopPoly = faceLoop as IfcPolyLoop;
                if (faceLoopPoly != null)
                {
                    //points (faceLoopPoly.Polygon)
                    if (faceLoopPoly.Polygon.Count == 3)
                    {
                        IfcCartesianPoint p1 = faceLoopPoly.Polygon[0];
                        IfcCartesianPoint p2 = faceLoopPoly.Polygon[1];
                        IfcCartesianPoint p3 = faceLoopPoly.Polygon[2];


                        vinnyMeshDef.AddFace(GetPointCoords(p1), GetPointCoords(p2), GetPointCoords(p3));
                    }
                    else if (faceLoopPoly.Polygon.Count > 3)
                    {
                        //make delanau triangulation
                        var points = new List<DelaunayTriangulation.Point>();
                        foreach (var ifcPoint in faceLoopPoly.Polygon)
                        {
                            double[] ifcPointCoords = GetPointCoords(ifcPoint);
                            points.Add(new DelaunayTriangulation.Point(ifcPointCoords[0], ifcPointCoords[1], ifcPointCoords[2]));
                        }

                        try
                        {
                            // Perform Delaunay triangulation
                            var triangles = DelaunayTriangulation.Triangulate(points);

                            foreach (DelaunayTriangulation.Triangle? triangle in triangles)
                            {
                                vinnyMeshDef.AddFace(triangle.A.GetXYZ(), triangle.B.GetXYZ(), triangle.C.GetXYZ());
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error: {ex.Message}");
                        }
                    }
                }
            }
        }

        private void IdentifyLocalPlacement(IfcObjectPlacement placement, ref VinnyLibDataStructureGeometryPlacementInfo gpi)
        {
            IfcLocalPlacement? ifcLocalPlacement = placement as IfcLocalPlacement;

            if (ifcLocalPlacement == null) return;
            IfcAxis2Placement2D? ifcLocalPlacement2D = ifcLocalPlacement.RelativePlacement as IfcAxis2Placement2D;
            IfcAxis2Placement3D? ifcLocalPlacement3D = ifcLocalPlacement.RelativePlacement as IfcAxis2Placement3D;

            if (ifcLocalPlacement2D != null)
            {
                gpi.Position = new double[3] {
                    gpi.Position[0] + ifcLocalPlacement2D.Location.CoordinateX * this.mIfcFile.ScaleSI,
                    gpi.Position[1] + ifcLocalPlacement2D.Location.CoordinateY * this.mIfcFile.ScaleSI,
                     gpi.Position[2]
                };
                gpi.VectorOZ_Rad = 0; //TODO: get from direction
            }

            if (ifcLocalPlacement3D != null)
            {
                gpi.Position = new double[3] {
                    gpi.Position[0] + ifcLocalPlacement3D.Location.CoordinateX * this.mIfcFile.ScaleSI,
                    gpi.Position[1] + ifcLocalPlacement3D.Location.CoordinateY * this.mIfcFile.ScaleSI,
                    gpi.Position[2] + ifcLocalPlacement3D.Location.CoordinateZ * this.mIfcFile.ScaleSI
                };
                gpi.VectorOZ_Rad = 0; //TODO: get from direction
            }

            if (ifcLocalPlacement.PlacementRelTo != null) IdentifyLocalPlacement(ifcLocalPlacement.PlacementRelTo, ref gpi);
        }

        private Dictionary<int, List<int>> ifcShapeModels2VinnyMesh = new Dictionary<int, List<int>>();

        private int ProcessGeometry(IfcRepresentationItem ifcRepresentationItem, int vunnyMaterialId, IfcObjectPlacement ObjectPlacement)
        {
            IfcFaceBasedSurfaceModel? ifcFaceBasedSurfaceModel = ifcRepresentationItem as IfcFaceBasedSurfaceModel;
            IfcFacetedBrep? ifcFacetedBrep = ifcRepresentationItem as IfcFacetedBrep;
            IfcMappedItem? ifcMappedItem = ifcRepresentationItem as IfcMappedItem;


            if (ifcFaceBasedSurfaceModel != null || ifcFacetedBrep != null)
            {
                VinnyLibDataStructureGeometryMesh vinnyMeshDef = mVinnyModelDef.GeometrtyManager.GetMeshGeometryById(mVinnyModelDef.GeometrtyManager.CreateGeometry(VinnyLibDataStructureGeometryType.Mesh));
                vinnyMeshDef.MaterialId = vunnyMaterialId;

                if (ifcFaceBasedSurfaceModel != null)
                {
                    var facesSetCollection = ifcFaceBasedSurfaceModel.FbsmFaces;
                    foreach (IfcConnectedFaceSet facesSet in facesSetCollection)
                    {
                        SetMeshDataFrom(vinnyMeshDef, facesSet.CfsFaces);
                    }
                }
                else if (ifcFacetedBrep != null)
                {
                    SetMeshDataFrom(vinnyMeshDef, ifcFacetedBrep.Outer.CfsFaces);
                }

                mVinnyModelDef.GeometrtyManager.SetMeshGeometry(vinnyMeshDef.Id, vinnyMeshDef);

                int vinnyMeshPIid = mVinnyModelDef.GeometrtyManager.CreateGeometryPlacementInfo(vinnyMeshDef.Id);
                VinnyLibDataStructureGeometryPlacementInfo VinnyGPI = mVinnyModelDef.GeometrtyManager.GetGeometryPlacementInfoById(vinnyMeshPIid);
                IdentifyLocalPlacement(ObjectPlacement, ref VinnyGPI);
                mVinnyModelDef.GeometrtyManager.SetMeshGeometryPlacementInfo(vinnyMeshPIid, VinnyGPI);
                return vinnyMeshPIid;
            }

            return -1;
        }
        private void ProcessIfcObject(IfcObjectDefinition ifcObjectDef, int vinnyParentId)
        {
            VinnyLibDataStructureObject vinnyObjectDef = this.mVinnyModelDef.ObjectsManager.GetObjectById(this.mVinnyModelDef.ObjectsManager.CreateObject());
            vinnyObjectDef.Name = ifcObjectDef.Name;
            vinnyObjectDef.UniqueId = ifcObjectDef.Guid.ToString("N");
            vinnyObjectDef.Description = ifcObjectDef.Description;
            vinnyObjectDef.ParentId = vinnyParentId;

            IfcBuiltElement? ifcObjectDefasBuiltElement = ifcObjectDef as IfcBuiltElement;

            if (ifcObjectDefasBuiltElement != null)
            {
                ProcessIfcProperties(ifcObjectDefasBuiltElement, vinnyObjectDef);

                //geometry
                IfcProductDefinitionShape representation = ifcObjectDefasBuiltElement.Representation;
                if (representation != null)
                {
                    foreach (IfcShapeModel ifcShapeModel in representation.Representations)
                    {
                        int vunnyMaterialId = this.mVinnyModelDef.MaterialsManager.CreateMaterial(new int[] { 0, 0, 0, 0 });

                        IfcPresentationLayerAssignment ifcPresentationLayerAssignment = ifcShapeModel.LayerAssignment;
                        if (ifcPresentationLayerAssignment != null)
                        {
                            foreach (IfcLayeredItem? it in ifcPresentationLayerAssignment.AssignedItems)
                            {
                                if (it != null)
                                {
                                    IfcPresentationStyleAssignment? it2 = it as IfcPresentationStyleAssignment;
                                    if (it2 != null)
                                    {
                                        IfcStyledItem ifcStyle = it2.StyledItems.First();

                                        if (ifcStyle != null && ifcStyle.Styles.Any())
                                        {
                                            IfcSurfaceStyle? ifcStyleSurface = ifcStyle.Styles.First() as IfcSurfaceStyle;
                                            if (ifcStyleSurface != null && ifcStyleSurface.Styles.Any())
                                            {
                                                IfcSurfaceStyleShading? ifcSurfaceStyleShading = ifcStyleSurface.Styles.First() as IfcSurfaceStyleShading;
                                                if (ifcSurfaceStyleShading != null)
                                                {
                                                    vunnyMaterialId = this.mVinnyModelDef.MaterialsManager.CreateMaterial(new int[] {
                                            Convert.ToInt32(ifcSurfaceStyleShading.SurfaceColour.Red * 255.0),
                                            Convert.ToInt32(ifcSurfaceStyleShading.SurfaceColour.Green * 255.0),
                                            Convert.ToInt32(ifcSurfaceStyleShading.SurfaceColour.Blue * 255.0),
                                            Convert.ToInt32(ifcSurfaceStyleShading.Transparency * 100.0) });
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        

                        IfcShapeRepresentation? ifcShapeRepresentation = ifcShapeModel as IfcShapeRepresentation;
                        if (ifcShapeRepresentation != null)
                        {
                            foreach (IfcRepresentationItem ifcRepresentationItem in ifcShapeRepresentation.Items)
                            {
                                IfcMappedItem? ifcRepresentationItem_asMappedItem = ifcRepresentationItem as IfcMappedItem;
                                if (ifcRepresentationItem_asMappedItem != null)
                                {
                                    IfcShapeModel ifcShapeModel2 = ifcRepresentationItem_asMappedItem.MappingSource.MappedRepresentation;
                                    
                                    IfcShapeRepresentation? ifcShapeRepresentation2 = ifcShapeModel2 as IfcShapeRepresentation;
                                    if (ifcShapeRepresentation2 != null)
                                    {
                                        foreach (IfcRepresentationItem ifcRepresentationItem2 in ifcShapeRepresentation2.Items)
                                        {
                                            int gpi2 = ProcessGeometry(ifcRepresentationItem, vunnyMaterialId, ifcObjectDefasBuiltElement.ObjectPlacement);
                                            if (gpi2 != -1)
                                            {
                                                VinnyLibDataStructureGeometryPlacementInfo gpiDef = mVinnyModelDef.GeometrtyManager.GetGeometryPlacementInfoById(gpi2);
                                                gpiDef.Position = new double[3]
                                                {
                                                    gpiDef.Position[0] + ifcRepresentationItem_asMappedItem.MappingTarget.LocalOrigin.CoordinateX * mIfcFile.ScaleSI,
                                                     gpiDef.Position[1] + ifcRepresentationItem_asMappedItem.MappingTarget.LocalOrigin.CoordinateY * mIfcFile.ScaleSI,
                                                      gpiDef.Position[2] + ifcRepresentationItem_asMappedItem.MappingTarget.LocalOrigin.CoordinateZ * mIfcFile.ScaleSI,
                                                };

                                                mVinnyModelDef.GeometrtyManager.SetMeshGeometryPlacementInfo(gpi2, gpiDef);
                                                vinnyObjectDef.GeometryPlacementInfoIds.Add(gpi2);
                                            }
                                        }
                                    }
                                }
                                else vinnyObjectDef.GeometryPlacementInfoIds.Add(ProcessGeometry(ifcRepresentationItem, vunnyMaterialId, ifcObjectDefasBuiltElement.ObjectPlacement));

                            }
                        }
                    }
                }
            }

            //only spatial elements can contain building elements
            IfcSpatialStructureElement? spatialElement = ifcObjectDef as IfcSpatialStructureElement;
            if (spatialElement != null)
            {
                //using IfcRelContainedInSpatialElement to get contained elements
                var containedElements = spatialElement.ContainsElements.SelectMany(rel => rel.RelatedElements);
                foreach (var element in containedElements)
                {
                    ProcessIfcObject(element, vinnyObjectDef.Id);
                }
            }

            foreach (var item in ifcObjectDef.IsDecomposedBy.SelectMany(r => r.RelatedObjects))
            {
                ProcessIfcObject(item, vinnyObjectDef.Id);
            }

            this.mVinnyModelDef.ObjectsManager.SetObject(vinnyObjectDef.Id, vinnyObjectDef);
        }

        private double[] GetPointCoords(IfcCartesianPoint ifcPoint)
        {
            double[] xyz = new double[3] {
                ifcPoint .CoordinateX * this.mIfcFile.ScaleSI,
                ifcPoint .CoordinateY * this.mIfcFile.ScaleSI,
                ifcPoint .CoordinateZ * this.mIfcFile.ScaleSI
            };
            return xyz;
        }

        public VinnyLibDataStructureModel Import(ImportExportParameters openParameters)
        {
            mVinnyModelDef = new VinnyLibDataStructureModel();

            mIfcFile = new DatabaseIfc(openParameters.Path);

            IfcProject ifcProject = mIfcFile.Project;
            IfcSpatialElement ifcRootElement = ifcProject.RootElement();
            ProcessIfcObject(ifcRootElement, -1);



            mIfcFile.Dispose();

            return mVinnyModelDef;
        }

        public void Export(VinnyLibDataStructureModel vinnyData, ImportExportParameters outputParameters)
        {
            mVinnyModelDef = vinnyData;
            vinnyData.SetCoordinatesTransformation(outputParameters.TransformationInfo);

            mIfcFile = new DatabaseIfc(ModelView.Ifc4DesignTransfer);
            IfcBuilding building = new IfcBuilding(mIfcFile, "IfcBuilding") { };
            IfcProject project = new IfcProject(building, "IfcProject", IfcUnitAssignment.Length.Metre) { };

            //TODO: Реализовать сохранение геометри через IfcMappedItem

            VinnyLibDataStructureObjectsManager.StructureInfo[] vinnyModelStructureInfo = vinnyData.ObjectsManager.GetAllStructure();
            foreach (VinnyLibDataStructureObjectsManager.StructureInfo vinnyModelStructureGroupInfo in vinnyModelStructureInfo)
            {
                ProcessExportObject(vinnyModelStructureGroupInfo, ref nwcStructure);
            }




            mIfcFile.WriteFile(outputParameters.Path);
        }

        void ProcessExportObject(StructureInfo obj, ref LcNwcGroupWrapper parentObject)
        {
            VinnyLibDataStructureObject vinnyObj = mVinnyModelDef.ObjectsManager.GetObjectById(obj.Id);
        }

        private VinnyLibDataStructureModel mVinnyModelDef;
        private DatabaseIfc mIfcFile;
    }
}
