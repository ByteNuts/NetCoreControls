using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ByteNuts.NetCoreControls.Middleware
{
    public class UseNccMiddleware
    {
        private readonly RequestDelegate _next;

        public UseNccMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            //Present here for future usage!!

            await _next.Invoke(context);
        }
    }
}