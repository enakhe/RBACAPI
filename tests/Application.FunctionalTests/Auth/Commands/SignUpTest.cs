using EcommerceAPI.Application.User.Commands.SignUp;
using EcommerceAPI.Infrastructure.Identity;
using EcommerceAPI.Infrastructure.Interface;
using EcommerceAPI.Infrastructure.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Application.Common.Models;
using Azure;

namespace EcommerceAPI.Application.FunctionalTests.Auth.Commands;
public class SignUpTest : BaseTestFixture
{
    [Test]
    public async Task SignUpAsync_Should_ReturnFailure_When_EmailIsAlreadyRegistered()
    {
        //// Arrange: Mock HttpContext
        //var mockHttpContext = new Mock<HttpContext>();
        //var mockHttpResponse = new Mock<HttpResponse>();
        //mockHttpContext.Setup(c => c.Response).Returns(mockHttpResponse.Object);

        //// Arrange: Mock JWTRepository
        //var mockJwtRepository = new Mock<IJWTService>();
        //mockJwtRepository
        //    .Setup(repo => repo.GenerateToken(It.IsAny<HttpContext>(), It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>()))
        //    .Returns("mocked_token");

        // Arrange: Create SignUpCommand
        var command = new SignUpCommand
        {
            Email = "test@gmail.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        var result = await Testing.SendAsync(command);

        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Errors.Should().Contain("The provided email is already used");
    }
}
