using Engr.Geometry.Datums;
using Engr.Geometry.Primatives;
using Engr.Maths.Vectors;
using System;
using System.Collections.Generic;

namespace Engr.CSG
{
    [Flags]
    enum PolygonType
    {
        Coplanar = 0,
        Front = 1,
        Back = 2,
        Spanning = 3        /// 3 is Front | Back - not a separate entry
    };

    public static class Extensions
    {
        public static IEnumerable<IPolygon> Union(this IEnumerable<IPolygon> aPoly, IEnumerable<IPolygon> bPoly)
        {
            return CSG.Union(aPoly, bPoly);
        }

        public static IEnumerable<IPolygon> Subtract(this IEnumerable<IPolygon> aPoly, IEnumerable<IPolygon> bPoly)
        {
            return CSG.Subtract(aPoly, bPoly);
        }

        public static IEnumerable<IPolygon> Intersect(this IEnumerable<IPolygon> aPoly, IEnumerable<IPolygon> bPoly)
        {
            return CSG.Intersect(aPoly, bPoly);
        }

        public static void SplitPolygon(this IPlane plane, IPolygon polygon, List<IPolygon> coplanarFront, List<IPolygon> coplanarBack, List<IPolygon> front, List<IPolygon> back)
        {
            PolygonType polygonType = 0;
            var types = new List<PolygonType>();
            for (int i = 0; i < polygon.Points.Count; i++)
            {
                var t = plane.Normal.DotProduct(polygon.Points[i]) - plane.Constant;
                var type = (t < -CSG.Epsilon) ? PolygonType.Back : ((t > CSG.Epsilon) ? PolygonType.Front : PolygonType.Coplanar);
                polygonType |= type;
                types.Add(type);
            }
            switch (polygonType)
            {
                case PolygonType.Coplanar:
                    if (plane.Normal.DotProduct(polygon.Plane.Normal) > 0)
                    {
                        coplanarFront.Add(polygon);
                    }
                    else
                    {
                        coplanarBack.Add(polygon);
                    }
                    break;
                case PolygonType.Front:
                    front.Add(polygon);
                    break;
                case PolygonType.Back:
                    back.Add(polygon);
                    break;
                case PolygonType.Spanning:
                    var f = new List<Vect3>();
                    var b = new List<Vect3>();
                    for (int i = 0; i < polygon.Points.Count; i++)
                    {
                        int j = (i + 1) % polygon.Points.Count;

                        PolygonType ti = types[i], tj = types[j];

                        var vi = polygon.Points[i];
                        var vj = polygon.Points[j];

                        if (ti != PolygonType.Back)
                        {
                            f.Add(vi);
                        }

                        if (ti != PolygonType.Front)
                        {
                            b.Add(vi);
                        }

                        if ((ti | tj) == PolygonType.Spanning)
                        {
                            var t = (plane.Constant - plane.Normal.DotProduct(vi)) / plane.Normal.DotProduct( vj - vi);
                            var v = vi.Lerp(vj, t);
                            f.Add(v);
                            b.Add(v);
                        }
                    }

                    if (f.Count >= 3) front.Add(new Polygon(f));
                    if (b.Count >= 3) back.Add(new Polygon(b));
                    break;
            }
        }
    }
}
