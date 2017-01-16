using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ByteNuts.NetCoreControls.Extensions
{
    public static class StringExtensions
    {
        public static string NccAddPrefix(this string str)
        {
            return $"{Constants.AttributePrefix}-{str}";
        }
    }
}
