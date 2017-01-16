using System;
using System.Threading.Tasks;
using ByteNuts.NetCoreControls.Models;
using ByteNuts.NetCoreControls.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ByteNuts.NetCoreControls.Controls.HtmlRender
{
    [HtmlTargetElement("NCC:HtmlRender")]
    [HtmlTargetElement("ncc:html-render")]
    public class HtmlRenderTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("Context")]
        public NccContext Context { get; set; }

        [HtmlAttributeName("RenderForm")]
        public bool RenderForm { get; set; }

        [HtmlAttributeName("EventHandler")]
        public string EventHandler { get; set; }

        private IDataProtector _protector;

        public HtmlRenderTagHelper(IDataProtectionProvider protector)
        {
            _protector = protector.CreateProtector(Constants.DataProtectionKey);
        }

        public override void Init(TagHelperContext tagContext)
        {
        }

        public override async Task ProcessAsync(TagHelperContext tagContext, TagHelperOutput output)
        {
            if (!string.IsNullOrEmpty(EventHandler))
                Context.EventHandlerClass = EventHandler;

            object service = null;

            if (!string.IsNullOrEmpty(EventHandler))
                service = ReflectionService.NccGetClassInstance(EventHandler, null);

            service?.NccInvokeMethod("Load", new object[] { Context, ViewContext });

            //Get grid id and share it with siblings parents
            if (string.IsNullOrEmpty(Context.Id))
                Context.Id = Guid.NewGuid().ToString();

            output.TagName = "div";
            output.Attributes.SetAttribute("id", Context.Id);

            if (Context.Visible)
            {
                if (RenderForm)
                {
                    var form = new TagBuilder("form");
                    form.TagRenderMode = TagRenderMode.StartTag;

                    output.PreContent.AppendHtml(form);
                }

                //Get data from datasource

                Context = DataService.GetControlData<NccContext>(Context, ViewContext.HttpContext);

                //if (!string.IsNullOrEmpty(EventHandler))
                //    (service as IHtmlContentEvents)?.DataBound(Context.DataObjects as IEnumerable);

                ViewContext.ViewData.Model = Context.DataObjects;

                var content = await output.GetChildContentAsync();


                output.Content.SetHtmlContent(content.GetContent());


                if (RenderForm)
                    output.PostContent.AppendHtml("</form>");
            }


            //Context.ActionUrl = ViewContext.ViewBag.ActionUrlCollection?.Count > 0 ? ViewContext.ViewBag.ActionUrlCollection?.Peek() : null;
            //Context.ActionOption = ViewContext.ViewBag.ActionOptionCollection?.Count > 0 ? ViewContext.ViewBag.ActionOptionCollection?.Peek() : null;



            var encContext = new TagBuilder("input");
            encContext.Attributes.Add("name", "encContext");
            encContext.Attributes.Add("id", $"{Context.Id}_context");
            encContext.Attributes.Add("type", "hidden");
            encContext.Attributes.Add("value", _protector.Protect(JsonService.SetObjectAsJson(Context)));
            output.PostContent.AppendHtml(encContext);

        }
    }
}
