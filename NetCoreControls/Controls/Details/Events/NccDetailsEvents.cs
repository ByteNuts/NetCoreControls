using ByteNuts.NetCoreControls.Core;
using ByteNuts.NetCoreControls.Core.Controls;
using ByteNuts.NetCoreControls.Core.Extensions;
using ByteNuts.NetCoreControls.Core.Models;
using ByteNuts.NetCoreControls.Core.Services;
using ByteNuts.NetCoreControls.Models.Details;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace ByteNuts.NetCoreControls.Controls.Details.Events
{
    public class NccDetailsEvents : NccControlEvents
    {
        public virtual async void Update(NccEventArgs eventArgs)
        {
            var detailsContext = (NccDetailsContext)eventArgs.NccControlContext;
            if (detailsContext == null) return;

            var type = Type.GetType(detailsContext.DatabaseModelType);
            if (type == null)
                throw new Exception("The DatabaseModelType was incorrectly set. Please correct it on the Grid context.");

            dynamic model = Activator.CreateInstance(type);
            if (model == null)
                throw new Exception("The DatabaseModelType specified could not be instantiated. Is it public?");

            var ok = await eventArgs.Controller.TryUpdateModelAsync(model, detailsContext.Id);

            if (!ok) throw new Exception("Error binding model to object or list");

            ok = NccEventService.NccBindDataKeys(model, detailsContext.DataKeysValues, 0);

            if (!ok) throw new Exception("DataKeys list is bigger than submited list. No match possible!!");

            if (model == null)
                throw new Exception("It was not possible to bind the forms value to model.");


            var options = NccReflectionService.NccGetClassInstanceWithDi(eventArgs.Controller.HttpContext, NccConstants.OptionsAssemblyName);
            var nccSettings = options != null ? ((IOptions<NccSettings>)options).Value : new NccSettings();

            var dbService = nccSettings.UseDependencyInjection ? NccReflectionService.NccGetClassInstanceWithDi(eventArgs.Controller.HttpContext, detailsContext.DataAccessClass) : NccReflectionService.NccGetClassInstance(detailsContext.DataAccessClass, detailsContext.DataAccessParameters);

            if (dbService == null)
                throw new Exception("Could not get an instance of DataAccessClass. Wrong assembly full name?");

            var updateParameters = detailsContext.UpdateParameters?.ToDictionary(x => x.Key, x => x.Value).NccToExpando() ?? new ExpandoObject() as IDictionary<string, object>;

            updateParameters["model"] = model;

            try
            {
                var result = dbService.NccInvokeMethod(detailsContext.UpdateMethod, (ExpandoObject)updateParameters);

                eventArgs.Controller.ViewBag.Message = "Updated successfully!";
                eventArgs.Controller.ViewBag.MessageType = "success";
            }
            catch (Exception)
            {
                eventArgs.Controller.ViewBag.Message = "An error occured!";
                eventArgs.Controller.ViewBag.MessageType = "danger";
            }

        }
    }
}
