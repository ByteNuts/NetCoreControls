using System.Collections.Generic;
using ByteNuts.NetCoreControls.Core.Models;

namespace ByteNuts.NetCoreControls.Models.Repeater
{
    public class NccRepeaterTagContext : NccTagContext
    {
        public string RepeaterHeader { get; set; }
        public string RepeaterFooter { get; set; }
        public List<string> RepeaterItems { get; set; } = new List<string>();
        public string NoData { get; set; }

        public string PreContent { get; set; }
        public string PostContent { get; set; }

    }
}
