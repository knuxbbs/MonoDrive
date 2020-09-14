using System;
using System.Threading.Tasks;
using Google.Apis.Oauth2.v2;
using MonoDrive.Application.Interfaces;

namespace MonoDrive.Application.Services
{
    public class GoogleOAuthAppService : IGoogleOAuthAppService
    {
        private readonly Lazy<Task<Oauth2Service>> _oauth2Service;

        public GoogleOAuthAppService(IGoogleApiServiceProvider serviceProvider)
        {
            _oauth2Service = new Lazy<Task<Oauth2Service>>(async () => 
                await serviceProvider.GetOauth2Service());
        }

        public async Task<GoogleUserInfo> GetUserInfo()
        {
            var oauthService = await _oauth2Service.Value;
            
            var userInfoPlus = await (await _oauth2Service.Value).Userinfo.Get().ExecuteAsync();

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