
using RBACAPI.Application.Auth.Commands.SignIn;

namespace RBACAPI.Application.FunctionalTests.Auth.Commands;

public class SignInTest(CustomWebApplicationFactory factory) : BaseFunctionalTest(factory)
{
    [Fact]
    public async Task Should_ReturnBadRequest_WhenEmailIsMissing()
    {
        var command = new SignInCommand()
        {
            Email = "",
            Password =
            "Password1234!"
        };

        HttpResponseMessage responseMessage = await HttpClient!.PostAsJsonAsync("/api/auth/signin", command);

        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
