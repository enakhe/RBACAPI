#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using RBACAPI.Application.Common.Interfaces;
using RBACAPI.Infrastructure.Identity;
using RBACAPI.Infrastructure.Interface;

namespace RBACAPI.Application.FunctionalTests.Auth.Commands
{
    [TestFixture]
    public class SignUpTest
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly Mock<IJWTService> _jwtServiceMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly IdentityService _identityService;

        public SignUpTest()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var authorizationServiceMock = new Mock<IAuthorizationService>();

            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(_userManagerMock.Object, contextAccessorMock.Object, null, null, null, null, null);
            _jwtServiceMock = new Mock<IJWTService>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            _identityService = new IdentityService(
                _userManagerMock.Object,
                userPrincipalFactoryMock.Object,
                authorizationServiceMock.Object,
                _jwtServiceMock.Object,
                _signInManagerMock.Object,
                _httpContextAccessorMock.Object,
                userStoreMock.Object,
                null);

        }

        [Test]
        public async Task SignInAsync_UserNotFound_ReturnsFailure()
        {
            // Arrange
            var email = "test@example.com";
            var password = "Password123";
            _userManagerMock.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _identityService.SignInAsync(email, password);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("Invalid sign-in attempt. The email or password is incorrect", result.Errors);
        }

    }
}
