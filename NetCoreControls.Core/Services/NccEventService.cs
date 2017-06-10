using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Encodings.Web;
using ByteNuts.NetCoreControls.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ByteNuts.NetCoreControls.Core.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;
using ByteNuts.NetCoreControls.Core.Models;

namespace ByteNuts.NetCoreControls.Core.Services
{
    public static  class NccEventService
    {
        public static void ProcessEvent<T>(object service, Controller controller, T controlCtx, IFormCollection formCollection, Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey($"{DefaultParameters.EventName.ToString().NccAddPrefix()}"))
                throw new Exception("An event has been called but no method name has been provided!");

            var methodInfo = service.GetType().GetMethod(parameters[DefaultParameters.EventName.ToString().NccAddPrefix()]);

            if (methodInfo == null)
                throw new Exception($"No method named '{parameters[DefaultParameters.EventName.ToString().NccAddPrefix()]}' was found in the provided event handler class.");

            var methodParams = methodInfo.GetParameters();

            if (methodParams.Length == 0)
            {
                methodInfo.Invoke(service, null);
            }
            else
            {
                var parametersArray = new List<object>();

                foreach (var parameter in methodParams)
                {
                    if (parameter.ParameterType == typeof(ControllerContext))
                    {
                        parametersArray.Add(controller.ControllerContext);
                    }
                    else if (parameter.ParameterType == typeof(Controller))
                    {
                        parametersArray.Add(controller);
                    }
                    else if (parameter.ParameterType == typeof(T))
                    {
                        parametersArray.Add(controlCtx);
                    }
                    else if (parameter.ParameterType == typeof(IFormCollection))
                    {
                        if (formCollection == null)
                            throw new Exception("To get the FormCollection the control must render a form. Please, change the control 'RenderForm' property to true. Also make sure the control is not already placed inside another form, in order to prevent unwanted behaviors. Nested forms are not allowed in HTML.");
                        parametersArray.Add(formCollection);
                    }
                    else if (parameter.ParameterType == typeof(NccEventArgs))
                    {
                        parametersArray.Add(new NccEventArgs { Controller = controller, NccControlContext = controlCtx, FormCollection = formCollection, Parameters = parameters });
                    }
                    else if (parameter.ParameterType == typeof(string))
                    {
                        if (parameters.ContainsKey($"{parameter.Name.NccAddPrefix()}"))
                            parametersArray.Add(parameters[$"{parameter.Name.NccAddPrefix()}"]);
                    }
                }

                if (methodParams.Length != parametersArray.Count)
                    throw new Exception("One or more of the method parameters were not mapped. Please, ensure that all parameters are feasible to obtain.");

                methodInfo.Invoke(service, parametersArray.ToArray());
            }
        }

        public static string ProcessError(Exception e)
        {
            var content = new TagBuilder("div")
            {
                Attributes = { { "class", "alert alert-danger" }, { "style", "text-align:center;" } }
            };
            var h4 = new TagBuilder("h4");
            var p = new TagBuilder("p");
            var strong = new TagBuilder("strong");
            strong.InnerHtml.Append("An error as occured!");
            var span = new TagBuilder("span");
            span.InnerHtml.Append(!string.IsNullOrEmpty(e.Message) ? e.Message : e.InnerException.Message);

            h4.InnerHtml.AppendHtml(strong);
            p.InnerHtml.AppendHtml(span);
            content.InnerHtml.AppendHtml(h4);
            content.InnerHtml.AppendHtml(p);

            var writer = new System.IO.StringWriter();
            content.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }


        //public static async Task<bool> NccBindModel<T>(NetCoreControlsController controller, T model, List<Dictionary<string, object>> dataKeys, string prefix = "") where T : class
        //{
        //    var ok = await controller.TryUpdateModelAsync(model, prefix);

        //    if (!ok) return false;

        //    var list = model as IList;
        //    if (list?.Count != dataKeys.Count) return false;

        //    for (var i = 0; i < list.Count; i++)
        //    {
        //        MapDataKeysToRow(list[i], dataKeys[i]);
        //    }

        //    return true;
        //}

        public static bool NccBindDataKeys<T>(T model, List<Dictionary<string, object>> dataKeys, int rowPos = -1) where T : class
        {
            if (rowPos >= 0)
                MapDataKeysToRow(model, dataKeys[rowPos]);
            else
            {
                var list = model as IList;
                if (list?.Count != dataKeys.Count) return false;

                for (var i = 0; i < list.Count; i++)
                {
                    MapDataKeysToRow(list[i], dataKeys[i]);
                }
            }

            return true;
        }

        public static bool NccUpdateModelWithDataKeys<T>(T model, List<Dictionary<string, object>> dataKeys) where T : class
        {
            var itemType = model.GetType().GetGenericArguments()[0]; // use this...

            var list = model as IList;

            for (var i = 0; i < dataKeys.Count; i++)
            {
                list.Add(Activator.CreateInstance(itemType));
                MapDataKeysToRow(list[i], dataKeys[i]);
            }

            return true;
        }


        private static void MapDataKeysToRow<T>(T row, Dictionary<string, object> dataKeysRow)
        {
            foreach (var dataKey in dataKeysRow)
            {
                var dkType = row.NccGetPropertyValue<object>(dataKey.Key)?.GetType();
                if (dkType != null)
                    row.NccSetPropertyValue(dataKey.Key, Convert.ChangeType(dataKey.Value, dkType));
            }
        }

    }
}