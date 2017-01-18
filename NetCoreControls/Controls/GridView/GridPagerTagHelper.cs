using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using ByteNuts.NetCoreControls.Models.GridView;

namespace ByteNuts.NetCoreControls.Controls.GridView
{
    [HtmlTargetElement("ncc:GridPager", ParentTag = "ncc:GridView", TagStructure = TagStructure.WithoutEndTag)]
    [HtmlTargetElement("ncc:grid-pager", ParentTag = "ncc:grid-view")]
    public class GridPagerTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }


        private GridViewNccTagContext _nccTagContext;
        private GridViewContext _context;


        public override void Init(TagHelperContext tagContext)
        {
            if (tagContext.Items.ContainsKey(typeof(GridViewNccTagContext)))
                _nccTagContext = (GridViewNccTagContext)tagContext.Items[typeof(GridViewNccTagContext)];
            else
                throw new Exception("GridViewNccTagContext was lost between tags...");
        }

        public override async Task ProcessAsync(TagHelperContext tagContext, TagHelperOutput output)
        {
            output.SuppressOutput();

            if (tagContext.Items.ContainsKey(typeof(GridViewContext)))
                _context = (GridViewContext)tagContext.Items[typeof(GridViewContext)];
            else
                return;

            var gridViewContext = (GridViewContext)tagContext.Items[typeof(GridViewContext)];

        }
    }
}
