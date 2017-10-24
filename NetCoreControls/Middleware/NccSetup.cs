using ByteNuts.NetCoreControls.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using ByteNuts.NetCoreControls.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ByteNuts.NetCoreControls.Middleware
{
    public static class NccSetup
    {
        public static void AddNetCoreControls(this IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.Configure<NccSettings>(options => configuration.GetSection("NccSettings").Bind(options));
        }
    }
}
