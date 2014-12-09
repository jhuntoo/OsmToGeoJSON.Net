using System.Collections.Generic;
using System.Linq;
using GeoJSON.Net;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace OsmToGeoJSON.Tests
{
    public abstract class GeometryTestBase
    {
        protected void AssertFeatureLineString(Feature feature, List<Coordinates> coordinateSetList)
        {
            var coordinates = ((LineString)feature.Geometry).Coordinates; ;

            for (int i = 0; i < coordinateSetList.Count; i++)
            {


                Assert.That(coordinateSetList[i].Lat == ((GeographicPosition)coordinates[i]).Latitude);
                Assert.That(coordinateSetList[i].Lon == ((GeographicPosition)coordinates[i]).Longitude);

            }

        }



        protected void AssertPropertiesAreCorrect(Feature feature, string type, string id, Dictionary<string, object> tags = null, TestRelation[] relations = null, Dictionary<string, object> meta = null, bool isTainted = false)
        {
            var expectedPropertyCount = 5;
            if (isTainted)
            {
                Assert.That(((bool)feature.Properties["tainted"])== true);
                expectedPropertyCount = 6;
            }
            Assert.That(feature.Properties.Count == expectedPropertyCount);
            Assert.That(feature.Properties["type"].ToString() == type);
            Assert.That(feature.Properties["id"].ToString() == id);
            AssertMetaAreEqual(feature.Properties["meta"].ToString(), meta);
            AssertPropertyRelations(feature.Properties["relations"].ToString(), relations);
            AssertTagsAreEqual(feature.Properties["tags"].ToString(), tags);

        }

        protected void AssertPropertyRelations(string actualJson, TestRelation[] relations)
        {
            var settings = new JsonSerializerSettings { Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() };
            if (relations == null) relations = new TestRelation[0];
            var normalizedActualJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<object[]>(actualJson, settings), settings);
            var normalizedExpectedJson = JsonConvert.SerializeObject(relations, settings);
            Assert.That(normalizedActualJson, Is.EqualTo(normalizedExpectedJson));
        }

        protected void AssertTagsAreEqual(string actualJson, Dictionary<string, object> expectedTags)
        {
            var settings = new JsonSerializerSettings { Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() };
            if (expectedTags == null) expectedTags = new Dictionary<string, object>();
            var normalizedActualJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<Dictionary<string, object>>(actualJson, settings), settings);
            var normalizedExpectedJson = JsonConvert.SerializeObject(new Dictionary<string, object>(expectedTags.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value)), settings);
            Assert.That(normalizedActualJson, Is.EqualTo(normalizedExpectedJson));
        }

        protected void AssertMetaAreEqual(string actualJson, Dictionary<string, object> expectedMeta)
        {
            var settings = new JsonSerializerSettings { Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() };
            if (expectedMeta == null) expectedMeta = new Dictionary<string, object>();
            var normalizedActualJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<Dictionary<string, object>>(actualJson, settings), settings);
            var normalizedExpectedJson = JsonConvert.SerializeObject(new Dictionary<string, object>(expectedMeta.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => (object)x.Value)), settings);
            Assert.That(normalizedActualJson, Is.EqualTo(normalizedExpectedJson));
        }

        protected void AssertFeaturePoint(Feature feature, double lat, double lon)
        {
            Assert.That(feature.Geometry.Type == GeoJSONObjectType.Point);
            var position = ((GeographicPosition)((Point)feature.Geometry).Coordinates);
            Assert.That(position.Latitude == lat);
            Assert.That(position.Longitude == lon);
        }

        protected void AssertLineString(Feature feature, params Coordinates[] coordinates)
        {
            Assert.That(feature.Geometry.Type == GeoJSONObjectType.LineString);
            int index = 0;
            foreach (var coordinate in ((LineString)feature.Geometry).Coordinates.Select(c => (GeographicPosition)c))
            {
                Assert.That(coordinate.Latitude == coordinates[index].Lat);
                Assert.That(coordinate.Longitude == coordinates[index].Lon);
                index++;
            }
        }

        protected void AssertPolygon(Feature feature, params Coordinates[] coordinates)
        {
            Assert.That(feature.Geometry.Type == GeoJSONObjectType.Polygon);

            foreach (var lineStrings in ((Polygon)feature.Geometry).Coordinates.Select(c => c))
            {
                int index = 0;
                for (int i = 0; i < lineStrings.Coordinates.Count; i++)
                {
                    var coordinate = (GeographicPosition)lineStrings.Coordinates[i];
                    Assert.That(coordinate.Latitude == coordinates[index].Lat);
                    Assert.That(coordinate.Longitude == coordinates[index].Lon);
                    index++;
                }
            }
        }

        protected void AssertFeatureMultiPolygon(Feature feature, List<List<Coordinates>> coordinateSetList)
        {
            var coordinates = ((MultiPolygon)feature.Geometry).Coordinates;
            //Assert.That(coordinates.Count == 2);

            for (int i = 0; i < coordinateSetList.Count; i++)
            {
                var coordinateSet = coordinateSetList[i];
                for (int j = 0; j < coordinateSet.Count; j++)
                {
                    Assert.That(coordinateSet[j].Lat == ((GeographicPosition)(coordinates[i].Coordinates[0].Coordinates[j])).Latitude);
                    Assert.That(coordinateSet[j].Lon == ((GeographicPosition)(coordinates[i].Coordinates[0].Coordinates[j])).Longitude);
                }
            }

        }

        protected void AssertFeaturePolygon(Feature feature, List<List<Coordinates>> coordinateSetList)
        {
            var coordinates = ((Polygon)feature.Geometry).Coordinates;

            for (int i = 0; i < coordinateSetList.Count; i++)
            {
                var coordinateSet = coordinateSetList[i];
                for (int j = 0; j < coordinateSet.Count; j++)
                {
                    Assert.That(coordinateSet[j].Lat == ((GeographicPosition)coordinates[i].Coordinates[j]).Latitude);
                    Assert.That(coordinateSet[j].Lon == ((GeographicPosition)coordinates[i].Coordinates[j]).Longitude);
                }
            }

        }
    }
}