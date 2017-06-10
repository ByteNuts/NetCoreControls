using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ByteNuts.NetCoreControls.Core;
using ByteNuts.NetCoreControls.Core.Models;
using ByteNuts.NetCoreControls.Core.Models.Enums;
using ByteNuts.NetCoreControls.Core.Services;
using ByteNuts.NetCoreControls.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ByteNuts.NetCoreControls.Models.Grid;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

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

        [HtmlAttributeName("CssClassTable")]
        public string CssClassTable { get; set; }

        [HtmlAttributeName("CssClassHeader")]
        public string CssClassHeader { get; set; }

        [HtmlAttributeName("CssClassBody")]
        public string CssClassBody { get; set; }

        [HtmlAttributeName("CssClassFooter")]
        public string CssClassFooter { get; set; }

        [HtmlAttributeName("CssClassHeaderContainer")]
        public string CssClassHeaderContainer { get; set; }

        [HtmlAttributeName("CssClassTableContainer")]
        public string CssClassTableContainer { get; set; }

        [HtmlAttributeName("CssClassFooterContainer")]
        public string CssClassFooterContainer { get; set; }

        [HtmlAttributeName("RenderForm")]
        public bool RenderForm { get; set; }

        [HtmlAttributeName("AllowPaging")]
        public bool AllowPaging { get; set; }

        [HtmlAttributeName("PageSize")]
        public int PageSize { get; set; }

        [HtmlAttributeName("PagerNavSize")]
        public int PagerNavSize { get; set; }

        [HtmlAttributeName("AutoGenerateEditButton")]
        public bool AutoGenerateEditButton { get; set; }


        private NccGridTagContext _nccTagContext;
        private IDataProtector _protector;
        private readonly NccSettings _nccSettings;
        protected IHtmlGenerator Generator { get; }

        public GridTagHelper(IDataProtectionProvider protector, IHtmlGenerator generator, IHttpContextAccessor contextAccessor)
        {
            var options = NccReflectionService.NccGetClassInstanceWithDi(contextAccessor.HttpContext, NccConstants.OptionsAssemblyName);
            _nccSettings = options != null ? ((IOptions<NccSettings>)options).Value : new NccSettings();

            _protector = protector.CreateProtector(_nccSettings.DataProtectionKey);
            Generator = generator;
        }

        public override void Init(TagHelperContext tagContext)
        {
            _nccTagContext = new NccGridTagContext();
            tagContext.Items.Add(typeof(NccGridTagContext), _nccTagContext);
        }

        public override async Task ProcessAsync(TagHelperContext tagContext, TagHelperOutput output)
        {
            if (Context == null) throw new Exception("The NccGridContext is null... Please check the reference.");

            _nccTagContext.CssClassGrid = CssClassTable;
            _nccTagContext.CssClassBody = CssClassBody;
            _nccTagContext.CssClassHeader = CssClassHeader;
            _nccTagContext.CssClassFooter = CssClassFooter;
            _nccTagContext.CssClassHeaderContainer = CssClassHeaderContainer;
            _nccTagContext.CssClassTableContainer = CssClassTableContainer;
            _nccTagContext.CssClassFooterContainer = CssClassFooterContainer;

            if (!string.IsNullOrEmpty(DataKeys) && DataKeys != Context.DataKeys)
                Context.DataKeys = DataKeys;

            Context.AutoGenerateEditButton = AutoGenerateEditButton ? AutoGenerateEditButton : Context.AutoGenerateEditButton;
            Context.AllowPaging = AllowPaging;
            if (AllowPaging)
            {
                Context.PagerNavSize = PagerNavSize > 0 ? PagerNavSize : 5;
                if (Context.Filters.ContainsKey("pageSize"))
                    Context.PageSize = Convert.ToInt32(Context.Filters["pageSize"]);
                else if (PageSize > 0)
                    Context.PageSize = PageSize;
                if (Context.PageSize == 0 || Context.PageSize == int.MaxValue)
                    Context.PageSize = 10;
            }
            else
                Context.PageSize = int.MaxValue;

            if (Context.Filters.ContainsKey("pageNumber"))
                Context.PageNumber = Convert.ToInt32(Context.Filters["pageNumber"]);

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
                tagContext.Items.Add(typeof(NccGridContext), Context);

                if (RenderForm)
                {
                    var form = new TagBuilder("form") { TagRenderMode = TagRenderMode.StartTag };

                    output.PreContent.AppendHtml(form);
                }

                NccActionsService.ExtraParameters<NccGridContext> setExtraParameters = GridService.GetExtraParameters;
                NccActionsService.DataResult<NccGridContext> setDataResult = GridService.SetDataResult;
                NccControlsService.BindData(Context, ViewContext.HttpContext, setExtraParameters, setDataResult);

                _nccTagContext.GridHeader = new GridRow { Cells = new List<GridCell>(), CssClass = CssClassHeader };

                await output.GetChildContentAsync();

                var tableContainerDiv = new TagBuilder("div");
                if (!string.IsNullOrEmpty(_nccTagContext.CssClassTableContainer))
                    tableContainerDiv.Attributes.Add("class", _nccTagContext.CssClassTableContainer);

                tableContainerDiv.InnerHtml.AppendHtml(GridService.BuildTableHtml(_nccTagContext, Context));

                output.Content.SetHtmlContent(tableContainerDiv);

                if (Context.AllowPaging)
                    output.Content.AppendHtml(GridService.BuildTablePager(_nccTagContext, Context));

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

            service?.NccInvokeMethod(NccEventsEnum.PreRender.ToString(), new object[] { new NccEventArgs { NccTagContext = _nccTagContext, NccControlContext = Context, ViewContext = ViewContext } });

            _nccTagContext.ControlContext = Context;

            output.PostContent.AppendHtml(NccControlsService.GetEncodedContext(_protector, Context.Id, Context));
            output.PostContent.AppendHtml(NccControlsService.GetAjaxLoaderOverlay());
            output.PostContent.AppendHtml(NccControlsService.GetAjaxLoaderImage());
        }
    }
}
