using System.Collections.Generic;
using GeoJSON.Net;
using GeoJSON.Net.Geometry;
using NUnit.Framework;

namespace OsmToGeoJSON.Tests
{
    [TestFixture]
    public class RelationTests : GeometryTestBase
    {
        [Test]
        public void empty_relations_should_not_render_anything()
        {
            var json = " {\n elements: [\n {\n type: \"relation\",\n id: 1,\n tags: { \"type\": \"multipolygon\" }\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 0);

        }

        [Test]
        public void when_a_toplevel_relation_is_not_tagged_as_multipolygon_its_child_relation_should_be_rendered_and_any_members_of_the_toplevel_relation()
        {
            var json = "{\n elements: [\n {\n type: \"way\",\n id: 1,\n tags: { \"foo\": \"bar\" },\n nodes: [1, 2, 3]\n },\n {\n type: \"way\",\n id: 2,\n nodes: [3, 1]\n },\n {\n type: \"node\",\n id: 1,\n lat: 1.0,\n lon: 1.0\n },\n {\n type: \"node\",\n id: 2,\n lat: 2.0,\n lon: 2.0\n },\n {\n type: \"node\",\n id: 3,\n lat: 1.0,\n lon: 2.0\n },\n {\n type: \"relation\",\n id: 1,\n tags: { \"foo\": \"bar\" },\n members: [\n {\n type: \"way\",\n ref: 1,\n role: \"asd\"\n },\n {\n type: \"node\",\n ref: 1,\n role: \"fasd\"\n },\n {\n type: \"relation\",\n ref: 2,\n role: \"\"\n }\n ]\n },\n {\n type: \"relation\",\n id: 2,\n tags: { \"type\": \"multipolygon\" },\n members: [\n {\n type: \"way\",\n ref: 1,\n role: \"outer\"\n },\n {\n type: \"way\",\n ref: 2,\n role: \"outer\"\n }\n ]\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 3);

            var relationFeature = featureCollection.Features[0];
            Assert.That(relationFeature.Type == GeoJSONObjectType.Feature);
            Assert.That(relationFeature.Id == "relation/2");
            AssertPropertiesAreCorrect(relationFeature, type: "relation", id: "2", tags: new Dictionary<string, object> { { "type", "multipolygon" } }, relations: new[] { new TestRelation { Rel = 1, Role = "", RelTags = new Tags { { "foo", "bar" } } } });
            AssertPolygon(relationFeature, 
                new Coordinates { Lon = 2.0, Lat = 1.0 },
                new Coordinates { Lon = 1.0, Lat = 1.0 },
                new Coordinates { Lon = 2.0, Lat = 2.0 },
                new Coordinates { Lon = 2.0, Lat = 1.0 });

            var way1Feature = featureCollection.Features[1];
            Assert.That(way1Feature.Type == GeoJSONObjectType.Feature);
            Assert.That(way1Feature.Id == "way/1");
            AssertPropertiesAreCorrect(way1Feature, type: "way", id: "1", tags: new Dictionary<string, object> { { "foo", "bar" } }, relations: new[] { new TestRelation { Rel = 1, Role = "asd", RelTags = new Tags { { "foo", "bar" } } }, new TestRelation { Rel = 2, Role = "outer", RelTags = new Tags { { "type", "multipolygon" } } } });
            AssertFeatureLineString(way1Feature, new List<Coordinates>() {
                new Coordinates { Lon = 1.0, Lat = 1.0 },
                new Coordinates { Lon = 2.0, Lat = 2.0 },
                new Coordinates { Lon = 2.0, Lat = 1.0 }});

            var node1Feature = featureCollection.Features[2];
            Assert.That(node1Feature.Type == GeoJSONObjectType.Feature);
            Assert.That(node1Feature.Id == "node/1");
            AssertPropertiesAreCorrect(node1Feature, type: "node", id: "1", tags: new Dictionary<string, object>(), relations: new[] { new TestRelation { Rel = 1, Role = "fasd", RelTags = new Tags { { "foo", "bar" } } } });
            AssertFeaturePoint(node1Feature, 1.0, 1.0);
        }

