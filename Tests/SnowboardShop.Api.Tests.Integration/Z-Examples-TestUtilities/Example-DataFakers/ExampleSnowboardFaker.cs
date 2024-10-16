using Bogus;

namespace SnowboardShop.Api.Tests.Integration.TestUtilities.DataFakers;

public class ExampleSnowboardFaker : Faker<ExampleSnowboardDTO_DeleteMe>
{
    public ExampleSnowboardFaker()
    {
        RuleFor(x => x.Name, f => f.Commerce.ProductName());
        RuleFor(x => x.Brand, f => f.Company.CompanyName());
        RuleFor(x => x.Season, f => f.Commerce.ProductMaterial());
    }
}