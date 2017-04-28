using System.Collections.Generic;
using ByteNuts.NetCoreControls.Core.Models;
using ByteNuts.NetCoreControls.Models.Enums;

namespace ByteNuts.NetCoreControls.Models.Grid
{
    public class NccGridContext : NccContext
    {
        public string DataKeys { get; set; }
        public List<Dictionary<string, object>> DataKeysValues { get; internal set; } = new List<Dictionary<string, object>>();
        public int PageNumber { get; set; } = 1;
        public int TotalItems { get; internal set; }
        public bool AllowPaging { get; set; }
        public int PageSize { get; set; } = 0;
        public bool AutoGenerateEditButton { get; set; }

        //Pager options
        public int PagerNavSize { get; set; } = 10;
        public bool ShowRecordsCount { get; set; } = true;
        public NccGridPagerPositionEnum GridPagerPosition { get; set; } = NccGridPagerPositionEnum.Right;
    }
}
