using System;
using System.Collections.Generic;
using System.Reflection;
using ByteNuts.NetCoreControls.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ByteNuts.NetCoreControls.Models.Enums;

namespace ByteNuts.NetCoreControls.Services
{
    public static  class EventService
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
    }
}
