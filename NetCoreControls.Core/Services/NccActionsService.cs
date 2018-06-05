using System.Collections.Generic;

namespace ByteNuts.NetCoreControls.Core.Services
{
    public static class NccActionsService
    {
        public delegate void ExtraParameters<T>(IDictionary<string, object> methodParams, T context);

        public delegate void DataResult<T>(T context, object result);

    }
}
