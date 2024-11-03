// using Bogus;
// using SnowboardShop.Application.Models;
// using SnowboardShop.Application.Repositories;
//
//
// namespace SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages.RatingsController.UpdateRatingData;
//
// public class UpdateRatingSeedData : RatingsDataSeedBase
// {
//     private readonly ISnowboardRepository _snowboardRepository;
//     private readonly Faker _snowboardFaker  = new();
//     private readonly List<Guid> _snowboardIdsToClear = new();
//     
//     public UpdateRatingSeedData(IRatingRepository ratingRepository, ISnowboardRepository snowboardRepository) : base(ratingRepository, snowboardRepository)
//     {
//     }
//
//     public override string Name => nameof(UpdateRatingSeedData);
//     protected override Dictionary<Guid, List<SnowboardRating>> CreateData()
//     {
//         throw new NotImplementedException();
//     }
// }
