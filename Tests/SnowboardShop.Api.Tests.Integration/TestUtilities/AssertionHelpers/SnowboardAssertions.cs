using FluentAssertions;
using SnowboardShop.Contracts.Requests;
using SnowboardShop.Contracts.Responses;

namespace SnowboardShop.Api.Tests.Integration.TestUtilities.AssertionHelpers;

public static class SnowboardAssertions
{
    public static void AssertCreateSnowboardResponseBody(SnowboardResponse? responseData, CreateSnowboardRequest expectedRequest)
    {
        responseData.Should().NotBeNull();
        responseData!.Id.Should().NotBe(Guid.Empty); 
        responseData.SnowboardBrand.Should().Be(expectedRequest.SnowboardBrand);
        responseData.Slug.Should().NotBeNullOrEmpty();
        responseData.YearOfRelease.Should().Be(expectedRequest.YearOfRelease);
        responseData.SnowboardLineup.Should().NotBeNull().And.BeEquivalentTo(expectedRequest.SnowboardLineup);
    }
}
