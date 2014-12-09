using System.Collections.Generic;
using GeoJSON.Net;
using GeoJSON.Net.Geometry;
using NUnit.Framework;

namespace OsmToGeoJSON.Tests
{
    [TestFixture]
    public class MultiPolygonTests : GeometryTestBase
    {
        [Test]
        public void simple_relation_multipolygon_should_parse()
        {
            const string nodeJson = "{\n elements: [\n {\n type: \"relation\",\n id: 1,\n tags: { \"type\": \"multipolygon\" },\n members: [\n {\n type: \"way\",\n ref: 2,\n role: \"outer\"\n },\n {\n type: \"way\",\n ref: 3,\n role: \"inner\"\n }\n ]\n },\n {\n type: \"way\",\n id: 2,\n nodes: [4, 5, 6, 7, 4],\n tags: { \"area\": \"yes\" },\n },\n {\n type: \"way\",\n id: 3,\n nodes: [8, 9, 10, 8]\n },\n {\n type: \"node\",\n id: 4,\n lat: -1.0,\n lon: -1.0\n },\n {\n type: \"node\",\n id: 5,\n lat: -1.0,\n lon: 1.0\n },\n {\n type: \"node\",\n id: 6,\n lat: 1.0,\n lon: 1.0\n },\n {\n type: \"node\",\n id: 7,\n lat: 1.0,\n lon: -1.0\n },\n {\n type: \"node\",\n id: 8,\n lat: -0.5,\n lon: 0.0\n },\n {\n type: \"node\",\n id: 9,\n lat: 0.5,\n lon: 0.0\n },\n {\n type: \"node\",\n id: 10,\n lat: 0.0,\n lon: 0.5\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(nodeJson); ;

            Assert.That(featureCollection.Features.Count == 1);
            var feature = featureCollection.Features[0];
            Assert.That(feature.Type == GeoJSONObjectType.Feature);
            Assert.That(feature.Id == "way/2");
            AssertPropertiesAreCorrect(feature, type: "way", id: "2", tags: new Tags { { "area", "yes" } }, relations: new[] { new TestRelation { Rel = 1, Role = "outer", RelTags = new Tags { { "type", "multipolygon" } } } });
            AssertFeaturePolygon(feature,
                new List<List<Coordinates>>
                    { new List<Coordinates>
                    {
                        new Coordinates { Lon = -1.0, Lat = -1.0},
                        new Coordinates { Lon = 1.0, Lat = -1.0},
                        new Coordinates { Lon = 1.0, Lat = 1.0},
                        new Coordinates { Lon = -1.0, Lat = 1.0},
                        new Coordinates { Lon = -1.0, Lat = -1.0}
                    }, new List<Coordinates>
                    {
                        new Coordinates { Lon = 0.0, Lat = -0.5},
                        new Coordinates { Lon = 0.0, Lat = 0.5},
                        new Coordinates { Lon = 0.5, Lat = 0.0},
                        new Coordinates { Lon = 0.0, Lat = -0.5}
                    }});
        }

