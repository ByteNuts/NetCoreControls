using System;
using System.Collections.Generic;
using System.Dynamic;
using ByteNuts.NetCoreControls.Models.Grid;
using ByteNuts.NetCoreControls.Services;
using Microsoft.Extensions.Options;
using System.Linq;
using ByteNuts.NetCoreControls.Core;
using ByteNuts.NetCoreControls.Core.Controls;
using ByteNuts.NetCoreControls.Core.Extensions;
using ByteNuts.NetCoreControls.Core.Models;
using ByteNuts.NetCoreControls.Core.Services;

namespace ByteNuts.NetCoreControls.Controls.Grid.Events
{
    public class NccGridEvents : NccControlEvents
    {
        public virtual void RowDataBound(NccEventArgs eventArgs, object rowData)
        {
        }

        public virtual void RowCreated(NccEventArgs eventArgs, GridRow row)
        {
        }

        public virtual async void UpdateRow(NccEventArgs eventArgs, int rowPos)
        {
            var gridContext = (NccGridContext)eventArgs.NccControlContext;
            if (gridContext == null) return;

            var type = Type.GetType(gridContext.DatabaseModelType);
            if (type == null)
                throw new Exception("The DatabaseModelType was incorrectly set. Please correct it on the Grid context.");

            dynamic model = Activator.CreateInstance(type);
            if (model == null)
                throw new Exception("The DatabaseModelType specified could not be instantiated. Is it public?");

            var ok = await eventArgs.Controller.TryUpdateModelAsync(model, gridContext.Id); //$"{gridContext.Id}[{rowPos}]"

            if (!ok) throw new Exception("Error binding model to object or list");

            ok = NccEventService.NccBindDataKeys(model, gridContext.DataKeysValues, rowPos);

            if (!ok) throw new Exception("DataKeys list is bigger than submited list. No match possible!!");

            if (model == null)
                throw new Exception("It was not possible to bind the forms value to model.");


            var options = NccReflectionService.NccGetClassInstanceWithDi(eventArgs.Controller.HttpContext, NccConstants.OptionsAssemblyName);
            var nccSettings = options != null ? ((IOptions<NccSettings>)options).Value : new NccSettings();

            var dbService = nccSettings.UseDependencyInjection ? NccReflectionService.NccGetClassInstanceWithDi(eventArgs.Controller.HttpContext, gridContext.DataAccessClass) : NccReflectionService.NccGetClassInstance(gridContext.DataAccessClass, gridContext.DataAccessParameters);

            if (dbService == null)
                throw new Exception("Could not get an instance of DataAccessClass. Wrong assembly full name?");

            var updateParameters = gridContext.UpdateParameters?.ToDictionary(x => x.Key, x => x.Value).NccToExpando() ?? new ExpandoObject() as IDictionary<string, object>;

            updateParameters["model"] = model;

            try
            {
                var result = dbService.NccInvokeMethod(gridContext.UpdateMethod, (ExpandoObject)updateParameters);

                eventArgs.Controller.ViewBag.Message = "Updated successfully!";
                eventArgs.Controller.ViewBag.MessageType = "success";

                gridContext.AdditionalData.Remove("EditRowNumber");
            }
            catch (Exception)
            {
                eventArgs.Controller.ViewBag.Message = "An error occured!";
                eventArgs.Controller.ViewBag.MessageType = "danger";
            }
        }

        public virtual void DeleteRow(NccEventArgs eventArgs, int rowPos)
        {
        }

        public virtual void Update(NccEventArgs eventArgs)
        {
        }
    }
}
