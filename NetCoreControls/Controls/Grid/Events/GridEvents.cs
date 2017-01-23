using ByteNuts.NetCoreControls.Models;
using ByteNuts.NetCoreControls.Models.Grid;

namespace ByteNuts.NetCoreControls.Controls.Grid.Events
{
    public class GridEvents : NccEvents
    {
        public virtual void RowDataBound(NccEventArgs eventArgs, object rowData)
        {
        }

        public virtual void RowCreated(NccEventArgs eventArgs, GridRow row)
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
