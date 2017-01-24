using System.Collections.Generic;
using ByteNuts.NetCoreControls.Models.HtmlRender;

namespace ByteNuts.NetCoreControls.Services
{
    public static class HtmlRenderService
    {
        public static IDictionary<string, object> GetExtraParameters(IDictionary<string, object> callParams, NccHtmlRenderContext context)
        {

            return callParams;
        }

        public static NccHtmlRenderContext SetDataResult(NccHtmlRenderContext context, object result)
        {
            return context;
        }
    }
}
