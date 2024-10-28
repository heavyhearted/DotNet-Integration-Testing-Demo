using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using SnowboardShop.Application.Services;

namespace SnowboardShop.Api.Tests.Integration.Core;

public class MocksProvider
{
    private readonly List<Mock> _allMocks;
    
    public MocksProvider()
    {
        _allMocks = new List<Mock>
        {
            UserContextServiceMock
        };
    }
    
    public Mock<IUserContextService> UserContextServiceMock { get; } = new();

    public void RegisterMocks(IServiceCollection serviceCollection)
    {
        foreach (Mock mock in _allMocks)
        {
            var concreteType = mock.GetType().GetGenericArguments().First();
            serviceCollection.RemoveAll(concreteType);
            serviceCollection.AddTransient(concreteType, _ => mock.Object);
        }
    }

    public void ResetAllMocks()
    {
        foreach (var mock in _allMocks)
        {
            mock.Reset();
        }
    }
    
    public void SetupUserContextService(Guid? userId)
    {
        UserContextServiceMock
            .Setup(x => x.UserId)
            .Returns(userId);
    }
}