using ByteNuts.NetCoreControls.Models.Repeater;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace ByteNuts.NetCoreControls.Controls.Repeater
{
    [HtmlTargetElement("ncc:repeater-footertemplate", ParentTag = "ncc:repeater")]
    public class RepeaterFootertemplateTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        private NccRepeaterTagContext _nccTagContext;
        private NccRepeaterContext _context;

        public override void Init(TagHelperContext tagContext)
        {
            if (tagContext.Items.ContainsKey(typeof(NccRepeaterTagContext)))
                _nccTagContext = (NccRepeaterTagContext)tagContext.Items[typeof(NccRepeaterTagContext)];
            else
                throw new Exception("RepeaterNccTagContext was lost between tags...");
        }

        public override async Task ProcessAsync(TagHelperContext tagContext, TagHelperOutput output)
        {
            output.SuppressOutput();

            if (tagContext.Items.ContainsKey(typeof(NccRepeaterContext)))
                _context = (NccRepeaterContext)tagContext.Items[typeof(NccRepeaterContext)];
            else
                return;

            var childContent = await output.GetChildContentAsync();

            _nccTagContext.RepeaterFooter = childContent.GetContent();
        }
    }
}
