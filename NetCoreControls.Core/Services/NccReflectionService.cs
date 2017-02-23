using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using ByteNuts.NetCoreControls.Core.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ByteNuts.NetCoreControls.Core.Services
{
    public static class NccReflectionService
    {
        public static object NccGetClassInstance(string assemblyName, object[] constructorParameters)
        {
            if (string.IsNullOrEmpty(assemblyName))
                return null;

            var type = Type.GetType(assemblyName);

            return NccGetClassInstance(type, constructorParameters);
        }

        public static object NccGetClassInstance(Type classType, object[] constructorParameters)
        {
            if (constructorParameters != null && constructorParameters.Length > 0)
                return Activator.CreateInstance(classType, constructorParameters);

            return Activator.CreateInstance(classType);
        }

        public static object NccGetClassInstanceWithDi(HttpContext httpContext, string assemblyName)
        {
            var type = Type.GetType(assemblyName);

            return NccGetClassInstanceWithDi(httpContext, type);
        }

        public static object NccGetClassInstanceWithDi(HttpContext httpContext, Type classType)
        {
            return httpContext.RequestServices.GetService(classType);
        }

        public static object NccInvokeMethod(this object classInstance, string methodName, ExpandoObject controlParameters)
        {
            var methodParams = (IDictionary<string, object>) controlParameters;
            object result = null;
            var methodInfo = classInstance.GetType().GetMethod(methodName);
            if (methodInfo != null)
            {
                var parameters = methodInfo.GetParameters();

                if (parameters.Length == 0)
                    result = methodInfo.Invoke(classInstance, null);
                else
                {
                    var parametersArray = new List<object>();

                    foreach (var parameter in parameters)
                    {
                        var parameterType = Nullable.GetUnderlyingType(parameter.ParameterType) ?? parameter.ParameterType;

                        parametersArray.Add(methodParams[parameter.Name] != null ? Convert.ChangeType(methodParams[parameter.Name], parameterType) : null);
                    }

                    result = methodInfo.Invoke(classInstance, parametersArray.ToArray());
                }
            }

            return result;
        }

        public static object NccInvokeMethod(this object classInstance, string methodName, object[] methodParams)
        {
            var methodInfo = classInstance.GetType().GetMethod(methodName);
            var parameters = methodInfo?.GetParameters();
            return methodInfo?.Invoke(classInstance, parameters.Length == 0 ? null : methodParams);
        }

        public static object NccInvokeMethod(this object classInstance, NccEventsEnum eventName, object[] methodParams)
        {
            return classInstance.NccInvokeMethod(eventName.ToString(), methodParams);
        }
        //public static object NccInvokeMethod(this object classInstance, NccGridEventsEnum eventName, object[] methodParams)
        //{
        //    return classInstance.NccInvokeMethod(eventName.ToString(), methodParams);
        //}
        //public static object NccInvokeMethod(this object classInstance, NccSelectEventsEnum eventName, object[] methodParams)
        //{
        //    return classInstance.NccInvokeMethod(eventName.ToString(), methodParams);
        //}
        //public static object NccInvokeMethod(this object classInstance, NccRepeaterEventsEnum eventName, object[] methodParams)
        //{
        //    return classInstance.NccInvokeMethod(eventName.ToString(), methodParams);
        //}


        public static T NccGetPropertyValue<T>(this object obj, string propertyName)
        {
            try
            {
                var expando = obj as IDictionary<string, object>;
                if (expando != null)
                    return (T)Convert.ChangeType(expando[propertyName], typeof(T));
                
                var propVal = obj.GetType().GetProperty(propertyName).GetValue(obj, null);
                return (T)Convert.ChangeType(propVal, typeof(T));
            }
            catch (Exception e)
            {
                return default(T);
            }
        }

        public static void NccSetPropertyValue(this object obj, string propertyName, object propertyValue)
        {
            obj.GetType().GetProperty(propertyName)?.SetValue(obj, propertyValue);
        }

        public static bool NccPropertyExists(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName) != null;
        }

        public static ExpandoObject ExtToExpandoObject(this object anonymousObject)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (var item in new RouteValueDictionary(anonymousObject))
                expando.Add(item);
            return (ExpandoObject)expando;
        }
    }
}
