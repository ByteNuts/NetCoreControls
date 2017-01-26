using ByteNuts.NetCoreControls.Models;
using ByteNuts.NetCoreControls.Models.Grid;

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

        public virtual void UpdateRow(NccEventArgs eventArgs, int rowPos)
        {
        }

        public virtual void DeleteRow(NccEventArgs eventArgs, int rowPos)
        {
        }

        public virtual void Update(NccEventArgs eventArgs)
        {
        }
    }
}
