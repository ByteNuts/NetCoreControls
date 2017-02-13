using ByteNuts.NetCoreControls.Extensions;
using ByteNuts.NetCoreControls.Models;
using ByteNuts.NetCoreControls.Models.Enums;
using ByteNuts.NetCoreControls.Models.Grid;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;

namespace ByteNuts.NetCoreControls.Controls.Select
{
    [HtmlTargetElement("ncc:select", Attributes = "ncc-select-ordered-targets")]
    public class SelectActionTagHelper : TagHelper
    {
        [HtmlAttributeName("ncc-select-ordered-targets")]
        public string SelectOrderedTargets { get; set; }

        //[HtmlAttributeName("ncc-select-action")]
        //public GridEvent SelectAction { get; set; }


        public override void Process(TagHelperContext tagContext, TagHelperOutput output)
        {
            var model = new NccActionModel
            {
                TargetIds = SelectOrderedTargets.Split(',')
            };
            model.Parameters.Add($"{DefaultParameters.ActionType.ToString().NccAddPrefix()}", "SelectAction");
            model.Parameters.Add($"{DefaultParameters.EventName.ToString().NccAddPrefix()}", "OnChange");


            var onchange = "";
            if (output.Attributes.ContainsName("onchange"))
                onchange = output.Attributes["onchange"].Value.ToString();

            output.Attributes.SetAttribute("onchange", $"{onchange} nccAction(event, $(this), '{JsonConvert.SerializeObject(model)}', '{Constants.AttributePrefix}');");
        }
    }
}
