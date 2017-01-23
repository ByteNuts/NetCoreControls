using System;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using ByteNuts.NetCoreControls.Models.Grid;

namespace ByteNuts.NetCoreControls.Controls.Grid
{
    [HtmlTargetElement("ncc:grid-headertemplate", ParentTag = "ncc:grid-columntemplate")]
    public class GridHeadertemplateTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("CssClass")]
        public string CssClass { get; set; }

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

            var data = _context.DataObjects as IList;
            if (_nccTagContext.RowNumber > 0 || data == null || data.Count == 0)
                return;

            object showHeader = null;
            if (tagContext.Items.ContainsKey("ShowHeader"))
                showHeader = tagContext.Items["ShowHeader"];
            if (showHeader == null || (bool)showHeader)
            {
                var childContent = await output.GetChildContentAsync();

                _nccTagContext.GridHeader.Cells.Add(new GridCell { Value = childContent.GetContent(), CssClass = CssClass });

                tagContext.Items.Remove("ShowHeader");
            }
            else
            {
                _nccTagContext.GridHeader.Cells.Add(new GridCell { Value = "", CssClass = CssClass });
            }

        }
    }
}
