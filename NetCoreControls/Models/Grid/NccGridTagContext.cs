using System.Collections.Generic;
using ByteNuts.NetCoreControls.Core.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ByteNuts.NetCoreControls.Models.Grid
{
    public class NccGridTagContext : NccTagContext
    {
        public int RowNumber { get; set; }
        public int ColCount { get; set; }
        public bool ColCountComplete { get; set; }

        public GridRow GridHeader { get; set; } = new GridRow();
        public List<GridRow> GridRows { get; set; } = new List<GridRow>();
        public GridRow EmptyRow { get; set; }

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

        //Grid Pager
        public string PagerRecordsCountContent { get; set; }
        public string CssClassRecordCountDiv { get; set; }
        public string CssClassFooterDiv { get; set; } = "row";
        public string CssClassPagerDiv { get; set; } = "nccGridPagerContainer";
        public string CssClassPagerUl { get; set; } = "nccGridPagerPagination";
        public string CssClassPagerLi { get; set; }
        public string CssClassPagerA { get; set; }
    }
}
