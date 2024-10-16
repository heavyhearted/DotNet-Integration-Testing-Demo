using Xunit.Abstractions;

namespace SnowboardShop.Api.Tests.Integration.Core.Factories;

public static class RequestLoggingDelegatingHandlerFactory
{
    public static RequestResponseLoggingDelegatingHandler Create(ITestOutputHelper testOutputHelper)
    {
        return new RequestResponseLoggingDelegatingHandler(testOutputHelper);
    }
}

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
        // Log request
        _testOutputHelper.WriteLine($"Request: {request.Method} {request.RequestUri}");
        _testOutputHelper.WriteLine($"Headers: {request.Headers}");

        if (request.Content is not null)
        {
            var requestBody = await request.Content.ReadAsStringAsync(cancellationToken);
            _testOutputHelper.WriteLine($"Request Body: {PrettyPrintJson(requestBody)}");
        }
        
        // Log response
        var response = await base.SendAsync(request, cancellationToken);
        _testOutputHelper.WriteLine($"Response: StatusCode: {(int)response.StatusCode} {response.ReasonPhrase}");
        _testOutputHelper.WriteLine($"Headers: {response.Headers}");

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (response.Content is not null)
        {
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            _testOutputHelper.WriteLine($"Response Body: {PrettyPrintJson(responseBody)}");
        }
        
        return response;
    }

    private string PrettyPrintJson(string json)
    {
        try
        {
            var parsedJson = Newtonsoft.Json.Linq.JToken.Parse(json);
            return parsedJson.ToString(Newtonsoft.Json.Formatting.Indented);
        }
        catch (Exception)
        {
            return json;
        }
    }
}

