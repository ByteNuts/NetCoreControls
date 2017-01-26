using System;
using System.Threading.Tasks;
using ByteNuts.NetCoreControls.Helpers;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ByteNuts.NetCoreControls.Services;
using ByteNuts.NetCoreControls.Models;
using ByteNuts.NetCoreControls.Extensions;

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

        private IDataProtector _protector;
        private IRazorViewEngine _razorViewEngine;

        public override async Task ProcessAsync(TagHelperContext tagContext, TagHelperOutput output)
        {
            try
            {
                _protector = ((IDataProtectionProvider)ViewContext.HttpContext.RequestServices.GetService(typeof(IDataProtectionProvider))).CreateProtector(Constants.DataProtectionKey);
                _razorViewEngine = (IRazorViewEngine)ViewContext.HttpContext.RequestServices.GetService(typeof(IRazorViewEngine));

                var controlViewPath = Context.NccGetPropertyValue<ViewsPathsModel>("ViewPaths").ViewPath;

                output.TagName = null;
                output.Content.SetHtmlContent(await ViewContext.NccRenderToStringAsync(_razorViewEngine, controlViewPath, null));
            }
            catch (Exception e)
            {
                var error = EventService.ProcessError(e);
                var id = Context.NccGetPropertyValue<string>("Id");
                if (!string.IsNullOrEmpty(id))
                {
                    output.TagName = "div";
                    output.Attributes.SetAttribute("id", id);

                    Context.NccSetPropertyValue("Error", error);
                    ViewContext.ViewData[id] = Context;

                    var encContext = new TagBuilder("input");
                    encContext.Attributes.Add("name", "encContext");
                    encContext.Attributes.Add("id", $"{id}_context");
                    encContext.Attributes.Add("type", "hidden");
                    encContext.Attributes.Add("value", _protector.Protect(NccJson.SetObjectAsJson(Context)));
                    output.PostContent.AppendHtml(encContext);

                }
                output.Content.SetHtmlContent(error);
            }
        }
    }
}
