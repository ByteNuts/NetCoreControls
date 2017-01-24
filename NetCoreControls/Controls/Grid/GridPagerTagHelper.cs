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

        private NccGridTagContext _nccTagContext;
        private NccGridContext _context;


        public override void Init(TagHelperContext tagContext)
        {
            if (tagContext.Items.ContainsKey(typeof(NccGridTagContext)))
                _nccTagContext = (NccGridTagContext)tagContext.Items[typeof(NccGridTagContext)];
            else
                throw new Exception("GridViewNccTagContext was lost between tags...");
        }

        public override void Process(TagHelperContext tagContext, TagHelperOutput output)
        {
            output.SuppressOutput();

            if (tagContext.Items.ContainsKey(typeof(NccGridContext)))
                _context = (NccGridContext)tagContext.Items[typeof(NccGridContext)];
            else
                return;


            _context.AllowPaging = true;
            if (_context.PageSize == int.MaxValue)
                _context.PageSize = 10;
        }


    }
}
