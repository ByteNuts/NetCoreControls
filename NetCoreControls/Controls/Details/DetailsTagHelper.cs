using System;
using System.Linq;
using System.Threading.Tasks;
using ByteNuts.NetCoreControls.Core;
using ByteNuts.NetCoreControls.Core.Models;
using ByteNuts.NetCoreControls.Core.Models.Enums;
using ByteNuts.NetCoreControls.Core.Services;
using ByteNuts.NetCoreControls.Models.Details;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace ByteNuts.NetCoreControls.Controls.Details
{
    [HtmlTargetElement("NCC:Details")]
    [HtmlTargetElement("ncc:details")]
    public class DetailsTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("Context")]
        public NccDetailsContext Context { get; set; }

        [HtmlAttributeName("DataKeys")]
        public string DataKeys { get; set; }

        [HtmlAttributeName("RenderForm")]
        public bool RenderForm { get; set; }

        [HtmlAttributeName("EventHandler")]
        public string EventHandler { get; set; }

        private IDataProtector _protector;
        private readonly NccSettings _nccSettings;

        public DetailsTagHelper(IDataProtectionProvider protector, IHttpContextAccessor contextAccessor)
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

            if (!string.IsNullOrEmpty(DataKeys) && DataKeys != Context.DataKeys)
                Context.DataKeys = DataKeys;

            object service = null;

            if (!string.IsNullOrEmpty(Context.EventHandlerClass)) service = NccReflectionService.NccGetClassInstance(Context.EventHandlerClass, null);

            service?.NccInvokeMethod(NccEventsEnum.Load.ToString(), new object[] { new NccEventArgs { NccControlContext = Context, ViewContext = ViewContext } });


            //Get grid id and share it with siblings parents
            if (string.IsNullOrEmpty(Context.Id))
                Context.Id = Guid.NewGuid().ToString();


            output.TagName = "div";
            output.Attributes.SetAttribute("id", Context.Id);
            output.Attributes.SetAttribute("style", "position:relative");

            if (Context.Visible)
            {
                if (RenderForm)
                {
                    var form = new TagBuilder("form");
                    form.TagRenderMode = TagRenderMode.StartTag;

                    output.PreContent.AppendHtml(form);
                }

                //Get data from datasource

                NccControlsService.BindData(Context, ViewContext.HttpContext, null, null);

                service?.NccInvokeMethod(NccEventsEnum.DataBound.ToString(), new object[] { new NccEventArgs { NccControlContext = Context, ViewContext = ViewContext, DataObjects = Context.DataObjects } });

                var data = Context.DataObjects.GetType().ToString().Contains("Microsoft.EntityFrameworkCore.Internal.InternalDbSet") ||
                    Context.DataObjects.GetType().ToString().Contains("Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryable") ?
                    ((IQueryable<object>)Context.DataObjects).FirstOrDefault() : Context.DataObjects as object;



                if (data != null)
                {
                    if (!string.IsNullOrEmpty(Context.DataKeys))
                    {
                        Context.DataKeysValues = new List<Dictionary<string, object>>();
                        var dataKeys = Context.DataKeys.Split(',')?.ToList();
                        var dataKeysRow = new Dictionary<string, object>();
                        foreach (var dataKey in dataKeys)
                        {
                            var keyValue = data.NccGetPropertyValue<object>(dataKey);
                            if (keyValue != null)
                                dataKeysRow[dataKey] = keyValue;
                        }
                        Context.DataKeysValues.Add(dataKeysRow);
                    }

                    ViewContext.ViewData.Model = data;

                    var content = await output.GetChildContentAsync();

                    output.Content.SetHtmlContent(content.GetContent());
                }

                if (RenderForm)
                    output.PostContent.AppendHtml(new TagBuilder("form") { TagRenderMode = TagRenderMode.EndTag });
            }
            else
                output.SuppressOutput();

            service?.NccInvokeMethod(NccEventsEnum.PreRender.ToString(), new object[] { new NccEventArgs { NccControlContext = Context, ViewContext = ViewContext } });


            output.PostContent.AppendHtml(NccControlsService.GetEncodedContext(_protector, Context.Id, Context));
            output.PostContent.AppendHtml(NccControlsService.GetAjaxLoaderOverlay());
            output.PostContent.AppendHtml(NccControlsService.GetAjaxLoaderImage());
        }
    }
}
