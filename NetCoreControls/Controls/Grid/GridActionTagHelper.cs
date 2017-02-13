using ByteNuts.NetCoreControls.Extensions;
using ByteNuts.NetCoreControls.Models;
using ByteNuts.NetCoreControls.Models.Enums;
using ByteNuts.NetCoreControls.Models.Grid;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;

namespace ByteNuts.NetCoreControls.Controls.Grid
{
    [HtmlTargetElement(Attributes = "ncc-grid-action,ncc-grid-action-target")]
    public class GridActionTagHelper : TagHelper
    {
        [HtmlAttributeName("ncc-grid-action-target")]
        public string GridActionTarget { get; set; }

        [HtmlAttributeName("ncc-grid-action")]
        public GridEvent GridAction { get; set; }

        [HtmlAttributeName("ncc-grid-row")]
        public int GridRowPos { get; set; }

        public override void Process(TagHelperContext tagContext, TagHelperOutput output)
        {
            var model = new NccActionModel
            {
                TargetIds = GridActionTarget.Split(',')
            };
            model.Parameters.Add($"{DefaultParameters.ActionType.ToString().NccAddPrefix()}", "GridAction");
            model.Parameters.Add($"{DefaultParameters.EventName.ToString().NccAddPrefix()}", GridAction.ToString());
            model.Parameters.Add($"{NccGridParametersEnum.RowNumber.ToString().NccAddPrefix()}", GridRowPos.ToString());

            if (output.TagName.ToLower() == "select")
            {
                var onchange = "";
                if (output.Attributes.ContainsName("onchange"))
                    onchange = output.Attributes["onchange"].Value.ToString();

                output.Attributes.SetAttribute("onchange", $"{onchange} nccAction(event, $(this), '{JsonConvert.SerializeObject(model)}', '{Constants.AttributePrefix}');");
            }
            else
            {
                var onclick = "";
                if (output.Attributes.ContainsName("onclick"))
                    onclick = output.Attributes["onclick"].Value.ToString();

                output.Attributes.SetAttribute("onclick", $"{onclick} nccAction(event, $(this), '{JsonConvert.SerializeObject(model)}', '{Constants.AttributePrefix}');");
            }
        }
    }
}
