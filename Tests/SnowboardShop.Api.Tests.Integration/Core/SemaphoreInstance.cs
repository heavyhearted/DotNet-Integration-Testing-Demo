namespace SnowboardShop.Api.Tests.Integration.Core;

/// <summary>
/// This class is used to create a single instance of a semaphore that can be used across multiple WebApplicationFactory instances as they are used as ClassFixtures in xUnit tests.
/// </summary>
public static class SemaphoreInstance
{
    public static SemaphoreSlim Instance { get; } = new SemaphoreSlim(1, 1);
}