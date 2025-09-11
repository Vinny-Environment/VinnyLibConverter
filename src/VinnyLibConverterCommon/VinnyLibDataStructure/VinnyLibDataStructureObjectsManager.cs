using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace VinnyLibConverterCommon.VinnyLibDataStructure
{
    
    public class VinnyLibDataStructureObjectsManager
    {
        public class StructureInfo
        {
            public StructureInfo()
            {
                Childs = new List<StructureInfo>();
            }
            public int Id { get; set; }
            public int ParentId { get; set; }
            public List<StructureInfo> Childs { get; set; }
        }
        public VinnyLibDataStructureObjectsManager()
        {
            mObjectIdCounter = 0;
            mObjects = new Dictionary<int, VinnyLibDataStructureObject>();
        }

        public int CreateObject()
        {
            VinnyLibDataStructureObject objectDef = new VinnyLibDataStructureObject(mObjectIdCounter);
            mObjects.Add(mObjectIdCounter, objectDef);
            mObjectIdCounter++;
            return mObjectIdCounter - 1;
        }

        public void SetObject(int id, VinnyLibDataStructureObject objDef)
        {
            mObjects[id] = objDef;
        }

        public VinnyLibDataStructureObject GetObjectById(int id)
        {
            VinnyLibDataStructureObject outputObjectDef = new VinnyLibDataStructureObject();
            if (mObjects.TryGetValue(id, out outputObjectDef)) return outputObjectDef;
            return null;
        }

        public VinnyLibDataStructureObject[] GetObjectsChilds(int ObjectId)
        {
            List<VinnyLibDataStructureObject> ObjectsTmp = new List<VinnyLibDataStructureObject>();
            foreach (var objData in mObjects)
            {
                if (objData.Value.ParentId == ObjectId) ObjectsTmp.Add(objData.Value);
            }
            return ObjectsTmp.ToArray();
        }

        public VinnyLibDataStructureObject GetParentObject(int ObjectId)
        {
            var obj = GetObjectById(ObjectId);
            if (obj == null) return null;

            foreach (var objData in mObjects)
            {
                if (objData.Value.Id == obj.ParentId) return objData.Value;
            }
            return null;
        }

        /// <summary>
        /// Возвращает все корневые объекты структуры.
        /// </summary>
        /// <returns></returns>
        public VinnyLibDataStructureObject[] GetRootObjects()
        {
            List<VinnyLibDataStructureObject> root = new List<VinnyLibDataStructureObject>();
            foreach (var objData in mObjects)
            {
                if (objData.Value.ParentId == -1) root.Add(objData.Value);
            }
            return root.ToArray();
        }

        /// <summary>
        /// Возвращает дерево структуры для данного объекта
        /// </summary>
        /// <param name="idObject"></param>
        /// <returns></returns>
        public StructureInfo GetStructure(int idObject)
        {
            VinnyLibDataStructureObject obj = mObjects[idObject];
            StructureInfo sInfo = new StructureInfo();
            sInfo.Id = obj.Id;
            sInfo.ParentId = obj.ParentId;

            foreach (var childObject in GetObjectsChilds(obj.Id))
            {
                sInfo.Childs.Add(GetStructure(childObject.Id));
            }

            return sInfo;
        }


        public StructureInfo[] GetAllStructure()
        {
            List<StructureInfo> info = new List<StructureInfo>();
            foreach (var objData in mObjects)
            {
                if (objData.Value.ParentId == -1) info.Add(GetStructure(objData.Value.Id));
            }
            return info.ToArray();  
        }


        [XmlIgnore]
        public Dictionary<int, VinnyLibDataStructureObject> mObjects { get; set; }

        [XmlArray("Objects")]
        public List<VinnyLibDataStructureObject> ObjectsForXML { get; set; }

        private int mObjectIdCounter;
    }

}