        [Test]
        public void when_a_relation_has_invalid_ways_then_only_valid_ways_are_rendered()
        {
            var json = "{\n elements: [\n {\n type: \"way\",\n id: 10,\n nodes: [2, 3, 5]\n },\n {\n type: \"way\",\n id: 11,\n nodes: [2, 3, 4, 5, 2],\n tags: { \"area\": \"yes\" }\n },\n {\n type: \"way\",\n id: 12,\n nodes: [2, 3, 4, 2],\n },\n {\n type: \"relation\",\n id: 100,\n tags: { \"type\": \"multipolygon\" },\n members: [\n {\n type: \"way\",\n ref: 12,\n role: \"outer\"\n },\n {\n type: \"way\",\n ref: 13,\n role: \"inner\"\n }\n ]\n },\n {\n type: \"node\",\n id: 2,\n lat: 1.0,\n lon: 0.0\n },\n {\n type: \"node\",\n id: 3,\n lat: 0.0,\n lon: 1.0\n },\n {\n type: \"node\",\n id: 4,\n lat: 1.0,\n lon: 1.0\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 3);

            var way10Feature = featureCollection.Features[1];
            Assert.That(way10Feature.Type == GeoJSONObjectType.Feature);
            Assert.That(way10Feature.Id == "way/10");
            AssertPropertiesAreCorrect(way10Feature, type: "way", id: "10", isTainted: true);
            AssertFeatureLineString(way10Feature, new List<Coordinates>() {
                new Coordinates { Lon = 0.0, Lat = 1.0 },
                new Coordinates { Lon = 1.0, Lat =0.0 }});

            var way11Feature = featureCollection.Features[2];
            Assert.That(way11Feature.Type == GeoJSONObjectType.Feature);
            Assert.That(way11Feature.Id == "way/11");
            AssertPropertiesAreCorrect(way11Feature, type: "way", id: "11", isTainted: true, tags: new Dictionary<string, object> { { "area", "yes" } });
            AssertFeaturePolygon(way11Feature,  new List<List<Coordinates>>
                    { new List<Coordinates>
                    {
                        new Coordinates { Lon = 0.0, Lat = 1.0},
                        new Coordinates { Lon = 1.0, Lat =0.0},
                        new Coordinates { Lon = 1.0, Lat =1.0 },
                        new Coordinates { Lon = 0.0, Lat =1.0}
                    }});

            var way12Feature = featureCollection.Features[0];
            Assert.That(way12Feature.Type == GeoJSONObjectType.Feature);
            Assert.That(way12Feature.Id == "way/12");
            AssertPropertiesAreCorrect(way12Feature, type: "way", id: "12", relations: new[] { new TestRelation { Rel = 100, Role = "outer", RelTags = new Tags { { "type", "multipolygon" } } } });
            AssertPolygon(way12Feature,  new Coordinates[]
                    
                    {
                        new Coordinates { Lon = 0.0, Lat = 1.0},
                        new Coordinates { Lon = 1.0, Lat =0.0},
                        new Coordinates { Lon = 1.0, Lat =1.0 },
                        new Coordinates { Lon = 0.0, Lat =1.0}
                    });

        }

        [Test]
        public void when_a_multipoygon_relation_is_missing_a_way_then_is_tainted_is_true_but_valid_ways_are_still_rendered()
        {
            var json ="{\n elements: [\n {\n type: \"relation\",\n tags: { \"type\": \"multipolygon\" },\n id: 1,\n members: [\n {\n type: \"way\",\n ref: 2,\n role: \"outer\"\n },\n {\n type: \"way\",\n ref: 3,\n role: \"outer\"\n }\n ]\n },\n {\n type: \"way\",\n id: 2,\n nodes: [4, 5, 6, 7, 4]\n },\n {\n type: \"way\",\n id: 3,\n nodes: [4, 5, 6, 4]\n },\n {\n type: \"node\",\n id: 4,\n lat: 0.0,\n lon: 0.0\n },\n {\n type: \"node\",\n id: 5,\n lat: 0.0,\n lon: 1.0\n },\n {\n type: \"node\",\n id: 6,\n lat: 1.0,\n lon: 0.0\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 1);

            var way = featureCollection.Features[0];
            Assert.That(way.Properties["id"].ToString() == "1");
            Assert.That((bool)way.Properties["tainted"] == true);
        }

