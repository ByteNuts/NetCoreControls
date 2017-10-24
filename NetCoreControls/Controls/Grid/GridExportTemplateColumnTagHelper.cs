using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using ByteNuts.NetCoreControls.Models.Grid;

namespace ByteNuts.NetCoreControls.Controls.Grid
{
    [HtmlTargetElement("ncc:grid-export-template-column", ParentTag = "ncc:grid-export-template")]
    public class GridExportTemplateColumnTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("ColumnTitle")]
        public string ColumnTitle { get; set; }

        [HtmlAttributeName("ColumnPropName")]
        public string ColumnPropName { get; set; }

        [HtmlAttributeName("Width")]
        public double? Width { get; set; }

        [HtmlAttributeName("HeaderHorizontalAlignment")]
        public string HeaderHorizontalAlignment { get; set; }

        [HtmlAttributeName("HeaderVerticalAlignment")]
        public string HeaderVerticalAlignment { get; set; }

        [HtmlAttributeName("RowHorizontalAlignment")]
        public string RowHorizontalAlignment { get; set; }

        [HtmlAttributeName("RowVerticalAlignment")]
        public string RowVerticalAlignment { get; set; }

        private NccGridTagContext _nccTagContext;
        private NccGridContext _context;


        public override void Init(TagHelperContext tagContext)
        {
            if (tagContext.Items.ContainsKey(typeof(NccGridTagContext)))
                _nccTagContext = (NccGridTagContext)tagContext.Items[typeof(NccGridTagContext)];
            else
                throw new Exception("GridViewNccTagContext was lost between tags...");
        }

        public override async Task ProcessAsync(TagHelperContext tagContext, TagHelperOutput output)
        {
            output.SuppressOutput();

            if (tagContext.Items.ContainsKey(typeof(NccGridContext)))
                _context = (NccGridContext)tagContext.Items[typeof(NccGridContext)];
            else
                return;

            _context.GridExportExcel.Columns.Add(new GridExportExcelColumn
            {
                ColumnTitle = ColumnTitle,
                ColumnPropName = ColumnPropName,
                HeaderHorizontalAlignment = HeaderHorizontalAlignment,
                HeaderVerticalAlignment = HeaderVerticalAlignment,
                RowHorizontalAlignment = RowHorizontalAlignment,
                RowVerticalAlignment = RowVerticalAlignment,
                Width = Width
            });
        }
    }
}
