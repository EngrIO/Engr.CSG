using Engr.Geometry.Primatives;
using System.Collections.Generic;

namespace Engr.CSG
{
    public static class CSG
    {
        public const float Epsilon = 0.00001f;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aPoly"></param>
        /// <param name="bPoly"></param>
        /// <returns></returns>
        /// <remarks>
        ///     +-------+            +-------+
        ///     |       |            |       |
        ///     |   A   |            |       |
        ///     |    +--+----+   =   |       +----+
        ///     +----+--+    |       +----+       |
        ///          |   B   |            |       |
        ///          |       |            |       |
        ///          +-------+            +-------+
        /// </remarks>
        public static IEnumerable<IPolygon> Union(IEnumerable<IPolygon> aPoly, IEnumerable<IPolygon> bPoly)
        {
            var a = new CSGNode(aPoly);
            var b = new CSGNode(aPoly);
            return a.Union(b).ToPolygons();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aPoly"></param>
        /// <param name="bPoly"></param>
        /// <returns></returns>
        /// <remarks>
        ///     +-------+            +-------+
        ///     |       |            |       |
        ///     |   A   |            |       |
        ///     |    +--+----+   =   |    +--+
        ///     +----+--+    |       +----+
        ///          |   B   |
        ///          |       |
        ///          +-------+
        /// </remarks>
        public static IEnumerable<IPolygon> Subtract(IEnumerable<IPolygon> aPoly, IEnumerable<IPolygon> bPoly)
        {
            var a = new CSGNode(aPoly);
            var b = new CSGNode(aPoly);
            return a.Subtract(b).ToPolygons();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aPoly"></param>
        /// <param name="bPoly"></param>
        /// <returns></returns>
        /// <remarks>
        ///     +-------+
        ///     |       |
        ///     |   A   |
        ///     |    +--+----+   =   +--+
        ///     +----+--+    |       +--+
        ///          |   B   |
        //           |       |
        ///          +-------+
        /// </remarks>
        public static IEnumerable<IPolygon> Intersect(IEnumerable<IPolygon> aPoly, IEnumerable<IPolygon> bPoly)
        {
            var a = new CSGNode(aPoly);
            var b = new CSGNode(aPoly);
            return a.Intersect(b).ToPolygons();
        }
    }
}
