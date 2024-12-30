using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcommerceAPI.Application.OAuth.Commands.FacebookSignIn;
using EcommerceAPI.Application.OAuth.Commands.GoogleSignIn;
using EcommerceAPI.Domain.Common;

namespace EcommerceAPI.Application.Common.Interfaces;
public interface IOAuthService
{
    Task<GoogleSignInResponse> GoogleSignIn(string code);
    Task<BaseResponse<FacebookTokenValidationResponse>> ValidateFacebookToken(string accessToken);
    Task<BaseResponse<FacebookUserInfoResponse>> GetFacebookUserInformation(string accessToken);
    Task<BaseResponse<FacebookSignInResponse>> FacebookSignIn(string accessToken);
}
