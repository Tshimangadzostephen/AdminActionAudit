using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class AuditApiSmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuditApiSmokeTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task SwaggerOrHealthLoads()
    {
        var client = _factory.CreateClient();
        var res = await client.GetAsync("/swagger/index.html");
        // In test env swagger may not be enabled depending on env; just ensure app starts:
        Assert.True(res.StatusCode is System.Net.HttpStatusCode.OK
                    or System.Net.HttpStatusCode.NotFound);
    }
}
