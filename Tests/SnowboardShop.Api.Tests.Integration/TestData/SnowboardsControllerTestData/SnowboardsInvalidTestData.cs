using Newtonsoft.Json;

namespace SnowboardShop.Api.Tests.Integration.TestData.SnowboardsControllerTestData;

public static class Ext
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

public class InvalidSnowboardGuidTheoryData : TheoryData<string>
{
    public InvalidSnowboardGuidTheoryData()
    {
        Add(Guid.NewGuid().ToString());
        Add("ACDA6A05-D8BA-4003-B0A5-D8BE9D101765");
        Add("4F176590-798A-4B45-A42B-7DDDF16BDC29");
    }
}



