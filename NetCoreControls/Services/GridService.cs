using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ByteNuts.NetCoreControls.Core;
using ByteNuts.NetCoreControls.Core.Extensions;
using ByteNuts.NetCoreControls.Core.Models;
using ByteNuts.NetCoreControls.Core.Models.Enums;
using ByteNuts.NetCoreControls.Models.Enums;
using ByteNuts.NetCoreControls.Models.Grid;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using OfficeOpenXml;
using ByteNuts.NetCoreControls.Core.Services;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml.Table;

namespace ByteNuts.NetCoreControls.Services
{
    public static class GridService
    {
        public static void GetExtraParameters(IDictionary<string, object> methodParams, NccGridContext context)
        {
            methodParams["pageNumber"] = context.PageNumber;
            methodParams["pageSize"] = context.PageSize;
        }
        public static void GetExtraNullableParameters(IDictionary<string, object> methodParams, NccGridContext context)
        {
            methodParams["pageNumber"] = 1;
            methodParams["pageSize"] = int.MaxValue;
        }

        public static void SetDataResult(NccGridContext context, object result)
        {
            if (result.GetType().ToString().Contains(typeof(Tuple).ToString()))
                context.TotalItems = Convert.ToInt32(result.GetType().GetProperty("Item2").GetValue(result));
            else if (result.GetType().ToString().Contains("Microsoft.EntityFrameworkCore.Internal.InternalDbSet") ||
                result.GetType().ToString().Contains("Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryable") ||
                result.GetType().ToString().Contains("System.Linq.IQueryable"))
            {
                context.TotalItems = ((IQueryable<object>)result).Count();
                if (context.TotalItems > context.PageSize)
                    context.DataObjects = ((IQueryable<object>)result).Skip(context.PageSize * (context.PageNumber - 1)).Take(context.PageSize).ToList();
            }
            else
            {
                var list = result as IList;
                context.TotalItems = list != null ? new List<object>(list.Cast<object>()).Count : 1;
            }


            //return context;
        }

        public static byte[] GetExcelPackage(NccGridContext context, HttpContext httpContext)
        {
            NccActionsService.ExtraParameters<NccGridContext> setExtraParameters = GridService.GetExtraNullableParameters;
            NccActionsService.DataResult<NccGridContext> setDataResult = GridService.SetDataResult;
            NccControlsService.BindData(context, httpContext, setExtraParameters, setDataResult);

            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                if (string.IsNullOrEmpty(context.GridExportExcel.Title))
                    context.GridExportExcel.Title = context.Id;

                package.Workbook.Properties.Title = context.GridExportExcel.Title;
                package.Workbook.Properties.Subject = context.GridExportExcel.Subject;
                package.Workbook.Properties.Company = context.GridExportExcel.Company;
                package.Workbook.Properties.Author = context.GridExportExcel.Author;
                package.Workbook.Properties.Keywords = context.GridExportExcel.Keywords;

                package.Workbook.Worksheets.Add(context.GridExportExcel.Title);

                var list = context.DataObjects as IList;
                var propInfo = list.GetType().GetTypeInfo().GenericTypeArguments[0].GetProperties();

                ExcelWorksheet sheet = package.Workbook.Worksheets[context.GridExportExcel.Title];

                foreach(var column in context.GridExportExcel.Columns)
                {
                    if (string.IsNullOrEmpty(column.ColumnTitle))
                        column.ColumnTitle = $"Coluna {col}";

                    sheet.Cells[1, col].Value = column.ColumnTitle;

                    //if (!string.IsNullOrEmpty(column.HeaderHorizontalAlignment))
                    //    sheet.Cells[1, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.

                    if (!context.GridExportExcel.AutoFitColumns.HasValue || !context.GridExportExcel.AutoFitColumns.Value)
                        sheet.Column(col).Width = column.Width.Value;

                    col++;
                }
                //foreach (var column in propInfo)
                //{
                //    if (column.PropertyType == typeof(string) || !typeof(IEnumerable).IsAssignableFrom(column.PropertyType))
                //    {
                //        sheet.Cells[1, col].Value = "Coluna " + col;
                //        sheet.Column(col++).Width = 18;
                //    }
                //}

                foreach (var gridRow in list)
                {
                    col = 1;
                    foreach (var column in context.GridExportExcel.Columns)
                    {
                        sheet.Cells[row, col++].Value = propInfo.FirstOrDefault(x => x.Name == column.ColumnPropName).GetValue(gridRow, null);
                    }
                    //foreach (var column in propInfo)
                    //{
                    //    if (column.PropertyType == typeof(string) || !typeof(IEnumerable).IsAssignableFrom(column.PropertyType))
                    //        sheet.Cells[row, col++].Value = column.GetValue(gridRow, null);
                    //}

                    row++;
                }

                // Add to table / Add summary row
                var tbl = sheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: row-1, toColumn: col-1), context.GridExportExcel.Title);
                if(context.GridExportExcel.ShowHeader.HasValue)
                    tbl.ShowHeader = context.GridExportExcel.ShowHeader.Value;
                //tbl.TableStyle = TableStyles.Medium13;
                if (context.GridExportExcel.ShowTotal.HasValue)
                    tbl.ShowTotal = context.GridExportExcel.ShowTotal.Value;
                //tbl.Columns[3].DataCellStyleName = dataCellStyleName;
                //tbl.Columns[3].TotalsRowFunction = RowFunctions.Sum;
                //sheet.Cells[5, 4].Style.Numberformat.Format = numberformat;

