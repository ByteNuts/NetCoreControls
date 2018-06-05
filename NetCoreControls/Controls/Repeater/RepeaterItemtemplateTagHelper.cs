using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using ByteNuts.NetCoreControls.Models.Repeater;
using System.Collections;
using ByteNuts.NetCoreControls.Core.Extensions;
using ByteNuts.NetCoreControls.Core.Models;
using ByteNuts.NetCoreControls.Core.Services;
using ByteNuts.NetCoreControls.Services;
using ByteNuts.NetCoreControls.Models.Enums;
using ByteNuts.NetCoreControls.Models;

namespace ByteNuts.NetCoreControls.Controls.Repeater
{
    [HtmlTargetElement("ncc:repeater-itemtemplate", ParentTag = "ncc:repeater")]
    public class RepeaterItemtemplateTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        private NccRepeaterTagContext _nccTagContext;
        private NccRepeaterContext _context;

        public override void Init(TagHelperContext tagContext)
        {
            if (tagContext.Items.ContainsKey(typeof(NccRepeaterTagContext)))
                _nccTagContext = (NccRepeaterTagContext)tagContext.Items[typeof(NccRepeaterTagContext)];
            else
                throw new Exception("RepeaterNccTagContext was lost between tags...");
        }

        public override async Task ProcessAsync(TagHelperContext tagContext, TagHelperOutput output)
        {
            output.SuppressOutput();

            if (tagContext.Items.ContainsKey(typeof(NccRepeaterContext)))
                _context = (NccRepeaterContext)tagContext.Items[typeof(NccRepeaterContext)];
            else
                return;

            var gridViewContext = (NccRepeaterContext)tagContext.Items[typeof(NccRepeaterContext)];

            var data = gridViewContext.DataObjects.GetType().ToString().Contains("Microsoft.EntityFrameworkCore.Internal.InternalDbSet") ||
                gridViewContext.DataObjects.GetType().ToString().Contains("Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryable") ?
                ((IQueryable<object>)gridViewContext.DataObjects).ToList() : gridViewContext.DataObjects as IList;

            if (data != null && data.Count > 0)
            {
                object service = null;
                if (!string.IsNullOrEmpty(_context.EventHandlerClass))
                    service = NccReflectionService.NccGetClassInstance(_context.EventHandlerClass, null);

                foreach (var item in data)
                {
                    service?.NccInvokeMethod(NccRepeaterEventsEnum.ItemDataBound.ToString(), new object[] { new NccEventArgs { NccTagContext = _nccTagContext, NccControlContext = _context, DataObjects = data }, item });

                    var itemData = item.NccToExpando() as IDictionary<string, object>;
                    ViewContext.ViewData.Model = itemData.ExtToExpandoObject();

                    var childContent = await output.GetChildContentAsync(false);

                    _nccTagContext.RepeaterItems.Add(childContent.GetContent());
                }
            }
        }
    }
}
