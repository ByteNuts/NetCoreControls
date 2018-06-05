using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ByteNuts.NetCoreControls.Models.Grid
{
    public class GridCell
    {
        public TagHelperContent Value { get; set; } = new DefaultTagHelperContent();
        public bool Aggregate { get; set; }
        public string CssClass { get; set; }
    }
}