        [Test]
        public void mulitpolygon_should_parse_and_any_way_which_has_interesting_tags_which_the_parent_relation_does_not_have_should_also_render()
        {
            const string nodeJson = "{\n elements: [\n {\n type: \"relation\",\n id: 1,\n tags: { \"type\": \"multipolygon\", \"building\": \"yes\" },\n members: [\n {\n type: \"way\",\n ref: 2,\n role: \"outer\"\n },\n {\n type: \"way\",\n ref: 3,\n role: \"inner\"\n },\n {\n type: \"way\",\n ref: 4,\n role: \"inner\"\n },\n {\n type: \"way\",\n ref: 5,\n role: \"outer\"\n }\n ]\n },\n {\n type: \"way\",\n id: 2,\n nodes: [4, 5, 6, 7, 4],\n tags: { \"building\": \"yes\" }\n },\n {\n type: \"way\",\n id: 3,\n nodes: [8, 9, 10, 8],\n tags: { \"area\": \"yes\" }\n },\n {\n type: \"way\",\n id: 4,\n nodes: [11, 12, 13, 11],\n tags: { \"barrier\": \"fence\" }\n },\n {\n type: \"way\",\n id: 5,\n nodes: [14, 15, 16, 14],\n tags: { \"building\": \"yes\", \"area\": \"yes\" }\n },\n {\n type: \"node\",\n id: 4,\n lat: -1.0,\n lon: -1.0\n },\n {\n type: \"node\",\n id: 5,\n lat: -1.0,\n lon: 1.0\n },\n {\n type: \"node\",\n id: 6,\n lat: 1.0,\n lon: 1.0\n },\n {\n type: \"node\",\n id: 7,\n lat: 1.0,\n lon: -1.0\n },\n {\n type: \"node\",\n id: 8,\n lat: -0.5,\n lon: 0.0\n },\n {\n type: \"node\",\n id: 9,\n lat: 0.5,\n lon: 0.0\n },\n {\n type: \"node\",\n id: 10,\n lat: 0.0,\n lon: 0.5\n },\n {\n type: \"node\",\n id: 11,\n lat: 0.1,\n lon: -0.1\n },\n {\n type: \"node\",\n id: 12,\n lat: -0.1,\n lon: -0.1\n },\n {\n type: \"node\",\n id: 13,\n lat: 0.0,\n lon: -0.2\n },\n {\n type: \"node\",\n id: 14,\n lat: 0.1,\n lon: -1.1\n },\n {\n type: \"node\",\n id: 15,\n lat: -0.1,\n lon: -1.1\n },\n {\n type: \"node\",\n id: 16,\n lat: 0.0,\n lon: -1.2\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(nodeJson);

            Assert.That(featureCollection.Features.Count == 4);
            var multiPolygonfeature = featureCollection.Features[0];
            Assert.That(multiPolygonfeature.Type == GeoJSONObjectType.Feature);
            Assert.That(multiPolygonfeature.Id == "relation/1");
            AssertPropertiesAreCorrect(multiPolygonfeature, type: "relation", id: "1", tags: new Tags { { "type", "multipolygon" }, { "building", "yes" } });
            AssertFeatureMultiPolygon(multiPolygonfeature,
                new List<List<Coordinates>>
                    { new List<Coordinates>
                    {
                        new Coordinates { Lon = -1.0, Lat = -1.0},
                        new Coordinates { Lon = 1.0, Lat = -1.0},
                        new Coordinates { Lon = 1.0, Lat = 1.0},
                        new Coordinates { Lon = -1.0, Lat = 1.0},
                        new Coordinates { Lon = -1.0, Lat = -1.0}
                    }, new List<Coordinates>
                    {
                        new Coordinates { Lon = 0.0, Lat = -0.5},
                        new Coordinates { Lon = 0.0, Lat = 0.5},
                        new Coordinates { Lon = 0.5, Lat = 0.0},
                        new Coordinates { Lon = 0.0, Lat = -0.5}
                    },  new List<Coordinates>
                    {
                        new Coordinates { Lon = -0.1, Lat = 0.1},
                        new Coordinates { Lon = -0.1, Lat = -0.1},
                        new Coordinates { Lon = -0.2, Lat = 0.0},
                        new Coordinates { Lon = -0.1, Lat = 0.1}
                    },new List<Coordinates>
                    {
                        new Coordinates { Lon = -1.1, Lat = 0.1},
                        new Coordinates { Lon = -1.1, Lat = -0.1},
                        new Coordinates { Lon = -1.2, Lat = 0.0},
                        new Coordinates { Lon = -1.1, Lat = 0.1}
                    }});

            var way3Feature = featureCollection.Features[1];
            Assert.That(way3Feature.Type == GeoJSONObjectType.Feature);
            Assert.That(way3Feature.Id == "way/3");
            AssertPropertiesAreCorrect(way3Feature, type: "way", id: "3", tags: new Tags { { "area", "yes" } }, relations: new[] { new TestRelation { Rel = 1, Role = "inner", RelTags = new Tags { { "type", "multipolygon" }, { "building", "yes" } } } });
            AssertFeaturePolygon(way3Feature,
                 new List<List<Coordinates>>
                    { new List<Coordinates>
                    {
                        new Coordinates { Lon = 0.0, Lat = -0.5},
                        new Coordinates { Lon = 0.0, Lat = 0.5},
                        new Coordinates { Lon = 0.5, Lat = 0.0},
                        new Coordinates { Lon = 0.0, Lat = -0.5}
                    }});

            var way4Feature = featureCollection.Features[2];
            Assert.That(way4Feature.Type == GeoJSONObjectType.Feature);
            Assert.That(way4Feature.Id == "way/4");
            AssertPropertiesAreCorrect(way4Feature, type: "way", id: "4", tags: new Tags { { "barrier", "fence" } }, relations: new[] { new TestRelation { Rel = 1, Role = "inner", RelTags = new Tags { { "type", "multipolygon" }, { "building", "yes" } } } });
            AssertFeatureLineString(way4Feature,
                  new List<Coordinates>
                    {
                        new Coordinates { Lon = -0.1, Lat = 0.1},
                        new Coordinates { Lon = -0.1, Lat = -0.1},
                        new Coordinates { Lon = -0.2, Lat = 0.0},
                        new Coordinates { Lon = -0.1, Lat = 0.1}
                    });

            var way5Feature = featureCollection.Features[3];
            Assert.That(way5Feature.Type == GeoJSONObjectType.Feature);
            Assert.That(way5Feature.Id == "way/5");
            AssertPropertiesAreCorrect(way5Feature, type: "way", id: "5", tags: new Tags { { "area", "yes" }, { "building", "yes" } }, relations: new[] { new TestRelation { Rel = 1, Role = "outer", RelTags = new Tags { { "type", "multipolygon" }, { "building", "yes" } } } });
            AssertFeaturePolygon(way5Feature,
                 new List<List<Coordinates>>
                    { new List<Coordinates>
                    {
                        new Coordinates { Lon = -1.1, Lat = 0.1},
                        new Coordinates { Lon = -1.1, Lat = -0.1},
                        new Coordinates { Lon = -1.2, Lat = 0.0},
                        new Coordinates { Lon = -1.1, Lat = 0.1}
                    }});

        }

