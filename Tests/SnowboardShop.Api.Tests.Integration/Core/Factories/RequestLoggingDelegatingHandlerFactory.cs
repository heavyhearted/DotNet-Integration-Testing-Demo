using Xunit.Abstractions;

namespace SnowboardShop.Api.Tests.Integration.Core.Factories;

public class RequestResponseLoggingDelegatingHandler : DelegatingHandler
{
    private readonly ITestOutputHelper _testOutputHelper;

    public RequestResponseLoggingDelegatingHandler(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        _testOutputHelper.WriteLine($"Request: {request}");
        var response = await base.SendAsync(request, cancellationToken);
        _testOutputHelper.WriteLine($"Response: {response}");
        return response;
    }
}

public static class RequestLoggingDelegatingHandlerFactory
{
    public static RequestResponseLoggingDelegatingHandler Create(ITestOutputHelper testOutputHelper)
    {
        return new RequestResponseLoggingDelegatingHandler(testOutputHelper);
    }
}