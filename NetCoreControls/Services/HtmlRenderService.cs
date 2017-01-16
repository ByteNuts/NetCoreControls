using System.Collections.Generic;
using ByteNuts.NetCoreControls.Models.HtmlRender;

namespace ByteNuts.NetCoreControls.Services
{
    public static class HtmlRenderService
    {
        public static Dictionary<string, object> GetExtraParameters(HtmlRenderContext context)
        {
            var extraParameters = new Dictionary<string, object>();

            return extraParameters;
        }

        public static HtmlRenderContext SetDataResult(HtmlRenderContext context, object result)
        {
            return context;
        }
    }
}