        [Test]
        public void when_a_multipoygon_relation_has_a_way_with_missing_a_node_then_relation_is_rendered_with_out_the_way_and_tainted_is_true()
        {
            var json = "{\n elements: [\n {\n type: \"relation\",\n tags: { \"type\": \"multipolygon\" },\n id: 1,\n members: [\n {\n type: \"way\",\n ref: 2,\n role: \"outer\"\n },\n {\n type: \"way\",\n ref: 3,\n role: \"outer\"\n }\n ]\n },\n {\n type: \"way\",\n id: 2,\n nodes: [4, 5, 6, 7, 4]\n },\n {\n type: \"way\",\n id: 3,\n nodes: [4, 5, 6, 4]\n },\n {\n type: \"node\",\n id: 4,\n lat: 0.0,\n lon: 0.0\n },\n {\n type: \"node\",\n id: 5,\n lat: 0.0,\n lon: 1.0\n },\n {\n type: \"node\",\n id: 6,\n lat: 1.0,\n lon: 0.0\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 1);

            var way = featureCollection.Features[0];
            Assert.That(way.Properties["id"].ToString() == "1");
            Assert.That((bool)way.Properties["tainted"] == true);
        }

        [Test]
        public void when_a_multipoygon_relation_has_ways_which_are_missing_all_their_nodes_then_nothing_renders()
        {
            var json = "{\n elements: [\n {\n type: \"relation\",\n tags: { \"type\": \"multipolygon\" },\n id: 1,\n members: [\n {\n type: \"way\",\n ref: 2,\n role: \"outer\"\n },\n {\n type: \"way\",\n ref: 3,\n role: \"outer\"\n }\n ]\n },\n {\n type: \"way\",\n id: 2,\n nodes: [4, 5, 6]\n },\n {\n type: \"way\",\n id: 3,\n nodes: [6, 4]\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 0);
        }

        [Test]
        public void when_a_multipoygon_relation_is_missing_an_outer_way_then_relation_does_not_render()
        {
            var json = "{\n elements: [\n {\n type: \"relation\",\n tags: { \"type\": \"multipolygon\" },\n id: 1,\n members: [\n {\n type: \"way\",\n ref: 2,\n role: \"inner\"\n }\n ]\n },\n {\n type: \"way\",\n id: 2,\n nodes: [3, 4, 5, 3]\n },\n {\n type: \"node\",\n id: 3,\n lat: 0.0,\n lon: 0.0\n },\n {\n type: \"node\",\n id: 4,\n lat: 1.0,\n lon: 1.0\n },\n {\n type: \"node\",\n id: 5,\n lat: 1.0,\n lon: 0.0\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 0);
        }

        [Test]
        public void when_a_multipoygon_relation_has_incomplete_outer_ring_then_do_not_render()
        {
            var json = "{\n elements: [\n {\n type: \"relation\",\n tags: { \"type\": \"multipolygon\" },\n id: 1,\n members: [\n {\n type: \"way\",\n ref: 2,\n role: \"outer\"\n },\n {\n type: \"way\",\n ref: 3,\n role: \"outer\"\n }\n ]\n },\n {\n type: \"way\",\n id: 2,\n nodes: [4, 5, 6, 4]\n },\n {\n type: \"node\",\n id: 4,\n lat: 0.0,\n lon: 0.0\n },\n {\n type: \"node\",\n id: 5,\n lat: 1.0,\n lon: 1.0\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 0);
        }

        [Test]
        public void when_a_relation_has_a_center_position_then_it_renders()
        {
            var json = "{\n elements: [\n {\n type: \"relation\",\n id: 1,\n center: {\n lat: 1.234,\n lon: 4.321\n }\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 1);
            var relationFeature = featureCollection.Features[0];
            Assert.That(relationFeature.Geometry.Type == GeoJSONObjectType.Point);
            AssertFeaturePoint(relationFeature, 1.234, 4.321);
            Assert.That(relationFeature.Properties["geometry"].ToString() == "center");
        }

