using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using ByteNuts.NetCoreControls.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.DataProtection;
using ByteNuts.NetCoreControls.Models;
using ByteNuts.NetCoreControls.Models.Enums;
using ByteNuts.NetCoreControls.Models.Select;
using Microsoft.AspNetCore.Http;
using ByteNuts.NetCoreControls.Services;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Options;

namespace ByteNuts.NetCoreControls.Controls.Select
{
    [HtmlTargetElement("ncc:select")]
    public class SelectTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("Context")]
        public NccSelectContext Context { get; set; }

        [HtmlAttributeName("ncc-text-field")]
        public string TextValue { get; set; }

        [HtmlAttributeName("ncc-value-field")]
        public string DataValue { get; set; }

        [HtmlAttributeName("ncc-selected-value")]
        public string SelectedValue { get; set; }

        [HtmlAttributeName("ncc-first-item")]
        public string FirstItem { get; set; }

        [HtmlAttributeName("ncc-control-target")]
        public string ControlTarget { get; set; }

        private IDataProtector _protector;
        private readonly NccSettings _nccSettings;

        public SelectTagHelper(IDataProtectionProvider protector, IHttpContextAccessor contextAccessor)
        {
            var options = ReflectionService.NccGetClassInstanceWithDi(contextAccessor.HttpContext, Constants.OptionsAssemblyName);
            _nccSettings = options != null ? ((IOptions<NccSettings>)options).Value : new NccSettings();

            _protector = protector.CreateProtector(_nccSettings.DataProtectionKey);
        }

        public override void Init(TagHelperContext tagContext)
        {
        }

        public override async Task ProcessAsync(TagHelperContext tagContext, TagHelperOutput output)
        {
            if (Context == null) throw new Exception("The NccSelectContext is null... Please check the reference.");

            object service = null;

            if (!string.IsNullOrEmpty(Context.EventHandlerClass)) service = ReflectionService.NccGetClassInstance(Context.EventHandlerClass, null);

            service?.NccInvokeMethod(NccEventsEnum.Load, new object[] { new NccEventArgs { NccControlContext = Context, ViewContext = ViewContext } });

            if (string.IsNullOrEmpty(Context.Id))
                Context.Id = Guid.NewGuid().ToString();

            output.TagName = "select";
            output.Attributes.SetAttribute("id", Context.Id);

            if (!string.IsNullOrEmpty(DataValue))
                Context.DataValue = DataValue;
            if (string.IsNullOrEmpty(Context.DataValue)) throw new Exception($"Please set the DataValue property on NccSelect with id {Context.Id}.");

            if (!string.IsNullOrEmpty(TextValue))
                Context.TextValue = TextValue;
            if (string.IsNullOrEmpty(Context.TextValue)) throw new Exception($"Please set the TextValue property on NccSelect with id {Context.Id}.");

            if (!string.IsNullOrEmpty(FirstItem))
                Context.FirstItem = FirstItem;

            if (Context.Visible)
            {
                if (Context.AutoBind)
                {
                    Context = DataService.GetControlData(Context, ViewContext.HttpContext);

                    service?.NccInvokeMethod(NccEventsEnum.DataBound, new object[] { new NccEventArgs { NccControlContext = Context, ViewContext = ViewContext, DataObjects = Context.DataObjects } });
                }
                else
                {
                    Context.DataObjects = Context.DataSource.GetType().ToString().Contains("Microsoft.EntityFrameworkCore.Internal.InternalDbSet") ||
                        Context.DataSource.GetType().ToString().Contains("Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryable") ||
                        Context.DataSource.GetType().ToString().Contains("System.Linq.IQueryable") ?
                        ((IQueryable<object>)Context.DataSource).ToList() : Context.DataSource;
                }

                output.Content.AppendHtml(await output.GetChildContentAsync());

                if (!string.IsNullOrEmpty(Context.FirstItem))
                {
                    var select = new TagBuilder("option")
                    {
                        Attributes = { { "value", "" } }
                    };
                    select.InnerHtml.SetContent(Context.FirstItem);

                    service?.NccInvokeMethod(NccSelectEventsEnum.OptionBound, new object[] { new NccEventArgs { NccControlContext = Context, ViewContext = ViewContext, DataObjects = Context.DataObjects } });

                    output.Content.AppendHtml(select);
                }


                var data = Context.DataObjects.GetType().ToString().Contains("Microsoft.EntityFrameworkCore.Internal.InternalDbSet") ||
                    Context.DataObjects.GetType().ToString().Contains("Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryable") ?
                    ((IQueryable<object>)Context.DataObjects).ToList() : Context.DataObjects as IList;

                if (data?.Count > 0)
                {
                    foreach (var elem in data)
                    {
                        var val = elem.NccGetPropertyValue<string>(Context.DataValue);
                        var txt = elem.NccGetPropertyValue<string>(Context.TextValue);
                        var select = new TagBuilder("option")
                        {
                            Attributes = { {"value", val} }
                        };
                        select.InnerHtml.Append(txt);
                        if (val == SelectedValue)
                            select.Attributes.Add("selected", "selected");

                        service?.NccInvokeMethod(NccSelectEventsEnum.OptionBound, new object[] { new NccEventArgs { NccControlContext = Context, ViewContext = ViewContext, DataObjects = Context.DataObjects } });

                        output.Content.AppendHtml(select);
                    }
                }
                else
                {
                    output.Attributes.Add("disabled", "disabled");
                }

                service?.NccInvokeMethod(NccEventsEnum.PreRender, new object[] { new NccEventArgs { NccControlContext = Context, ViewContext = ViewContext } });

                var encContext = new TagBuilder("input");
                encContext.Attributes.Add("name", "encContext");
                encContext.Attributes.Add("id", $"{Context.Id}_context");
                encContext.Attributes.Add("type", "hidden");
                encContext.Attributes.Add("value", _protector.Protect(NccJson.SetObjectAsJson(Context)));
                output.PostContent.AppendHtml(encContext);

                var overlayAjaxLoader = new TagBuilder("div")
                {
                    Attributes = { { "class", "overlayAjaxLoader" } }
                };
                output.PostContent.AppendHtml(overlayAjaxLoader);

                var ajaxLoader = new TagBuilder("img")
                {
                    Attributes =
                {
                    {"class", "ajaxLoader"},
                    {"alt", "Loading..." },
                    {"src", $"data:image/png;base64,{Constants.AjaxLoaderImg}" }
                }
                };
                output.PostContent.AppendHtml(ajaxLoader);


                var divContainer = new TagBuilder("div")
                {
                    Attributes = { { "id", Context.Id }, { "style", "position:relative" } },
                    TagRenderMode = TagRenderMode.StartTag
                };
                output.PreElement.AppendHtml(divContainer);

                var endDivContainer = new TagBuilder("div")
                {
                    TagRenderMode = TagRenderMode.EndTag
                };
                output.PostElement.AppendHtml(endDivContainer);

            }
        }
    }
}
