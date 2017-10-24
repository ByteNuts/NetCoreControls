using System;
using System.Collections.Generic;
using System.Text;

namespace ByteNuts.NetCoreControls.Models.Grid
{
    public class GridExportExcel
    {
        public string Title { get; set; }
        public string Subject { get; set; }
        public string Author { get; set; }
        public string Company { get; set; }
        public string Keywords { get; set; }
        public bool? AutoFitColumns { get; set; }
        public bool? ShowHeader { get; set; }
        public bool? ShowTotal { get; set; }
        public List<GridExportExcelColumn> Columns { get; set; }
    }
}
