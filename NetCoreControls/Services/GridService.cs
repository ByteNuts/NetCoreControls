﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ByteNuts.NetCoreControls.Extensions;
using ByteNuts.NetCoreControls.Models;
using ByteNuts.NetCoreControls.Models.Enums;
using ByteNuts.NetCoreControls.Models.Grid;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace ByteNuts.NetCoreControls.Services
{
    public static class GridService
    {
        public static IDictionary<string, object> GetExtraParameters(IDictionary<string, object> methodParams, GridContext context)
        {
            methodParams["pageNumber"] = context.PageNumber;
            methodParams["pageSize"] = context.PageSize;

            return methodParams;
        }

        public static GridContext SetDataResult(GridContext context, object result)
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

        public static TagBuilder BuildTableHtml(GridNccTagContext context, GridContext gridContext)
        {
            var table = new TagBuilder("table");
            if (!string.IsNullOrEmpty(context.CssClassGrid)) table.Attributes.Add("class", context.CssClassGrid);
            
            table.InnerHtml.AppendHtml(BuildTableHeader(context));

            table.InnerHtml.AppendHtml(BuildTableBody(context));

            if (gridContext.AllowPaging)
                table.InnerHtml.AppendHtml(BuildTablePager(context, gridContext));

            return table;
        }

        private static TagBuilder BuildTableHeader(GridNccTagContext context)
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

        private static TagBuilder BuildTableBody(GridNccTagContext context)
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

        public static TagBuilder BuildTablePager(GridNccTagContext context, GridContext gridContext)
        {
            var tableFooter = new TagBuilder("tfoot");
            var tr = new TagBuilder("tr");
            var td = new TagBuilder("td") {Attributes = {{"colspan", context.ColCount.ToString()}}};

            var div = new TagBuilder("div") { Attributes = { { "class", "nccGridPagerContainer" } } };
            var ul = new TagBuilder("ul") { Attributes = { { "class", "nccGridPagerPagination" } } };

            var totalPages = gridContext.TotalItems % gridContext.PageSize > 0
                ? gridContext.TotalItems / gridContext.PageSize + 1
                : gridContext.TotalItems / gridContext.PageSize;

            if (totalPages == 1)
                return null;

            var model = new NccActionModel { TargetIds = new [] { gridContext.Id } };
            model.Parameters.Add($"{DefaultParameters.ActionType.ToString().NccAddPrefix()}", "Filter");

            var firstLink = BuilPagerLink(1, model, "<<", false, gridContext.PageNumber == 1);
            var prevLink = BuilPagerLink(gridContext.PageNumber - 1, model, "<", false, gridContext.PageNumber == 1);

            ul.InnerHtml.AppendHtml(firstLink);
            ul.InnerHtml.AppendHtml(prevLink);

            var nextLink = BuilPagerLink(gridContext.PageNumber + 1, model, ">", false, gridContext.PageNumber == totalPages);
            var lastLink = BuilPagerLink(totalPages, model, ">>", false, gridContext.PageNumber == totalPages);

            var navSize = gridContext.PagerNavigationSize >= totalPages ? totalPages : gridContext.PagerNavigationSize;
            var linksBefore = navSize / 2 < gridContext.PageNumber
                ? navSize / 2
                : gridContext.PageNumber - 1;
            var linksAfter = navSize / 2 < totalPages - gridContext.PageNumber
                ? navSize - linksBefore
                : totalPages - gridContext.PageNumber;

            if (navSize / 2 > linksAfter)
                linksBefore = linksBefore + (navSize / 2 - linksAfter);
            if (navSize % 2 == 0)
                if (linksBefore > 0 && linksBefore >= navSize / 2)
                    linksBefore--;
                else
                    linksAfter--;

            for (var i = 0; i < linksBefore; i++)
            {
                var link = BuilPagerLink(gridContext.PageNumber - linksBefore + i, model);
                ul.InnerHtml.AppendHtml(link);
            }
            ul.InnerHtml.AppendHtml(BuilPagerLink(gridContext.PageNumber, model, null, true));
            for (var i = 0; i < linksAfter; i++)
            {
                var link = BuilPagerLink(gridContext.PageNumber + i + 1, model);
                ul.InnerHtml.AppendHtml(link);
            }

            ul.InnerHtml.AppendHtml(nextLink);
            ul.InnerHtml.AppendHtml(lastLink);

            div.InnerHtml.AppendHtml(ul);

            td.InnerHtml.AppendHtml(div);
            tr.InnerHtml.AppendHtml(td);
            tableFooter.InnerHtml.AppendHtml(tr);

            return tableFooter;
        }

        private static TagBuilder BuilPagerLink(int pageNumber, NccActionModel model, string htmlContent = null, bool active = false, bool disabled = false)
        {
            var li = new TagBuilder("li");
            var link = new TagBuilder("a")
            {
                Attributes =
                    {
                        {"href", "#"},
                        {"name", "pageNumber" },
                        {"value", pageNumber.ToString() },
                        {"onclick", $"nccAction(null, $(this), '{JsonConvert.SerializeObject(model)}', '{Constants.AttributePrefix}');" }
                    }
            };
            link.InnerHtml.Append(string.IsNullOrEmpty(htmlContent) ? pageNumber.ToString() : htmlContent);
            if (active) link.Attributes.Add("class", "active");
            if (disabled)
            {
                link.Attributes.Add("class", "disabled");
                link.Attributes.Remove("onclick");
            }

            li.InnerHtml.AppendHtml(link);

            return li;
        }

        #endregion


    }
}