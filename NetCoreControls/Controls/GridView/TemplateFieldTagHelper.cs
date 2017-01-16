using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ByteNuts.NetCoreControls.Models.GridView;

namespace ByteNuts.NetCoreControls.Controls.GridView
{
    [HtmlTargetElement("ncc:TemplateField", ParentTag = "ncc:Columns")]
    [HtmlTargetElement("ncc:template-field", ParentTag = "ncc:columns")]
    [RestrictChildren("ncc:HeaderTemplate", "ncc:header-template", "ncc:ItemTemplate", "ncc:item-template")]
    public class TemplateFieldTagHelper : TagHelper
    {
        [HtmlAttributeName("ShowHeader")]
        public bool ShowHeader { get; set; } = true;

        [HtmlAttributeName("Visible")]
        public bool Visible { get; set; } = true;

        private GridViewNccTagContext _nccTagContext;


        public override void Init(TagHelperContext tagContext)
        {
            if (tagContext.Items.ContainsKey(typeof(GridViewNccTagContext)))
                _nccTagContext = (GridViewNccTagContext)tagContext.Items[typeof(GridViewNccTagContext)];
            else
                throw new Exception("GridViewNccTagContext was lost between tags...");
        }

        public override async Task ProcessAsync(TagHelperContext tagContext, TagHelperOutput output)
        {
            if (!tagContext.Items.ContainsKey(typeof(GridViewContext)))
            {
                output.SuppressOutput();
                return;
            }

            if (Visible)
            {
                if (!_nccTagContext.ColCountComplete)
                    _nccTagContext.ColCount++;

                if (!ShowHeader)
                    tagContext.Items.Add("ShowHeader", false);

                await output.GetChildContentAsync();
            }

            output.TagName = null;
            output.SuppressOutput();
        }
    }
}
