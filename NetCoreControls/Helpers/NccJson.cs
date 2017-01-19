using Newtonsoft.Json;

namespace ByteNuts.NetCoreControls.Helpers
{
    public static class NccJson
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        public static string SetObjectAsJson(object value, bool includeType = true)
        {
            return includeType ? JsonConvert.SerializeObject(value, JsonSerializerSettings) : JsonConvert.SerializeObject(value);
        }

        public static T GetObjectFromJson<T>(string value)
        {
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value, JsonSerializerSettings);
        }
    }
}
