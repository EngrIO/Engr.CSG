using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Engr.Geometry.Shapes;
using Engr.Maths.Vectors;

namespace Engr.CSG.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var triangle = new Triangle(Vect3.Zero, Vect3.Zero, Vect3.Zero);
        }
    }
}
