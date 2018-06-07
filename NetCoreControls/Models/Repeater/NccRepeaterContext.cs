using System.Collections.Generic;
using ByteNuts.NetCoreControls.Core.Models;

namespace ByteNuts.NetCoreControls.Models.Repeater
{
    public class NccRepeaterContext : NccContext
    {
        public string DataKeys { get; set; }
        public List<Dictionary<string, object>> DataKeysValues { get; internal set; } = new List<Dictionary<string, object>>();
        public int PageNumber { get; set; } = 1;
        public int TotalItems { get; internal set; }
        public bool AutoGenerateEditButton { get; set; }
    }
}
