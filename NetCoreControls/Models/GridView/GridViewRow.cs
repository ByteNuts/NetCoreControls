using System.Collections.Generic;

namespace ByteNuts.NetCoreControls.Models.GridView
{
    public class GridViewRow
    {
        public List<GridViewCell> Cells { get; set; }
        public int RowNumber { get; set; }
        public string CssClass { get; set; }
    }
}
