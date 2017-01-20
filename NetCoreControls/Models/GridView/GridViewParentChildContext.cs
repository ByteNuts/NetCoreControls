using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ByteNuts.NetCoreControls.Models.GridView
{
    public class GridViewNccTagContext : NccTagContext
    {
        public int RowNumber { get; set; }
        public int ColCount { get; set; }
        public bool ColCountComplete { get; set; }

        public GridViewRow GridHeader { get; set; } = new GridViewRow();
        public List<GridViewRow> GridRows { get; set; } = new List<GridViewRow>();

        public string PreContent { get; set; }
        public string PostContent { get; set; }
        public TagHelperContent HeaderContent { get; set; } = new DefaultTagHelperContent();
        public TagHelperContent BodyContent { get; set; } = new DefaultTagHelperContent();
        public TagHelperContent FooterContent { get; set; } = new DefaultTagHelperContent();
        public TagHelperContent PagerContent { get; set; } = new DefaultTagHelperContent();

        public string CssClassGrid { get; set; }
        public string CssClassBody { get; set; }
        public string CssClassHeader { get; set; }
        public string CssClassFooter { get; set; }

    }
}
