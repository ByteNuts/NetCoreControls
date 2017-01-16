using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ByteNuts.NetCoreControls.Models
{
    public class NccTagContext
    {
        public object ControlContext { get; set; }

        public bool Visible { get; set; } = true;
        public TagHelperContent ControlContent { get; set; } = new DefaultTagHelperContent();
        public Dictionary<string, string> Filters { get; set; } = new Dictionary<string, string>();

    }
}
