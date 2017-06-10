using System;
using System.Threading.Tasks;
using ByteNuts.NetCoreControls.Models.Enums;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using ByteNuts.NetCoreControls.Models.Grid;

namespace ByteNuts.NetCoreControls.Controls.Grid
{
    [RestrictChildren("ncc:grid-pagerrecordstemplate")]
    [HtmlTargetElement("ncc:grid-pager", ParentTag = "ncc:grid")]
    public class GridPagerTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        //[HtmlAttributeName("PagerNavigationSize")]
        //public int? PagerNavigationSize { get; set; }

        [HtmlAttributeName("ShowRecordsCount")]
        public bool? ShowRecordsCount { get; set; }

        [HtmlAttributeName("GridPagerPosition")]
        public NccGridPagerPositionEnum? GridPagerPosition { get; set; }

        [HtmlAttributeName("CssClassRecordCountDiv")]
        public string CssClassRecordCountDiv { get; set; }


        [HtmlAttributeName("CssClassPagerDiv")]
        public string CssClassPagerDiv { get; set; }

        [HtmlAttributeName("CssClassPagerUl")]
        public string CssClassPagerUl { get; set; }

        [HtmlAttributeName("CssClassPagerLi")]
        public string CssClassPagerLi { get; set; }

        [HtmlAttributeName("CssClassPagerA")]
        public string CssClassPagerA { get; set; }

        private NccGridTagContext _nccTagContext;
        private NccGridContext _context;


        public override void Init(TagHelperContext tagContext)
        {
            if (tagContext.Items.ContainsKey(typeof(NccGridTagContext)))
                _nccTagContext = (NccGridTagContext)tagContext.Items[typeof(NccGridTagContext)];
            else
                throw new Exception("GridViewNccTagContext was lost between tags...");
        }

        public override async Task ProcessAsync(TagHelperContext tagContext, TagHelperOutput output)
        {
            output.SuppressOutput();

            if (tagContext.Items.ContainsKey(typeof(NccGridContext)))
                _context = (NccGridContext)tagContext.Items[typeof(NccGridContext)];
            else
                return;

            _context.AllowPaging = true;
            if (_context.PageSize == int.MaxValue)
                _context.PageSize = 10;

            if (ShowRecordsCount.HasValue)
                _context.ShowRecordsCount = ShowRecordsCount.Value;
            if (GridPagerPosition.HasValue)
                _context.GridPagerPosition = GridPagerPosition.Value;

            _nccTagContext.CssClassRecordCountDiv = CssClassRecordCountDiv;
            _nccTagContext.CssClassPagerDiv = CssClassPagerDiv;
            _nccTagContext.CssClassPagerUl = CssClassPagerUl;
            _nccTagContext.CssClassPagerLi = CssClassPagerLi;
            _nccTagContext.CssClassPagerA = CssClassPagerA;

            await output.GetChildContentAsync();
        }
    }
}
