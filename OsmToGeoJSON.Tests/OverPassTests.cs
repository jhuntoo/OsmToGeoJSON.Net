using GeoJSON.Net;
using NUnit.Framework;

namespace OsmToGeoJSON.Tests
{
    [TestFixture]
    public class OverPassTests : GeometryTestBase
    {
        [Test]
        public void area_type_does_not_render()
        {
            const string json = "{\n elements: [\n {\n type: \"area\",\n id: 1,\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 0);
           
        }

        [Test]
        public void id_only_node_does_not_render()
        {
            const string json = " {\n elements: [\n {\n type: \"node\",\n id: 1\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 0);

        }

        [Test]
        public void id_only_way_does_not_render()
        {
            const string json = " {\n elements: [\n {\n type: \"way\",\n id: 1\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 0);

        }

        [Test]
        public void id_only_relation_does_not_render()
        {
            const string json = " {\n elements: [\n {\n type: \"relation\",\n id: 1\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 0);

        }

        [Test]
        public void center_geometry_renders()
        {
            const string json = "{\n elements: [\n {\n type: \"way\",\n id: 1,\n center: {\n lat: 1.234,\n lon: 4.321\n }\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            var wayFeature = featureCollection.Features[0];
            Assert.That(featureCollection.Features.Count == 1);

            Assert.That(wayFeature.Geometry.Type == GeoJSONObjectType.Point);
            Assert.That(wayFeature.Properties["geometry"].ToString() == "center");
            AssertFeaturePoint(wayFeature, 1.234, 4.321);

        }
    }
}
