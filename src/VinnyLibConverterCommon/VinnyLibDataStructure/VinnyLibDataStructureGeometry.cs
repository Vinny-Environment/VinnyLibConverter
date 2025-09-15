using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace VinnyLibConverterCommon.VinnyLibDataStructure
{
    public enum VinnyLibDataStructureGeometryType : int
    {
        _Unknown = 0,
        Mesh = 1,
        Cad = 2,
        Brep = 3
    };

    [Serializable]
    [XmlInclude(typeof(VinnyLibDataStructureGeometryMesh))]
    public abstract class VinnyLibDataStructureGeometry
    {
        public virtual VinnyLibDataStructureGeometryType GetGeometryType()
        {
            return VinnyLibDataStructureGeometryType._Unknown;
        }

        /// <summary>
        /// Возвращает массив из 6 чисел (2 координаты -- минимальная и максимальная точка, соответствующие BBOX объекта в его локальных координатах)
        /// Xmin, Ymin, Zmin, Xmax, Ymax, Zmax
        /// </summary>
        /// <returns></returns>
        public virtual double[] ComputeBounds()
        {
            return new double[6];
        }

        /// <summary>
        /// Идентификатор материала, назначенного геометрии (если нет информации о соответствии материала грани). По умолчанию, черный
        /// </summary>
        public int MaterialId { get; set; } = 0;

        public int Id { get; set; }
    }
}
