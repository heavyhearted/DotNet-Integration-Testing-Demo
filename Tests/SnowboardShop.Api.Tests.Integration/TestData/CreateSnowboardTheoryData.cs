using System.Collections;
using Bogus;
using Newtonsoft.Json;
using SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataFakers;
using SnowboardShop.Contracts.Requests;

namespace SnowboardShop.Api.Tests.Integration.TestData
{
    public class CreateSnowboardTheoryData : TheoryData<CreateSnowboardRequest>
    {
        private readonly CreateSnowboardFaker _faker = new();

        public CreateSnowboardTheoryData()
        {
            for (int i = 0; i < 5; i++) // Generate 5 unique snowboard requests
            {
                Add(_faker.Generate());
            }
        }
    }

    public class NonExistingSnowboardGuidTheoryData : TheoryData<string>
    {
        private const int NumberOfGuidsToGenerate = 3;

        public NonExistingSnowboardGuidTheoryData()
        {
            for (int i = 0; i < NumberOfGuidsToGenerate; i++)
            {
                Add(Guid.NewGuid().ToString());
            }
        }
    }

    public class MissingSnowboardProperties : TheoryData<string>
    {
        public MissingSnowboardProperties()
        {
            Add(@"{ ""YearOfRelease"": 2025, ""SnowboardLineup"": [""Maverick LTD"", ""Viper X""] }"); // Missing SnowboardBrand
            Add(@"{ ""SnowboardBrand"": ""Flow"", ""SnowboardLineup"": [""Maverick LTD"", ""Viper X""] }"); // Missing YearOfRelease
            Add(@"{ ""SnowboardBrand"": ""Flow"", ""YearOfRelease"": 2025 }"); // Missing SnowboardLineup
        }
    }

    public class InvalidYearOfReleaseTestData : TheoryData<CreateSnowboardRequest>
    {
        public InvalidYearOfReleaseTestData()
        {
            var faker = new Faker();
            var currentYear = DateTime.UtcNow.Year;

            var snowboardBrand = faker.PickRandom(SnowboardGenerationConstants.ValidSnowboardBrands);
            var snowboardLineup = faker.Random.ArrayElements(SnowboardGenerationConstants.SnowboardLineupList,
                faker.Random.Int(1, 5));

            CreateSnowboardRequest CreateRequest(int yearOfRelease) => new()
            {
                SnowboardBrand = snowboardBrand,
                YearOfRelease = yearOfRelease,
                SnowboardLineup = snowboardLineup
            };

            Add(CreateRequest(1964)); // Invalid: before 1965
            Add(CreateRequest(currentYear + 2)); // Invalid: beyond current year + 1
            Add(CreateRequest(1950)); // Invalid: far past year
            Add(CreateRequest(2050)); // Invalid: far future year
        }
    }
    
    public class EmptyStringPropertiesTestData : TheoryData<CreateSnowboardRequest>
    {
        public EmptyStringPropertiesTestData()
        {
            var currentYear = DateTime.UtcNow.Year;
            var validSnowboardLineup = new List<string> { "Freestyle", "All-Mountain" };
            var lineupWithEmptyString = new List<string> { "" }; 
            var lineupEmptyArray = new List<string>(); 

            // Case 1: All properties empty or null (YearOfRelease is set to null)
            Add(CreateRequest("", null, lineupEmptyArray)); 

            // Case 2: Empty SnowboardBrand
            Add(CreateRequest("", currentYear, validSnowboardLineup)); 

            // Case 3: Empty SnowboardLineup (empty string in the lineup)
            Add(CreateRequest("Burton", currentYear, lineupWithEmptyString)); 

            // Case 4: Empty SnowboardLineup (empty array)
            Add(CreateRequest("Burton", currentYear, lineupEmptyArray));
        }
        
        private CreateSnowboardRequest CreateRequest(string snowboardBrand, int? yearOfRelease, IEnumerable<string> snowboardLineup)
        {
            return new CreateSnowboardRequest
            {
                SnowboardBrand = snowboardBrand,
                YearOfRelease = yearOfRelease ?? default, 
                SnowboardLineup = snowboardLineup
            };
        }
    }

}