using SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataHelpers;
using SnowboardShop.Contracts.Requests;

public class SnowboardRatingsTheoryData : TheoryData<Dictionary<CreateSnowboardRequest, RateSnowboardRequest>>
{
    public SnowboardRatingsTheoryData()
    {
        var snowboardFaker = new CreateSnowboardFaker();
        var snowboardDictionary = new Dictionary<CreateSnowboardRequest, RateSnowboardRequest>();
        
        snowboardDictionary.Add(snowboardFaker.Generate(), new() {Rating = 1});
        snowboardDictionary.Add(snowboardFaker.Generate(), new() {Rating = 2});
        snowboardDictionary.Add(snowboardFaker.Generate(), new() {Rating = 3});
        snowboardDictionary.Add(snowboardFaker.Generate(), new() {Rating = 4});
        snowboardDictionary.Add(snowboardFaker.Generate(), new() {Rating = 5});
        
        Add(snowboardDictionary);
    }
}