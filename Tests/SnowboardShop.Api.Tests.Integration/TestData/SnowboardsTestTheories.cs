using System.Collections;
using Newtonsoft.Json;
using SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataFakers;

namespace SnowboardShop.Api.Tests.Integration.TestData
{
    public class SnowboardsTestTheories : IEnumerable<object[]>
    {
        private readonly CreateSnowboardFaker _faker = new CreateSnowboardFaker();

        public IEnumerator<object[]> GetEnumerator()
        {
            for (int i = 0; i < 5; i++) // Generate 5 different instances
            {
                yield return new object[] { _faker.Generate() };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public static class TestObjectExtensions
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
        private const int NumberOfGuidsToGenerate = 3; 

        public InvalidSnowboardGuidTheoryData()
        {
            for (int i = 0; i < NumberOfGuidsToGenerate; i++)
            {
                Add(Guid.NewGuid().ToString());
            }
        }
    }
}