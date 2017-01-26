using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;
using System.Threading.Tasks;
using ByteNuts.NetCoreControls.Models;
using ByteNuts.NetCoreControls.Models.Enums;
using ByteNuts.NetCoreControls.Models.Grid;
using ByteNuts.NetCoreControls.Services;

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
                    service = ReflectionService.NccGetClassInstance(_context.EventHandlerClass, null);

                _nccTagContext.GridRows = new List<GridRow>();

                if (data.Count > _context.PageSize)
                    data = new List<object>(data.Cast<object>()).Skip(_context.PageSize * (_context.PageNumber - 1)).Take(_context.PageSize).ToList();

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

                    service?.NccInvokeMethod(GridViewEvents.RowDataBound, new object[] { new NccEventArgs { NccTagContext = _nccTagContext, NccControlContext = _context, DataObjects = data }, row });

                    _nccTagContext.GridRows.Add(new GridRow { Cells = new List<GridCell>(), RowNumber = _nccTagContext.RowNumber });

                    var rowData = row.NccToExpando() as IDictionary<string, object>;
                    rowData["NccRowNumber"] = _nccTagContext.RowNumber;

                    ViewContext.ViewData.Model = rowData.ExtToExpandoObject();

                    await output.GetChildContentAsync(false);

                    //ViewContext.ViewBag.RowCount
                    _nccTagContext.RowNumber++;
                    _nccTagContext.ColCountComplete = true;

                    service?.NccInvokeMethod(GridViewEvents.RowCreated, new object[] { new NccEventArgs { NccTagContext = _nccTagContext, NccControlContext = _context, DataObjects = data }, _nccTagContext.GridRows.LastOrDefault() });
                }
            }
        }
    }
}
