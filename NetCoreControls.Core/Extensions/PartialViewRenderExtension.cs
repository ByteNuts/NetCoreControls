using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ByteNuts.NetCoreControls.Core.Extensions
{
    public static class PartialViewRenderExtension
    {
        public static async Task<string> NccRenderToStringAsync(this Controller context, IRazorViewEngine razorViewEngine, string viewPath, object model)
        {
            using (var sw = new StringWriter())
            {
                var viewResult = razorViewEngine.GetView("", viewPath, false);

                if (viewResult.View == null)
                    throw new ArgumentNullException($"{viewPath} does not match any available view");

                if (model != null)
                    context.ViewData.Model = model;

                var viewContext = new ViewContext(
                    context.ControllerContext,
                    viewResult.View,
                    context.ViewData,
                    context.TempData,
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }

        public static async Task<string> NccRenderToStringAsync(this ViewContext context, IRazorViewEngine razorViewEngine, string viewPath, object model)
        {
            using (var sw = new StringWriter())
            {
                var viewResult = razorViewEngine.GetView("", viewPath, false);

                if (viewResult.View == null)
                    throw new ArgumentNullException($"{viewPath} does not match any available view");

                if (model != null)
                    context.ViewData.Model = model;

                var viewContext = new ViewContext(
                    context,
                    viewResult.View,
                    context.ViewData,
                    context.TempData,
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }
    }
}
