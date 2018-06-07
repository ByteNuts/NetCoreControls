using System;
using System.Collections.Generic;
using System.Text;

namespace ByteNuts.NetCoreControls.Models.Grid
{
    public class GridExportExcelColumn
    {
        public string ColumnTitle { get; set; }
        public string ColumnPropName { get; set; }
        public double? Width { get; set; }
        public string HeaderHorizontalAlignment { get; set; }
        public string HeaderVerticalAlignment { get; set; }
        public string RowHorizontalAlignment { get; set; }
        public string RowVerticalAlignment { get; set; }
    }
}
