using Microsoft.AspNetCore.Http;

namespace ByteNuts.NetCoreControls.Core.Extensions
{
    public static class HttpRequestExtensions
    {
        public static string NccGetBaseUrl(this HttpContext context)
        {
            var request = context.Request;

            var host = request.Host.ToUriComponent();

            var pathBase = request.PathBase.ToUriComponent();

            var baseUrl = string.Format("{0}://{1}{2}", request.Scheme, host, pathBase);

            return baseUrl;
        }
    }
}
