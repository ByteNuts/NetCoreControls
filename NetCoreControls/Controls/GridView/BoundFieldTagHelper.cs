using System;
using System.Collections;
using System.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using ByteNuts.NetCoreControls.Models.GridView;

namespace ByteNuts.NetCoreControls.Controls.GridView
{
    [HtmlTargetElement("ncc:BoundField", ParentTag = "ncc:Columns")]
    [HtmlTargetElement("ncc:bound-field", ParentTag = "ncc:columns")]
    public class BoundFieldTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("HeaderText")]
        public string HeaderText { get; set; }

        [HtmlAttributeName("DataValue")]
        public string DataValue { get; set; }

        [HtmlAttributeName("CssClass")]
        public string CssClass { get; set; }

        [HtmlAttributeName("ShowHeader")]
        public bool ShowHeader { get; set; } = true;

        [HtmlAttributeName("Visible")]
        public bool Visible { get; set; } = true;

        [HtmlAttributeName("Aggregate")]
        public bool Aggregate { get; set; }

        private GridViewNccTagContext _nccTagContext;
        private GridViewContext _context;

        public override void Init(TagHelperContext tagContext)
        {
            if (tagContext.Items.ContainsKey(typeof(GridViewNccTagContext)))
                _nccTagContext = (GridViewNccTagContext)tagContext.Items[typeof(GridViewNccTagContext)];
            else
                throw new Exception("GridViewNccTagContext was lost between tags...");
        }

        public override void Process(TagHelperContext tagContext, TagHelperOutput output)
        {
            output.SuppressOutput();

            if (tagContext.Items.ContainsKey(typeof(GridViewContext)))
                _context = (GridViewContext)tagContext.Items[typeof(GridViewContext)];
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
                    if (ViewContext.ViewBag.RowCount == 0)
                    {
                        _nccTagContext.GridHeader.Cells.Add(string.IsNullOrEmpty(HeaderText)
                            ? new GridViewCell { Value = DataValue }
                            : new GridViewCell { Value = HeaderText });
                    }
                }
                else
                    _nccTagContext.GridHeader.Cells.Add(new GridViewCell { Value = "" });

                if (!ViewContext.ViewBag.EmptyData)
                {
                    var cell = new GridViewCell
                    {
                        Value = DataValue,
                        CssClass = CssClass,
                        Aggregate = Aggregate
                    };

                    var row = _nccTagContext.GridRows.LastOrDefault();

                    if (row != null)
                    {
                        row.Cells.Add(cell);
                        row.CssClass = CssClass;
                    }
                }
            }

            output.TagName = null;
            output.SuppressOutput();
        }
    }
}
