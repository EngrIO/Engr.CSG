using Engr.Geometry.Datums;
using Engr.Geometry.Primatives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engr.CSG
{
    internal class CSGNode
    {
        private List<IPolygon> _polygons;
        private CSGNode _front;
		private CSGNode _back;
        private IPlane _plane;

        public CSGNode()
        {
        }

        public CSGNode(IEnumerable<IPolygon> polygons)
        {
            _polygons = polygons.ToList();
        }

        public CSGNode(List<IPolygon> list, IPlane plane, CSGNode front, CSGNode back)
        {
            _polygons = list;
            _plane = plane;
            _front = front;
            _back = back;
        }

        public void ClipTo(CSGNode other)
        {
            _polygons = other.ClipPolygons(_polygons);
            if (_front != null) _front.ClipTo(other);
            if (_back != null) _back.ClipTo(other);
        }

        public void Invert()
        {
            _polygons = _polygons.Select(p => p.Flipped()).ToList();
            _plane = _plane.Flipped();

            if (_front != null) _front.Invert();
            if (_back != null) _back.Invert();

            var temp = _front;
            _front = _back;
            _back = temp;
        }

        private void Build(List<IPolygon> list)
        {
            if (!list.Any()) throw new Exception();

            if (_plane == null || !(_plane.Normal.Length > 0))
            {
                //TODO Check valid is correct
                _plane = new Plane(list.First().Plane.Normal, list.First().Plane.Constant);
            }
            
            if (_polygons == null) _polygons = new List<IPolygon>();

            var listFront = new List<IPolygon>();
            var listBack = new List<IPolygon>();

            for (int i = 0; i < list.Count; i++)
            {
                _plane.SplitPolygon(list[i], _polygons, _polygons, listFront, listBack);
            }

            if (listFront.Count > 0)
            {
                if (_front == null) _front = new CSGNode();
                _front.Build(listFront);
            }

            if (listBack.Count > 0)
            {
                if (_back == null) _back = new CSGNode();
                _back.Build(listBack);
            }
        }

        public List<IPolygon> ClipPolygons(List<IPolygon> list)
        {
            if(!(_plane.Normal.Length > 0))
            {
                return list;
            }

            var listFront = new List<IPolygon>();
            var listBack = new List<IPolygon>();

            for (int i = 0; i < list.Count; i++)
            {
                _plane.SplitPolygon(list[i], listFront, listBack, listFront, listBack);
            }

            if (_front != null)
            {
                listFront = _front.ClipPolygons(listFront);
            }

            if (_back != null)
            {
                listBack = _back.ClipPolygons(listBack);
            }
            else
            {
                listBack.Clear();
            }
            listFront.AddRange(listBack);
            return listFront;
        }

        private List<IPolygon> AllPolygons()
        {
            var list = new List<IPolygon>(_polygons);
            if (_front != null) list.AddRange(_front.AllPolygons());
            if (_back != null) list.AddRange(_back.AllPolygons());
            return list;
        }

        private CSGNode Clone()
        {
            //TODO this probably doesnt clone correctly
            return new CSGNode(_polygons, _plane, _front, _back);
        }

        public CSGNode Union(CSGNode other)
        {
            var a = Clone();
            var b = other.Clone();
            a.ClipTo(b);
            b.ClipTo(a);
            b.Invert();
            b.ClipTo(a);
            b.Invert();
            a.Build(b.AllPolygons());
            return new CSGNode(a.AllPolygons());
        }

        public CSGNode Subtract(CSGNode other)
        {
            var a = Clone();
            var b = other.Clone();
            a.Invert();
            a.ClipTo(b);
            b.ClipTo(a);
            b.Invert();
            b.ClipTo(a);
            b.Invert();
            a.Build(b.AllPolygons());
            a.Invert();
            return new CSGNode(a.AllPolygons());
        }

        public CSGNode Intersect(CSGNode other)
        {
            var a = Clone();
            var b = other.Clone();
            a.Invert();
            b.ClipTo(a);
            b.Invert();
            a.ClipTo(b);
            b.ClipTo(a);
            a.Build(b.AllPolygons());
            a.Invert();
            return new CSGNode(a.AllPolygons());
        }

        public IEnumerable<IPolygon> ToPolygons()
        {
            return AllPolygons();
        }
    }
}