                if (context.GridExportExcel.AutoFitColumns.HasValue && context.GridExportExcel.AutoFitColumns.Value)
                    sheet.Cells[1, 1, row-1, col-1].AutoFitColumns();

                return package.GetAsByteArray();
            }
        }

        #region Table Html

        public static TagBuilder BuildTableHtml(NccGridTagContext context, NccGridContext gridContext)
        {
            var table = new TagBuilder("table");
            table.Attributes.Add("class", !string.IsNullOrEmpty(context.CssClassGrid) ? context.CssClassGrid : "table");

            table.InnerHtml.AppendHtml(BuildTableHeader(context));

            table.InnerHtml.AppendHtml(BuildTableBody(context));

            //if (gridContext.AllowPaging)
            //    table.InnerHtml.AppendHtml(BuildTablePager(context, gridContext));

            return table;
        }

        private static TagBuilder BuildTableHeader(NccGridTagContext context)
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

        private static TagBuilder BuildTableBody(NccGridTagContext context)
        {
            var gridContent = context.GridRows;

            var tableBody = new TagBuilder("tbody");
            if (!string.IsNullOrEmpty(context.CssClassBody)) tableBody.Attributes.Add("class", context.CssClassBody);
            if (gridContent != null && gridContent.Count > 0)
            {
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
                            if (i == 0 || !(i > 0 && gridContent[i - 1].Cells[j].Value.GetContent() == cell.Value.GetContent()))
                            {
                                var sameCount = 1;
                                while (i + sameCount < gridContent.Count && gridContent[i + sameCount].Cells[j].Value.GetContent() == cell.Value.GetContent())
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
            }
            else if (context.EmptyRow != null)
            {
                var tr = new TagBuilder("tr");
                if (!string.IsNullOrEmpty(context.EmptyRow.CssClass)) tr.Attributes.Add("class", context.EmptyRow.CssClass);

                var emptyCell = context.EmptyRow.Cells.FirstOrDefault();

                var td = new TagBuilder("td")
                {
                    Attributes = { { "colspan", context.ColCount.ToString() } }
                };
                if (!string.IsNullOrEmpty(emptyCell.CssClass)) td.Attributes.Add("class", emptyCell.CssClass);

                td.InnerHtml.AppendHtml(emptyCell.Value);
                tr.InnerHtml.AppendHtml(td);
                tableBody.InnerHtml.AppendHtml(tr);
            }

            return tableBody;
        }

        public static TagBuilder BuildTablePager(NccGridTagContext context, NccGridContext gridContext)
        {
            var pageSize = gridContext.PageSize;
            if (gridContext.Filters.ContainsKey("pageSize"))
                pageSize = Convert.ToInt32(gridContext.Filters["pageSize"]);

            var totalPages = gridContext.TotalItems % pageSize > 0
                ? gridContext.TotalItems / pageSize + 1
                : gridContext.TotalItems / pageSize;

            if (totalPages <= 1)
                return null;

            var model = new NccActionModel { TargetIds = new [] { gridContext.Id } };
            model.Parameters.Add($"{DefaultParameters.ActionType.ToString().NccAddPrefix()}", "Filter");

            var footerContainerDiv = new TagBuilder("div") { Attributes = { { "class", context.CssClassFooterContainer ?? "row" } } };

            var divColLeft = new TagBuilder("div") {Attributes = {{"class", "col-sm-12 col-md-6"}}};
            var divColRight = new TagBuilder("div") { Attributes = { { "class", "col-sm-12 col-md-6" } } };
            var divRecordCount = new TagBuilder("div");
            if (!string.IsNullOrEmpty(context.CssClassRecordCountDiv))
                divRecordCount.Attributes.Add("class", context.CssClassRecordCountDiv);
            //TODO: Add RecordCount html

            var div = new TagBuilder("div") { Attributes = { { "class", context.CssClassPagerDiv ?? "nccGridPagerContainer" } } };
            var ul = new TagBuilder("ul") { Attributes = { { "class", context.CssClassPagerUl ?? "nccGridPagerPagination" } } };

            var firstLink = BuilPagerLink(1, model, context, "<<", false, gridContext.PageNumber == 1);
            var prevLink = BuilPagerLink(gridContext.PageNumber - 1, model, context, "<", false, gridContext.PageNumber == 1);

            ul.InnerHtml.AppendHtml(firstLink);
            ul.InnerHtml.AppendHtml(prevLink);

            var nextLink = BuilPagerLink(gridContext.PageNumber + 1, model, context, ">", false, gridContext.PageNumber == totalPages);
            var lastLink = BuilPagerLink(totalPages, model, context, ">>", false, gridContext.PageNumber == totalPages);

            var navSize = gridContext.PagerNavigationSize >= totalPages ? totalPages : gridContext.PagerNavigationSize;
            var linksBefore = navSize / 2 < gridContext.PageNumber
                ? navSize / 2
                : gridContext.PageNumber - 1;
            var linksAfter = navSize / 2 <= totalPages - gridContext.PageNumber
                ? navSize - linksBefore
                : totalPages - gridContext.PageNumber;

            if (navSize / 2 > linksAfter)
                linksBefore = linksBefore + (navSize / 2 - linksAfter);
            if (navSize % 2 == 0)
                if (linksBefore > 0 && linksBefore >= navSize / 2)
                    linksBefore--;
                else
                    linksAfter--;
            else if(navSize < linksBefore + linksAfter + 1)
                linksAfter--;

            for (var i = 0; i < linksBefore; i++)
            {
                var link = BuilPagerLink(gridContext.PageNumber - linksBefore + i, model, context);
                ul.InnerHtml.AppendHtml(link);
            }
            ul.InnerHtml.AppendHtml(BuilPagerLink(gridContext.PageNumber, model, context, null, true));
            for (var i = 0; i < linksAfter; i++)
            {
                var link = BuilPagerLink(gridContext.PageNumber + i + 1, model, context);
                ul.InnerHtml.AppendHtml(link);
            }

            ul.InnerHtml.AppendHtml(nextLink);
            ul.InnerHtml.AppendHtml(lastLink);

            div.InnerHtml.AppendHtml(ul);
            if (gridContext.GridPagerPosition == NccGridPagerPositionEnum.Left)
            {
                divColLeft.InnerHtml.AppendHtml(div);
                if (gridContext.ShowRecordsCount && gridContext.GridRecordCountPosition == NccGridPagerPositionEnum.Left) divColLeft.InnerHtml.AppendHtml(divRecordCount);
                else if (gridContext.ShowRecordsCount && gridContext.GridRecordCountPosition == NccGridPagerPositionEnum.Right) divColRight.InnerHtml.AppendHtml(divRecordCount);
            }
            else
            {
                if (gridContext.ShowRecordsCount && gridContext.GridRecordCountPosition == NccGridPagerPositionEnum.Left) divColLeft.InnerHtml.AppendHtml(divRecordCount);
                else if (gridContext.ShowRecordsCount && gridContext.GridRecordCountPosition == NccGridPagerPositionEnum.Right) divColRight.InnerHtml.AppendHtml(divRecordCount);
                divColRight.InnerHtml.AppendHtml(div);
            }

            if (gridContext.ShowRecordsCount && string.IsNullOrEmpty(context.PagerRecordsCountContent))
            {
                var firstRecord = pageSize * (gridContext.PageNumber - 1) + 1;
                var lastRecord = pageSize * (gridContext.PageNumber - 1) + pageSize;
                if (lastRecord > gridContext.TotalItems) lastRecord = gridContext.TotalItems;

                divRecordCount.InnerHtml.AppendHtml(string.Format($"<span>{gridContext.GridRecordsTemplate}</span>", firstRecord, lastRecord, gridContext.TotalItems));
            }

            footerContainerDiv.InnerHtml.AppendHtml(divColLeft);
            footerContainerDiv.InnerHtml.AppendHtml(divColRight);

            return footerContainerDiv;
        }

        private static TagBuilder BuilPagerLink(int pageNumber, NccActionModel model, NccGridTagContext context, string htmlContent = null, bool active = false, bool disabled = false)
        {
            var li = new TagBuilder("li");
            if (!string.IsNullOrEmpty(context.CssClassPagerLi)) li.Attributes.Add("class", context.CssClassPagerLi);
            var link = new TagBuilder("a")
            {
                Attributes =
                    {
                        {"name", "pageNumber" },
                        {"value", pageNumber.ToString() },
                        {"onclick", $"nccAction(null, $(this), '{JsonConvert.SerializeObject(model)}', '{NccConstants.AttributePrefix}');" }
                    }
            };
            link.InnerHtml.Append(string.IsNullOrEmpty(htmlContent) ? pageNumber.ToString() : htmlContent);
            if (active)
            {
                if (link.Attributes.ContainsKey("class"))
                    link.Attributes["class"] = li.Attributes["class"] + " active";
                else
                    link.Attributes.Add("class", "active");
            }
            else if (disabled)
            {
                if (link.Attributes.ContainsKey("class"))
                    link.Attributes["class"] = link.Attributes["class"] + " disabled";
                else
                    link.Attributes.Add("class", "disabled");
                link.Attributes.Remove("onclick");
            }
                        
            if (!disabled)
            {
                link.Attributes.Add("style", "cursor: pointer;");
            }

            if (!string.IsNullOrEmpty(context.CssClassPagerA))
                link.Attributes.Add("class", context.CssClassPagerA);

            li.InnerHtml.AppendHtml(link);

            return li;
        }

        #endregion


    }
}
