using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using ByteNuts.NetCoreControls.Models.Grid;

namespace ByteNuts.NetCoreControls.Controls.Grid
{
    [HtmlTargetElement("ncc:grid-itemtemplate", ParentTag = "ncc:grid-columntemplate")]
    public class GridItemtemplateTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("CssClass")]
        public string CssClass { get; set; }

        [HtmlAttributeName("Aggregate")]
        public bool Aggregate { get; set; }

        private GridNccTagContext _nccTagContext;
        private GridContext _context;

        public override void Init(TagHelperContext tagContext)
        {
            if (tagContext.Items.ContainsKey(typeof(GridNccTagContext)))
                _nccTagContext = (GridNccTagContext)tagContext.Items[typeof(GridNccTagContext)];
            else
            {
                _nccTagContext = new GridNccTagContext();
                tagContext.Items.Add(typeof(GridNccTagContext), _nccTagContext);
            }
        }

        public override async Task ProcessAsync(TagHelperContext tagContext, TagHelperOutput output)
        {
            output.SuppressOutput();

            if (tagContext.Items.ContainsKey(typeof(GridContext)))
                _context = (GridContext)tagContext.Items[typeof(GridContext)];
            else
                return;

            var data = _context.DataObjects as IList;
            if (data == null || data.Count == 0)
                return;

            var childContent = await output.GetChildContentAsync();

            var cell = new GridCell
            {
                Value = childContent.GetContent(),
                CssClass = CssClass,
                Aggregate = Aggregate
            };

            var row = _nccTagContext.GridRows.LastOrDefault();

            row?.Cells.Add(cell);
        }
    }
}
