using System.Collections.Generic;
using NUnit.Framework;
using OsmToGeoJSON.Util;

namespace OsmToGeoJSON.Tests
{
    [TestFixture]
    public class GeometryTests
    {
        [Test]
        public void polygon_completely_inside_other_polygon_should_intersect()
        {
            var outers = new List<Node>
            {
                new Node {Lat = -1.0, Lon = -1.0},
                new Node {Lat = -1.0, Lon = 1.0},
                new Node {Lat = 1.0, Lon = -1.0},
                new Node {Lat = 1.0, Lon = -1.0},
                new Node {Lat = -1.0, Lon = -1.0}
            };

            var inners = new List<Node>
            {
                new Node {Lat = 0.1, Lon = -0.1},
                new Node {Lat = -0.1, Lon = -0.1},
                new Node {Lat = 0.0, Lon = -0.02},
                new Node {Lat = 0.1, Lon = -0.1}
            };
            Assert.IsTrue(Geometry.PolygonIntersectsPolygon(outers, inners));
        }
        [Test]
        public void polygon_outside_other_polygon_should_not_intersect()
        {
            var newZealandBox = new List<Node>
            {
                new Node {Lat = -48.92249926375824, Lon = 165.234375},
                new Node {Lat = -32.8426736319543, Lon = 165.234375},
                new Node {Lat = -32.8426736319543, Lon = 181.05468749999997},
                new Node {Lat = -48.92249926375824, Lon = 181.05468749999997},
                new Node {Lat = -48.92249926375824, Lon =  165.234375}
            };

            var autraliaBox = new List<Node>
             {   new Node {Lat = -44.33956524809713, Lon = 112.1484375},
                new Node {Lat = -9.79567758282973, Lon = 112.1484375},
                new Node {Lat = -9.79567758282973, Lon = 155.7421875},
                new Node {Lat = -44.33956524809713, Lon = 155.7421875},
                new Node {Lat = -44.33956524809713, Lon = 112.1484375}
            };
            Assert.IsFalse(Geometry.PolygonIntersectsPolygon(newZealandBox, autraliaBox));
        }
        
    }
}