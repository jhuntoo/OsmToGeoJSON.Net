using System.Collections.Generic;
using GeoJSON.Net;
using NUnit.Framework;

namespace OsmToGeoJSON.Tests
{
    [TestFixture]
    public class NodeTests : GeometryTestBase
    {
        [Test]
        public void simple_node_should_parse()
        {
            var nodeJson = "{\n elements: [\n {\n type: \"node\",\n id: 1,\n lat: 1.234,\n lon: 4.321\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(nodeJson);

            Assert.That(featureCollection.Features.Count == 1);
            var feature = featureCollection.Features[0];
            Assert.That(feature.Type == GeoJSONObjectType.Feature);
            Assert.That(feature.Id == "node/1");
            AssertPropertiesAreCorrect(feature, type: "node", id: "1");
            AssertFeaturePoint(feature, lat: 1.234, lon: 4.321);
        }

         [Test]
        public void node_with_metadata_should_render()
        {
            var nodeJson = "{\n elements: [\n {\n type: \"node\",\n id: 1,\n lat: 1.234,\n lon: 4.321,\n timestamp: \"2013-01-13T22:56:07Z\",\n version: 7,\n changeset: 1234,\n user: \"johndoe\",\n uid: 666\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(nodeJson);

            Assert.That(featureCollection.Features.Count == 1);
            var feature = featureCollection.Features[0];
            Assert.That(feature.Type == GeoJSONObjectType.Feature);
            Assert.That(feature.Id == "node/1");
            AssertPropertiesAreCorrect(feature, type: "node", id: "1", relations: new TestRelation[] {}, tags: new Dictionary<string, object>(), meta: new Dictionary<string, object> { { "timestamp", "2013-01-13T22:56:07Z" }, { "version", 7 }, { "changeset", 1234 }, { "user", "johndoe" }, { "uid", 666 } });
            AssertFeaturePoint(feature, lat: 1.234, lon: 4.321);
        }
    }
    
}