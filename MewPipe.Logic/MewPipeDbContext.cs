using System.Data.Entity;
using MewPipe.Logic.Models;
using MewPipe.Logic.Models.Oauth;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MewPipe.Logic
{
    public class MewPipeDbContext : IdentityDbContext<User>
    {
        public MewPipeDbContext()
            : base("MewPipeConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Video> Videos { get; set; }

        public DbSet<OauthClient> OauthClients { get; set; }
        public DbSet<OauthUserTrust> OauthUserTrusts { get; set; }
        public DbSet<OauthAuthorizationCode> OauthAuthorizationCodes { get; set; }
        public DbSet<OauthAccessToken> OauthAccessTokens { get; set; }
        public DbSet<OauthRefreshToken> OauthRefreshTokens { get; set; }

        public static MewPipeDbContext Create()
        {
            return new MewPipeDbContext();
        }
    }
}
