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
        public NccGridContentEnum ContentType { get; set; }


        private NccGridTagContext _nccTagContext;

        public override void Init(TagHelperContext tagContext)
        {
            if (tagContext.Items.ContainsKey(typeof(NccGridTagContext)))
                _nccTagContext = (NccGridTagContext)tagContext.Items[typeof(NccGridTagContext)];
            else
                throw new Exception("GridViewNccTagContext was lost between tags...");
        }

        public override async Task ProcessAsync(TagHelperContext tagContext, TagHelperOutput output)
        {
            output.TagName = null;
            if (!tagContext.Items.ContainsKey(typeof(NccGridContext)))
            {
                output.SuppressOutput();
                return;
            }

            switch (ContentType)
            {
                case NccGridContentEnum.PreContent:
                    _nccTagContext.PreContent += (await output.GetChildContentAsync()).GetContent();
                    break;
                case NccGridContentEnum.PostContent:
                    _nccTagContext.PostContent += (await output.GetChildContentAsync()).GetContent();
                    break;
                default:
                    _nccTagContext.PostContent += (await output.GetChildContentAsync()).GetContent();
                    break;
            }
        }
    }
}
