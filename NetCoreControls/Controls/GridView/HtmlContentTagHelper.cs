using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ByteNuts.NetCoreControls.Models.Enums;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using ByteNuts.NetCoreControls.Models.GridView;

namespace ByteNuts.NetCoreControls.Controls.GridView
{
    [HtmlTargetElement("ncc:HtmlContent", ParentTag = "ncc:GridView")]
    [HtmlTargetElement("ncc:html-content", ParentTag = "ncc:grid-view")]
    public class HtmlContentTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }


        [HtmlAttributeName("ContentType")]
        public GridViewHtmlContentType ContentType { get; set; }


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
            if (!tagContext.Items.ContainsKey(typeof(GridViewNccTagContext)))
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
