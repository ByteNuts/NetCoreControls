using System;
using System.Collections.Generic;
using System.Dynamic;
using ByteNuts.NetCoreControls.Models;
using ByteNuts.NetCoreControls.Models.Grid;
using ByteNuts.NetCoreControls.Extensions;
using ByteNuts.NetCoreControls.Services;
using Microsoft.Extensions.Options;
using System.Linq;

namespace ByteNuts.NetCoreControls.Controls.Grid.Events
{
    public class NccGridEvents : NccEvents
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

            await EventService.NccBindModel(eventArgs.Controller, model, gridContext.DataKeysValues, gridContext.Id);

            if (model == null)
                throw new Exception("It was not possible to bind the forms value to model.");


            var options = ReflectionService.NccGetClassInstanceWithDi(eventArgs.Controller.HttpContext, Constants.OptionsAssemblyName);
            var nccSettings = options != null ? ((IOptions<NccSettings>)options).Value : new NccSettings();

            var dbService = nccSettings.UseDependencyInjection ? ReflectionService.NccGetClassInstanceWithDi(eventArgs.Controller.HttpContext, gridContext.DataAccessClass) : ReflectionService.NccGetClassInstance(gridContext.DataAccessClass, gridContext.DataAccessParameters);

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
            catch (Exception e)
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
