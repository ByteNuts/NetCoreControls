using System.Collections.Generic;

namespace ByteNuts.NetCoreControls.Models.Grid
{
    public class GridRow
    {
        public List<GridCell> Cells { get; set; }
        public int RowNumber { get; set; }
        public string CssClass { get; set; }
    }
}
