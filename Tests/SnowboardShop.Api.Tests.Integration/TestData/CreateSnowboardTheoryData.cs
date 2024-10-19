using System.Text.Json;
using Bogus;
using SnowboardShop.Api.Tests.Integration.TestUtilities;
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
            string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData",
                "missing_snowboard_properties.json");


            var jsonData = File.ReadAllText(jsonFilePath);
            var testCases = JsonSerializer.Deserialize<object[]>(jsonData);

            foreach (var testCase in testCases!)
            {
                string jsonString = JsonSerializer.Serialize(testCase, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                Add(jsonString);
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


    public class ExtraUnmappedFieldsTestData : TheoryData<object>
    {
        public ExtraUnmappedFieldsTestData()
        {
            var faker = new CreateSnowboardFaker();
            var baseRequest = faker.Generate(); 

            
            var requestWithOneExtraField = baseRequest.DeepClone();
            var requestWithSingleUnmappedField = new
            {
                ExtraField1 = "UnexpectedValue1" 
            };
            Add(requestWithSingleUnmappedField);

            
            var requestWithTwoExtraFields = baseRequest.DeepClone();
            var requestWithDoubleUnmappedFields = new
            {
                ExtraField1 = "UnexpectedValue1", 
                ExtraField2 = "UnexpectedValue2" 
            };
            Add(requestWithDoubleUnmappedFields);
            
            
            var requestWithThreeExtraFields = baseRequest.DeepClone();
            var requestWithTripleUnmappedFields = new
            {
                ExtraField1 = "UnexpectedValue1", 
                ExtraField2 = "UnexpectedValue2", 
                ExtraField3 = "UnexpectedValue3" 
            };
            Add(requestWithTripleUnmappedFields);
        }
    }
    
    public class NullPropertiesTestData : TheoryData<CreateSnowboardRequest>
    {
        public NullPropertiesTestData()
        {
            var faker = new Faker();
            var currentYear = DateTime.UtcNow.Year;
            var validSnowboardBrand = faker.PickRandom(SnowboardGenerationConstants.ValidSnowboardBrands);
            var validSnowboardLineup = faker.Random.ArrayElements(SnowboardGenerationConstants.SnowboardLineupList, faker.Random.Int(1, 5)).ToList();

            // Factory method to create request
            CreateSnowboardRequest CreateRequest(string? snowboardBrand, int? yearOfRelease, List<string>? snowboardLineup) => new()
            {
                SnowboardBrand = snowboardBrand!,
                YearOfRelease = yearOfRelease ?? default,  // If null, default to 0 or a valid year
                SnowboardLineup = snowboardLineup!
            };

            Add(CreateRequest(null, currentYear, validSnowboardLineup));

            Add(CreateRequest(validSnowboardBrand, currentYear, null));

            Add(CreateRequest(null, currentYear, null));

            Add(CreateRequest(validSnowboardBrand, null, validSnowboardLineup));

            Add(CreateRequest(null, null, null));
        }
    }

}