using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ByteNuts.NetCoreControls.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ByteNuts.NetCoreControls.Controls.Select.Events
{
    public class NccSelectEvents : NccEvents
    {
        public virtual void OptionBound(NccEventArgs eventArgs, object elemData, TagBuilder tag)
        {
        }
    }
}
