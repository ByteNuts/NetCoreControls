using ByteNuts.NetCoreControls.Core;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using ByteNuts.NetCoreControls.Core.Models;
using ByteNuts.NetCoreControls.Core.Models.Enums;
using ByteNuts.NetCoreControls.Core.Extensions;
using ByteNuts.NetCoreControls.Models.Enums;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ByteNuts.NetCoreControls.Controls.Actions
{
    [HtmlTargetElement(Attributes = "ncc-confirm")]
    public class NccConfirmTagHelper : TagHelper
    {
        [HtmlAttributeName("ncc-confirm")]
        public string NccConfirm { get; set; }


        public override void Init(TagHelperContext tagContext)
        {
        }

        public override void Process(TagHelperContext tagContext, TagHelperOutput output)
        {
            if (output.TagName.ToLower() == "select")
            {
                var onchange = "";
                if (output.Attributes.ContainsName("onchange"))
                    onchange = output.Attributes["onchange"].Value.ToString();

                output.Attributes.SetAttribute("onchange", $"if(!confirm('{NccConfirm}')) {{ return false; }} {onchange}");
            }
            else
            {
                var onclick = "";
                if (output.Attributes.ContainsName("onclick"))
                    onclick = output.Attributes["onclick"].Value.ToString();

                output.Attributes.SetAttribute("onclick", $"if(!confirm('{NccConfirm}')) {{ return false; }} {onclick}");
            }
        }
    }
}
