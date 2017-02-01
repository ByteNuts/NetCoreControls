using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ByteNuts.NetCoreControls.Controllers;
using ByteNuts.NetCoreControls.Services;

namespace ByteNuts.NetCoreControls.Extensions
{
    public static class ControllerExtensions
    {
        //public static async Task<bool> NccBindModel<T>(this NetCoreControlsController controller, T model, List<Dictionary<string, object>> dataKeys, string prefix = "") where T : class
        //{
        //    var ok = await controller.TryUpdateModelAsync(model, prefix);

        //    if (!ok) return false;

        //    var list = model as IList;
        //    if (list?.Count != dataKeys.Count) return false;

        //    for (var i = 0; i < list.Count; i++)
        //    {
        //        MapDataKeysToRow(list[i], dataKeys[i]);
        //    }

        //    return true;
        //}


        //private static void MapDataKeysToRow<T>(T row, Dictionary<string, object> dataKeysRow)
        //{
        //    foreach (var dataKey in dataKeysRow)
        //    {
        //        var dkType = row.NccGetPropertyValue<object>(dataKey.Key).GetType();
        //        row.NccSetPropertyValue(dataKey.Key, Convert.ChangeType(dataKey.Value, dkType));
        //    }
        //}
    }
}
