namespace SnowboardShop.Api.Tests.Integration.TestData.TheoryData.RatingsController;

public class InvalidRatingPropertyTheoryData : TheoryData<string>
{
    public InvalidRatingPropertyTheoryData()
    {
        Add("{ \"Rating\": \"invalid_string\" }"); // String instead of int
        Add("{ \"Rating\": \"'; DROP TABLE Ratings;--\" }"); // SQL Injection
        Add("{ \"Rating\": true }"); // Boolean instead of int
        Add("{ \"Rating\": 5.5 }"); // Decimal instead of int
        Add("{ \"Rating\": [] }"); // Array instead of int
        Add("{ \"Rating\": {} }"); // Object instead of int
        
    }
}