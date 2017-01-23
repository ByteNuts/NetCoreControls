using System;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using ByteNuts.NetCoreControls.Models.Grid;

namespace ByteNuts.NetCoreControls.Controls.Grid
{
    [HtmlTargetElement("ncc:grid-pager", ParentTag = "ncc:grid", TagStructure = TagStructure.WithoutEndTag)]
    public class GridPagerTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("PagerNavigationSize")]
        public int PagerNavigationSize { get; set; }

        private GridNccTagContext _nccTagContext;
        private GridContext _context;


        public override void Init(TagHelperContext tagContext)
        {
            if (tagContext.Items.ContainsKey(typeof(GridNccTagContext)))
                _nccTagContext = (GridNccTagContext)tagContext.Items[typeof(GridNccTagContext)];
            else
                throw new Exception("GridViewNccTagContext was lost between tags...");
        }

        public override void Process(TagHelperContext tagContext, TagHelperOutput output)
        {
            output.SuppressOutput();

            if (tagContext.Items.ContainsKey(typeof(GridContext)))
                _context = (GridContext)tagContext.Items[typeof(GridContext)];
            else
                return;

            _context.AllowPaging = true;
            if (_context.PageSize == int.MaxValue)
                _context.PageSize = 10;
        }


    }
}
