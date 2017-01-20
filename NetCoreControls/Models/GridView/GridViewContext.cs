using System.Collections.Generic;

namespace ByteNuts.NetCoreControls.Models.GridView
{
    public class GridViewContext : NccContext
    {
        public string DataKeys { get; set; }
        public List<Dictionary<string, object>> DataKeysValues { get; internal set; } = new List<Dictionary<string, object>>();
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 0;
        public int TotalItems { get; set; }
        public bool AllowPaging { get; set; } = false;
        public int PagerNavigationSize { get; set; } = 10;
    }
}
