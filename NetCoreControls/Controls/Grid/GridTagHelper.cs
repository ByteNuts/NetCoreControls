using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ByteNuts.NetCoreControls.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ByteNuts.NetCoreControls.Helpers;
using ByteNuts.NetCoreControls.Models;
using ByteNuts.NetCoreControls.Models.Grid;

namespace ByteNuts.NetCoreControls.Controls.Grid
{
    [RestrictChildren("ncc:grid-columns", "ncc:grid-content", "ncc:grid-pager", "ncc:grid-emptyrow")]
    [HtmlTargetElement("ncc:grid")]
    public class GridTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("Context")]
        public NccGridContext Context { get; set; }

        [HtmlAttributeName("DataKeys")]
        public string DataKeys { get; set; }

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

        [HtmlAttributeName("AllowPaging")]
        public bool AllowPaging { get; set; }

        [HtmlAttributeName("PageSize")]
        public int PageSize { get; set; }

        [HtmlAttributeName("PagerNavSize")]
        public int PagerNavSize { get; set; }
        
        private NccGridTagContext _nccTagContext;
        private IDataProtector _protector;
        protected IHtmlGenerator Generator { get; }

        public GridTagHelper(IDataProtectionProvider protector, IHtmlGenerator generator)
        {
            _protector = protector.CreateProtector(Constants.DataProtectionKey);
            Generator = generator;
        }

        public override void Init(TagHelperContext tagContext)
        {
            _nccTagContext = new NccGridTagContext();
            tagContext.Items.Add(typeof(NccGridTagContext), _nccTagContext);
        }

        public override async Task ProcessAsync(TagHelperContext tagContext, TagHelperOutput output)
        {
            if (Context == null) throw new Exception("The GridViewContext is null... Please check the reference.");

            _nccTagContext.CssClassGrid = CssClass;
            _nccTagContext.CssClassBody = BodyCssClass;
            _nccTagContext.CssClassHeader = HeaderCssClass;
            _nccTagContext.CssClassFooter = FooterCssClass;

            if (!string.IsNullOrEmpty(DataKeys) && DataKeys != Context.DataKeys)
                Context.DataKeys = DataKeys;

            Context.AllowPaging = AllowPaging;
            if (AllowPaging)
            {
                Context.PagerNavSize = PagerNavSize;
                if (PageSize > 0)
                    Context.PageSize = PageSize;
                if (Context.PageSize == 0 || Context.PageSize == int.MaxValue)
                    Context.PageSize = 10;
            }
            else
                Context.PageSize = int.MaxValue;

            if (Context.Filters.ContainsKey("pageNumber"))
                Context.PageNumber = Convert.ToInt32(Context.Filters["pageNumber"]);

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
                tagContext.Items.Add(typeof(NccGridContext), Context);

                if (RenderForm)
                {
                    var form = new TagBuilder("form") { TagRenderMode = TagRenderMode.StartTag };

                    output.PreContent.AppendHtml(form);
                }

                if (Context.AutoBind)
                {
                    Context = DataService.GetControlData(Context, ViewContext.HttpContext);

                    service?.NccInvokeMethod(Models.Enums.NccEvents.DataBound, new object[] { new NccEventArgs { NccTagContext = _nccTagContext, NccControlContext = Context, ViewContext = ViewContext, DataObjects = Context.DataObjects } });
                }
                else
                    Context.DataObjects = Context.DataSource;

                _nccTagContext.GridHeader = new GridRow { Cells = new List<GridCell>(), CssClass = HeaderCssClass };

                await output.GetChildContentAsync();

                output.Content.SetHtmlContent(GridService.BuildTableHtml(_nccTagContext, Context));

                output.PreContent.AppendHtml(_nccTagContext.PreContent);
                output.PostContent.AppendHtml(_nccTagContext.PostContent);

                if (RenderForm)
                {
                    var antiforgeryTag = Generator.GenerateAntiforgery(ViewContext);
                    if (antiforgeryTag != null)
                    {
                        output.PostContent.AppendHtml(antiforgeryTag);
                    }
                    output.PostContent.AppendHtml(new TagBuilder("form") { TagRenderMode = TagRenderMode.EndTag });
                }
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
