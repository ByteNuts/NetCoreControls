using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ByteNuts.NetCoreControls.Models.Select
{
    public class NccSelectContext : NccContext
    {
        public string TextValue { get; set; }
        public string DataValue { get; set; }
        public string SelectedValue { get; set; }
        public string FirstItem { get; set; }
    }
}
