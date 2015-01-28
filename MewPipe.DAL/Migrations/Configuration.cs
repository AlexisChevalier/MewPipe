using MewPipe.DAL.Models.Oauth;

namespace MewPipe.DAL.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MewPipe.DAL.MewPipeDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MewPipe.DAL.MewPipeDbContext context)
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
                });
        }
    }
}
