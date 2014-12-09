using System.Collections.Generic;
using GeoJSON.Net;
using NUnit.Framework;

namespace OsmToGeoJSON.Tests
{
    [TestFixture]
    public class PolygonTests : GeometryTestBase
    {
        [Test]
        public void simple_polyglon_way_should_parse()
        {
            const string nodeJson = "{\n elements: [\n {\n type: \"way\",\n id: 1,\n nodes: [2, 3, 4, 5, 2],\n tags: { area: \"yes\" }\n },\n {\n type: \"node\",\n id: 2,\n lat: 0.0,\n lon: 0.0\n },\n {\n type: \"node\",\n id: 3,\n lat: 0.0,\n lon: 1.0\n },\n {\n type: \"node\",\n id: 4,\n lat: 1.0,\n lon: 1.0\n },\n {\n type: \"node\",\n id: 5,\n lat: 1.0,\n lon: 0.0\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(nodeJson);
            Assert.That(featureCollection.Features.Count == 1);
            var feature = featureCollection.Features[0];
            Assert.That(feature.Type == GeoJSONObjectType.Feature);
            Assert.That(feature.Id == "way/1");
            AssertPropertiesAreCorrect(feature, type: "way", id: "1", tags: new Dictionary<string, object> { { "area", "yes" } });
            AssertPolygon(feature, new Coordinates { Lon = 0.0, Lat = 0.0 },
                new Coordinates { Lon = 1.0, Lat = 0.0 },
                new Coordinates { Lon = 1.0, Lat = 1.0 },
                new Coordinates { Lon = 0.0, Lat = 1.0 },
                new Coordinates { Lon = 0.0, Lat = 0.0 });
        }


        [Test]
        public void custom_tagging_detection_rules()
        {
            const string nodeJson = "{\n elements: [\n {\n type: \"way\",\n id: 1,\n tags: { \"is_polygon_key\": \"*\" },\n nodes: [1, 2, 3, 1]\n },\n {\n type: \"way\",\n id: 2,\n tags: { \"is_polygon_key_value\": \"included_value\" },\n nodes: [1, 2, 3, 1]\n },\n {\n type: \"way\",\n id: 3,\n tags: { \"is_polygon_key_excluded_value\": \"*\" },\n nodes: [1, 2, 3, 1]\n },\n {\n type: \"way\",\n id: 4,\n tags: { \"is_polygon_key\": \"no\" },\n nodes: [1, 2, 3, 1]\n },\n {\n type: \"way\",\n id: 5,\n tags: { \"is_polygon_key_value\": \"not_included_value\" },\n nodes: [1, 2, 3, 1]\n },\n {\n type: \"way\",\n id: 6,\n tags: { \"is_polygon_key_excluded_value\": \"excluded_value\" },\n nodes: [1, 2, 3, 1]\n },\n {\n type: \"node\",\n id: 1,\n lat: 1.0,\n lon: 0.0\n },\n {\n type: \"node\",\n id: 2,\n lat: 2.0,\n lon: 0.0\n },\n {\n type: \"node\",\n id: 3,\n lat: 0.0,\n lon: 3.0\n }\n ]\n }";
            var converter = new Converter(additionalPolygonFeatures: new Dictionary<string, object>
                {
                    { "is_polygon_key", true },
                    {"is_polygon_key_value", new Dictionary<string, object> { {"included_values", new Dictionary<string, object> {{"included_value", true}}}}},
                    {"is_polygon_key_excluded_value", new Dictionary<string, object> { {"excluded_values", new Dictionary<string, object> {{"excluded_value", true}}}}}
                });
            var featureCollection = converter.OsmToFeatureCollection(nodeJson);
            Assert.That(featureCollection.Features.Count == 6);
            Assert.That(featureCollection.Features[0].Geometry.Type == GeoJSONObjectType.Polygon);
            Assert.That(featureCollection.Features[1].Geometry.Type == GeoJSONObjectType.Polygon);
            Assert.That(featureCollection.Features[2].Geometry.Type == GeoJSONObjectType.Polygon);
            Assert.That(featureCollection.Features[3].Geometry.Type == GeoJSONObjectType.LineString);
            Assert.That(featureCollection.Features[4].Geometry.Type == GeoJSONObjectType.LineString);
            Assert.That(featureCollection.Features[5].Geometry.Type == GeoJSONObjectType.LineString);
            
        }
    }
}