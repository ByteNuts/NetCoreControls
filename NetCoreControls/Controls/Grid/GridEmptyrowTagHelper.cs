using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using ByteNuts.NetCoreControls.Models.Grid;

namespace ByteNuts.NetCoreControls.Controls.Grid
{
    [HtmlTargetElement("ncc:grid-emptyrow", ParentTag = "ncc:grid")]
    public class GridEmptyrowTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("RowCssClass")]
        public string RowCssClass { get; set; }

        [HtmlAttributeName("CellCssClass")]
        public string CellCssClass { get; set; }

        private GridNccTagContext _nccTagContext;
        private GridContext _context;


        public override void Init(TagHelperContext tagContext)
        {
            if (tagContext.Items.ContainsKey(typeof(GridNccTagContext)))
                _nccTagContext = (GridNccTagContext)tagContext.Items[typeof(GridNccTagContext)];
            else
                throw new Exception("GridViewNccTagContext was lost between tags...");
        }

        public override async Task ProcessAsync(TagHelperContext tagContext, TagHelperOutput output)
        {
            output.SuppressOutput();

            if (tagContext.Items.ContainsKey(typeof(GridContext)))
                _context = (GridContext)tagContext.Items[typeof(GridContext)];
            else
                return;

            var childContent = await output.GetChildContentAsync();

            _nccTagContext.EmptyRow = new GridRow
            {
                CssClass = RowCssClass,
                Cells = new List<GridCell>
                {
                    new GridCell
                    {
                        Value = childContent.GetContent(),
                        CssClass = CellCssClass,
                        Aggregate = false
                    }
                }
            };
        }
    }
}
