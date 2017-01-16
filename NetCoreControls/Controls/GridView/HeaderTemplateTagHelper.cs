using System;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using ByteNuts.NetCoreControls.Models.GridView;

namespace ByteNuts.NetCoreControls.Controls.GridView
{
    [HtmlTargetElement("ncc:HeaderTemplate", ParentTag = "ncc:TemplateField")]
    [HtmlTargetElement("ncc:header-template", ParentTag = "ncc:template-field")]
    public class HeaderTemplateTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("CssClass")]
        public string CssClass { get; set; }

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

            var data = _context.DataObjects as IList;
            if (ViewContext.ViewBag.RowCount > 0 || data == null || data.Count == 0)
                return;

            object showHeader = null;
            if (tagContext.Items.ContainsKey("ShowHeader"))
                showHeader = tagContext.Items["ShowHeader"];
            if (showHeader == null || (bool)showHeader)
            {
                var childContent = await output.GetChildContentAsync();

                _nccTagContext.GridHeader.Cells.Add(new GridViewCell { Value = childContent.GetContent(), CssClass = CssClass });

                tagContext.Items.Remove("ShowHeader");
            }
            else
            {
                _nccTagContext.GridHeader.Cells.Add(new GridViewCell { Value = "", CssClass = CssClass });
            }

        }
    }
}
