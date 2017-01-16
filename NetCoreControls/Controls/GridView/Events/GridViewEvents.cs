using ByteNuts.NetCoreControls.Controllers;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using ByteNuts.NetCoreControls.Models;
using ByteNuts.NetCoreControls.Models.GridView;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ByteNuts.NetCoreControls.Controls.GridView.Events
{
    public class GridViewEvents : NccEvents
    {
        public virtual void RowDataBound(NccEventArgs eventArgs, object rowData)
        {
        }

        public virtual void RowCreated(NccEventArgs eventArgs, GridViewRow row)
        {
        }

        /// <summary>
        /// Properties available:
        /// - Controller;
        /// - NccControlContext;
        /// - FormCollection
        /// </summary>
        /// <param name="eventArgs"></param>
        public virtual void Update(NccEventArgs eventArgs)
        {
        }

        public virtual void DeleteRow(NccEventArgs eventArgs, int rowPos)
        {
        }
    }
}
