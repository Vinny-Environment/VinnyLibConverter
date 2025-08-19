using System;
using System.Collections.Generic;
using System.Text;

namespace VinnyLibConverterCommon.VinnyLibDataStructure
{
    public class VinnyLibDataStructureObjectsManager
    {
        public VinnyLibDataStructureObjectsManager()
        {
            mObjectIdCounter = 0;
            Objects = new Dictionary<int, VinnyLibDataStructureObject>();
        }

        public int CreateObject()
        {
            VinnyLibDataStructureObject objectDef = new VinnyLibDataStructureObject(mObjectIdCounter);
            Objects.Add(mObjectIdCounter, objectDef);
            mObjectIdCounter++;
            return mObjectIdCounter - 1;
        }

        public void SetObject(int id, VinnyLibDataStructureObject objDef)
        {
            Objects[id] = objDef;
        }

        public VinnyLibDataStructureObject GetObjectById(int id)
        {
            VinnyLibDataStructureObject outputObjectDef = new VinnyLibDataStructureObject();
            if (Objects.TryGetValue(id, out outputObjectDef)) return outputObjectDef;
            return null;
        }

        public VinnyLibDataStructureObject[] GetObjectsChilds(int ObjectId)
        {
            List<VinnyLibDataStructureObject> ObjectsTmp = new List<VinnyLibDataStructureObject>();
            foreach (var objData in Objects)
            {
                if (objData.Value.ParentId == ObjectId) ObjectsTmp.Add(objData.Value);
            }
            return ObjectsTmp.ToArray();
        }

        public VinnyLibDataStructureObject GetParentObject(int ObjectId)
        {
            var obj = GetObjectById(ObjectId);
            if (obj == null) return null;

            foreach (var objData in Objects)
            {
                if (objData.Value.Id == obj.ParentId) return objData.Value;
            }
            return null;
        }

        public Dictionary<int, VinnyLibDataStructureObject> Objects { get; internal set; }
        private int mObjectIdCounter;
    }
}
