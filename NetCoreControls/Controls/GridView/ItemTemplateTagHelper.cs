using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using ByteNuts.NetCoreControls.Models.GridView;

namespace ByteNuts.NetCoreControls.Controls.GridView
{
    [HtmlTargetElement("ncc:ItemTemplate", ParentTag = "ncc:TemplateField")]
    [HtmlTargetElement("ncc:item-template", ParentTag = "ncc:template-field")]
    public class ItemTemplateTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("CssClass")]
        public string CssClass { get; set; }

        [HtmlAttributeName("Aggregate")]
        public bool Aggregate { get; set; }

        private GridViewNccTagContext _nccTagContext;
        private GridViewContext _context;

        public override void Init(TagHelperContext tagContext)
        {
            if (tagContext.Items.ContainsKey(typeof(GridViewNccTagContext)))
                _nccTagContext = (GridViewNccTagContext)tagContext.Items[typeof(GridViewNccTagContext)];
            else
            {
                _nccTagContext = new GridViewNccTagContext();
                tagContext.Items.Add(typeof(GridViewNccTagContext), _nccTagContext);
            }
        }

        public override async Task ProcessAsync(TagHelperContext tagContext, TagHelperOutput output)
        {
            output.SuppressOutput();

            if (tagContext.Items.ContainsKey(typeof(GridViewContext)))
                _context = (GridViewContext)tagContext.Items[typeof(GridViewContext)];
            else
                return;

            var data = _context.DataObjects as IList;
            if (data == null || data.Count == 0)
                return;

            var childContent = await output.GetChildContentAsync();

            var cell = new GridViewCell
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
