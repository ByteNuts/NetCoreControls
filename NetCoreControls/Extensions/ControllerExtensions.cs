using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ByteNuts.NetCoreControls.Controllers;
using ByteNuts.NetCoreControls.Services;

namespace ByteNuts.NetCoreControls.Extensions
{
    public static class ControllerExtensions
    {
        public static async Task NccBindModel<T>(this NetCoreControlsController controller, T model, List<Dictionary<string, object>> dataKeys, string prefix = "") where T : class 
        {
            var ok = await controller.TryUpdateModelAsync(model, prefix);

            if (!ok) return;

            var list = model as IList;
            if (list?.Count != dataKeys.Count) return;

            for (var i = 0; i < list.Count; i++)
            {
                MapDataKeysToRow(list[i], dataKeys[i]);
            }
        }


        private static void MapDataKeysToRow<T>(T row, Dictionary<string, object> dataKeysRow)
        {
            foreach (var dataKey in dataKeysRow)
            {
                row.NccSetPropertyValue(dataKey.Key, dataKey.Value);
            }
        }
    }
}
