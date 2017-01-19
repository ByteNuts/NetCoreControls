using ByteNuts.NetCoreControls.Models;
using ByteNuts.NetCoreControls.Services;

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
            eventArgs.NccControlContext.NccSetPropertyValue("Visible", true);
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
