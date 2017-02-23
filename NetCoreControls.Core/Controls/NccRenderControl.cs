using System;
using System.Threading.Tasks;
using ByteNuts.NetCoreControls.Core;
using ByteNuts.NetCoreControls.Core.Extensions;
using ByteNuts.NetCoreControls.Core.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ByteNuts.NetCoreControls.Core.Services;

namespace ByteNuts.NetCoreControls.Controls
{
    [HtmlTargetElement("ncc:render-control")]
    public class NccRenderControl : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("Context")]
        public object Context { get; set; }

        private readonly IDataProtector _protector;
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly NccSettings _nccSettings;

        public NccRenderControl(IDataProtectionProvider protector, IRazorViewEngine razorViewEngine, IHttpContextAccessor contextAccessor)
        {
            var options = NccReflectionService.NccGetClassInstanceWithDi(contextAccessor.HttpContext, NccConstants.OptionsAssemblyName);
            _nccSettings = options != null ? ((IOptions<NccSettings>)options).Value : new NccSettings();

            _protector = protector.CreateProtector(_nccSettings.DataProtectionKey);
            _razorViewEngine = razorViewEngine;
        }

        public override async Task ProcessAsync(TagHelperContext tagContext, TagHelperOutput output)
        {
            try
            {
                var controlViewPath = Context.NccGetPropertyValue<ViewsPathsModel>("ViewPaths").ViewPath;

                output.TagName = null;
                output.Content.SetHtmlContent(await ViewContext.NccRenderToStringAsync(_razorViewEngine, controlViewPath, null));
            }
            catch (Exception e)
            {
                var error = NccEventService.ProcessError(e);
                var id = Context.NccGetPropertyValue<string>("Id");
                if (!string.IsNullOrEmpty(id))
                {
                    output.TagName = "div";
                    output.Attributes.SetAttribute("id", id);

                    //Context.NccSetPropertyValue("Error", error);
                    ViewContext.ViewData[id] = Context;

                    var encContext = new TagBuilder("input");
                    encContext.Attributes.Add("name", "encContext");
                    encContext.Attributes.Add("id", $"{id}_context");
                    encContext.Attributes.Add("type", "hidden");
                    encContext.Attributes.Add("value", _protector.Protect(NccJsonService.SetObjectAsJson(Context)));
                    output.PostContent.AppendHtml(encContext);

                }
                output.Content.SetHtmlContent(error);
            }
        }
    }
}
