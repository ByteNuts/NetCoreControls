using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ByteNuts.NetCoreControls.Controls.GridView.Events;
using ByteNuts.NetCoreControls.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections;
using ByteNuts.NetCoreControls.Helpers;
using ByteNuts.NetCoreControls.Models;
using ByteNuts.NetCoreControls.Models.GridView;

namespace ByteNuts.NetCoreControls.Controls.GridView
{
    [RestrictChildren("ncc:Columns", "ncc:columns", "ncc:HtmlContent", "ncc:html-content")]
    [HtmlTargetElement("ncc:grid-view")]
    [HtmlTargetElement("NCC:GridView")]
    public class GridViewTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("Context")]
        public GridViewContext Context { get; set; }

        [HtmlAttributeName("CssClass")]
        public string CssClass { get; set; }

        [HtmlAttributeName("HeaderCssClass")]
        public string HeaderCssClass { get; set; }

        [HtmlAttributeName("BodyCssClass")]
        public string BodyCssClass { get; set; }

        [HtmlAttributeName("FooterCssClass")]
        public string FooterCssClass { get; set; }

        [HtmlAttributeName("RenderForm")]
        public bool RenderForm { get; set; }

        private GridViewNccTagContext _nccTagContext;
        private IDataProtector _protector;

        public GridViewTagHelper(IDataProtectionProvider protector)
        {
            _protector = protector.CreateProtector(Constants.DataProtectionKey);
        }

        public override void Init(TagHelperContext tagContext)
        {
            _nccTagContext = new GridViewNccTagContext();
            tagContext.Items.Add(typeof(GridViewNccTagContext), _nccTagContext);
        }

        public override async Task ProcessAsync(TagHelperContext tagContext, TagHelperOutput output)
        {
            if (Context == null) throw new Exception("The GridViewContext is null... Please check the reference.");

            _nccTagContext.CssClassGrid = CssClass;
            _nccTagContext.CssClassBody = BodyCssClass;
            _nccTagContext.CssClassHeader = HeaderCssClass;
            _nccTagContext.CssClassFooter = FooterCssClass;

            object service = null;

            if (!string.IsNullOrEmpty(Context.EventHandlerClass)) service = ReflectionService.NccGetClassInstance(Context.EventHandlerClass, null);

            service?.NccInvokeMethod(Models.Enums.NccEvents.Load, new object[] { new NccEventArgs { NccTagContext = _nccTagContext, NccControlContext = Context, ViewContext = ViewContext } });

            //Get grid id and share it with siblings parents
            if (string.IsNullOrEmpty(Context.Id))
                Context.Id = Guid.NewGuid().ToString();

            //_nccTagContext.GridId = Context.Id;

            output.TagName = "div";
            output.Attributes.SetAttribute("id", Context.Id);

            if (Context.Visible)
            {
                tagContext.Items.Add(typeof(GridViewContext), Context);

                if (RenderForm)
                {
                    var form = new TagBuilder("form");
                    form.TagRenderMode = TagRenderMode.StartTag;

                    output.PreContent.AppendHtml(form);
                }

                if (Context.PageSize == 0)
                    Context.PageSize = int.MaxValue;

                if (Context.AutoBind)
                {
                    Context = DataService.GetControlData(Context, ViewContext.HttpContext);

                    service?.NccInvokeMethod(Models.Enums.NccEvents.DataBound, new object[] { new NccEventArgs { NccTagContext = _nccTagContext, NccControlContext = Context, ViewContext = ViewContext, DataObjects = Context.DataObjects } });
                }
                else
                    Context.DataObjects = Context.DataSource;

                _nccTagContext.GridHeader = new GridViewRow { Cells = new List<GridViewCell>(), CssClass = HeaderCssClass };

                await output.GetChildContentAsync();

                output.Content.SetHtmlContent(GridViewService.BuildTableHtml(_nccTagContext));

                output.PreContent.AppendHtml(_nccTagContext.PreContent);
                output.PostContent.AppendHtml(_nccTagContext.PostContent);

                if (RenderForm)
                    output.PostContent.AppendHtml(new TagBuilder("form") { TagRenderMode = TagRenderMode.EndTag});
            }

            service?.NccInvokeMethod(Models.Enums.NccEvents.PreRender, new object[] { new NccEventArgs { NccTagContext = _nccTagContext, NccControlContext = Context, ViewContext = ViewContext } });

            _nccTagContext.ControlContext = Context;

            var encContext = new TagBuilder("input");
            encContext.Attributes.Add("name", "encContext");
            encContext.Attributes.Add("id", $"{Context.Id}_context");
            encContext.Attributes.Add("type", "hidden");
            encContext.Attributes.Add("value", _protector.Protect(NccJson.SetObjectAsJson(Context)));
            output.PostContent.AppendHtml(encContext);
        }
    }
}
