using System.Collections.Generic;
using System.Dynamic;
using Microsoft.AspNetCore.Routing;

namespace ByteNuts.NetCoreControls.Services
{
    public static class Extensions
    {
        public static ExpandoObject NccToExpando(this object anonymousObject)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (var item in new RouteValueDictionary(anonymousObject))
                expando.Add(item);
            return (ExpandoObject)expando;
        }

    }
}
