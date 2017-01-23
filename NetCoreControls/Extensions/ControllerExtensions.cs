using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ByteNuts.NetCoreControls.Controllers;
using ByteNuts.NetCoreControls.Services;

namespace ByteNuts.NetCoreControls.Extensions
{
    public static class ControllerExtensions
    {
        public static async Task<T> NccBindModel<T>(this NetCoreControlsController controller, T model, List<Dictionary<string, object>> dataKeys) where T : class 
        {
            var ok = await controller.TryUpdateModelAsync(model);

            if (!ok) return model;

            var list = model as IList;
            if (list?.Count == dataKeys.Count)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    MapDataKeysToRow(list[i], dataKeys[i]);
                }
            }
            return model;
        }


        private static T MapDataKeysToRow<T>(T row, Dictionary<string, object> dataKeysRow)
        {
            foreach (var dataKey in dataKeysRow)
            {
                row.NccSetPropertyValue(dataKey.Key, dataKey.Value);
            }

            return row;
        }
    }
}
