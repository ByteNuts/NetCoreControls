using ByteNuts.NetCoreControls.Extensions;
using ByteNuts.NetCoreControls.Models;
using ByteNuts.NetCoreControls.Models.Enums;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;

namespace ByteNuts.NetCoreControls.Controls.Actions
{
    [HtmlTargetElement("ncc:select", Attributes = "ncc-link-targets")]
    public class LinkTagHelper : TagHelper
    {
        [HtmlAttributeName("ncc-link-targets")]
        public string LinkTargets { get; set; }

        public override void Process(TagHelperContext tagContext, TagHelperOutput output)
        {
            var model = new NccActionModel
            {
                TargetIds = LinkTargets.Split(',')
            };
            model.Parameters.Add($"{DefaultParameters.ActionType.ToString().NccAddPrefix()}", "LinkAction");
            model.Parameters.Add($"{DefaultParameters.EventName.ToString().NccAddPrefix()}", "OnChange");


            var onchange = "";
            if (output.Attributes.ContainsName("onchange"))
                onchange = output.Attributes["onchange"].Value.ToString();

            output.Attributes.SetAttribute("onchange", $"{onchange} nccAction(event, $(this), '{JsonConvert.SerializeObject(model)}', '{Constants.AttributePrefix}');");
        }
    }
}
