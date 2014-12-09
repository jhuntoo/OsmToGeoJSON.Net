using System.Collections.Generic;
using GeoJSON.Net;
using NUnit.Framework;

namespace OsmToGeoJSON.Tests
{   
    [TestFixture]
    public class TagsPointOfInterestTests : GeometryTestBase
    {
        [Test]
        public void when_a_way_with_3nodes_of_which_2_are_interesting_then_way_and_2_nodes_are_rendered()
        {
            var json = "{\n elements: [\n {\n type: \"way\",\n id: 1,\n nodes: [2, 3, 4],\n tags: { \"foo\": \"bar\" }\n },\n {\n type: \"node\",\n id: 2,\n lat: 0.0,\n lon: 1.0\n },\n {\n type: \"node\",\n id: 3,\n lat: 0.0,\n lon: 1.1,\n tags: { \"asd\": \"fasd\" }\n },\n {\n type: \"node\",\n id: 4,\n lat: 0.1,\n lon: 1.2,\n tags: { \"created_by\": \"me\" }\n },\n {\n type: \"node\",\n id: 5,\n lat: 0.0,\n lon: 0.0\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 3);
            var wayFeature = featureCollection.Features[0];
            Assert.That(wayFeature.Type == GeoJSONObjectType.Feature);
            Assert.That(wayFeature.Id == "way/1");
            AssertPropertiesAreCorrect(wayFeature, type: "way", id: "1", tags: new Dictionary<string, object> { { "foo", "bar" } });
            AssertLineString(wayFeature, new[]
            {
                new Coordinates {Lon = 1.0, Lat = 0.0},
                new Coordinates {Lon = 1.1, Lat = 0.0},
                new Coordinates {Lon = 1.2, Lat = 0.1}
            });

            var node3Feature = featureCollection.Features[1];
            Assert.That(node3Feature.Type == GeoJSONObjectType.Feature);
            Assert.That(node3Feature.Id == "node/3");
            AssertPropertiesAreCorrect(node3Feature, type: "node", id: "3", tags: new Dictionary<string, object> { { "asd", "fasd" } });
            AssertFeaturePoint(node3Feature, lat: 0.0, lon: 1.1);

            var node5Feature = featureCollection.Features[2];
            Assert.That(node5Feature.Type == GeoJSONObjectType.Feature);
            Assert.That(node5Feature.Id == "node/5");
            AssertPropertiesAreCorrect(node5Feature, type: "node", id: "5");
            AssertFeaturePoint(node5Feature, lat: 0.0, lon: 0.0);
               
        }
    }
}
