using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json;

namespace ByteNuts.NetCoreControls.Models
{
    public abstract class NccContext
    {
        public string Id { get; set; }
        public bool UseDependencyInjection { get; set; }
        public bool RenderForm { get; set; } = true;
        public bool AutoBind { get; set; } = true;

        public object DataSource { get; set; }

        public bool Visible { get; set; } = true;

        [JsonIgnore]
        public object DataObjects { get; internal set; }
        /// <summary>
        /// Assembly Qualified Name
        /// Usage: 'InstanceOfClass.GetType().AssemblyQualifiedName'
        /// </summary>
        public string DataAccessClass { get; set; }

        /// <summary>
        /// Assembly Qualified Name
        /// Usage: 'InstanceOfClass.GetType().AssemblyQualifiedName'
        /// </summary>
        public string EventHandlerClass { get; set; }

        public ExpandoObject SelectParameters { get; set; } = new ExpandoObject();


        public string SelectMethod { get; set; }

        /// <summary>
        /// USED ONLY WHEN UseDependencyInjection is set to FALSE.
        /// Parameters required to instantiate the class which contains the method.
        /// </summary>
        public object[] DataAccessParameters { get; set; }

        /// <summary>
        /// Overrides select parameters used on Control Context
        /// </summary>
        public Dictionary<string, string> Filters { get; set; } = new Dictionary<string, string>();

        public ViewsPathsModel ViewPaths { get; set; }

        /// <summary>
        /// Additional data to store in control context
        /// </summary>
        public Dictionary<string, object> AdditionalData { get; set; } = new Dictionary<string, object>();
    }
}
