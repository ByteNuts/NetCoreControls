using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ByteNuts.NetCoreControls.Extensions;
using ByteNuts.NetCoreControls.Models;
using ByteNuts.NetCoreControls.Models.Enums;
using ByteNuts.NetCoreControls.Models.GridView;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;

namespace ByteNuts.NetCoreControls.Controls.GridView
{
    [HtmlTargetElement(Attributes = "grid-view-event,grid-view-event-target")]
    public class GridViewEventTagHelper : TagHelper
    {
        [HtmlAttributeName("grid-view-event-target")]
        public string GridViewEventTarget { get; set; }

        [HtmlAttributeName("grid-view-event")]
        public GridViewEvent GridViewEvent { get; set; }

        [HtmlAttributeName("grid-view-row")]
        public int GridRowPos { get; set; }

        public override void Process(TagHelperContext tagContext, TagHelperOutput output)
        {
            var model = new NccActionModel();
            model.TargetIds = GridViewEventTarget.Split(',');
            model.Parameters.Add($"{DefaultParameters.ActionType.ToString().NccAddPrefix()}", "GridViewEvent");
            model.Parameters.Add($"{DefaultParameters.EventName.ToString().NccAddPrefix()}", GridViewEvent.ToString());
            model.Parameters.Add($"{GridViewParameters.RowNumber.ToString().NccAddPrefix()}", GridRowPos.ToString());

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
