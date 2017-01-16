using System;
using System.Collections.Generic;

namespace ByteNuts.NetCoreControls.Models
{
    public class NccActionModel
    {
        public string[] TargetIds { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}
