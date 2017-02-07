using System.Collections.Generic;

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
        public int PagerNavSize { get; set; } = 10;
        public bool AutoGenerateEditButton { get; set; }
    }
}
