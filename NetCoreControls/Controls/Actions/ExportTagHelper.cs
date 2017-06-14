using ByteNuts.NetCoreControls.Core;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using ByteNuts.NetCoreControls.Core.Models;
using ByteNuts.NetCoreControls.Core.Models.Enums;
using ByteNuts.NetCoreControls.Core.Extensions;
using System.Linq;

namespace ByteNuts.NetCoreControls.Controls.Actions
{
    [HtmlTargetElement("a", Attributes = "ncc-export-format,ncc-target-ids")]
    [HtmlTargetElement("button", Attributes = "ncc-export-format,ncc-target-ids")]
    public class ExportTagHelper : TagHelper
    {
        /// <summary>
        /// Ids of the controls separated by commas
        /// </summary>
        [HtmlAttributeName("ncc-export-format")]
        public string NccExportFormat { get; set; }

        [HtmlAttributeName("ncc-target-ids")]
        public string NccTargetIds { get; set; }


        public override void Init(TagHelperContext tagContext)
        {
        }

        public override void Process(TagHelperContext tagContext, TagHelperOutput output)
        {
            var exportFormats = new string[] { "exportExcel" };
            if (!exportFormats.Any(NccExportFormat.Contains))
                NccExportFormat = "exportExcel";
            var model = new NccActionModel {TargetIds = NccTargetIds.Split(',')};
            model.Parameters.Add($"{DefaultParameters.ActionType.ToString().NccAddPrefix()}", NccExportFormat);

            var existingJsEvent = "";
            if (output.Attributes.ContainsName("onclick"))
                existingJsEvent = output.Attributes["onclick"].Value.ToString();

            output.Attributes.SetAttribute("onclick", $"{existingJsEvent} nccAction(null, null, '{JsonConvert.SerializeObject(model)}', '{NccConstants.AttributePrefix}');");

        }
    }
}
