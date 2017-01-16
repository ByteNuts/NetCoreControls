using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ByteNuts.NetCoreControls.Models.GridView;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ByteNuts.NetCoreControls.Services
{
    public static class GridViewService
    {
        public static IDictionary<string, object> GetExtraParameters(IDictionary<string, object> methodParams, GridViewContext context)
        {
            methodParams["pageNumber"] = context.PageNumber;
            methodParams["pageSize"] = context.PageSize;

            return methodParams;
        }

        public static GridViewContext SetDataResult(GridViewContext context, object result)
        {
            if (result.GetType().ToString().Contains(typeof(Tuple).ToString()))
                context.TotalItems = Convert.ToInt32(result.GetType().GetProperty("Item2").GetValue(result));
            else
            {
                var list = result as IList;
                context.TotalItems = list != null ? new List<object>(list.Cast<object>()).Count : 1;
            }

            return context;
        }


        #region Table Html

        public static TagBuilder BuildTableHtml(GridViewNccTagContext context)
        {
            var table = new TagBuilder("table");
            if (!string.IsNullOrEmpty(context.CssClassGrid)) table.Attributes.Add("class", context.CssClassGrid);
            
            table.InnerHtml.AppendHtml(BuildTableHeader(context));

            table.InnerHtml.AppendHtml(BuildTableBody(context));

            return table;
        }

        private static TagBuilder BuildTableHeader(GridViewNccTagContext context)
        {
            var tableHeader = new TagBuilder("thead");
            if (!string.IsNullOrEmpty(context.GridHeader.CssClass)) tableHeader.Attributes.Add("class", context.GridHeader.CssClass);
            foreach (var headerCell in context.GridHeader.Cells)
            {
                var th = new TagBuilder("th");
                th.InnerHtml.AppendHtml(headerCell.Value);
                tableHeader.InnerHtml.AppendHtml(th);
            }

            return tableHeader;
        }

        private static TagBuilder BuildTableBody(GridViewNccTagContext context)
        {
            var gridContent = context.GridRows;

            var tableBody = new TagBuilder("tbody");
            if (!string.IsNullOrEmpty(context.CssClassBody)) tableBody.Attributes.Add("class", context.CssClassBody);
            for (var i = 0; i < gridContent.Count; i++)
            {
                var row = gridContent[i];
                var tr = new TagBuilder("tr");
                if (!string.IsNullOrEmpty(row.CssClass)) tr.Attributes.Add("class", row.CssClass);
                for (var j = 0; j < row.Cells.Count; j++)
                {
                    var cell = row.Cells[j];

                    if (!cell.Aggregate)
                    {
                        var td = new TagBuilder("td");
                        if (!string.IsNullOrEmpty(cell.CssClass)) td.Attributes.Add("class", cell.CssClass);
                        td.InnerHtml.AppendHtml(cell.Value);
                        tr.InnerHtml.AppendHtml(td);
                    }
                    else
                    {
                        if (i == 0 || !(i > 0 && gridContent[i - 1].Cells[j].Value == cell.Value))
                        {
                            var sameCount = 1;
                            while (i + sameCount < gridContent.Count && gridContent[i + sameCount].Cells[j].Value == cell.Value)
                            {
                                sameCount++;
                            }
                            var td = new TagBuilder("td");
                            if (!string.IsNullOrEmpty(cell.CssClass)) td.Attributes.Add("class", cell.CssClass);
                            if (sameCount > 0) td.Attributes.Add("rowspan", sameCount.ToString());
                            td.InnerHtml.AppendHtml(cell.Value);
                            tr.InnerHtml.AppendHtml(td);
                        }
                    }
                }

                tableBody.InnerHtml.AppendHtml(tr);
            }

            return tableBody;
        }

        #endregion
    }
}
