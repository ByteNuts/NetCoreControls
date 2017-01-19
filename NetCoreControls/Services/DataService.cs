using System.Collections.Generic;
using System.Dynamic;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using ByteNuts.NetCoreControls.Models.GridView;
using ByteNuts.NetCoreControls.Models.HtmlRender;

namespace ByteNuts.NetCoreControls.Services
{
    public static class DataService
    {
        public static T GetControlData<T>(T context, HttpContext httpContext) 
        {
            if (context == null || httpContext == null) return context;

            var assemblyName = context.NccGetPropertyValue<string>("DataAccessClass");
            var autoBind = context.NccGetPropertyValue<bool>("AutoBind");
            var useDi = context.NccGetPropertyValue<bool>("UseDependencyInjection");
            var constructorParams = context.NccGetPropertyValue<object[]>("DataAccessParameters");

            if (autoBind)
            {
                var dbService = useDi ? ReflectionService.NccGetClassInstanceWithDi(httpContext, assemblyName) : ReflectionService.NccGetClassInstance(assemblyName, constructorParams);

                if (dbService == null) return context;

                var methodName = context.NccGetPropertyValue<string>("SelectMethod");


                var methodParams = context.NccGetPropertyValue<ExpandoObject>("SelectParameters") as IDictionary<string, object>;
                var callParameters = methodParams.ToDictionary(x => x.Key, x => x.Value).NccToExpando() as IDictionary<string, object>;

                var filters = context.NccGetPropertyValue<Dictionary<string, string>>("Filters") ?? new Dictionary<string, string>();

                callParameters = AddFiltersToParameters(callParameters, filters);
                callParameters = AddExtraToParameters(callParameters, context);

                var result = dbService.NccInvokeMethod(methodName, (ExpandoObject)callParameters);

                if (result != null)
                {
                    context = ProcessDataResult(context, result);
                }
            }

            return context;
        }


        private static T ProcessDataResult<T>(T context, object result)
        {
            if (result == null) return default(T);

            if (result.GetType().ToString().Contains(typeof(Tuple).ToString()))
            {
                var item1 = result.GetType().GetProperty("Item1").GetValue(result);

                var list = item1 as IList;
                context.NccSetPropertyValue("DataObjects", list != null ? new List<object>(list.Cast<object>()) : item1);
            }
            else
            {
                var list = result as IList;
                context.NccSetPropertyValue("DataObjects", list != null ? new List<object>(list.Cast<object>()) : result);
            }

            context = ProcessControlDataResult(context, result);

            return context;
        }


        private static IDictionary<string, object> AddFiltersToParameters(IDictionary<string, object> methodParams, Dictionary<string, string> filters)
        {
            foreach (var filter in filters)
            {
                methodParams[filter.Key] = filter.Value;
            }

            return methodParams;
        }


        private static IDictionary<string, object> AddExtraToParameters(IDictionary<string, object> callParams, object context)
        {
            if (context is GridViewContext)
            {
                return GridViewService.GetExtraParameters(callParams, context as GridViewContext);
            }
            else if (context is HtmlRenderContext)
            {
                return HtmlRenderService.GetExtraParameters(callParams, context as HtmlRenderContext);
            }

            return null;
        }


        private static T ProcessControlDataResult<T>(T context, object result)
        {
            if (context is GridViewContext)
            {
                return (T)Convert.ChangeType(GridViewService.SetDataResult(context as GridViewContext, result), typeof(T));
            }
            else if (context is HtmlRenderContext)
            {
                return (T)Convert.ChangeType(HtmlRenderService.SetDataResult(context as HtmlRenderContext, result), typeof(T));
            }

            return default(T);
        }

    }
}
