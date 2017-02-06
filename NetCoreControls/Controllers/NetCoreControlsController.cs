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
using ByteNuts.NetCoreControls.Helpers;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Text.Encodings.Web;
using ByteNuts.NetCoreControls.Controls.Grid.Events;
using Microsoft.Extensions.Options;

namespace ByteNuts.NetCoreControls.Controllers
{
    public class NetCoreControlsController : Controller
    {
        private readonly IDataProtector _protector;
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly NccSettings _nccSettings;

        public NetCoreControlsController(IDataProtectionProvider protector, IRazorViewEngine razorViewEngine, IHttpContextAccessor contextAccessor)
        {
            var options = ReflectionService.NccGetClassInstanceWithDi(contextAccessor.HttpContext, Constants.OptionsAssemblyName);
            _nccSettings = options != null ? ((IOptions<NccSettings>) options).Value : new NccSettings();

            _protector = protector.CreateProtector(_nccSettings.DataProtectionKey);
            _razorViewEngine = razorViewEngine;
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ControlAction(IFormCollection formCollection, string[] context, Dictionary<string, string> parameters)
        {
            var controlCtxs = new List<object>();
            var viewResult = new List<string>();

            foreach (var ctx in context)
            {
                try
                {
                    if (context.Length == 0)
                        throw new Exception("An error as occured when processing the controls context. Please, verify that all target id's are correct.");

                    controlCtxs.Add(NccJson.GetObjectFromJson<object>(_protector.Unprotect(ctx)));
                }
                catch (Exception e)
                {
                    viewResult.Add(EventService.ProcessError(e));
                }
            }

            foreach (var controlCtx in controlCtxs)
            {
                try
                {
                    if (controlCtx == null)
                        throw new Exception("The control context is invalid! Please, refresh the page to get a new valid context.");
                    if (controlCtx.NccGetPropertyValue<string>("Error") != null)
                        continue;

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

                            var additionalData = controlCtx.NccGetPropertyValue<Dictionary<string, object>>("AdditionalData");
                            additionalData.Remove("EditRowNumber");
                            controlCtx.NccSetPropertyValue("AdditionalData", additionalData);

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
                            if (!(controlCtx is NccGridContext))
                                throw new Exception("A GridAction was specified but the context is not of type GridViewContext!");
                            if (service == null) service = ReflectionService.NccGetClassInstance(typeof(NccGridEvents).AssemblyQualifiedName, null);

                            switch (parametersList[$"{DefaultParameters.EventName.ToString().NccAddPrefix()}"].ToLower())
                            {
                                case "update":
                                    service.NccInvokeMethod(GridViewEvents.Update, new object[] { new NccEventArgs { Controller = this, NccControlContext = controlCtx, FormCollection = formCollection } });
                                    break;
                                case "updaterow":
                                    if (!parametersList.ContainsKey($"{GridViewParameters.RowNumber.ToString().NccAddPrefix()}"))
                                        throw new Exception("The row number wasn't received... Something wrong has happened...");

                                    var updateRowPos = Convert.ToInt32(parametersList[$"{GridViewParameters.RowNumber.ToString().NccAddPrefix()}"]);

                                    service.NccInvokeMethod(GridViewEvents.UpdateRow, new object[] { new NccEventArgs { Controller = this, NccControlContext = controlCtx, FormCollection = formCollection }, updateRowPos });
                                    break;
                                case "editrow":
                                    if (!parametersList.ContainsKey($"{GridViewParameters.RowNumber.ToString().NccAddPrefix()}"))
                                        throw new Exception("The row number wasn't received... Something wrong has happened...");

                                    var editRowPos = Convert.ToInt32(parametersList[$"{GridViewParameters.RowNumber.ToString().NccAddPrefix()}"]);

                                    var additionalData2 = controlCtx.NccGetPropertyValue<Dictionary<string, object>>("AdditionalData");
                                    additionalData2["EditRowNumber"] = editRowPos;
                                    controlCtx.NccSetPropertyValue("AdditionalData", additionalData2);

                                    break;
                                case "canceleditrow":
                                    if (!parametersList.ContainsKey($"{GridViewParameters.RowNumber.ToString().NccAddPrefix()}"))
                                        throw new Exception("The row number wasn't received... Something wrong has happened...");

                                    var additionalData3 = controlCtx.NccGetPropertyValue<Dictionary<string, object>>("AdditionalData");
                                    additionalData3.Remove("EditRowNumber");
                                    controlCtx.NccSetPropertyValue("AdditionalData", additionalData3);

                                    break;
                                case "deleterow":
                                    if (!parametersList.ContainsKey($"{GridViewParameters.RowNumber.ToString().NccAddPrefix()}"))
                                        throw new Exception("The row number wasn't received... Something wrong has happened...");

                                    var deleteRowPos = Convert.ToInt32(parametersList[$"{GridViewParameters.RowNumber.ToString().NccAddPrefix()}"]);

                                    service.NccInvokeMethod(GridViewEvents.DeleteRow, new object[] { new NccEventArgs { Controller = this, NccControlContext = controlCtx, FormCollection = formCollection }, deleteRowPos });

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

                    viewResult.Add(await this.NccRenderToStringAsync(_razorViewEngine, controlViewPath, null));
                }
                catch (Exception e)
                {
                    var div = new TagBuilder("div");
                    var encContext = new TagBuilder("input");
                    var error = EventService.ProcessError(e);
                    var id = controlCtx.NccGetPropertyValue<string>("Id");
                    if (!string.IsNullOrEmpty(id))
                    {
                        div.Attributes["id"] = id;
                        ViewData[id] = controlCtx;

                        encContext.Attributes.Add("name", "encContext");
                        encContext.Attributes.Add("id", $"{id}_context");
                        encContext.Attributes.Add("type", "hidden");
                        encContext.Attributes.Add("value", _protector.Protect(NccJson.SetObjectAsJson(controlCtx)));
                    }
                    div.InnerHtml.AppendHtml(error);
                    div.InnerHtml.AppendHtml(encContext);

                    var writer = new StringWriter();
                    div.WriteTo(writer, HtmlEncoder.Default);

                    viewResult.Add(writer.ToString());
                }
            }

            return Json(viewResult);

        }

        public FileResult GetNccJsFile()
        {
            var assembly = this.GetType().GetTypeInfo().Assembly;

            var stream = assembly.GetManifestResourceStream("NetCoreControls.Scripts.ncc.js");


            return File(stream, "application/javascript");
        }

        public FileResult GetNccCssFile()
        {
            var assembly = this.GetType().GetTypeInfo().Assembly;

            var stream = assembly.GetManifestResourceStream("NetCoreControls.Styles.ncc.css");


            return File(stream, "text/css");
        }
    }
}
