using System.Collections.Generic;
using ByteNuts.NetCoreControls.Models.Select;

namespace ByteNuts.NetCoreControls.Services
{
    public static class SelectService
    {
        public static void GetExtraParameters(IDictionary<string, object> callParams, NccSelectContext context)
        {
            ;
        }

        public static NccSelectContext SetDataResult(NccSelectContext context, object result)
        {
            return context;
        }
    }
}
