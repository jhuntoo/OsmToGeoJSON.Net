using System.Collections.Generic;
using GeoJSON.Net;
using GeoJSON.Net.Geometry;
using NUnit.Framework;

namespace OsmToGeoJSON.Tests
{
    [TestFixture]
    public class WayTests : GeometryTestBase
    {
        [Test]
        public void one_node_ways_should_not_render()
        {
            var json = "{\n elements: [\n {\n type: \"way\",\n id: 1,\n nodes: [2],\n tags: { \"foo\": \"bar\" }\n },\n {\n type: \"node\",\n id: 2,\n lat: 0.0,\n lon: 0.0\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 0);
            
        }

        [Test]
        public void given_a_way_with_an_interesting_node_both_should_be_rendered()
        {
            var json = "{\n elements: [\n {\n type: \"way\",\n id: 1,\n nodes: [1, 2]\n },\n {\n type: \"node\",\n id: 1,\n tags: { \"created_by\": \"foo\" },\n lat: 1.0,\n lon: 0.0\n },\n {\n type: \"node\",\n id: 2,\n tags: { \"interesting\": \"yes\" },\n lat: 2.0,\n lon: 0.0\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 2);
            Assert.That(featureCollection.Features[0].Geometry.Type == GeoJSONObjectType.LineString);
            Assert.That(featureCollection.Features[1].Geometry.Type == GeoJSONObjectType.Point);
            Assert.That(featureCollection.Features[1].Properties["id"].ToString() == "2");

        }

        [Test]
        public void given_a_way_with_an_uninteresting_node_then_the_node_should_not_be_rendered()
        {
            var json = "{\n elements: [\n {\n type: \"way\",\n id: 1,\n nodes: [2, 3]\n },\n {\n type: \"node\",\n id: 2,\n tags: { \"foo\": \"bar\" },\n user: \"johndoe\",\n lat: 1.0,\n lon: 0.0\n },\n {\n type: \"node\",\n id: 3,\n tags: { \"foo\": \"bar\", \"asd\": \"fasd\" },\n user: \"johndoe\",\n lat: 2.0,\n lon: 0.0\n }\n ]\n }";
            var converter = new Converter(uninterestingTags: new Dictionary<string, object> { { "foo", "true" } });
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 2);
            Assert.That(featureCollection.Features[1].Properties["id"].ToString() == "3");

        }

        [Test]
        public void given_a_way_with_a_missing_node_then_tainted_should_be_true()
        {
            var json = "{\n elements: [\n {\n type: \"way\",\n id: 1,\n nodes: [2, 3, 4]\n },\n {\n type: \"node\",\n id: 2,\n lat: 0.0,\n lon: 0.0\n },\n {\n type: \"node\",\n id: 4,\n lat: 1.0,\n lon: 1.0\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 1);
            Assert.That(featureCollection.Features[0].Properties["id"].ToString() == "1");
            Assert.That((bool)featureCollection.Features[0].Properties["tainted"] == true);
            Assert.That(((LineString) featureCollection.Features[0].Geometry).Coordinates.Count == 2);
        }

        [Test]
        public void given_a_way_with_bounds_then_bounds_are_rendered()
        {
            var json = "{\n elements: [\n {\n type: \"way\",\n id: 1,\n bounds: {\n minlat: 1.234,\n minlon: 4.321,\n maxlat: 2.234,\n maxlon: 5.321\n }\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 1);
            var wayFeature = featureCollection.Features[0];
            Assert.That(wayFeature.Id == "way/1");
            Assert.That(wayFeature .Geometry.Type== GeoJSONObjectType.LineString);
            Assert.That(wayFeature.Properties["geometry"].ToString() == "bounds");
        }

        [Test]
        public void given_a_way_with_bounds_and_geometry_a_polygon_is_rendered()
        {
            var json = "{\n elements: [\n {\n type: \"way\",\n id: 1,\n bounds: {\n minlat: 0,\n minlon: 0,\n maxlat: 1,\n maxlon: 1\n },\n nodes: [\n 1,\n 2,\n 3,\n 1\n ],\n geometry: [\n { lat: 0, lon: 0 },\n { lat: 0, lon: 1 },\n { lat: 1, lon: 1 },\n { lat: 0, lon: 0 }\n ],\n tags: {\n \"area\": \"yes\"\n }\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 1);
            var wayFeature = featureCollection.Features[0];
            Assert.That(wayFeature.Id == "way/1");
            Assert.That(wayFeature.Geometry.Type == GeoJSONObjectType.Polygon);
            Assert.That(((Polygon)wayFeature.Geometry).Coordinates[0].Coordinates.Count == 4);
        }

        [Test]
        public void given_a_way_with_no_node_references_and_geometry_a_polygon_is_rendered()
        {
            var json = "{\n elements: [\n {\n type: \"way\",\n id: 1,\n bounds: {\n minlat: 0,\n minlon: 0,\n maxlat: 1,\n maxlon: 1\n },\n geometry: [\n { lat: 0, lon: 0 },\n { lat: 0, lon: 1 },\n { lat: 1, lon: 1 },\n { lat: 0, lon: 0 }\n ],\n tags: {\n \"area\": \"yes\"\n }\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 1);
            var wayFeature = featureCollection.Features[0];
            Assert.That(wayFeature.Id == "way/1");
            Assert.That(wayFeature.Geometry.Type == GeoJSONObjectType.Polygon);
            Assert.That(((Polygon)wayFeature.Geometry).Coordinates[0].Coordinates.Count == 4);
        }

        [Test]
        public void given_a_tainted_way_then_partial_geometry_is_rendered()
        {
            var json = "{\n elements: [\n {\n type: \"way\",\n id: 1,\n nodes: [\n 1,\n 2,\n 3,\n 4\n ],\n geometry: [\n null,\n { lat: 1, lon: 2 },\n { lat: 2, lon: 2 },\n null\n ]\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 1);
            var wayFeature = featureCollection.Features[0];
            Assert.That(wayFeature.Id == "way/1");
            Assert.That(wayFeature.Geometry.Type == GeoJSONObjectType.LineString);
            Assert.That((bool)wayFeature.Properties["tainted"] == true);
        }

        [Test]
        public void given_a_tainted_way_and_no_node_references_partial_geometry_is_rendered()
        {
            var json = "{\n elements: [\n {\n type: \"way\",\n id: 1,\n geometry: [\n null,\n { lat: 1, lon: 2 },\n { lat: 2, lon: 2 },\n null\n ]\n }\n ]\n }";
            var converter = new Converter();
            var featureCollection = converter.OsmToFeatureCollection(json);
            Assert.That(featureCollection.Features.Count == 1);
            var wayFeature = featureCollection.Features[0];
            Assert.That(wayFeature.Id == "way/1");
            Assert.That(wayFeature.Geometry.Type == GeoJSONObjectType.LineString);
        }


        
         
    }
}