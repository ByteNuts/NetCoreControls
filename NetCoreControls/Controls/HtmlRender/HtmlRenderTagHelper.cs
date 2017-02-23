﻿using System;
using System.Threading.Tasks;
using ByteNuts.NetCoreControls.Core;
using ByteNuts.NetCoreControls.Core.Models;
using ByteNuts.NetCoreControls.Core.Models.Enums;
using ByteNuts.NetCoreControls.Core.Services;
using ByteNuts.NetCoreControls.Models.HtmlRender;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

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
        public NccHtmlRenderContext Context { get; set; }

        [HtmlAttributeName("RenderForm")]
        public bool RenderForm { get; set; }

        [HtmlAttributeName("EventHandler")]
        public string EventHandler { get; set; }

        private IDataProtector _protector;
        private readonly NccSettings _nccSettings;

        public HtmlRenderTagHelper(IDataProtectionProvider protector, IHttpContextAccessor contextAccessor)
        {
            var options = NccReflectionService.NccGetClassInstanceWithDi(contextAccessor.HttpContext, NccConstants.OptionsAssemblyName);
            _nccSettings = options != null ? ((IOptions<NccSettings>)options).Value : new NccSettings();

            _protector = protector.CreateProtector(_nccSettings.DataProtectionKey);
        }

        public override void Init(TagHelperContext tagContext)
        {
        }

        public override async Task ProcessAsync(TagHelperContext tagContext, TagHelperOutput output)
        {
            if (!string.IsNullOrEmpty(EventHandler))
                Context.EventHandlerClass = EventHandler;

            object service = null;

            if (!string.IsNullOrEmpty(Context.EventHandlerClass)) service = NccReflectionService.NccGetClassInstance(Context.EventHandlerClass, null);

            service?.NccInvokeMethod(NccEventsEnum.Load.ToString(), new object[] { new NccEventArgs { NccControlContext = Context, ViewContext = ViewContext } });


            //Get grid id and share it with siblings parents
            if (string.IsNullOrEmpty(Context.Id))
                Context.Id = Guid.NewGuid().ToString();

            if (Context.Visible)
            {
                if (RenderForm)
                {
                    var form = new TagBuilder("form");
                    form.TagRenderMode = TagRenderMode.StartTag;

                    output.PreContent.AppendHtml(form);
                }

                //Get data from datasource

                //DataService.GetControlData(Context, ViewContext.HttpContext);

                service?.NccInvokeMethod(NccEventsEnum.DataBound.ToString(), new object[] { new NccEventArgs { NccControlContext = Context, ViewContext = ViewContext, DataObjects = Context.DataObjects } });

                ViewContext.ViewData.Model = Context.DataObjects;

                var content = await output.GetChildContentAsync();


                output.Content.SetHtmlContent(content.GetContent());


                if (RenderForm)
                    output.PostContent.AppendHtml(new TagBuilder("form") { TagRenderMode = TagRenderMode.EndTag });
            }
            else
                output.SuppressOutput();

            service?.NccInvokeMethod(NccEventsEnum.PreRender.ToString(), new object[] { new NccEventArgs { NccControlContext = Context, ViewContext = ViewContext } });

            output.TagName = "div";
            output.Attributes.SetAttribute("id", Context.Id);

            var encContext = new TagBuilder("input");
            encContext.Attributes.Add("name", "encContext");
            encContext.Attributes.Add("id", $"{Context.Id}_context");
            encContext.Attributes.Add("type", "hidden");
            encContext.Attributes.Add("value", _protector.Protect(NccJsonService.SetObjectAsJson(Context)));
            output.PostContent.AppendHtml(encContext);

        }
    }
}
