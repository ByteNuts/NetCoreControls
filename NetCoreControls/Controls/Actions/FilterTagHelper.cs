using ByteNuts.NetCoreControls.Extensions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ByteNuts.NetCoreControls.Models;
using ByteNuts.NetCoreControls.Models.Enums;
using Newtonsoft.Json;

namespace ByteNuts.NetCoreControls.Controls.Actions
{
    [HtmlTargetElement("input", Attributes = "ncc-filter-targets")]
    [HtmlTargetElement("select", Attributes = "ncc-filter-targets")]
    [HtmlTargetElement("button", Attributes = "ncc-filter-targets,ncc-filter-id")]
    public class FilterTagHelper : TagHelper
    {
        /// <summary>
        /// Ids of the controls separated by commas
        /// </summary>
        [HtmlAttributeName("ncc-filter-targets")]
        public string NccFilterTargets { get; set; }

        [HtmlAttributeName("ncc-filter-id")]
        public string NccFilterId { get; set; }


        public override void Init(TagHelperContext tagContext)
        {
        }

        public override void Process(TagHelperContext tagContext, TagHelperOutput output)
        {
            var model = new NccActionModel();
            model.TargetIds = NccFilterTargets.Split(',');
            model.Parameters.Add($"{DefaultParameters.ActionType.ToString().NccAddPrefix()}", "Filter");

            if (output.TagName.ToLower() == "select")
            {
                var onchange = "";
                if (output.Attributes.ContainsName("onchange"))
                    onchange = output.Attributes["onchange"].Value.ToString();

                output.Attributes.SetAttribute("onchange", $"{onchange} nccAction(null, " + (string.IsNullOrEmpty(NccFilterId) ? "$(this)" : $"$('#{NccFilterId}')") + $", '{JsonConvert.SerializeObject(model)}', '{Constants.AttributePrefix}');");
            }
            else if (output.TagName == "input" && output.Attributes["type"]?.Value.ToString() == "text")
            {
                var onkeyup = "";
                if (output.Attributes.ContainsName("onkeyup"))
                    onkeyup = output.Attributes["onkeyup"].Value.ToString();

                output.Attributes.SetAttribute("onkeyup", $"{onkeyup} nccAction(null, " + (string.IsNullOrEmpty(NccFilterId) ? "$(this)" : $"$('#{NccFilterId}')") + $", '{JsonConvert.SerializeObject(model)}', '{Constants.AttributePrefix}');");
            }
            else
            {
                var onclick = "";
                if (output.Attributes.ContainsName("onclick"))
                    onclick = output.Attributes["onclick"].Value.ToString();

                output.Attributes.SetAttribute("onclick", $"{onclick} nccAction(null, " + (string.IsNullOrEmpty(NccFilterId) ? "$(this)" : $"$('#{NccFilterId}')") + $", '{JsonConvert.SerializeObject(model)}', '{Constants.AttributePrefix}');");
            }
        }
    }
}
