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
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<IUserStore<ApplicationUser>> _mockUserStore;
        private Mock<IUserEmailStore<ApplicationUser>> _mockEmailStore;
        private Mock<IJWTService> _mockJwtService;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private IdentityService _identityService;

        [SetUp]
        public void SetUp()
        {
            _mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                _mockUserStore.Object, null, null, null, null, null, null, null, null);

            var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                _mockUserManager.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null, null, null, null);

            _mockEmailStore = new Mock<IUserEmailStore<ApplicationUser>>();
            _mockJwtService = new Mock<IJWTService>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            _mockJwtService.Setup(j => j.GenerateToken(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>()))
                .Returns("mocked-jwt-token");

            var mockHttpContext = new DefaultHttpContext();
            _mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(mockHttpContext);

            _identityService = new IdentityService(
                _mockUserManager.Object,
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                Mock.Of<IAuthorizationService>(),
                _mockJwtService.Object,
                signInManagerMock.Object,
                _mockHttpContextAccessor.Object,
                _mockUserStore.Object,
                _mockEmailStore.Object,
                Mock.Of<IOTPService>());
        }

        [Test]
        public async Task SignUpAsync_ShouldFail_WhenEmailAlreadyExists()
        {
            string existingEmail = "administrator@localhost";
            string testPassword = "StrongP@ssword123";

            var existingUser = new ApplicationUser { UserName = existingEmail, Email = existingEmail };

            await Testing.AddAsync(existingUser);
            _mockUserManager
    .Setup(x => x.FindByEmailAsync(existingEmail))
    .ReturnsAsync(new ApplicationUser { Id = "123", Email = existingEmail, UserName = existingEmail });

            //// Arrange: Mock JWTRepository
            //var mockJwtRepository = new Mock<IJWTService>();
            //mockJwtRepository
            //    .Setup(repo => repo.GenerateToken(It.IsAny<HttpContext>(), It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>()))
            //    .Returns("mocked_token");

            _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);

            var result = await _identityService.SignUpAsync(existingEmail, testPassword);

            Assert.IsFalse(result.Succeeded, "Sign-up should fail when the email already exists.");
            Assert.Contains("The provided email is already used", result.Errors);
        }
    }
}
