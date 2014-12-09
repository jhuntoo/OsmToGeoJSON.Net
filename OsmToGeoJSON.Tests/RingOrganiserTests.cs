using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace OsmToGeoJSON.Tests
{
    [TestFixture]
    public class RingOrganiserTests
    {
        [Test]
        public void if_2_ways_create_a_ring_then_join_them()
        {
            var ringOrganiser = new RingOrganiser();
            var c1 = new Coordinates {Lon = 1.0, Lat = 1.0};
            var c2 = new Coordinates {Lon = 2.0, Lat = 2.0};
            var c3 = new Coordinates {Lon = 1.0, Lat = 2.0};

            var way1 = new Way() {Geometry = new List<Coordinates> {c1, c2, c3}};
            var way2 = new Way() { Geometry = new List<Coordinates> { c3, c1 } };
            SetNodes(way1);
            SetNodes(way2);
            var joined = ringOrganiser.AssignToRings(new List<Way> {way1, way2});

            Assert.That(joined[0].Count == 4);
            Assert.That(NodeMatchesCoordinates(joined[0][0],c3));
            Assert.That(NodeMatchesCoordinates(joined[0][1], c1));
            Assert.That(NodeMatchesCoordinates(joined[0][2], c2));
            Assert.That(NodeMatchesCoordinates(joined[0][3], c3));

        }

        [Test]
        public void if_3_ways_create_a_ring_then_join_them()
        {
            var ringOrganiser = new RingOrganiser();
            var c1 = new Coordinates { Lon = 150.91094970703125, Lat = -23.26027250312138 };
            var c2 = new Coordinates { Lon = 150.86151123046875, Lat = -23.28928776494912 };
            var c3 = new Coordinates { Lon = 150.84228515625, Lat = -23.342255835130516 };

            var c4 = new Coordinates { Lon = 150.87799072265625, Lat = -23.26027250312138 };
            var c5 = new Coordinates { Lon = 150.9624481201172, Lat = -23.357385691338713 };
            var c6 = new Coordinates { Lon = 150.96656799316406, Lat = -23.295594594020272 };

            var way1 = new Way { Id = "1", Geometry = new List<Coordinates> {  c1, c2, c3 } };
            var way2 = new Way { Id = "2", Geometry = new List<Coordinates> { c3, c4, c5 } };
            var way3 = new Way { Id = "3", Geometry = new List<Coordinates> { c5, c6, c1 } };
            SetNodes(way1);
            SetNodes(way2);
            SetNodes(way3);
            var joined = ringOrganiser.AssignToRings(new List<Way> { way1, way2, way3 });

            Assert.That(joined[0].Count == 7);
            Assert.That(NodeMatchesCoordinates(joined[0][0], c5));
            Assert.That(NodeMatchesCoordinates(joined[0][1], c6));
            Assert.That(NodeMatchesCoordinates(joined[0][2], c1));
            Assert.That(NodeMatchesCoordinates(joined[0][3], c2));
            Assert.That(NodeMatchesCoordinates(joined[0][4], c3));
            Assert.That(NodeMatchesCoordinates(joined[0][5], c4));
            Assert.That(NodeMatchesCoordinates(joined[0][6], c5));

            var joined2 = ringOrganiser.AssignToRings(new List<Way> { way2, way3, way1 });

            Assert.That(NodeMatchesCoordinates(joined2[0][0], c1));
            Assert.That(NodeMatchesCoordinates(joined2[0][1], c2));
            Assert.That(NodeMatchesCoordinates(joined2[0][2], c3));
            Assert.That(NodeMatchesCoordinates(joined2[0][3], c4));
            Assert.That(NodeMatchesCoordinates(joined2[0][4], c5));
            Assert.That(NodeMatchesCoordinates(joined2[0][5], c6));
            Assert.That(NodeMatchesCoordinates(joined2[0][6], c1));

        }

        [Test]
        public void if_3_ways_with_1_reversed_create_a_ring_then_join_them()
        {
            var ringOrganiser = new RingOrganiser();
            var c1 = new Coordinates { Lon = 150.91094970703125, Lat = -23.26027250312138 };
            var c2 = new Coordinates { Lon = 150.86151123046875, Lat = -23.28928776494912 };
            var c3 = new Coordinates { Lon = 150.84228515625, Lat = -23.342255835130516 };

            var c4 = new Coordinates { Lon = 150.87799072265625, Lat = -23.26027250312138 };
            var c5 = new Coordinates { Lon = 150.9624481201172, Lat = -23.357385691338713 };
            var c6 = new Coordinates { Lon = 150.96656799316406, Lat = -23.295594594020272 };

            var way1 = new Way { Id = "1", Geometry = new List<Coordinates> { c1, c2, c3 } };
            var way2 = new Way { Id = "2", Geometry = new List<Coordinates> { c5, c4, c3 } };
            var way3 = new Way { Id = "3", Geometry = new List<Coordinates> { c5, c6, c1 } };
            SetNodes(way1);
            SetNodes(way2);
            SetNodes(way3);
            var joined = ringOrganiser.AssignToRings(new List<Way> { way1, way2, way3 });

            Assert.That(joined[0].Count == 7);
            Assert.That(NodeMatchesCoordinates(joined[0][0], c5));
            Assert.That(NodeMatchesCoordinates(joined[0][1], c6));
            Assert.That(NodeMatchesCoordinates(joined[0][2], c1));
            Assert.That(NodeMatchesCoordinates(joined[0][3], c2));
            Assert.That(NodeMatchesCoordinates(joined[0][4], c3));
            Assert.That(NodeMatchesCoordinates(joined[0][5], c4));
            Assert.That(NodeMatchesCoordinates(joined[0][6], c5));

        }

        [Test]
        public void if_3_ways_with_1_reversed__create_a_ring_then_join_them()
        {
            // tests when the last node of of way, matches the last node of another way
            var ringOrganiser = new RingOrganiser();
            var c1 = new Coordinates { Lon = 150.91094970703125, Lat = -23.26027250312138 };
            var c2 = new Coordinates { Lon = 150.86151123046875, Lat = -23.28928776494912 };
            var c3 = new Coordinates { Lon = 150.84228515625, Lat = -23.342255835130516 };
            var c4 = new Coordinates { Lon = 150.87799072265625, Lat = -23.26027250312138 };
            var c5 = new Coordinates { Lon = 150.9624481201172, Lat = -23.357385691338713 };
            var c6 = new Coordinates { Lon = 150.96656799316406, Lat = -23.295594594020272 };

            var way1 = new Way { Id = "1", Geometry = new List<Coordinates> { c3, c2, c1 } };
            var way2 = new Way { Id = "2", Geometry = new List<Coordinates> { c5, c4, c3 } };
            var way3 = new Way { Id = "3", Geometry = new List<Coordinates> { c5, c6, c1 } };
            SetNodes(way1);
            SetNodes(way2);
            SetNodes(way3);
            var joined = ringOrganiser.AssignToRings(new List<Way> { way1, way2, way3 });

            Assert.That(joined[0].Count == 7);
            Assert.That(NodeMatchesCoordinates(joined[0][0], c5));
            Assert.That(NodeMatchesCoordinates(joined[0][1], c6));
            Assert.That(NodeMatchesCoordinates(joined[0][2], c1));
            Assert.That(NodeMatchesCoordinates(joined[0][3], c2));
            Assert.That(NodeMatchesCoordinates(joined[0][4], c3));
            Assert.That(NodeMatchesCoordinates(joined[0][5], c4));
            Assert.That(NodeMatchesCoordinates(joined[0][6], c5));

        }

        [Test]
        public void if_3_ways_with_1_reversed_Mathing_first_last__create_a_ring_then_join_them()
        {
            // tests when the last node of of way, matches the last node of another way
            var ringOrganiser = new RingOrganiser();
            var c1 = new Coordinates { Lon = 150.91094970703125, Lat = -23.26027250312138 };
            var c2 = new Coordinates { Lon = 150.86151123046875, Lat = -23.28928776494912 };
            var c3 = new Coordinates { Lon = 150.84228515625, Lat = -23.342255835130516 };
            var c4 = new Coordinates { Lon = 150.87799072265625, Lat = -23.26027250312138 };
            var c5 = new Coordinates { Lon = 150.9624481201172, Lat = -23.357385691338713 };
            var c6 = new Coordinates { Lon = 150.96656799316406, Lat = -23.295594594020272 };

            var way1 = new Way { Id = "1", Geometry = new List<Coordinates> { c3, c2, c1 } };
            var way2 = new Way { Id = "2", Geometry = new List<Coordinates> { c3, c4, c5 } };
            var way3 = new Way { Id = "3", Geometry = new List<Coordinates> { c5, c6, c1 } };
            SetNodes(way1);
            SetNodes(way2);
            SetNodes(way3);
            var joined = ringOrganiser.AssignToRings(new List<Way> { way1, way2, way3 });

            Assert.That(joined[0].Count == 7);
            Assert.That(NodeMatchesCoordinates(joined[0][0], c5));
            Assert.That(NodeMatchesCoordinates(joined[0][1], c6));
            Assert.That(NodeMatchesCoordinates(joined[0][2], c1));
            Assert.That(NodeMatchesCoordinates(joined[0][3], c2));
            Assert.That(NodeMatchesCoordinates(joined[0][4], c3));
            Assert.That(NodeMatchesCoordinates(joined[0][5], c4));
            Assert.That(NodeMatchesCoordinates(joined[0][6], c5));

        }

        [Test]
        public void reversed()
        {
            // tests when the last node of of way, matches the last node of another way
            var ringOrganiser = new RingOrganiser();
            var c1 = new Coordinates { Lon = 150.91094970703125, Lat = -23.26027250312138 };
            var c2 = new Coordinates { Lon = 150.86151123046875, Lat = -23.28928776494912 };
            var c3 = new Coordinates { Lon = 150.84228515625, Lat = -23.342255835130516 };
            var c4 = new Coordinates { Lon = 150.87799072265625, Lat = -23.26027250312138 };
            var c5 = new Coordinates { Lon = 150.9624481201172, Lat = -23.357385691338713 };
            var c6 = new Coordinates { Lon = 150.96656799316406, Lat = -23.295594594020272 };

            var way1 = new Way { Id = "1", Geometry = new List<Coordinates> { c3, c2, c1 } };
            var way2 = new Way { Id = "2", Geometry = new List<Coordinates> { c3, c4, c5 } };
            var way3 = new Way { Id = "3", Geometry = new List<Coordinates> { c5, c6, c1 } };
            SetNodes(way1);
            SetNodes(way2);
            SetNodes(way3);
            var joined = ringOrganiser.AssignToRings(new List<Way> { way1, way2, way3 });

            Assert.That(joined[0].Count == 7);
            Assert.That(NodeMatchesCoordinates(joined[0][0], c5));
            Assert.That(NodeMatchesCoordinates(joined[0][1], c6));
            Assert.That(NodeMatchesCoordinates(joined[0][2], c1));
            Assert.That(NodeMatchesCoordinates(joined[0][3], c2));
            Assert.That(NodeMatchesCoordinates(joined[0][4], c3));
            Assert.That(NodeMatchesCoordinates(joined[0][5], c4));
            Assert.That(NodeMatchesCoordinates(joined[0][6], c5));

        }

        private bool NodeMatchesCoordinates(Node node, Coordinates c)
        {
            return (node.Lat == c.Lat && node.Lon == c.Lon);
        }

        private void SetNodes(Way way)
        {
            way.ResolvedNodes = way.Geometry.Select(r =>
            {
                return new Node {Id = string.Format("{0}/{1}", r.Lon, r.Lat), Lon = r.Lon, Lat = r.Lat};

            }).ToDictionary(x => x.Id);

            way.Nodes = way.ResolvedNodes.Keys.ToList();
        }
    }
}