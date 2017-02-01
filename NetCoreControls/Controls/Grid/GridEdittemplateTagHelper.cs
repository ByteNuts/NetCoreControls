using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using ByteNuts.NetCoreControls.Models.Grid;

namespace ByteNuts.NetCoreControls.Controls.Grid
{
    [HtmlTargetElement("ncc:grid-edittemplate", ParentTag = "ncc:grid-columntemplate")]
    public class GridEdittemplateTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("CssClass")]
        public string CssClass { get; set; }

        [HtmlAttributeName("Aggregate")]
        public bool Aggregate { get; set; }

        private NccGridTagContext _nccTagContext;
        private NccGridContext _context;

        public override void Init(TagHelperContext tagContext)
        {
            if (tagContext.Items.ContainsKey(typeof(NccGridTagContext)))
                _nccTagContext = (NccGridTagContext)tagContext.Items[typeof(NccGridTagContext)];
            else
            {
                _nccTagContext = new NccGridTagContext();
                tagContext.Items.Add(typeof(NccGridTagContext), _nccTagContext);
            }
        }

        public override async Task ProcessAsync(TagHelperContext tagContext, TagHelperOutput output)
        {
            output.SuppressOutput();

            if (tagContext.Items.ContainsKey(typeof(NccGridContext)))
                _context = (NccGridContext)tagContext.Items[typeof(NccGridContext)];
            else
                return;

            if (_context.AdditionalData.ContainsKey("EditRowNumber") && _context.AdditionalData["EditRowNumber"].ToString() == _nccTagContext.RowNumber.ToString())
            {
                var data = _context.DataObjects as IList;
                if (data == null || data.Count == 0)
                    return;

                var childContent = await output.GetChildContentAsync();

                var cell = new GridCell
                {
                    Value = childContent,
                    CssClass = CssClass,
                    Aggregate = Aggregate
                };

                var row = _nccTagContext.GridRows.LastOrDefault();

                row?.Cells.Add(cell);
            }
        }
    }
}
