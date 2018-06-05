using System.Collections.Generic;
using ByteNuts.NetCoreControls.Core.Models;

namespace ByteNuts.NetCoreControls.Models.Details
{
    public class NccDetailsContext : NccContext
    {
        public string DataKeys { get; set; }
        public List<Dictionary<string, object>> DataKeysValues { get; internal set; } = new List<Dictionary<string, object>>();
    }
}
