using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections;
using System.Threading.Tasks;
using ByteNuts.NetCoreControls.Core;
using ByteNuts.NetCoreControls.Core.Models;
using ByteNuts.NetCoreControls.Core.Models.Enums;
using ByteNuts.NetCoreControls.Core.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.DataProtection;
using ByteNuts.NetCoreControls.Models.Enums;
using ByteNuts.NetCoreControls.Models.Select;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Options;
using ByteNuts.NetCoreControls.Services;

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

        private IDataProtector _protector;
        private readonly NccSettings _nccSettings;

        public SelectTagHelper(IDataProtectionProvider protector, IHttpContextAccessor contextAccessor)
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
            if (Context == null) throw new Exception("The NccSelectContext is null... Please check the reference.");

            object service = null;

            if (!string.IsNullOrEmpty(Context.EventHandlerClass)) service = NccReflectionService.NccGetClassInstance(Context.EventHandlerClass, null);

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
                NccActionsService.ExtraParameters<NccSelectContext> setExtraParameters = SelectService.GetExtraParameters;
                NccActionsService.DataResult<NccSelectContext> setDataResult = SelectService.SetDataResult;
                NccControlsService.BindData(Context, ViewContext.HttpContext, setExtraParameters, setDataResult);

                output.Content.AppendHtml(await output.GetChildContentAsync());

                if (!string.IsNullOrEmpty(Context.FirstItem))
                {
                    var select = new TagBuilder("option")
                    {
                        Attributes = { { "value", "" } }
                    };
                    select.InnerHtml.SetContent(Context.FirstItem);

                    service?.NccInvokeMethod(NccSelectEventsEnum.OptionBound.ToString(), new object[] { new NccEventArgs { NccControlContext = Context, ViewContext = ViewContext, DataObjects = Context.DataObjects } });

                    output.Content.AppendHtml(select);
                }


                //var data = Context.DataObjects.GetType().ToString().Contains("Microsoft.EntityFrameworkCore.Internal.InternalDbSet") ||
                //    Context.DataObjects.GetType().ToString().Contains("Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryable") ?
                //    ((IQueryable<object>)Context.DataObjects).ToList() : Context.DataObjects as IList;
                var data = Context.DataObjects as IList;

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

                        service?.NccInvokeMethod(NccSelectEventsEnum.OptionBound.ToString(), new object[] { new NccEventArgs { NccControlContext = Context, ViewContext = ViewContext, DataObjects = Context.DataObjects } });

                        output.Content.AppendHtml(select);
                    }
                }
                else
                {
                    output.Attributes.Add("disabled", "disabled");
                }

                service?.NccInvokeMethod(NccEventsEnum.PreRender, new object[] { new NccEventArgs { NccControlContext = Context, ViewContext = ViewContext } });

                output.PostContent.AppendHtml(NccControlsService.GetEncodedContext(_protector, Context.Id, Context));
                output.PostContent.AppendHtml(NccControlsService.GetAjaxLoaderOverlay());
                output.PostContent.AppendHtml(NccControlsService.GetAjaxLoaderImage());


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