        [Test]
        public void multipolygons_with_interesting_tags_should_be_rendered()
        {
            const string nodeJson = "{\n elements: [\n {\n type: \"relation\",\n id: 1,\n tags: { \"type\": \"multipolygon\", \"amenity\": \"xxx\" },\n members: [\n {\n type: \"way\",\n ref: 2,\n role: \"outer\"\n },\n {\n type: \"way\",\n ref: 3,\n role: \"inner\"\n }\n ]\n },\n {\n type: \"way\",\n id: 2,\n nodes: [4, 5, 6, 7, 4],\n tags: { \"amenity\": \"yyy\" }\n },\n {\n type: \"way\",\n id: 3,\n nodes: [8, 9, 10, 8]\n },\n {\n type: \"node\",\n id: 4,\n lat: -1.0,\n lon: -1.0\n },\n {\n type: \"node\",\n id: 5,\n lat: -1.0,\n lon: 1.0\n },\n {\n type: \"node\",\n id: 6,\n lat: 1.0,\n lon: 1.0\n },\n {\n type: \"node\",\n id: 7,\n lat: 1.0,\n lon: -1.0\n },\n {\n type: \"node\",\n id: 8,\n lat: -0.5,\n lon: 0.0\n },\n {\n type: \"node\",\n id: 9,\n lat: 0.5,\n lon: 0.0\n },\n {\n type: \"node\",\n id: 10,\n lat: 0.0,\n lon: 0.5\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(nodeJson);
            var realationFeature = featureCollection.Features[0];
            var interestingWayFeature = featureCollection.Features[1];

             Assert.That(realationFeature.Properties["id"].ToString() == "1");
             Assert.That(realationFeature.Id  == "relation/1");
             Assert.That(interestingWayFeature.Properties["id"].ToString() == "2");
             Assert.That(interestingWayFeature.Id == "way/2");
        }

