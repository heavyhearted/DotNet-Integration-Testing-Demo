using FluentAssertions;
using SnowboardShop.Contracts.Responses;

namespace SnowboardShop.Api.Tests.Integration.TestUtilities;

public static class SnowboardAssertions
{
    public static void AssertSnowboardResponseBody(SnowboardResponse? responseData,
        dynamic expectedRequest) // dynamic is used to accept both CreateSnowboardRequest and UpdateSnowboardRequest since they have the same properties.
    {
        responseData.Should().NotBeNull();
        responseData!.Id.Should().NotBe(Guid.Empty);
        responseData.SnowboardBrand.Should().Be(expectedRequest.SnowboardBrand);
        responseData.Slug.Should().NotBeNullOrEmpty();
        responseData.YearOfRelease.Should().Be(expectedRequest.YearOfRelease);
        responseData.YearOfRelease.Should().BeInRange(1965, DateTime.Now.Year + 1);
        responseData.SnowboardLineup.Should().NotBeNull().And.BeEquivalentTo(expectedRequest.SnowboardLineup);
    }

    public static void AssertMultipleSnowboardResponses(IEnumerable<SnowboardResponse> returnedSnowboards,
        IEnumerable<SnowboardResponse> expectedSnowboards)
    {
        returnedSnowboards.Should().NotBeNullOrEmpty();

        foreach (var expectedSnowboard in expectedSnowboards)
        {
            var returnedSnowboard = returnedSnowboards.FirstOrDefault(sb => sb.Id == expectedSnowboard.Id);
            returnedSnowboard.Should().NotBeNull();

            returnedSnowboard!.Id.Should().Be(expectedSnowboard.Id);
            returnedSnowboard.SnowboardBrand.Should().Be(expectedSnowboard.SnowboardBrand);
            returnedSnowboard.Slug.Should().NotBeNullOrEmpty();
            returnedSnowboard.YearOfRelease.Should().Be(expectedSnowboard.YearOfRelease);
            returnedSnowboard.YearOfRelease.Should().BeInRange(1965, DateTime.Now.Year + 1);
            returnedSnowboard.SnowboardLineup.Should().BeEquivalentTo(expectedSnowboard.SnowboardLineup);
            returnedSnowboard.Rating.Should().BeNull();
            returnedSnowboard.UserRating.Should().BeNull();
        }
    }
}