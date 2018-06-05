using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;
using System.Threading.Tasks;
using ByteNuts.NetCoreControls.Core;
using ByteNuts.NetCoreControls.Core.Extensions;
using ByteNuts.NetCoreControls.Core.Models;
using ByteNuts.NetCoreControls.Core.Models.Enums;
using ByteNuts.NetCoreControls.Core.Services;
using ByteNuts.NetCoreControls.Models.Enums;
using ByteNuts.NetCoreControls.Models.Grid;
using Newtonsoft.Json;

namespace ByteNuts.NetCoreControls.Controls.Grid
{
    [RestrictChildren("ncc:grid-columnbound", "ncc:grid-columntemplate")]
    [HtmlTargetElement("ncc:grid-columns", ParentTag = "ncc:grid")]
    public class GridColumnsTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }


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

            var gridViewContext = (NccGridContext)tagContext.Items[typeof(NccGridContext)];

            var global = ViewContext.ViewData.Model;

            output.TagName = null;

            var data = gridViewContext.DataObjects as IList;

            _nccTagContext.RowNumber = 0;
            ViewContext.ViewBag.EmptyData = false;

            if (data != null && data.Count > 0)
            {
                object service = null;
                if (!string.IsNullOrEmpty(_context.EventHandlerClass))
                    service = NccReflectionService.NccGetClassInstance(_context.EventHandlerClass, null);

                _nccTagContext.GridRows = new List<GridRow>();

                if (data.Count > _context.PageSize)
                {
                    if (_context.Filters.ContainsKey("pageSize"))
                    {
                        var pageSize = Convert.ToInt32(_context.Filters["pageSize"]);
                        data = new List<object>(data.Cast<object>()).Skip(pageSize * (_context.PageNumber - 1)).Take(pageSize).ToList();
                    }
                    else
                        data = new List<object>(data.Cast<object>()).Skip(_context.PageSize * (_context.PageNumber - 1)).Take(_context.PageSize).ToList();
                }

                _context.DataObjects = data;

                var dataKeys = _context.DataKeys?.Split(',')?.ToList();
                if (dataKeys != null) _context.DataKeysValues = new List<Dictionary<string, object>>();

                foreach (var row in data)
                {
                    if (dataKeys != null)
                    {
                        var dataKeysRow = new Dictionary<string, object>();
                        foreach (var dataKey in dataKeys)
                        {
                            var keyValue = row.NccGetPropertyValue<object>(dataKey);
                            if (keyValue != null)
                                dataKeysRow[dataKey] = keyValue;
                        }
                        _context.DataKeysValues.Add(dataKeysRow);
                    }

                    service?.NccInvokeMethod(NccGridEventsEnum.RowDataBound.ToString(), new object[] { new NccEventArgs { NccTagContext = _nccTagContext, NccControlContext = _context, ViewContext = ViewContext, DataObjects = data }, row });

                    _nccTagContext.GridRows.Add(new GridRow { Cells = new List<GridCell>(), RowNumber = _nccTagContext.RowNumber });

                    var rowData = row.NccToExpando() as IDictionary<string, object>;
                    rowData["NccRowNumber"] = _nccTagContext.RowNumber;

                    ViewContext.ViewData.Model = rowData.NccToExpando();

                    await output.GetChildContentAsync(false);

                    if (_context.AutoGenerateEditButton)
                    {
                        var rowModel = _nccTagContext.GridRows.LastOrDefault();

                        if (_context.AdditionalData.ContainsKey("EditRowNumber") && _context.AdditionalData["EditRowNumber"].ToString() == _nccTagContext.RowNumber.ToString())
                        {
                            var model = new NccActionModel
                            {
                                TargetIds = new[] { _context.Id }
                            };
                            model.Parameters.Add($"{DefaultParameters.ActionType.ToString().NccAddPrefix()}", "GridAction");
                            model.Parameters.Add($"{DefaultParameters.EventName.ToString().NccAddPrefix()}", "CancelEditRow");
                            model.Parameters.Add($"{NccGridParametersEnum.RowNumber.ToString().NccAddPrefix()}", _nccTagContext.RowNumber.ToString());
                            var link = new TagBuilder("a")
                            {
                                Attributes =
                                    {
                                        {"style", "cursor:pointer;" },
                                        {"onclick", $"nccAction(null, $(this), '{JsonConvert.SerializeObject(model)}', '{NccConstants.AttributePrefix}');" }
                                    }
                            };
                            link.InnerHtml.AppendHtml("Cancel");

                            model.Parameters[$"{DefaultParameters.EventName.ToString().NccAddPrefix()}"] = "UpdateRow";
                            var link2 = new TagBuilder("a")
                            {
                                Attributes =
                                    {
                                        {"style", "cursor:pointer;" },
                                        {"onclick", $"nccAction(null, $(this), '{JsonConvert.SerializeObject(model)}', '{NccConstants.AttributePrefix}');" }
                                    }
                            };
                            link2.InnerHtml.AppendHtml("Save");

                            var cell = new GridCell();
                            cell.Value.AppendHtml(link);
                            rowModel?.Cells.Add(cell);
                            var cell2 = new GridCell();
                            cell2.Value.SetHtmlContent(link2);
                            rowModel?.Cells.Add(cell2);

                        }
                        else
                        {
                            var model = new NccActionModel
                            {
                                TargetIds = new[] {_context.Id}
                            };
                            model.Parameters.Add($"{DefaultParameters.ActionType.ToString().NccAddPrefix()}", "GridAction");
                            model.Parameters.Add($"{DefaultParameters.EventName.ToString().NccAddPrefix()}", "EditRow");
                            model.Parameters.Add($"{NccGridParametersEnum.RowNumber.ToString().NccAddPrefix()}", _nccTagContext.RowNumber.ToString());

                            var link = new TagBuilder("a")
                            {
                                Attributes =
                                    {
                                        {"style", "cursor:pointer;" },
                                        {"onclick", $"nccAction(null, $(this), '{JsonConvert.SerializeObject(model)}', '{NccConstants.AttributePrefix}');" }
                                    }
                            };
                            link.InnerHtml.AppendHtml("Edit");

                            var cell = new GridCell();
                            cell.Value.AppendHtml(link);
                            rowModel?.Cells.Add(cell);
                        }
                    }
                    //ViewContext.ViewBag.RowCount
                    _nccTagContext.RowNumber++;
                    _nccTagContext.ColCountComplete = true;

                    service?.NccInvokeMethod(NccGridEventsEnum.RowCreated.ToString(), new object[] { new NccEventArgs { NccTagContext = _nccTagContext, NccControlContext = _context, DataObjects = data }, _nccTagContext.GridRows.LastOrDefault() });
                }
            }
        }
    }
}
