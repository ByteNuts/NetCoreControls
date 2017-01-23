using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ByteNuts.NetCoreControls.Models.Enums;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using ByteNuts.NetCoreControls.Models.Grid;

namespace ByteNuts.NetCoreControls.Controls.Grid
{
    [HtmlTargetElement("ncc:grid-content", ParentTag = "ncc:grid")]
    public class GridContentTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }


        [HtmlAttributeName("ContentType")]
        public GridViewHtmlContentType ContentType { get; set; }


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
            output.TagName = null;
            if (!tagContext.Items.ContainsKey(typeof(GridContext)))
            {
                output.SuppressOutput();
                return;
            }

            switch (ContentType)
            {
                case GridViewHtmlContentType.PreContent:
                    _nccTagContext.PreContent += (await output.GetChildContentAsync()).GetContent();
                    break;
                case GridViewHtmlContentType.PostContent:
                    _nccTagContext.PostContent += (await output.GetChildContentAsync()).GetContent();
                    break;
                default:
                    _nccTagContext.PostContent += (await output.GetChildContentAsync()).GetContent();
                    break;
            }
        }
    }
}
