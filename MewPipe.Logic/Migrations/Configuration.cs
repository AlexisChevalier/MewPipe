using System.Data.Entity.Migrations;
using MewPipe.Logic.Models.Oauth;

namespace MewPipe.Logic.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<MewPipeDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MewPipeDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            context.OauthClients.AddOrUpdate(
                c => c.ClientId,
                new OauthClient
                {
                    ClientId = "my_client_id",
                    ClientSecret = "my_client_secret",
                    Description = "my description",
                    RedirectUri = "http://localhost/testOauth"
                },
                new OauthClient
                {
                    ClientId = "daa2fb46-fae8-4552-898c-4f27b3fd9003",
                    ClientSecret = "207cfd69-838e-4aa0-ab0a-f456a949532c",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Morbi consectetur dolor mi, eget faucibus metus elementum nec. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Phasellus eleifend eu elit eu auctor. Aliquam aliquam tempor nibh, at posuere neque tincidunt quis.",
                    DialogDisabled = true,
                    Name = "MewPipe Website",
                    RedirectUri = "http://mewpipe.local:44402/auth/HandleOAuthRedirect",
                    ImageSrc = "http://s.ytimg.com/yts/img/youtube_logo_stacked-vfl225ZTx.png"
                });
        }
    }
}
