using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Microsoft.AspNetCore.Routing;

namespace ByteNuts.NetCoreControls.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static ExpandoObject NccToExpando(this object anonymousObject)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (var item in new RouteValueDictionary(anonymousObject))
                expando.Add(item);
            return (ExpandoObject)expando;
        }

        public static string NccGetDictionaryValue(this Dictionary<string, string> dictionary, string key)
        {
            return dictionary.FirstOrDefault(x => x.Key.StartsWith(key.NccAddPrefix())).Value;
        }

        public static List<T> NccCreateListByType<T>(T obj)
        {
            return new List<T>();
        }
    }
}
