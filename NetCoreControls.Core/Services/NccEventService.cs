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

namespace ByteNuts.NetCoreControls.Core.Services
{
    public static  class NccEventService
    {
        public static void ProcessEvent<T>(object service, ControllerContext controllerContext, T controlCtx, IFormCollection formCollection, Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey($"{DefaultParameters.EventName.ToString().NccAddPrefix()}"))
                throw new Exception("An event has been called but no method name has been provided!");

            var methodInfo = service.GetType().GetMethod(parameters[$"{DefaultParameters.EventName.ToString().NccAddPrefix()}"]);

            if (methodInfo == null)
                throw new Exception("Não foi encontrado um método com o nome do evento indicado.");

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
                        parametersArray.Add(controllerContext);
                    }
                    else if (parameter.ParameterType == typeof(T))
                    {
                        parametersArray.Add(controlCtx);
                    }
                    else if (parameter.ParameterType == typeof(IFormCollection))
                    {
                        if (formCollection == null)
                            throw new Exception("Para aceder à Form Collection deve renderizar um form no controlo. Por favor, coloque a propriedade RenderForm=\"true\" no controlo.");
                        parametersArray.Add(formCollection);
                    }
                    else if (parameter.ParameterType == typeof(string))
                    {
                        if (parameters.ContainsKey($"{parameter.Name.NccAddPrefix()}"))
                            parametersArray.Add(parameters[$"{parameter.Name.NccAddPrefix()}"]);
                    }
                }

                if (methodParams.Length != parametersArray.Count)
                    throw new Exception("Um ou mais parâmetros do método não puderam ser mapeados. Por favor, verifique se é possível obter os parâmetros indicados.");

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
            strong.InnerHtml.Append("Ocorreu um erro!");
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


        private static void MapDataKeysToRow<T>(T row, Dictionary<string, object> dataKeysRow)
        {
            foreach (var dataKey in dataKeysRow)
            {
                var dkType = row.NccGetPropertyValue<object>(dataKey.Key).GetType();
                row.NccSetPropertyValue(dataKey.Key, Convert.ChangeType(dataKey.Value, dkType));
            }
        }

    }
}