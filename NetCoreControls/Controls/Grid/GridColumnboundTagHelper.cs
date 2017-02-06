using System;
using System.Collections;
using System.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using ByteNuts.NetCoreControls.Models.Grid;
using ByteNuts.NetCoreControls.Services;

namespace ByteNuts.NetCoreControls.Controls.Grid
{
    [HtmlTargetElement("ncc:grid-columnbound", ParentTag = "ncc:grid-columns")]
    public class GridColumnboundTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("HeaderText")]
        public string HeaderText { get; set; }

        [HtmlAttributeName("DataValue")]
        public string DataValue { get; set; }

        [HtmlAttributeName("DataField")]
        public string DataField { get; set; }

        [HtmlAttributeName("CssClass")]
        public string CssClass { get; set; }

        [HtmlAttributeName("ShowHeader")]
        public bool ShowHeader { get; set; } = true;

        [HtmlAttributeName("Visible")]
        public bool Visible { get; set; } = true;

        [HtmlAttributeName("Aggregate")]
        public bool Aggregate { get; set; }

        private NccGridTagContext _nccTagContext;
        private NccGridContext _context;

        public override void Init(TagHelperContext tagContext)
        {
            if (tagContext.Items.ContainsKey(typeof(NccGridTagContext)))
                _nccTagContext = (NccGridTagContext)tagContext.Items[typeof(NccGridTagContext)];
            else
                throw new Exception("NccGridTagContext was lost between tags...");
        }

        public override void Process(TagHelperContext tagContext, TagHelperOutput output)
        {
            output.SuppressOutput();

            if (tagContext.Items.ContainsKey(typeof(NccGridContext)))
                _context = (NccGridContext)tagContext.Items[typeof(NccGridContext)];
            else
                return;

            var data = _context.DataObjects as IList;
            if (data == null || data.Count == 0)
                return;

            if (Visible)
            {
                if (!_nccTagContext.ColCountComplete)
                    _nccTagContext.ColCount++;

                if (ShowHeader)
                {
                    if (_nccTagContext.RowNumber == 0)
                    {
                        var cell = new GridCell();
                        cell.Value.AppendHtml(string.IsNullOrEmpty(HeaderText) ? DataValue : HeaderText);
                        _nccTagContext.GridHeader.Cells.Add(cell);
                    }
                }
                else
                {
                    var cell = new GridCell();
                    cell.Value.AppendHtml("");
                    _nccTagContext.GridHeader.Cells.Add(cell);
                }

                if (!ViewContext.ViewBag.EmptyData)
                {
                    if (!string.IsNullOrEmpty(DataField) && _context.AdditionalData.ContainsKey("EditRowNumber") && _context.AdditionalData["EditRowNumber"].ToString() == _nccTagContext.RowNumber.ToString())
                    {
                        var cell = new GridCell
                        {
                            CssClass = CssClass
                        };
                        var val = ViewContext.ViewData.Model.NccGetPropertyValue<string>(DataField);
                        var input = new TagBuilder("input")
                        {
                            Attributes =
                            {
                                { "class", "form-control" },
                                { "name", DataField },
                                { "value", val }
                            }
                        };
                        cell.Value.AppendHtml(input);

                        var row = _nccTagContext.GridRows.LastOrDefault();

                        row?.Cells.Add(cell);
                    }
                    else
                    {
                        var cell = new GridCell
                        {
                            CssClass = CssClass,
                            Aggregate = Aggregate
                        };
                        cell.Value.AppendHtml(DataValue);

                        var row = _nccTagContext.GridRows.LastOrDefault();

                        if (row != null)
                        {
                            row.Cells.Add(cell);
                            row.CssClass = CssClass;
                        }
                    }
                }
            }

            output.TagName = null;
            output.SuppressOutput();
        }
    }
}
