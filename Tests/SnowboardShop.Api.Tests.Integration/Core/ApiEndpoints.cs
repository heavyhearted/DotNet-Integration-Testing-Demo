namespace SnowboardShop.Api.Tests.Integration.Core
{
    public static class ApiEndpoints
    {
        public const string ApiBase = "api";

        public const string Base = $"{ApiBase}/snowboards";

        public static class Snowboards
        {
            public const string Create = Base;
            public const string Get = $"{Base}/{{idOrSlug}}";
            public const string GetAll = Base;
            public const string Update = $"{Base}{{id}}";
            public const string Delete = $"{Base}/{{id}}";
        }

        public static class Ratings
        {
            public const string Rate = $"{Base}/{{id}}/ratings";
            public const string GetUserRatings = $"{ApiBase}/ratings/me";
            public const string DeleteRating = $"{Base}/{{id}}/ratings";
        }
    }
}