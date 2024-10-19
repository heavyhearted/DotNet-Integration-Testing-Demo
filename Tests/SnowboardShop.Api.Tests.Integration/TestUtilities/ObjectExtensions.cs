using Newtonsoft.Json;

namespace SnowboardShop.Api.Tests.Integration.TestUtilities;

public static class ObjectExtensions
{
    public static T DeepClone<T>(this T obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj), "Object cannot be null");
        }

        var jsonString = JsonConvert.SerializeObject(obj);
        return JsonConvert.DeserializeObject<T>(jsonString)!;
    }
}