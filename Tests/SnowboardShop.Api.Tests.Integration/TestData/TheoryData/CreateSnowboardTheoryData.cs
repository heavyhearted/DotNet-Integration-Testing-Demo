using Bogus;
using SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataFakers;
using SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataHelpers;
using SnowboardShop.Contracts.Requests;

namespace SnowboardShop.Api.Tests.Integration.TestData.TheoryData
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
            Add(CreateRequest(-1998)); // Invalid: negative year
        }
    }

    public class EmptyStringPropertiesTestData : TheoryData<CreateSnowboardRequest>
    {
        public EmptyStringPropertiesTestData()
        {
            var faker = new Faker();
            var currentYear = DateTime.UtcNow.Year;
            var validSnowboardBrand = faker.PickRandom(SnowboardGenerationConstants.ValidSnowboardBrands);
            var validSnowboardLineup = faker.Random.ArrayElements(SnowboardGenerationConstants.SnowboardLineupList, faker.Random.Int(1, 5)).ToList();
            var lineupWithEmptyString = new List<string> { "" };

            // Factory method to create request
            CreateSnowboardRequest CreateRequest(string snowboardBrand, int yearOfRelease, List<string> snowboardLineup) => new()
            {
                SnowboardBrand = snowboardBrand,
                YearOfRelease = yearOfRelease,
                SnowboardLineup = snowboardLineup
            };


            Add(CreateRequest("", currentYear, validSnowboardLineup));
            
            Add(CreateRequest(validSnowboardBrand, currentYear, lineupWithEmptyString));
            
            Add(CreateRequest("", currentYear, lineupWithEmptyString));
        }
    }
    

    public class MissingSnowboardProperties : TheoryData<string>
    {
        public MissingSnowboardProperties()
        {
            string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData",
                "JsonData", "missing_snowboard_properties.json");
            
            foreach (var jsonTestCase in JsonDataHelper.LoadJsonTestCases(jsonFilePath))
            {
                Add(jsonTestCase); 
            }
        }
    }
    
    public class NullSnowboardProperties : TheoryData<string>
    {
        public NullSnowboardProperties()
        {
            string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", 
                "JsonData", "null_snowboard_properties.json");
            
            foreach (var jsonTestCase in JsonDataHelper.LoadJsonTestCases(jsonFilePath))
            {
                Add(jsonTestCase); 
            }
        }
    }
    
    public class InvalidSnowboardProperties : TheoryData<string>
    {
        public InvalidSnowboardProperties()
        {
            string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", 
                "JsonData", "invalid_snowboard_properties.json");
            
            foreach (var jsonTestCase in JsonDataHelper.LoadJsonTestCases(jsonFilePath))
            {
                Add(jsonTestCase); 
            }
        }
    }
}