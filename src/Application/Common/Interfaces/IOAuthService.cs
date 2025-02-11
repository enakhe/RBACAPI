using RBACAPI.Application.OAuth.Commands.FacebookSignIn;
using RBACAPI.Application.OAuth.Commands.GoogleSignIn;
using RBACAPI.Domain.Common;

namespace RBACAPI.Application.Common.Interfaces;
public interface IOAuthService
{
    Task<GoogleSignInResponse> GoogleSignIn(string code);
    Task<BaseResponse<FacebookTokenValidationResponse>> ValidateFacebookToken(string accessToken);
    Task<BaseResponse<FacebookUserInfoResponse>> GetFacebookUserInformation(string accessToken);
    Task<BaseResponse<FacebookSignInResponse>> FacebookSignIn(string accessToken);
}
