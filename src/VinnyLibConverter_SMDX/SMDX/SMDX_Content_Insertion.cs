using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinnyLibConverter_SMDX.SMDX
{
    /// <summary>
    /// Класс для описания расположения элемента в пространстве
    /// </summary>
    public class SMDX_Content_Insertion
    {
        /// <summary>
        /// номер элемента из коллекции SMDX_Content.geometry
        /// </summary>
        public int geometry { get; set; }

        /// <summary>
        /// номер элемента из коллекции SMDX_Content.groups
        /// </summary>
        public int group { get; set; }

        /// <summary>
        /// Приращения координат к точке вставки SMDX_Content.wcs ИЛИ какая-то херь с frames
        /// </summary>
        public object? position { get; set; }

        /// <summary>
        /// массив значений масштаба по осям XYZ. По умолчанию = 1 1 1
        /// </summary>
        public object? scale { get; set; }

        /// <summary>
        /// Угол поворота элемента относительно нормали в радианах. По умолчанию = 0
        /// </summary>
        public float angle { get; set; } = 0.0f;

        /// <summary>
        /// Массив координат XYZ вектора к плоскости, в которой расположен элемент. По умолчанию 0 0 1 ИЛИ какая-то херь с frames
        /// </summary>
        public object? normal { get; set; }
    }
}
