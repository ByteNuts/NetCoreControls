using ByteNuts.NetCoreControls.Extensions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ByteNuts.NetCoreControls.Models;
using ByteNuts.NetCoreControls.Models.Enums;
using Newtonsoft.Json;

namespace ByteNuts.NetCoreControls.Controls.Actions
{
    [HtmlTargetElement("input", Attributes = "ncc-filter-targets")]
    [HtmlTargetElement("select", Attributes = "ncc-filter-targets")]
    [HtmlTargetElement("button", Attributes = "ncc-filter-targets,ncc-filter-ids")]
    public class FilterTagHelper : TagHelper
    {
        /// <summary>
        /// Ids of the controls separated by commas
        /// </summary>
        [HtmlAttributeName("ncc-filter-targets")]
        public string NccFilterTargets { get; set; }

        [HtmlAttributeName("ncc-filter-ids")]
        public string NccFilterIds { get; set; }

        [HtmlAttributeName("ncc-js-events")]
        public string NccJsEvents { get; set; }


        public override void Init(TagHelperContext tagContext)
        {
        }

        public override void Process(TagHelperContext tagContext, TagHelperOutput output)
        {
            var model = new NccActionModel {TargetIds = NccFilterTargets.Split(',')};
            model.Parameters.Add($"{DefaultParameters.ActionType.ToString().NccAddPrefix()}", "Filter");

            if (string.IsNullOrEmpty(NccJsEvents))
            {
                if (output.TagName.ToLower() == "select")
                    NccJsEvents = "onchange";
                else if (output.TagName == "input" && output.Attributes["type"]?.Value.ToString() == "text")
                    NccJsEvents = "onkeyup";
                else
                    NccJsEvents = "onclick";
            }
            foreach (var jsEvent in NccJsEvents.Replace(" ", "").Split(','))
            {
                var existingJsEvent = "";
                if (output.Attributes.ContainsName(jsEvent.ToLower()))
                    existingJsEvent = output.Attributes[jsEvent.ToLower()].Value.ToString();

                output.Attributes.SetAttribute(jsEvent.ToLower(), $"{existingJsEvent} nccAction(null, " + (string.IsNullOrEmpty(NccFilterIds) ? "$(this)" : $"{JsonConvert.SerializeObject(NccFilterIds.Split(','))}") + $", '{JsonConvert.SerializeObject(model)}', '{Constants.AttributePrefix}');");
                //output.Attributes.SetAttribute(jsEvent.ToLower(), $"{existingJsEvent} nccAction(null, " + (string.IsNullOrEmpty(NccFilterId) ? "$(this)" : $"$('#{NccFilterId}')") + $", '{JsonConvert.SerializeObject(model)}', '{Constants.AttributePrefix}');");
            }

        }
    }
}
