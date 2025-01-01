using EcommerceAPI.Application.User.Commands.SignUp;
using EcommerceAPI.Infrastructure.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Application.FunctionalTests.Auth.Commands;
public class SignUpTest : BaseTestFixture
{
    [Test]
    public async Task SignUpAsync_Should_ReturnFailure_When_EmailIsAlreadyRegistered()
    {
        var email = "administrator@local";
        var password = "Password123!";
        var confirmPassword = "Password123!";

        var command = new SignUpCommand
        {
            Email = email,
            Password = password,
            ConfirmPassword = confirmPassword
        };

        var response = await Testing.SendAsync(command);

        var result = response as BadRequestObjectResult;
        result.Should().NotBeNull();
        var errorResponse = result!.Value as dynamic;
        errorResponse!.Should().NotBeNull();
        ((IEnumerable<string>)errorResponse.error).Should().Contain("The provided email is already used");
    }
}
