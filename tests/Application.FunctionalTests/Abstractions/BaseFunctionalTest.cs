using Xunit;

namespace RBACAPI.Application.FunctionalTests.Abstractions;
public class BaseFunctionalTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory = factory;
    protected HttpClient? HttpClient { get; private set; }

    public async Task InitializeAsync()
    {
        await _factory.InitializeAsync();
        HttpClient = _factory.CreateClient();
    }

    public Task DisposeAsync()
    {
        return _factory.DisposeAsync();
    }
}
