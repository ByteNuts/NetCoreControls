namespace ByteNuts.NetCoreControls.Core.Extensions
{
    public static class StringExtensions
    {
        public static string NccAddPrefix(this string str)
        {
            return $"{NccConstants.AttributePrefix}-{str}";
        }
    }
}
