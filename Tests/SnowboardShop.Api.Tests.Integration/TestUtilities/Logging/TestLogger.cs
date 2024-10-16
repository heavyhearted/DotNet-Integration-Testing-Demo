using System.Text.Json;
using RestSharp;
using Xunit.Abstractions;

namespace SnowboardShop.Api.Tests.Integration.TestUtilities.Logging;

public static class TestLogger
{
    public static void LogRequestResponse(ITestOutputHelper testOutputHelper, RestRequest request, RestResponse response)
    {
        // Log response status code
        testOutputHelper.WriteLine($"Status Code: {(int)response.StatusCode} {response.StatusDescription}");
        
        // Log request headers
        foreach (var header in request.Parameters.Where(p 
                     => p.Type == ParameterType.HttpHeader && !string.Equals(p.Name, "Authorization", StringComparison.OrdinalIgnoreCase)))
        {
            testOutputHelper.WriteLine($"Request Header: {header.Name} = {header.Value}");
        }


        // Log request body (if exists)
        var requestBody = request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody)?.Value;
        if (requestBody != null)
        {
            string serializedRequestBody = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            testOutputHelper.WriteLine($"Request Body: {serializedRequestBody}");
        }
        
        // Log response headers
        foreach (var header in response.Headers)
        {
            testOutputHelper.WriteLine($"Response Header: {header.Name} = {header.Value}");
        }

        // Log response body (pretty-printed if it's JSON)
        if (!string.IsNullOrEmpty(response.Content))
        {
            testOutputHelper.WriteLine("Response Body:");
            try
            {
                // Attempt to parse the response content as JSON and pretty print it
                var jsonDocument = JsonDocument.Parse(response.Content);
                string prettyJson = JsonSerializer.Serialize(jsonDocument, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                testOutputHelper.WriteLine(prettyJson);
            }
            catch (JsonException)
            {
                // If response content is not valid JSON, log it as is
                testOutputHelper.WriteLine(response.Content);
            }
        }
    }
    
    
}