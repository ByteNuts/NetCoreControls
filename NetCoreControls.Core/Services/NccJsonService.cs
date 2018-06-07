using Newtonsoft.Json;

namespace ByteNuts.NetCoreControls.Core.Services
{
    public static class NccJsonService
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, PreserveReferencesHandling = PreserveReferencesHandling.Objects};
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