        [Test]
        public void when_a_relation_has_bounds_then_it_renders()
        {
            var json = "{\n elements: [\n {\n type: \"relation\",\n id: 1,\n bounds: {\n minlat: 1.234,\n minlon: 4.321,\n maxlat: 2.234,\n maxlon: 5.321\n }\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 1);
            var relationFeature = featureCollection.Features[0];
            Assert.That(relationFeature.Geometry.Type == GeoJSONObjectType.Polygon);
            Assert.That(relationFeature.Properties["geometry"].ToString() == "bounds");
        }

        [Test]
        public void given_a_relation_with_geometry_on_members_then_a_multipolgyon_renders()
        {
            var json = "{\n elements: [\n {\n type: \"relation\",\n id: 1,\n tags: {\n \"type\": \"boundary\"\n },\n bounds: {\n minlat: 0,\n minlon: 0,\n maxlat: 1,\n maxlon: 1\n },\n members: [\n {\n type: \"way\",\n ref: 1,\n role: \"outer\",\n geometry: [\n { lat: 0, lon: 0 },\n { lat: 0, lon: 1 },\n { lat: 1, lon: 1 },\n { lat: 1, lon: 0 },\n { lat: 0, lon: 0 }\n ]\n },\n {\n type: \"way\",\n ref: 2,\n role: \"outer\",\n geometry: [\n { lat: 1.1, lon: 1.1 },\n { lat: 1.1, lon: 1.2 },\n { lat: 1.2, lon: 1.2 },\n { lat: 1.1, lon: 1.1 }\n ]\n },\n {\n type: \"node\",\n ref: 1,\n role: \"admin_centre\",\n lat: 0.5,\n lon: 0.5\n }\n ]\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 2);
            var realationFeature = featureCollection.Features[0];
            Assert.That(realationFeature.Id == "relation/1");
            Assert.That(realationFeature.Geometry.Type == GeoJSONObjectType.MultiPolygon);
            var coordinateArray = ((MultiPolygon)realationFeature.Geometry).Coordinates;
            Assert.That(coordinateArray.Count == 2);
            AssertFeatureMultiPolygon(realationFeature, new List<List<Coordinates>>
            {
                new List<Coordinates>
                {
                    new Coordinates { Lon = 0, Lat = 0},
                    new Coordinates { Lon = 1, Lat = 0},
                    new Coordinates { Lon = 1, Lat = 1},
                    new Coordinates { Lon = 0, Lat = 1},
                    new Coordinates { Lon = 0, Lat = 0},
                },
                new List<Coordinates>
                {
                    new Coordinates { Lon = 1.1, Lat = 1.1},
                    new Coordinates { Lon = 1.2, Lat = 1.1},
                    new Coordinates { Lon = 1.2, Lat = 1.2},
                    new Coordinates { Lon = 1.1, Lat = 1.1}
                }
            });
        }

