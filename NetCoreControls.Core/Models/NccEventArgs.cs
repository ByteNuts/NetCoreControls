using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ByteNuts.NetCoreControls.Core.Models
{
    public class NccEventArgs
    {
        public Controller Controller { get; set; }

        public ViewContext ViewContext { get; set; }

        public IFormCollection FormCollection { get; set; }

        public Dictionary<string, string> Parameters { get; set; }

        public object NccControlContext { get; set; }

        public object NccTagContext { get; set; }

        public object DataObjects { get; set; }
    }
}
