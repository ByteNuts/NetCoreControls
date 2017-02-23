using ByteNuts.NetCoreControls.Core.Models;
using ByteNuts.NetCoreControls.Core.Services;

namespace ByteNuts.NetCoreControls.Core.Controls
{
    public class NccControlEvents
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
