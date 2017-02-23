using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;
using ByteNuts.NetCoreControls.Core.Models;

namespace ByteNuts.NetCoreControls.Core.Services
{
    public static class NccControlsService
    {
        public static TagBuilder GetEncodedContext(IDataProtector dataProtector, string id, object context)
        {
            var encContext = new TagBuilder("input");
            encContext.Attributes.Add("name", "encContext");
            encContext.Attributes.Add("id", $"{id}_context");
            encContext.Attributes.Add("type", "hidden");
            encContext.Attributes.Add("value", dataProtector.Protect(NccJsonService.SetObjectAsJson(context)));

            return encContext;
        }

        public static TagBuilder GetAjaxLoaderOverlay()
        {
            var overlayAjaxLoader = new TagBuilder("div")
            {
                Attributes = { { "class", "overlayAjaxLoader" } }
            };

            return overlayAjaxLoader;
        }

        public static TagBuilder GetAjaxLoaderImage()
        {
            var ajaxLoader = new TagBuilder("img")
            {
                Attributes =
                {
                    {"class", "ajaxLoader"},
                    {"alt", "Loading..." },
                    {"src", $"data:image/png;base64,{NccConstants.AjaxLoaderImg}" }
                }
            };

            return ajaxLoader;
        }

        public static void BindData<T>(T context, HttpContext httpContext, NccActionsService.ExtraParameters<T> extraParameters, NccActionsService.DataResult<T> dataResult)
        {
            var autoBind = context.NccGetPropertyValue<bool>("AutoBind");
            if (autoBind)
            {
                DataService.GetControlData(context, httpContext, extraParameters, dataResult);
            }
            else
            {

                var dataSource = (context as NccContext)?.DataSource;
                var dataObjects = dataSource.GetType().ToString().Contains("Microsoft.EntityFrameworkCore.Internal.InternalDbSet") ||
                    dataSource.GetType().ToString().Contains("Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryable") ||
                    dataSource.GetType().ToString().Contains("System.Linq.IQueryable") ?
                    ((IQueryable<object>)dataSource).ToList() : dataSource;

                context.NccSetPropertyValue("DataSource", dataObjects);
                context.NccSetPropertyValue("DataObjects", dataObjects);

                if (context.NccPropertyExists("TotalItems"))
                {
                    var data = dataObjects as IList;
                    context.NccSetPropertyValue("TotalItems", data?.Count ?? 0);
                }
            }
        }
    }
}
