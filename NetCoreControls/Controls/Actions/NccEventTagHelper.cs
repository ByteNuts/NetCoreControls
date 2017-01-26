using ByteNuts.NetCoreControls.Extensions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ByteNuts.NetCoreControls.Models;
using ByteNuts.NetCoreControls.Models.Enums;
using Newtonsoft.Json;

namespace ByteNuts.NetCoreControls.Controls.Actions
{
    [HtmlTargetElement(Attributes = "ncc-event,ncc-event-target")]
    public class NccEventTagHelper : TagHelper
    {
        [HtmlAttributeName("ncc-event")]
        public string NccEvent { get; set; }

        [HtmlAttributeName("ncc-event-target")]
        public string NccEventTarget { get; set; }


        public override void Init(TagHelperContext tagContext)
        {
        }

        public override void Process(TagHelperContext tagContext, TagHelperOutput output)
        {
            var model = new NccActionModel {TargetIds = NccEventTarget.Split(',')};
            model.Parameters.Add($"{DefaultParameters.ActionType.ToString().NccAddPrefix()}", "Event");
            model.Parameters.Add($"{DefaultParameters.EventName.ToString().NccAddPrefix()}", NccEvent);

            if (output.TagName.ToLower() == "select")
            {
                var onchange = "";
                if (output.Attributes.ContainsName("onchange"))
                    onchange = output.Attributes["onchange"].Value.ToString();

                output.Attributes.SetAttribute("onchange", $"{onchange} nccAction(null, $(this), '{JsonConvert.SerializeObject(model)}', '{Constants.AttributePrefix}');");
            }
            else
            {
                var onclick = "";
                if (output.Attributes.ContainsName("onclick"))
                    onclick = output.Attributes["onclick"].Value.ToString();

                output.Attributes.SetAttribute("onclick", $"{onclick} nccAction(null, $(this), '{JsonConvert.SerializeObject(model)}', '{Constants.AttributePrefix}');");
            }
        }
    }
}
