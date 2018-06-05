using Microsoft.AspNetCore.Builder;

namespace ByteNuts.NetCoreControls.Middleware
{
    public static class MiddlewareExtension
    {
        public static IApplicationBuilder UseFormSubmitRewriteMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UseNccMiddleware>();
        }
    }
}
