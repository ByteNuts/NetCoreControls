using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using ByteNuts.NetCoreControls.Models.Repeater;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.DataProtection;
using ByteNuts.NetCoreControls.Core;
using ByteNuts.NetCoreControls.Core.Models;
using ByteNuts.NetCoreControls.Core.Models.Enums;
using ByteNuts.NetCoreControls.Core.Services;

namespace ByteNuts.NetCoreControls.Controls.Repeater
{
    [RestrictChildren("ncc:repeater-headertemplate", "ncc:repeater-itemtemplate", "ncc:repeater-footertemplate")]
    [HtmlTargetElement("ncc:repeater")]
    public class RepeaterTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("Context")]
        public NccRepeaterContext Context { get; set; }


        [HtmlAttributeName("AutoGenerateEditButton")]
        public bool AutoGenerateEditButton { get; set; }


        private NccRepeaterTagContext _nccTagContext;
        private IDataProtector _protector;
        private readonly NccSettings _nccSettings;
        protected IHtmlGenerator Generator { get; }

        public RepeaterTagHelper(IDataProtectionProvider protector, IHtmlGenerator generator, IHttpContextAccessor contextAccessor)
        {
            var options = NccReflectionService.NccGetClassInstanceWithDi(contextAccessor.HttpContext, NccConstants.OptionsAssemblyName);
            _nccSettings = options != null ? ((IOptions<NccSettings>)options).Value : new NccSettings();

            _protector = protector.CreateProtector(_nccSettings.DataProtectionKey);
            Generator = generator;
        }

        public override void Init(TagHelperContext tagContext)
        {
            _nccTagContext = new NccRepeaterTagContext();
            tagContext.Items.Add(typeof(NccRepeaterTagContext), _nccTagContext);
        }

        public override async Task ProcessAsync(TagHelperContext tagContext, TagHelperOutput output)
        {
            if (Context == null) throw new Exception("The NccRepeaterContext is null... Please check the reference.");

            object service = null;

            if (!string.IsNullOrEmpty(Context.EventHandlerClass)) service = NccReflectionService.NccGetClassInstance(Context.EventHandlerClass, null);

            service?.NccInvokeMethod(NccEventsEnum.Load.ToString(), new object[] { new NccEventArgs { NccTagContext = _nccTagContext, NccControlContext = Context, ViewContext = ViewContext } });

            //Get grid id and share it with siblings parents
            if (string.IsNullOrEmpty(Context.Id))
                Context.Id = Guid.NewGuid().ToString();

            output.TagName = "div";
            output.Attributes.SetAttribute("id", Context.Id);
            output.Attributes.SetAttribute("style", "position:relative");

            if (Context.Visible)
            {
                tagContext.Items.Add(typeof(NccRepeaterContext), Context);

                //NccActionsService.ExtraParameters<NccRepeaterContext> setExtraParameters = RepeaterService.GetExtraParameters;
                //NccActionsService.DataResult<NccRepeaterContext> setDataResult = RepeaterService.SetDataResult;
                NccControlsService.BindData(Context, ViewContext.HttpContext, null, null);

                await output.GetChildContentAsync();

            }

            var content = "";
            foreach (var repeaterItem in _nccTagContext.RepeaterItems)
            {
                content += repeaterItem;
            }

            output.Content.AppendHtml(_nccTagContext.RepeaterHeader);
            output.Content.AppendHtml(content);
            output.Content.AppendHtml(_nccTagContext.RepeaterFooter);

            service?.NccInvokeMethod(NccEventsEnum.PreRender.ToString(), new object[] { new NccEventArgs { NccTagContext = _nccTagContext, NccControlContext = Context, ViewContext = ViewContext } });

            _nccTagContext.ControlContext = Context;

            output.PostContent.AppendHtml(NccControlsService.GetEncodedContext(_protector, Context.Id, Context));
            output.PostContent.AppendHtml(NccControlsService.GetAjaxLoaderOverlay());
            output.PostContent.AppendHtml(NccControlsService.GetAjaxLoaderImage());
        }
    }
}
