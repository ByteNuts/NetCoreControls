using System;
using System.Threading.Tasks;
using ByteNuts.NetCoreControls.Core.Extensions;
using ByteNuts.NetCoreControls.Models.Enums;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using ByteNuts.NetCoreControls.Models.Grid;

namespace ByteNuts.NetCoreControls.Controls.Grid
{
    [HtmlTargetElement("ncc:grid-pagerrecordstemplate", ParentTag = "ncc:grid-pager")]
    public class GridPagerRecordsTemplateTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

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

            ViewContext.ViewData.Model = new { NccPageSize = _context.PageSize, NccTotalRows = _context.TotalItems }.NccToExpando();

            var childContent = await output.GetChildContentAsync(false);

            _nccTagContext.PagerRecordsCountContent = childContent.GetContent();
        }
    }
}
