using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ByteNuts.NetCoreControls.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ByteNuts.NetCoreControls.Models
{
    public class NccEventArgs
    {
        public NetCoreControlsController Controller { get; set; }

        public ViewContext ViewContext { get; set; }

        public IFormCollection FormCollection { get; set; }

        public object NccControlContext { get; set; }

        public object NccTagContext { get; set; }

        public object DataObjects { get; set; }
    }
}
