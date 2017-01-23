using System.Collections.Generic;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ByteNuts.NetCoreControls.Services;
using ByteNuts.NetCoreControls.Models.Grid;
using System;
using System.Linq;
using System.Reflection;
using ByteNuts.NetCoreControls.Models.Enums;
using ByteNuts.NetCoreControls.Extensions;
using ByteNuts.NetCoreControls.Models;
using System.Runtime.Loader;
using ByteNuts.NetCoreControls.Helpers;

namespace ByteNuts.NetCoreControls.Controllers
{
    public class NetCoreControlsController : Controller
    {
        private readonly IDataProtector _protector;

        public NetCoreControlsController(IDataProtectionProvider protector)
        {
            _protector = protector.CreateProtector(Constants.DataProtectionKey);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult ControlAction(IFormCollection formCollection, string context, Dictionary<string, string> parameters)
        {
            if (context == null)
                throw new Exception("The control context wasn't submitted. Please, verify if the target id is correct.");

            var controlCtx = NccJson.GetObjectFromJson<object>(_protector.Unprotect(context));

            if (controlCtx == null)
                throw new Exception("The control context is invalid! Please, refresh the page to get a new valid context.");

            var parametersList = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (parameters != null && parameters.Count > 0)
                parametersList = new Dictionary<string, string>(parameters, StringComparer.OrdinalIgnoreCase);

            if (!parametersList.ContainsKey($"{DefaultParameters.ActionType.ToString().NccAddPrefix()}"))
                throw new Exception("No action type was specified!");

            var eventHandler = controlCtx.NccGetPropertyValue<string>("EventHandlerClass");
            var service = ReflectionService.NccGetClassInstance(eventHandler, null);

            service?.NccInvokeMethod(NccEvents.PostBack, new object[] { new NccEventArgs { Controller = this, NccControlContext = controlCtx, FormCollection = formCollection } });

            switch (parametersList[$"{DefaultParameters.ActionType.ToString().NccAddPrefix()}"].ToLower())
            {
                case "filter":
                    if (!(parametersList.Keys.Any(k => k.StartsWith($"{DefaultParameters.ElemName.ToString().NccAddPrefix()}")) && parametersList.Keys.Any(k => k.StartsWith($"{DefaultParameters.ElemValue.ToString().NccAddPrefix()}"))))
                        throw new Exception("The filter doesn't contain the necessary name and value pair.");

                    var filters = controlCtx.NccGetPropertyValue<Dictionary<string, string>>("Filters") ?? new Dictionary<string, string>();
                    var resetPaging = true;
                    foreach (var inputFilters in parametersList.Where(x => x.Key.StartsWith($"{DefaultParameters.ElemName.ToString().NccAddPrefix()}")).ToList())
                    {
                        if (inputFilters.Value.Contains("pageNumber"))
                            resetPaging = false;
                        var filterId = inputFilters.Key.Replace($"{DefaultParameters.ElemName.ToString().NccAddPrefix()}", "");
                        filters[inputFilters.Value] = parametersList[$"{DefaultParameters.ElemValue.ToString().NccAddPrefix()}{filterId}"];
                    }

                    if (resetPaging)
                        filters["pageNumber"] = "1";

                    controlCtx.NccSetPropertyValue("Filters", filters);
                    
                    break;
                case "event":
                    if (service == null) throw new Exception("EventHandler must be registered and must exist to process events");
                    EventService.ProcessEvent(service, ControllerContext, controlCtx, formCollection, parametersList);
                    break;
                    //Events from Controls are mapped here
                case "gridaction":
                    if (!parametersList.ContainsKey($"{DefaultParameters.EventName.ToString().NccAddPrefix()}"))
                        throw new Exception("No EventName specified for the GridView action!");
                    if (!(controlCtx is GridContext))
                        throw new Exception("A GridAction was specified but the context is not of type GridViewContext!");
                    switch (parametersList[$"{DefaultParameters.EventName.ToString().NccAddPrefix()}"].ToLower())
                    {
                        case "update":
                            if (service == null) throw new Exception("EventHandler must be registered and must exist to process events");
                            service.NccInvokeMethod(GridViewEvents.Update, new object[] { new NccEventArgs { Controller = this, NccControlContext = controlCtx, FormCollection = formCollection } });
                            break;
                        case "updaterow":
                            break;
                        case "deleterow":
                            if (!parametersList.ContainsKey($"{GridViewParameters.RowNumber.ToString().NccAddPrefix()}"))
                                throw new Exception("The row number wasn't received... Something wrong has happened...");

                            var rowPos = Convert.ToInt32(parametersList[$"{GridViewParameters.RowNumber.ToString().NccAddPrefix()}"]);

                            if (service == null) throw new Exception("EventHandler must be registered and must exist to process events");
                            service.NccInvokeMethod(GridViewEvents.DeleteRow, new object[] { new NccEventArgs { Controller = this, NccControlContext = controlCtx, FormCollection = formCollection }, rowPos });

                            break;
                        default:
                            throw new Exception("The specified EventName it's not supported on the GridView Component!");
                    }
                    break;
                default:
                    throw new Exception("The specified ActionType it's not supported on the NetCoreControls!");
            }


            var id = controlCtx.NccGetPropertyValue<string>("Id");
            var controlViewPath = controlCtx.NccGetPropertyValue<ViewsPathsModel>("ViewPaths").ViewPath;
            ViewData[id] = controlCtx;

            return PartialView(controlViewPath);
        }

        public FileResult GetNccJsFile()
        {
            var assembly = this.GetType().GetTypeInfo().Assembly;
            //(typeof(NetCoreControls.Constants).Assembly).GetManifestResourceStream("NetCoreControls.Scripts.ncc.js")

            var stream = assembly.GetManifestResourceStream("NetCoreControls.Scripts.ncc.js");


            return File(stream, "application/javascript");
        }

        public FileResult GetNccCssFile()
        {
            var assembly = this.GetType().GetTypeInfo().Assembly;
            //(typeof(NetCoreControls.Constants).Assembly).GetManifestResourceStream("NetCoreControls.Scripts.ncc.js")

            var stream = assembly.GetManifestResourceStream("NetCoreControls.Styles.ncc.css");


            return File(stream, "text/css");
        }
    }
}
