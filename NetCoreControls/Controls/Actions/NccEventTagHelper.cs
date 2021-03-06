﻿using ByteNuts.NetCoreControls.Core;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using ByteNuts.NetCoreControls.Core.Models;
using ByteNuts.NetCoreControls.Core.Models.Enums;
using ByteNuts.NetCoreControls.Core.Extensions;
using ByteNuts.NetCoreControls.Models.Enums;

namespace ByteNuts.NetCoreControls.Controls.Actions
{
    [HtmlTargetElement(Attributes = "ncc-event,ncc-event-target")]
    public class NccEventTagHelper : TagHelper
    {
        [HtmlAttributeName("ncc-event")]
        public string NccEvent { get; set; }

        [HtmlAttributeName("ncc-event-target")]
        public string NccEventTarget { get; set; }

        [HtmlAttributeName("ncc-row")]
        public int RowPos { get; set; }

        public override void Init(TagHelperContext tagContext)
        {
        }

        public override void Process(TagHelperContext tagContext, TagHelperOutput output)
        {
            var model = new NccActionModel {TargetIds = NccEventTarget.Split(',')};
            model.Parameters.Add($"{DefaultParameters.ActionType.ToString().NccAddPrefix()}", "Event");
            model.Parameters.Add($"{DefaultParameters.EventName.ToString().NccAddPrefix()}", NccEvent);
            model.Parameters.Add($"{"RowNumber".NccAddPrefix()}", RowPos.ToString());

            if (output.TagName.ToLower() == "select")
            {
                var onchange = "";
                if (output.Attributes.ContainsName("onchange"))
                    onchange = output.Attributes["onchange"].Value.ToString();

                output.Attributes.SetAttribute("onchange", $"{onchange} nccAction(null, $(this), '{JsonConvert.SerializeObject(model)}', '{NccConstants.AttributePrefix}');");
            }
            else
            {
                var onclick = "";
                if (output.Attributes.ContainsName("onclick"))
                    onclick = output.Attributes["onclick"].Value.ToString();

                output.Attributes.SetAttribute("onclick", $"{onclick} nccAction(null, $(this), '{JsonConvert.SerializeObject(model)}', '{NccConstants.AttributePrefix}');");
            }
        }
    }
}
