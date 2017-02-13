using System.Collections.Generic;
using System.Dynamic;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using System;
using System.Collections;
using System.Linq;
using ByteNuts.NetCoreControls.Models.Grid;
using ByteNuts.NetCoreControls.Models.HtmlRender;
using Microsoft.Extensions.Options;
using ByteNuts.NetCoreControls.Models;
using ByteNuts.NetCoreControls.Models.Select;

namespace ByteNuts.NetCoreControls.Services
{
    public static class DataService
    {
        public static T GetControlData<T>(T context, HttpContext httpContext) 
        {
            if (context == null || httpContext == null) return context;

            var options = ReflectionService.NccGetClassInstanceWithDi(httpContext, Constants.OptionsAssemblyName);
            var nccSettings = options != null ? ((IOptions<NccSettings>)options).Value : new NccSettings();

            var assemblyName = context.NccGetPropertyValue<string>("DataAccessClass");
            var autoBind = context.NccGetPropertyValue<bool>("AutoBind");
            var constructorParams = context.NccGetPropertyValue<object[]>("DataAccessParameters");

            if (!autoBind) return context;

            var dbService = nccSettings.UseDependencyInjection ? ReflectionService.NccGetClassInstanceWithDi(httpContext, assemblyName) : ReflectionService.NccGetClassInstance(assemblyName, constructorParams);

            if (dbService == null) return context;

            var methodName = context.NccGetPropertyValue<string>("SelectMethod");


            var methodParams = context.NccGetPropertyValue<ExpandoObject>("SelectParameters") as IDictionary<string, object>;
            var callParameters = methodParams.ToDictionary(x => x.Key, x => x.Value).NccToExpando() as IDictionary<string, object>;

            var filters = context.NccGetPropertyValue<Dictionary<string, string>>("Filters") ?? new Dictionary<string, string>();

            var renderDefault = context.NccGetPropertyValue<bool>("RenderDefault");

            if (!renderDefault)
            {
                AddFiltersToParameters(callParameters, filters);
                AddExtraToParameters(callParameters, context);
            }

            var result = dbService.NccInvokeMethod(methodName, (ExpandoObject)callParameters);

            if (result != null)
            {
                context = ProcessDataResult(context, result);
            }

            return context;
        }


        private static T ProcessDataResult<T>(T context, object result)
        {
            if (result == null) return default(T);

            if (result.GetType().ToString().Contains(typeof(Tuple).ToString()))
            {
                var item1 = result.GetType().GetProperty("Item1").GetValue(result);

                if (result.GetType().ToString().Contains("System.Linq.IQueryable"))
                    context.NccSetPropertyValue("DataObjects", ((IQueryable<object>)item1).ToList());
                else
                {
                    var list = item1 as IList;
                    context.NccSetPropertyValue("DataObjects", list != null ? new List<object>(list.Cast<object>()) : item1);
                }
            }
            else
            {
                context.NccSetPropertyValue("DataObjects", result);
            }

            //else if (result.GetType().ToString().Contains("Microsoft.EntityFrameworkCore.Internal.InternalDbSet"))
            //{
            //    context.NccSetPropertyValue("DataObjects", result);
            //}
            //else
            //{
            //    var list = result as IList;
            //    context.NccSetPropertyValue("DataObjects", list != null ? new List<object>(list.Cast<object>()) : result);
            //}

            context = ProcessControlDataResult(context, result);

            return context;
        }


        private static void AddFiltersToParameters(IDictionary<string, object> methodParams, Dictionary<string, string> filters)
        {
            foreach (var filter in filters)
            {
                methodParams[filter.Key] = filter.Value;
            }
        }


        private static void AddExtraToParameters(IDictionary<string, object> callParams, object context)
        {
            if (context is NccGridContext)
            {
                GridService.GetExtraParameters(callParams, context as NccGridContext);
            }
            else if (context is NccSelectContext)
            {
                SelectService.GetExtraParameters(callParams, context as NccSelectContext);
            }
            else if (context is NccHtmlRenderContext)
            {
                HtmlRenderService.GetExtraParameters(callParams, context as NccHtmlRenderContext);
            }
        }


        private static T ProcessControlDataResult<T>(T context, object result)
        {
            if (context is NccGridContext)
            {
                return (T)Convert.ChangeType(GridService.SetDataResult(context as NccGridContext, result), typeof(T));
            }
            else if (context is NccSelectContext)
            {
                return (T)Convert.ChangeType(SelectService.SetDataResult(context as NccSelectContext, result), typeof(T));
            }
            else if (context is NccHtmlRenderContext)
            {
                return (T)Convert.ChangeType(HtmlRenderService.SetDataResult(context as NccHtmlRenderContext, result), typeof(T));
            }

            return context;
        }

    }
}
