using System.Collections.Generic;
using ByteNuts.NetCoreControls.Models.HtmlRender;

namespace ByteNuts.NetCoreControls.Services
{
    public static class HtmlRenderService
    {
        public static IDictionary<string, object> GetExtraParameters(IDictionary<string, object> callParams, HtmlRenderContext context)
        {

            return callParams;
        }

        public static HtmlRenderContext SetDataResult(HtmlRenderContext context, object result)
        {
            return context;
        }
    }
}
