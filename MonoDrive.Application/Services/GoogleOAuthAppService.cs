using System.Threading.Tasks;
using Google.Apis.Oauth2.v2;
using LiteDB.Async;
using MonoDrive.Application.Interfaces;
using MonoDrive.Application.Models;

namespace MonoDrive.Application.Services
{
    public class GoogleOAuthAppService : IGoogleOAuthAppService
    {
        private readonly Oauth2Service _oauth2Service;
        private readonly ILiteDatabaseAsync _liteDatabaseAsync;

        public GoogleOAuthAppService(IGoogleApiServiceProvider serviceProvider, ILiteDatabaseAsync liteDatabaseAsync)
        {
            _oauth2Service = serviceProvider.GetOauth2Service();
            _liteDatabaseAsync = liteDatabaseAsync;
        }

        public async Task<GoogleUserInfo> GetUserInfo()
        {
            var collection = _liteDatabaseAsync.GetCollection<GoogleUserInfo>();
            var googleUserInfo = await collection.Query().SingleOrDefaultAsync();

            if (googleUserInfo != null)
            {
                return googleUserInfo;
            }

            var userInfoPlus = await _oauth2Service.Userinfo.Get().ExecuteAsync();

            var newGoogleUserInfo = new GoogleUserInfo
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
            
            await collection.InsertAsync(newGoogleUserInfo);

            return newGoogleUserInfo;
        }
    }
}