        [Test]
        public void multipolygon_that_has_an_outer_ring_with_a_non_matching_inner_ring_only_renders_outer_ring()
        {
            const string nodeJson = "{\n elements: [\n {\n type: \"relation\",\n tags: { \"type\": \"multipolygon\" },\n id: 1,\n members: [\n {\n type: \"way\",\n ref: 2,\n role: \"outer\"\n },\n {\n type: \"way\",\n ref: 3,\n role: \"inner\"\n }\n ]\n },\n {\n type: \"way\",\n id: 2,\n nodes: [4, 5, 6, 7, 4]\n },\n {\n type: \"node\",\n id: 4,\n lat: 0.0,\n lon: 0.0\n },\n {\n type: \"node\",\n id: 5,\n lat: 1.0,\n lon: 0.0\n },\n {\n type: \"node\",\n id: 6,\n lat: 1.0,\n lon: 1.0\n },\n {\n type: \"node\",\n id: 7,\n lat: 0.0,\n lon: 1.0\n },\n {\n type: \"way\",\n id: 3,\n nodes: [8, 9, 10, 8]\n },\n {\n type: \"node\",\n id: 8,\n lat: 3.0,\n lon: 3.0\n },\n {\n type: \"node\",\n id: 9,\n lat: 4.0,\n lon: 3.0\n },\n {\n type: \"node\",\n id: 10,\n lat: 3.0,\n lon: 4.0\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(nodeJson);
            var realationFeature = featureCollection.Features[0];
            
            Assert.That(realationFeature.Properties["id"].ToString() == "2");
            Assert.That(realationFeature.Id == "way/2");
            Assert.That(realationFeature.Geometry.Type == GeoJSONObjectType.Polygon);
            Assert.That(((Polygon)realationFeature.Geometry).Coordinates.Count == 1);
        }

        [Test]
        public void multipolygon_3_ways_are_merge_into_1_polygon()
        {
            const string nodeJson = "{\n elements: [\n {\n type: \"relation\",\n tags: { \"type\": \"multipolygon\" },\n id: 1,\n members: [\n {\n type: \"way\",\n ref: 1,\n role: \"outer\"\n },\n {\n type: \"way\",\n ref: 3,\n role: \"outer\"\n },\n {\n type: \"way\",\n ref: 2,\n role: \"outer\"\n }\n ]\n },\n {\n type: \"way\",\n id: 1,\n nodes: [1, 2]\n },\n {\n type: \"way\",\n id: 2,\n nodes: [2, 3]\n },\n {\n type: \"way\",\n id: 3,\n nodes: [3, 1]\n },\n {\n type: \"node\",\n id: 1,\n lat: 1.0,\n lon: 0.0\n },\n {\n type: \"node\",\n id: 2,\n lat: 2.0,\n lon: 0.0\n },\n {\n type: \"node\",\n id: 3,\n lat: 3.0,\n lon: 0.0\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(nodeJson);
            var realationFeature = featureCollection.Features[0];

            Assert.That(realationFeature.Properties["id"].ToString() == "1");
            Assert.That(realationFeature.Id == "relation/1");
            Assert.That(realationFeature.Geometry.Type == GeoJSONObjectType.Polygon);
            Assert.That(((Polygon)realationFeature.Geometry).Coordinates.Count == 1);
        }

        [Test]
        public void multipolygon_with_unclosed_ring_does_not_render()
        {
            const string json = "{\n elements: [\n {\n type: \"relation\",\n tags: { \"type\": \"multipolygon\" },\n id: 1,\n members: [\n {\n type: \"way\",\n ref: 1,\n role: \"outer\"\n },\n {\n type: \"way\",\n ref: 2,\n role: \"outer\"\n }\n ]\n },\n {\n type: \"way\",\n id: 1,\n nodes: [1, 2, 3, 4]\n },\n {\n type: \"way\",\n id: 2,\n nodes: [3, 2]\n },\n {\n type: \"node\",\n id: 1,\n lat: 1.0,\n lon: 0.0\n },\n {\n type: \"node\",\n id: 2,\n lat: 2.0,\n lon: 0.0\n },\n {\n type: \"node\",\n id: 3,\n lat: 3.0,\n lon: 0.0\n },\n {\n type: \"node\",\n id: 4,\n lat: 4.0,\n lon: 0.0\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 0);
        }
        
    }
}