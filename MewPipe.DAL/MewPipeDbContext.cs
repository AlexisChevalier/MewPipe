using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Compilation;
using MewPipe.DAL.Models;
using MewPipe.DAL.Models.Oauth;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MewPipe.DAL
{
    public class MewPipeDbContext : IdentityDbContext<User>
    {
        public MewPipeDbContext()
            : base("MewPipeConnection", throwIfV1Schema: false)
        {
        }

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
