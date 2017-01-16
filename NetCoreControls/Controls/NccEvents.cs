using ByteNuts.NetCoreControls.Controllers;
using System.Collections;
using ByteNuts.NetCoreControls.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ByteNuts.NetCoreControls.Controls
{
    public class NccEvents
    {
        /// <summary>
        /// Properties available:
        /// - 
        /// - ViewContext
        /// </summary>
        /// <param name="eventArgs"></param>
        public virtual void Load(NccEventArgs eventArgs)
        {
        }
        public virtual void DataBound(NccEventArgs eventArgs)
        {
        }

        public virtual void PreRender(NccEventArgs eventArgs)
        {
        }

        public virtual void PostBack(NccEventArgs eventArgs)
        {

        }
    }
}
