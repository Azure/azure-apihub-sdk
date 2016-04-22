
namespace Microsoft.Azure.ApiHub.Sdk.Extensions
{
    internal static class ArrayExtensions
    {
        public static T[] Coalesce<T>(this T[] array)
        {
            return array ?? new T[0];
        }
    }
}
