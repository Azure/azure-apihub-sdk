
namespace Microsoft.Azure.ApiHub.Sdk.Extensions
{
    internal static class ObjectExtensions
    {
        public static T Coalesce<T>(this T obj)
            where T: class, new()
        {
            return obj ?? new T();
        }
    }
}