        [Test]
        public void given_a_2relation_with_geometry_members_then_2_relations_render()
        {
            var json = "{\n elements: [\n {\n type: \"relation\",\n id: 1,\n tags: {\n \"type\": \"multipolygon\"\n },\n members: [\n {\n type: \"way\",\n ref: 1,\n role: \"outer\",\n geometry: [\n { lat: 0, lon: 0 },\n { lat: 0, lon: 1 }\n ]\n },\n {\n type: \"way\",\n ref: 2,\n role: \"outer\",\n geometry: [\n { lat: 0, lon: 1 },\n { lat: 1, lon: 1 }\n ]\n },\n {\n type: \"way\",\n ref: 3,\n role: \"outer\",\n geometry: [\n { lat: 1, lon: 1 },\n { lat: 0, lon: 0 }\n ]\n }\n ]\n },\n {\n type: \"relation\",\n id: 2,\n tags: {\n \"type\": \"multipolygon\"\n },\n members: [\n {\n type: \"way\",\n ref: 4,\n role: \"outer\",\n geometry: [\n { lat: 0, lon: 0 },\n { lat: 1, lon: 0 }\n ]\n },\n {\n type: \"way\",\n ref: 5,\n role: \"outer\",\n geometry: [\n { lat: 1, lon: 0 },\n { lat: 1, lon: 1 }\n ]\n },\n {\n type: \"way\",\n ref: 3,\n role: \"outer\",\n geometry: [\n { lat: 1, lon: 1 },\n { lat: 0, lon: 0 }\n ]\n }\n ]\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 2);
            var realation1Feature = featureCollection.Features[0];
            Assert.That(realation1Feature.Id == "relation/1");
            Assert.That(realation1Feature.Geometry.Type == GeoJSONObjectType.Polygon);
            AssertFeaturePolygon(realation1Feature, new List<List<Coordinates>>
            {
                new List<Coordinates>
                {
                    new Coordinates { Lon = 1, Lat = 1},
                    new Coordinates { Lon = 0, Lat = 0},
                    new Coordinates { Lon = 1, Lat = 0},
                    new Coordinates { Lon = 1, Lat = 1},
                }
            });

            var realation2Feature = featureCollection.Features[1];
            Assert.That(realation2Feature.Id == "relation/2");
            Assert.That(realation2Feature.Geometry.Type == GeoJSONObjectType.Polygon);
            AssertFeaturePolygon(realation2Feature, new List<List<Coordinates>>
            {
                new List<Coordinates>
                {
                    new Coordinates { Lon = 1, Lat = 1},
                    new Coordinates { Lon = 0, Lat = 0},
                    new Coordinates { Lon = 0, Lat = 1},
                    new Coordinates { Lon = 1, Lat = 1}
                }
            });
        }

        [Test]
        public void given_a_2_relations_with_geometry_members_then_2_ways_render()
        {
            var json = "{\n elements: [\n {\n type: \"relation\",\n id: 1,\n tags: {\n \"type\": \"multipolygon\"\n },\n members: [\n {\n type: \"way\",\n ref: 1,\n role: \"outer\",\n geometry: [\n { lat: 0, lon: 0 },\n { lat: 0, lon: 1 },\n { lat: 1, lon: 1 },\n { lat: 1, lon: 0 },\n { lat: 0, lon: 0 }\n ]\n },\n {\n type: \"way\",\n ref: 2,\n role: \"inner\",\n geometry: null\n }\n ]\n },\n {\n type: \"relation\",\n id: 2,\n tags: {\n \"type\": \"multipolygon\"\n },\n members: [\n {\n type: \"way\",\n ref: 3,\n role: \"outer\",\n geometry: [\n { lat: 1, lon: 1 },\n { lat: 1, lon: 2 },\n { lat: 2, lon: 2 },\n null,\n { lat: 1, lon: 1 }\n ]\n }\n ]\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 2);
            var way1Feature = featureCollection.Features[0];
            Assert.That(way1Feature.Id == "way/1");
            Assert.That(way1Feature.Geometry.Type == GeoJSONObjectType.Polygon);
            AssertFeaturePolygon(way1Feature, new List<List<Coordinates>>
            {
                new List<Coordinates>
                {
                    new Coordinates { Lon = 0, Lat = 0},
                    new Coordinates { Lon = 1, Lat = 0},
                    new Coordinates { Lon = 1, Lat = 1},
                    new Coordinates { Lon = 0, Lat = 1},
                    new Coordinates { Lon = 0, Lat = 0}
                }
            });

            var realation2Feature = featureCollection.Features[1];
            Assert.That(realation2Feature.Id == "way/3");
            Assert.That(realation2Feature.Geometry.Type == GeoJSONObjectType.Polygon);
            AssertFeaturePolygon(realation2Feature, new List<List<Coordinates>>
            {
                new List<Coordinates>
                {
                    new Coordinates { Lon = 1, Lat = 1},
                    new Coordinates { Lon = 2, Lat = 1},
                    new Coordinates { Lon = 2, Lat = 2},
                    new Coordinates { Lon = 1, Lat = 1},
                }
            });
        }


        
    }
}