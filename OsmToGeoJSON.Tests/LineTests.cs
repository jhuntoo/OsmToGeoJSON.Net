using GeoJSON.Net;
using NUnit.Framework;

namespace OsmToGeoJSON.Tests
{
    [TestFixture]
    public class LineTests : GeometryTestBase
    {
        
        [Test]
        public void simple_line_way_should_parse()
        {
            const string nodeJson = "{\n elements: [\n {\n type: \"way\",\n id: 1,\n nodes: [2, 3, 4]\n },\n {\n type: \"node\",\n id: 2,\n lat: 0.0,\n lon: 1.0\n },\n {\n type: \"node\",\n id: 3,\n lat: 0.0,\n lon: 1.1\n },\n {\n type: \"node\",\n id: 4,\n lat: 0.1,\n lon: 1.2\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(nodeJson); 
            Assert.That(featureCollection.Features.Count == 1);
            var feature = featureCollection.Features[0];
            Assert.That(feature.Type == GeoJSONObjectType.Feature);
            Assert.That(feature.Id == "way/1");
            AssertPropertiesAreCorrect(feature, type: "way", id: "1");
            AssertLineString(feature, new Coordinates {Lon = 1.0, Lat = 0.0}, new Coordinates {Lon = 1.1, Lat = 0.0}, new Coordinates {Lon = 1.2, Lat = 0.1});
        }

        [Test]
        public void two_simple_line_ways_should_parse()
        {
            const string nodeJson = "{\n elements: [\n {\n type: \"way\",\n id: 1,\n nodes: [2, 3, 4]\n },\n {\n type: \"node\",\n id: 2,\n lat: 0.0,\n lon: 1.0\n },\n {\n type: \"node\",\n id: 3,\n lat: 0.0,\n lon: 1.1\n },\n {\n type: \"node\",\n id: 4,\n lat: 0.1,\n lon: 1.2\n },\n {\n type: \"way\",\n id: 5,\n nodes: [6, 7, 8]\n },\n {\n type: \"node\",\n id: 6,\n lat: 100.0,\n lon: 101.0\n },\n {\n type: \"node\",\n id: 7,\n lat: 100.0,\n lon: 101.1\n },\n {\n type: \"node\",\n id: 8,\n lat: 100.1,\n lon: 101.2\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(nodeJson); 
            Assert.That(featureCollection.Features.Count == 2);
            var feature1 = featureCollection.Features[0];
            Assert.That(feature1.Type == GeoJSONObjectType.Feature);
            Assert.That(feature1.Id == "way/1");
            AssertPropertiesAreCorrect(feature1, type: "way", id: "1");
            AssertLineString(feature1, new Coordinates { Lon = 1.0, Lat = 0.0 }, new Coordinates { Lon = 1.1, Lat = 0.0 }, new Coordinates { Lon = 1.2, Lat = 0.1 });

            var feature2 = featureCollection.Features[1];
            Assert.That(feature2.Type == GeoJSONObjectType.Feature);
            Assert.That(feature2.Id == "way/5");
            AssertPropertiesAreCorrect(feature2, type: "way", id: "5");
            AssertLineString(feature2, new Coordinates { Lon = 101.0, Lat = 100.0 }, new Coordinates { Lon = 101.1, Lat = 100.0 }, new Coordinates { Lon = 101.2, Lat = 100.1 });
        }

       

        
        

        

        

        


    }
}
