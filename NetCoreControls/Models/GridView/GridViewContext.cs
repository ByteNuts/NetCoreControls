namespace ByteNuts.NetCoreControls.Models.GridView
{
    public class GridViewContext : NccContext
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 0;
        public int TotalItems { get; set; }
        public bool AllowPaging { get; set; } = false;
        public int PagerNavigationSize { get; set; } = 10;
    }
}
