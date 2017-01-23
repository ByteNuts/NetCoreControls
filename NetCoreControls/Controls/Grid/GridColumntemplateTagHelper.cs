using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ByteNuts.NetCoreControls.Models.Grid;

namespace ByteNuts.NetCoreControls.Controls.Grid
{
    [HtmlTargetElement("ncc:grid-columntemplate", ParentTag = "ncc:grid-columns")]
    [RestrictChildren("ncc:grid-headertemplate", "ncc:grid-itemtemplate")]
    public class GridColumntemplateTagHelper : TagHelper
    {
        [HtmlAttributeName("ShowHeader")]
        public bool ShowHeader { get; set; } = true;

        [HtmlAttributeName("Visible")]
        public bool Visible { get; set; } = true;

        private GridNccTagContext _nccTagContext;


        public override void Init(TagHelperContext tagContext)
        {
            if (tagContext.Items.ContainsKey(typeof(GridNccTagContext)))
                _nccTagContext = (GridNccTagContext)tagContext.Items[typeof(GridNccTagContext)];
            else
                throw new Exception("GridViewNccTagContext was lost between tags...");
        }

        public override async Task ProcessAsync(TagHelperContext tagContext, TagHelperOutput output)
        {
            if (!tagContext.Items.ContainsKey(typeof(GridContext)))
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
