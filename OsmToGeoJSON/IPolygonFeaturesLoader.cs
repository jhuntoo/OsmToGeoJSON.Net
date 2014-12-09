using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace OsmToGeoJSON
{
    public interface IPolygonFeaturesLoader
    {
        Dictionary<string, object> Load();
    }

    public class PolygonFeaturesLoader : IPolygonFeaturesLoader
    {
        public Dictionary<string, object> Load()
        {
            return JsonConvert.DeserializeObject<Dictionary<string, object>>
            (File.ReadAllText("polygonFeatures.json"), new JsonConverter[] { new PolygonFeatureDictionaryConverter() });
        }
    }
}