using System;
using System.Threading.Tasks;
using Google.Apis.Oauth2.v2;
using MonoDrive.Application.Interfaces;

namespace MonoDrive.Application.Services
{
    public class GoogleOAuthAppService : IGoogleOAuthAppService
    {
        private readonly Oauth2Service _oauth2Service;

        public GoogleOAuthAppService(IGoogleApiServiceProvider serviceProvider)
        {
            _oauth2Service = serviceProvider.GetOauth2Service();
        }

        public async Task<GoogleUserInfo> GetUserInfo()
        {
            var userInfoPlus = await _oauth2Service.Userinfo.Get().ExecuteAsync();

            return new GoogleUserInfo
            {
                Email = userInfoPlus.Email,
                ETag = userInfoPlus.ETag,
                FamilyName = userInfoPlus.FamilyName,
                Gender = userInfoPlus.Gender,
                GivenName = userInfoPlus.GivenName,
                Hd = userInfoPlus.Hd,
                Id = userInfoPlus.Id,
                Link = userInfoPlus.Link,
                Locale = userInfoPlus.Locale,
                Name = userInfoPlus.Name,
                Picture = userInfoPlus.Picture,
                VerifiedEmail = userInfoPlus.VerifiedEmail
            };
        }
    }
}