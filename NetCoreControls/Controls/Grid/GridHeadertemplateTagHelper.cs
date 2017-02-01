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

            var data = _context.DataObjects as IList;
            if (_nccTagContext.RowNumber > 0 || data == null || data.Count == 0)
                return;

            object showHeader = null;
            if (tagContext.Items.ContainsKey("ShowHeader"))
                showHeader = tagContext.Items["ShowHeader"];
            if (showHeader == null || (bool)showHeader)
            {
                var childContent = await output.GetChildContentAsync();

                _nccTagContext.GridHeader.Cells.Add(new GridCell { Value = childContent, CssClass = CssClass });

                tagContext.Items.Remove("ShowHeader");
            }
            else
            {
                var cell = new GridCell();
                cell.Value.AppendHtml("");
                cell.CssClass = CssClass;
                _nccTagContext.GridHeader.Cells.Add(cell);
            }

        }
    }
}
