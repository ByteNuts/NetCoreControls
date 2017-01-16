using Newtonsoft.Json;

namespace ByteNuts.NetCoreControls.Services
{
    public static class JsonService
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        public static string SetObjectAsJson(object value)
        {
            return JsonConvert.SerializeObject(value, JsonSerializerSettings);
        }

        public static T GetObjectFromJson<T>(string value)
        {
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value, JsonSerializerSettings);
        }
    }
}
