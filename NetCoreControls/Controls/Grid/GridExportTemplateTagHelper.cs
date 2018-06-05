using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using ByteNuts.NetCoreControls.Models.Grid;

namespace ByteNuts.NetCoreControls.Controls.Grid
{
    [HtmlTargetElement("ncc:grid-export-template", ParentTag = "ncc:grid")]
    public class GridExportTemplateTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("DocTitle")]
        public string DocTitle { get; set; }

        [HtmlAttributeName("DocSubject")]
        public string DocSubject { get; set; }

        [HtmlAttributeName("DocAuthor")]
        public string DocAuthor { get; set; }

        [HtmlAttributeName("DocCompany")]
        public string DocCompany { get; set; }

        [HtmlAttributeName("DocKeywords")]
        public string DocKeywords { get; set; }

        [HtmlAttributeName("AutoFitColumns")]
        public bool? AutoFitColumns { get; set; }

        [HtmlAttributeName("ShowHeader")]
        public bool? ShowHeader { get; set; }

        [HtmlAttributeName("ShowTotal")]
        public bool? ShowTotal { get; set; }

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

            if (_context.GridExportExcel == null)
            {
                _context.GridExportExcel = new GridExportExcel
                {
                    Title = DocTitle,
                    Subject = DocSubject,
                    Author = DocAuthor,
                    Company = DocCompany,
                    Keywords = DocKeywords,
                    AutoFitColumns = AutoFitColumns,
                    ShowHeader = ShowHeader,
                    ShowTotal = ShowTotal,
                    Columns = new List<GridExportExcelColumn>()
                };

                await output.GetChildContentAsync();
            }
        }
    }
}
