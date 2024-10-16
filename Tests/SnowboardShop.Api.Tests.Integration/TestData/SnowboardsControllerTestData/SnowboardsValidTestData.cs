using SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataFakers;
using System.Collections;

namespace SnowboardShop.Api.Tests.Integration.TestData.SnowboardsControllerTestData
{
    public class SnowboardsValidTestData : IEnumerable<object[]>
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
}
